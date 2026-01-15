using FluentValidation;
using FluentValidation.AspNetCore;
using IdentityService.Application.Validators;
using IdentityService.DependencyInjection;
using IdentityService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IdentityService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers();
           
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();


            builder.Services.AddIdentityInfrastructure(builder.Configuration);
            builder.Services.AddIdentityApplication(builder.Configuration);

            builder.Services.AddApiSecurity(builder.Configuration);

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
            
            builder.Services.AddMessageBus(builder.Configuration); 


            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

            builder.Host.UseSerilog();

            // Health Checks
            builder.Services.AddHealthChecks();


            var app = builder.Build();

            // ==========================================================================
            // Database Migration at Startup
            // ==========================================================================
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var dbContext = services.GetRequiredService<IdentityDbContext>();
                    dbContext.Database.Migrate();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating the identity database.");
                }
            }

            // Configure the HTTP request pipeline.
            // Swagger her zaman açık (Development ve Production)
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CityDiscovery Identity Service API v1");
                //c.RoutePrefix = string.Empty; // Swagger'ı root'ta göster
                c.DocumentTitle = "Identity Service API Documentation";
            });

            app.UseAuthentication();
            app.UseAuthorization();

            // Health Check Endpoint
            app.MapHealthChecks("/health");

            app.MapControllers();

            app.Run();
        }
    }
}
