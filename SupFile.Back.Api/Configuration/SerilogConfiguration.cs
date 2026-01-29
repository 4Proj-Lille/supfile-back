using Serilog;

namespace SupFile.Back.Api.Configuration;

internal static class SerilogConfiguration
{
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, configuration) =>
        {
            var writeToSection = context.Configuration.GetSection("Serilog:WriteTo");
            var hasWriteTo = writeToSection.GetChildren().Any();

            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext();

            if (!hasWriteTo)
            {
                configuration.WriteTo.Console();
            }
        });

        return builder;
    }
}
