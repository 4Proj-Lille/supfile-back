using System.Globalization;
using Microsoft.Extensions.Logging;

namespace SupFile.Back.Core.Helpers;

/// <summary>
///     Helper class to log messages
/// </summary>
public static class LogHelper
{
    private static readonly Action<ILogger, string, string, Exception?> s_logErrorDelegate =
        LoggerMessage.Define<string, string>(
            LogLevel.Error,
            new EventId(1, "LogError"),
            "{Category} {Message}"
        );

    private static readonly Action<ILogger, string, string, Exception?> s_logInformationDelegate =
        LoggerMessage.Define<string, string>(
            LogLevel.Information,
            new EventId(2, "LogInformation"),
            "{Category} {Message}"
        );


    public static void LogInformation(ILogger logger, string category, string message, params object?[] args)
    {
        var formattedMessage = string.Format(CultureInfo.InvariantCulture, message, args);
        s_logInformationDelegate(logger, category, formattedMessage, null);
    }


    public static void LogError(ILogger logger, string category, Exception? ex, string message, params object?[] args)
    {
        var formattedMessage = string.Format(CultureInfo.InvariantCulture, message, args);

        s_logErrorDelegate(logger, category, formattedMessage, ex);
    }
}
