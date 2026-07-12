namespace RisContentPipeline.GUI.Models;

/// <summary>
/// Represents a log message with severity level.
/// </summary>
public record LogMessage(MessageLogLevel Level, string Message, DateTime Timestamp)
{
    public static LogMessage Info(string message) => new(MessageLogLevel.INFO, message, DateTime.Now);
    public static LogMessage Warning(string message) => new(MessageLogLevel.WARNING, message, DateTime.Now);
    public static LogMessage Error(string message) => new(MessageLogLevel.ERROR, message, DateTime.Now);
}
