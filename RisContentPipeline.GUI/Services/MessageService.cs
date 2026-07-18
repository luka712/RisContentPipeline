using RisContentPipeline.GUI.Models;
using RisContentPipeline.GUI.ViewModels;

namespace RisContentPipeline.GUI.Services;

/// <summary>
/// The message service.
/// </summary>
public class MessageService
{
    private readonly IPipelineSystem _pipelineSystem;
    private readonly List<LogMessageViewModel> _messages = new();

    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="pipelineSystem">The <see cref="IPipelineSystem"/>.</param>
    public MessageService(IPipelineSystem pipelineSystem)
    {
        _pipelineSystem = pipelineSystem;
        
        _pipelineSystem.OnConversionException += (sender, e) =>
        {
            var msg = $"Conversion from {e.SourceFilePath} to {e.TargetFilePath} failed. {e.Message}";
            Error(msg);
        };
    }
    
    /// <summary>
    /// Indicates that a message has been added.
    /// </summary>
    public event Action<LogMessageViewModel>? OnMessage;

    /// <summary>
    /// Indicates that the messages have been cleared.
    /// </summary>
    public event Action? OnClear;
    
    /// <summary>
    /// Clears the messages.
    /// </summary>
    public void Clear()
    {
        _messages.Clear();
        OnClear?.Invoke();
    }

    /// <summary>
    /// Adds an info log message.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Info(string message)
    {
        var msg = new LogMessageViewModel(message, MessageLogLevel.INFO);
        _messages.Add(msg);
        OnMessage?.Invoke(msg);
    }

    /// <summary>
    /// Adds a warning log message.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Warning(string message)
    {
        var msg = new LogMessageViewModel(message, MessageLogLevel.WARNING);
        _messages.Add(msg);
        OnMessage?.Invoke(msg);
    }

    /// <summary>
    /// Adds an error log message.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Error(string message)
    {
        var msg = new LogMessageViewModel(message, MessageLogLevel.ERROR);
        _messages.Add(msg);
        OnMessage?.Invoke(msg);
    }
}