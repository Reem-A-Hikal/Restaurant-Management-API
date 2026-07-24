using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Rest.API.Middleware;
using Rest.API.Responses;
using Rest.Application.Interfaces;
using Rest.Application.Interfaces.IRepositories;
using Rest.Application.Interfaces.IServices;
using Rest.Application.Interfaces.IServices.StrategyFactory;
using Rest.Application.Profiles;
using Rest.Application.Services;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;
using Rest.Infrastructure.Data;
using Rest.Infrastructure.Implementations;
using Rest.Infrastructure.Implementations.Repositories;
using Rest.Infrastructure.Implementations.Services;
using Rest.Infrastructure.Implementations.StrategyFactory;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.RateLimiting;

namespace Rest.API
{
    /// <summary>
    /// Main entry point for the application.
    /// </summary>
    public class Program
    {
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

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

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

            // Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IAddressService, AddressService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IDeliveryService, DeliveryService>();
            builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IImageUploadService>(
                sp => new ImageUploadService( builder.Environment.WebRootPath));
            builder.Services.AddScoped<IDashboardService, DashboardService>();

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
                        }));

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.ContentType = "application/json";
                    var response = ApiResponse<string>.FailResponse(
                        new List<string> { "Too many attempts. Please try again later." },
                        "Rate limit exceeded");

                    await context.HttpContext.Response.WriteAsync(
                        System.Text.Json.JsonSerializer.Serialize(response,
                            new System.Text.Json.JsonSerializerOptions
                            {
                                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                            }),
                        cancellationToken);
                };
            });

            // Role Strategies
            builder.Services.AddScoped<IRoleStrategy, ChefStrategy>();
            builder.Services.AddScoped<IRoleStrategy, DeliveryPersonStrategy>();
            builder.Services.AddScoped<IRoleStrategy, AdminStrategy>();
            builder.Services.AddScoped<IRoleStrategy, CustomerStrategy>();

            builder.Services.AddScoped<IRoleStrategyResolver, RoleStrategyResolver>();
            builder.Services.AddScoped<UserCreationHelper>();

            // CORS
            string txt = "AllowAll";
            builder.Services.AddCors( options =>
            {
                options.AddPolicy(txt,
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
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
            })
            .AddEntityFrameworkStores<RestDbContext>()
            .AddDefaultTokenProviders();

            //JWT Configuration
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"]!;
            var key = Encoding.ASCII.GetBytes(secretKey);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
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
            });

            // Authorization
            builder.Services.AddAuthorizationBuilder()
            .AddPolicy("SelfOrAdmin", policy =>
                policy.RequireAssertion(context =>
                {
                    var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var routeUserId = context.Resource is HttpContext httpContext
                    ? httpContext.Request.RouteValues["userId"]?.ToString()
                    : null;

                    return userIdClaim == routeUserId || context.User.IsInRole("Admin");
                })
            );

            // AutoMapper
            builder.Services.AddAutoMapper(typeof(UserProfile).Assembly);
            //builder.Services.AddAutoMapper(typeof(ProductProfile));
            //builder.Services.AddAutoMapper(typeof(AddressProfile));
            //builder.Services.AddAutoMapper(typeof(OrderProfile));
            //builder.Services.AddAutoMapper(typeof(CategoryProfile));

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

                // JWT in Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
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

            var app = builder.Build();

            // Seed Roles
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider
                    .GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                string[] roles = ["Admin", "Chef", "DeliveryPerson", "Customer"];
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }

                string adminEmail = "admin@restora.com";
                string adminPassword = "Admin@123456";
                string adminUserName = adminEmail.Split('@')[0];

                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = User.Create(
                        adminEmail,
                        adminUserName,
                        "System Administrator",
                        "01065235485",
                        "1.png");

                    var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                    if (createResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                    else
                    {
                        var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                        Console.WriteLine($"❌ Failed to seed admin: {errors}");
                    }
                }
            }

            // Middleware Pipeline
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(op => op.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"));
            }

            app.UseHttpsRedirection();
            app.UseCors(txt);
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();
            app.MapControllers();

            app.Run();
        }
    }
}