using Microsoft.EntityFrameworkCore;
using Rest.API.Profiles;
using Rest.API.Services.Implementations;
using Rest.API.Services.Interfaces;

namespace Rest.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<RestDbContext>(options =>
                options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            builder.Services.AddScoped<IAuthService, AuthService>();

            string txt = "";
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
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(op => op.SwaggerEndpoint("/openapi/v1.json", "v1"));
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseCors(txt);


            app.MapControllers();

            app.Run();
        }
    }
}
