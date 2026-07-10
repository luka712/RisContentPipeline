namespace RisContentPipeline.GUI.Models;

/// <summary>
/// Represents a log message with severity level.
/// </summary>
public record LogMessage(LogLevel Level, string Message, DateTime Timestamp)
{
    public static LogMessage Info(string message) => new(LogLevel.Info, message, DateTime.Now);
    public static LogMessage Warning(string message) => new(LogLevel.Warning, message, DateTime.Now);
    public static LogMessage Error(string message) => new(LogLevel.Error, message, DateTime.Now);
}

/// <summary>
/// Log message severity levels.
/// </summary>
public enum LogLevel
{
    Info,
    Warning,
    Error
}
