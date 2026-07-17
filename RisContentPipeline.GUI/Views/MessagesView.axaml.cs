using Avalonia;
using Avalonia.Controls;
using RisContentPipeline.GUI.Models;

namespace RisContentPipeline.GUI.Views;

    /// <summary>
    /// View for displaying log messages with filtering by log level.
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
        /// Avalonia DirectProperty for binding to error log level filter.
        /// </summary>
        public static readonly DirectProperty<MessagesView, MessageLogLevel> ErrorLogLevel = 
            AvaloniaProperty.RegisterDirect<MessagesView, MessageLogLevel>(nameof(ErrorLevel), o => o.ErrorLevel);

    /// <summary>
    /// The error level.
    /// </summary>
    protected MessageLogLevel ErrorLevel => Models.MessageLogLevel.ERROR;

    /// <summary>
    /// Avalonia DirectProperty for binding to warning log level filter.
    /// </summary>
    internal static readonly DirectProperty<MessagesView, MessageLogLevel> WarningLogLevel = 
        AvaloniaProperty.RegisterDirect<MessagesView, MessageLogLevel>(nameof(WarningLevel), o => o.WarningLevel);
    
    /// <summary>
    /// The warning level.
    /// </summary>
    protected MessageLogLevel WarningLevel => MessageLogLevel.WARNING;
    
    /// <summary>
    /// Avalonia DirectProperty for binding to info log level filter.
    /// </summary>
    internal static readonly DirectProperty<MessagesView, MessageLogLevel> InfoLogLevel =
        AvaloniaProperty.RegisterDirect<MessagesView, MessageLogLevel>(nameof(InfoLevel), o => o.InfoLevel);
    
    /// <summary>
    /// The info level.
    /// </summary>
    protected MessageLogLevel InfoLevel => MessageLogLevel.INFO;
}
