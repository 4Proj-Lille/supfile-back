using Microsoft.Extensions.Logging;

namespace SupFile.Back.Core.Configuration;

public static class StaticLoggerConfiguration
{

    private static ILoggerFactory LoggerFactory { get; set; } = new LoggerFactory();

    public static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();

    public static ILogger CreateLogger(string CategoryName) => LoggerFactory.CreateLogger(CategoryName);

    public static void SetStaticLoggerFactory(ILoggerFactory loggerFactory)
    {
        LoggerFactory = loggerFactory;
    }

}
