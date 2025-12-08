using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace SupFile.Back.Api.Configuration;

internal static class SerilogConfiguration
{
    public static ILogger AddSerilog(this WebApplicationBuilder builder)
    {
        var loggerConfiguration = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration);
        var logger = loggerConfiguration.CreateLogger();

        builder.Host.UseSerilog(logger);


#pragma warning disable CA2000
        StaticLoggerConfiguration.SetStaticLoggerFactory((LoggerFactory)new LoggerFactory().AddSerilog(logger));
#pragma warning restore CA2000

        return StaticLoggerConfiguration.CreateLogger<Program>();
    }
}
