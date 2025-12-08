using System.Reflection;
using Microsoft.OpenApi.Models;

namespace SupFile.Back.Api.Extensions;

internal static class ServiceCollectionExtensions
{
    public static void AddMapster(this IServiceCollection services)
    {
        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton(typeAdapterConfig);
    }

    public static IServiceCollection ConfigureLocalization(this IServiceCollection services, AppSettings config)
    {
        services.AddLocalization();

        var supportedCultures = config.SupportedCultures.ToArray();

        var localizationOptions = new RequestLocalizationOptions()
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures)
            .SetDefaultCulture(supportedCultures[0]);

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.SupportedCultures = localizationOptions.SupportedCultures;
            options.SupportedUICultures = localizationOptions.SupportedUICultures;
            options.DefaultRequestCulture = localizationOptions.DefaultRequestCulture;
        });

        return services;
    }

    public static IServiceCollection ConfigureSwagger(this IServiceCollection services, AppSettings config)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(config.Version,
                new OpenApiInfo { Title = config.Name, Version = config.Version, Description = config.Description });


            // Define the JWT Bearer scheme properly
            options.AddSecurityDefinition("Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token below:"
                });

            // Apply Bearer token globally
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}
