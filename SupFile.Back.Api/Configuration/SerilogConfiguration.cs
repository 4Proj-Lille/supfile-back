using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace SupFile.Back.Api.Configuration;

internal static class SerilogConfiguration
{
    public static ILogger AddSerilog(this WebApplicationBuilder builder)
    {
        var loggerConfiguration = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration);
        var logger = loggerConfiguration.CreateLogger();

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

#pragma warning disable CA2000
        StaticLoggerConfiguration.SetStaticLoggerFactory((LoggerFactory)new LoggerFactory().AddSerilog(logger));
#pragma warning restore CA2000

        return StaticLoggerConfiguration.CreateLogger<Program>();
    }
}
