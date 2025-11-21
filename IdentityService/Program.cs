
using FluentValidation;
using FluentValidation.AspNetCore;
using IdentityService.Application.Validators;
using IdentityService.DependencyInjection;
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
           

          
            builder.Services.AddIdentityInfrastructure(builder.Configuration);
            builder.Services.AddIdentityApplication(builder.Configuration);

            builder.Services.AddApiSecurity(builder.Configuration);

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
            
            builder.Services.AddMessageBus(builder.Configuration); 


            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

            builder.Host.UseSerilog();





            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
