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

            // Swagger Configuration
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CityDiscovery Identity Service API",
                    Version = "v1",
                    Description = "Identity ve Authentication servisi için RESTful API. Kullanıcı yönetimi, JWT token yönetimi ve yetkilendirme işlemlerini içerir.",
                    Contact = new OpenApiContact
                    {
                        Name = "CityDiscovery Team",
                        Email = "support@citydiscovery.com"
                    }
                });

                // XML Comments'i dahil et
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                // JWT Bearer Token Security Definition
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Örnek: \"Bearer {token}\""
                });

                // Tüm endpoint'lerde Bearer token kullanımını zorunlu kıl
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

            return services;
        }

        public static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration)
        {
            // Appsettings'den okuyamazsa varsayılan değerleri kullan
            var host = configuration["RabbitMq:Host"] ?? "localhost";

            // VirtualHost: Diğer servislerde "/" kullanıyorsan burada da "/" olmalı!
            var vhost = configuration["RabbitMq:VirtualHost"];
            if (string.IsNullOrEmpty(vhost)) vhost = "/";

            var user = configuration["RabbitMq:Username"] ?? "guest";
            var pass = configuration["RabbitMq:Password"] ?? "guest";

            services.AddMassTransit(x =>
            {
                // Identity genelde sadece mesaj üretir (Publisher). 
                // Ama ileride consumer eklerseniz buraya x.AddConsumer<...>(); yazarsınız.

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(host, vhost, h =>
                    {
                        h.Username(user);
                        h.Password(pass);
                    });

                    // Endpoint yapılandırmasını otomatik yap
                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
