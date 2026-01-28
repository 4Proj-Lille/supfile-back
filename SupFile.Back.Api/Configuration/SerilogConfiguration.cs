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
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
            // write to console should not be needed but is because of docker .env variables
            // if we found a way to stop using .env and only use an appsettings.docker.json
            // we would be able to remove the following line
            .WriteTo.Console();
                
        });

#pragma warning disable CA2000
        StaticLoggerConfiguration.SetStaticLoggerFactory((LoggerFactory)new LoggerFactory().AddSerilog(logger));
#pragma warning restore CA2000

        return StaticLoggerConfiguration.CreateLogger<Program>();
    }
}
