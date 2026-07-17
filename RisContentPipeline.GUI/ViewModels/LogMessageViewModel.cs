using RisContentPipeline.GUI.Models;

namespace RisContentPipeline.GUI.ViewModels;

/// <summary>
/// The log message.
/// </summary>
public class LogMessageViewModel : ViewModelBase
{
    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="logLevel">The message level.</param>
    public LogMessageViewModel(string message, MessageLogLevel logLevel)
    {
        Message = message;
        LogLevel = logLevel;
    }

    /// <summary>
    /// The message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// The log level.
    /// </summary>
    public MessageLogLevel LogLevel { get; }

    /// <summary>
    /// The timestamp.
    /// </summary>
    public DateTime Timestamp { get; } = DateTime.Now;
}