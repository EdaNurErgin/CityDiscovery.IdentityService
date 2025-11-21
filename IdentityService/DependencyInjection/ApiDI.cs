using System;
using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace IdentityService.DependencyInjection
{
    public static class ApiDI
    {
        public static IServiceCollection AddApiSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            // JWT
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });


            services.AddAuthorization();

            // Swagger Security
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "IdentityService", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Bearer {token}"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                  {
                    new OpenApiSecurityScheme
                    {
                      Reference = new OpenApiReference{ Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                  }
                });
            });

            return services;
        }

        // İstersen MassTransit’i buraya da koyabilirsin:
        public static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration)
        {
            var host = configuration["RabbitMq:Host"] ?? "rabbitmq";
            var vhost = configuration["RabbitMq:VirtualHost"] ?? "/";
            var user = configuration["RabbitMq:Username"] ?? "guest";
            var pass = configuration["RabbitMq:Password"] ?? "guest";

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(host, vhost, h =>
                    {
                        h.Username(user);
                        h.Password(pass);
                    });
                });
            });

            return services;
        }
    }
}
