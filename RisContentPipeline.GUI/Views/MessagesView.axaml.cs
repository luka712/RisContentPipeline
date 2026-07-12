using Avalonia;
using Avalonia.Controls;
using RisContentPipeline.GUI.Models;

namespace RisContentPipeline.GUI.Views;

/// <summary>
/// TODO: doc comment
/// </summary>
public partial class MessagesView : UserControl
{
    /// <summary>
    /// The constructor.
    /// </summary>
    public MessagesView()
    {
        InitializeComponent();
    }
    
    /// <summary>
    /// TODO: doc comment
    /// </summary>
    public static readonly DirectProperty<MessagesView, MessageLogLevel> ErrorLogLevel = 
        AvaloniaProperty.RegisterDirect<MessagesView, MessageLogLevel>(nameof(ErrorLevel), o => o.ErrorLevel);

    /// <summary>
    /// The error level.
    /// </summary>
    protected MessageLogLevel ErrorLevel => Models.MessageLogLevel.ERROR;

    /// <summary>
    /// TODO: doc comment
    /// </summary>
    internal static readonly DirectProperty<MessagesView, MessageLogLevel> WarningLogLevel = 
        AvaloniaProperty.RegisterDirect<MessagesView, MessageLogLevel>(nameof(WarningLevel), o => o.WarningLevel);
    
    /// <summary>
    /// The warning level.
    /// </summary>
    protected MessageLogLevel WarningLevel => MessageLogLevel.WARNING;
    
    /// <summary>
    /// The info log level.
    /// </summary>
    internal static readonly DirectProperty<MessagesView, MessageLogLevel> InfoLogLevel =
        AvaloniaProperty.RegisterDirect<MessagesView, MessageLogLevel>(nameof(InfoLevel), o => o.InfoLevel);
    
    /// <summary>
    /// The info level.
    /// </summary>
    protected MessageLogLevel InfoLevel => MessageLogLevel.INFO;
}