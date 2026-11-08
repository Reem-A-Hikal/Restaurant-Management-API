using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Rest.API.Auth;
using Rest.API.BackgroundServices;
using Rest.API.Converters;
using Rest.API.Middleware;
using Rest.API.Responses;
using Rest.Application.Interfaces;
using Rest.Application.Interfaces.IRepositories;
using Rest.Application.Interfaces.IServices;
using Rest.Application.Interfaces.IServices.StrategyFactory;
using Rest.Application.Options;
using Rest.Application.Profiles;
using Rest.Application.Services;
using Rest.Domain.Entities;
using Rest.Infrastructure.Data;
using Rest.Infrastructure.Implementations;
using Rest.Infrastructure.Implementations.Repositories;
using Rest.Infrastructure.Implementations.Services;
using Rest.Infrastructure.Implementations.StrategyFactory;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;

namespace Rest.API
{
    /// <summary>
    /// Main entry point for the application.
    /// </summary>
    public class Program
    {
        private static readonly JsonSerializerOptions ErrorJson = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        /// <summary>
        /// Writes an <see cref="ApiResponse{T}"/>-shaped error so every failure
        /// (middleware, auth, rate limit, validation) has the same body.
        /// </summary>
        private static Task WriteErrorAsync(
            HttpResponse response, int statusCode, string title, string error,
            CancellationToken cancellationToken = default)
        {
            response.StatusCode = statusCode;
            response.ContentType = "application/json";

            var body = ApiResponse<string>.FailResponse(new List<string> { error }, title);
            return response.WriteAsync(JsonSerializer.Serialize(body, ErrorJson), cancellationToken);
        }

        /// <summary>
        /// Main method to run the application.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Database
            builder.Services.AddDbContext<RestDbContext>(options =>
                options.UseLazyLoadingProxies()
                        .UseSqlServer(builder.Configuration
                            .GetConnectionString("DefaultConnection")));

            // Controllers + one error shape for model-validation failures (M4)
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new UtcDateTimeJsonConverter());
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context.ModelState
                            .Where(e => e.Value?.Errors.Count > 0)
                            .SelectMany(e => e.Value!.Errors.Select(err =>
                                !string.IsNullOrWhiteSpace(err.ErrorMessage)
                                    ? err.ErrorMessage
                                    : string.IsNullOrEmpty(e.Key)
                                        ? "Invalid request body."
                                        : $"Invalid value for '{e.Key}'."))
                            .Distinct()
                            .ToList();

                        return new BadRequestObjectResult(
                            ApiResponse<string>.FailResponse(errors, "Validation Error"));
                    };
                });

            // Repositories
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IAddressRepository, AddressRepository>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IChefRepository, ChefRepository>();
            builder.Services.AddScoped<IDeliveryPersonRepository, DeliveryPersonRepository>();
            builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            // Unit of Work
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Images: wwwroot may not exist in a fresh clone / deployment (WebRootPath = null)
            var webRoot = builder.Environment.WebRootPath
                          ?? Path.Combine(builder.Environment.ContentRootPath, "wwwroot");
            Directory.CreateDirectory(webRoot);
            builder.Services.AddSingleton<IImageUploadService>(new ImageUploadService(webRoot));

            // Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IAddressService, AddressService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IDeliveryService, DeliveryService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<IMenuService, MenuService>();

            builder.Services.AddSingleton<IKitchenSchedule, KitchenSchedule>();
            builder.Services.AddSingleton<IDeliveryZone, DeliveryZone>();

            builder.Services.Configure<ImageCleanupOptions>(
                builder.Configuration.GetSection(ImageCleanupOptions.SectionName));
            builder.Services.AddHostedService<OrphanImageCleanupService>();

            // Rate limiting
            builder.Services.AddRateLimiter(options =>
            {
                options.AddPolicy("AuthPolicy", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                        })
                    );

                options.AddPolicy("UploadPolicy", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                                      ?? httpContext.Connection.RemoteIpAddress?.ToString()
                                      ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        })
                    );

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.OnRejected = (context, cancellationToken) =>
                    new ValueTask(WriteErrorAsync(
                        context.HttpContext.Response,
                        StatusCodes.Status429TooManyRequests,
                        "Rate limit exceeded",
                        "Too many attempts. Please try again later.",
                        cancellationToken));
            });

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            // Role Strategies
            builder.Services.AddScoped<IRoleStrategy, ChefStrategy>();
            builder.Services.AddScoped<IRoleStrategy, DeliveryPersonStrategy>();
            builder.Services.AddScoped<IRoleStrategy, AdminStrategy>();
            builder.Services.AddScoped<IRoleStrategy, CustomerStrategy>();

            builder.Services.AddScoped<IRoleStrategyResolver, RoleStrategyResolver>();
            builder.Services.AddScoped<UserCreationHelper>();

            // CORS: explicit origins from configuration (Cors:AllowedOrigins)
            const string corsPolicy = "AllowedOrigins";
            var allowedOrigins = builder.Configuration
                .GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

            if (allowedOrigins.Length == 0)
            {
                if (!builder.Environment.IsDevelopment())
                    throw new InvalidOperationException(
                        "Cors:AllowedOrigins is not configured (set Cors__AllowedOrigins__0, __1 ...).");

                allowedOrigins = 
                [
                    "http://localhost:4200",
                    "http://localhost:4201",
                    "http://myrest.runasp.net",
                    "https://myrest.runasp.net",
                ];
            }

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(corsPolicy, policy =>
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader());
            });

            // Identity
            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddEntityFrameworkStores<RestDbContext>()
            .AddDefaultTokenProviders();

            // JWT Configuration
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            if (string.IsNullOrWhiteSpace(secretKey) || Encoding.ASCII.GetByteCount(secretKey) < 32)
                throw new InvalidOperationException(
                    "JwtSettings:SecretKey is missing or shorter than 32 bytes.");

            var key = Encoding.ASCII.GetBytes(secretKey);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero, // Remove delay of token when expired
                    RequireExpirationTime = true
                };

                options.Events = new JwtBearerEvents
                {
                    // Rejects suspended / inactive / deleted users even with a valid token
                    OnTokenValidated = ActiveUserTokenValidator.ValidateAsync,

                    // Same body shape as every other error. Status stays 401 so the
                    // frontend's refresh-token flow keeps working.
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        return WriteErrorAsync(
                            context.Response,
                            StatusCodes.Status401Unauthorized,
                            "Unauthorized",
                            "Authentication is required.");
                    },

                    OnForbidden = context => WriteErrorAsync(
                        context.Response,
                        StatusCodes.Status403Forbidden,
                        "Access denied",
                        "You are not allowed to perform this action.")
                };
            });

            // Authorization
            builder.Services.AddAuthorizationBuilder()
                .AddPolicy("SelfOrAdmin", policy =>
                    policy.RequireAuthenticatedUser().RequireAssertion(context =>
                    {
                        if (context.User.IsInRole("Admin")) return true;

                        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        var routeUserId = (context.Resource as HttpContext)?
                            .Request.RouteValues["userId"]?.ToString();

                        // Both must exist: null == null must never grant access.
                        return userIdClaim is not null && userIdClaim == routeUserId;
                    }));

            // AutoMapper
            builder.Services.AddAutoMapper(typeof(UserProfile).Assembly);

            // Swagger
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Restaurant Management API",
                    Version = "v1",
                    Description = "API for managing restaurant operations",
                    Contact = new OpenApiContact
                    {
                        Name = "Reem Heikal"
                    }
                });
                c.EnableAnnotations();
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                // JWT in Swagger: paste the token only (no "Bearer " prefix)
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // Options (validated at startup so a bad setting fails fast, not on a customer's order)
            builder.Services.AddOptions<OrderPricingOptions>()
                .Bind(builder.Configuration.GetSection(OrderPricingOptions.SectionName))
                .Validate(o => o.DeliveryFee >= 0, "OrderPricing:DeliveryFee must be >= 0.")
                .ValidateOnStart();

            builder.Services.AddOptions<DeliveryEstimateOptions>()
                .Bind(builder.Configuration.GetSection(DeliveryEstimateOptions.SectionName))
                .Validate(o => o.MinMinutes > 0 && o.MinMinutes <= o.MaxMinutes,
                          "DeliveryEstimate: 0 < MinMinutes <= MaxMinutes required.")
                .ValidateOnStart();


            builder.Services.AddOptions<KitchenHoursOptions>()
                .Bind(builder.Configuration.GetSection(KitchenHoursOptions.SectionName))
                .Validate(o => o.OrdersOpenAt != o.OrdersCloseAt, "KitchenHours: OpensAt and ClosesAt must differ.")
                .ValidateOnStart();

            builder.Services.AddOptions<DeliveryZoneOptions>()
                .Bind(builder.Configuration.GetSection(DeliveryZoneOptions.SectionName))
                .Validate(o => o.Areas?.Count > 0, "DeliveryZone: at least one area required.")
                .ValidateOnStart();

            var app = builder.Build();

            // Seed roles + optional bootstrap admin (credentials come from configuration only)
            using (var scope = app.Services.CreateScope())
            {
                await DbSeeder.SeedAsync(scope.ServiceProvider);
            }

            // Middleware pipeline
            app.UseForwardedHeaders();
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            //if (app.Environment.IsDevelopment())
            //{
                app.UseSwagger();
                app.UseSwaggerUI(op => op.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"));
            //}
            //else
            //{
            //app.UseHsts();
            //}
            app.MapGet("/", () => Results.Redirect("/swagger"));

            app.UseHttpsRedirection();
            app.UseCors(corsPolicy);
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                    ctx.Context.Response.Headers["X-Content-Type-Options"] = "nosniff"
            });

            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();
            app.MapControllers();

            app.Run();
        }
    }
}