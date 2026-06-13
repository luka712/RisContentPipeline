using Eto.Drawing;
using Eto.Forms;
using RisContentPipeline.GUI.Services;

namespace RisContentPipeline.GUI.Views
{
    /// <summary>
    /// Renders build various messages produced by the <see cref="MessageLogger"/>.
    /// </summary>
    internal class MessageView
    {
        private readonly Context _context;
        private readonly TreeGridView _messagesTreeView;
        private readonly TreeGridItem _rootItem;

        /// <summary>
        /// Gets the panel control containing the build output tree view.
        /// </summary>
        public Panel Content { get; }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="context">The context for the message view, which provides access to the messenger for receiving build messages.</param>
        public MessageView(Context context)
        {
            _context = context;
            _messagesTreeView = new TreeGridView
            {
                ShowHeader = true,
                AllowMultipleSelection = false,
                ContextMenu = new ContextMenu()
                {
                    Items =
                    {
                        new ButtonMenuItem
                        {
                            Text = "Clear", Command = new Command((_, _) => ClearMessages())
                        }
                    }
                }
            };

            // Add a column to the tree view
            _messagesTreeView.Columns.Add(new GridColumn
            {
                HeaderText = "Messages",
                AutoSize = true,
                Expand = true,
                DataCell = new ImageTextCell(0, 1),
            });

            _messagesTreeView.Columns.Add(new GridColumn
            {
                HeaderText = "Timestamp",
                AutoSize = true,
                Expand = true,
                DataCell = new TextBoxCell(2),
            });

            // Create a placeholder root item that hosts every log entry as a child.
            _rootItem = new TreeGridItem
            {
                Values = [Icons.FolderIcon, "Message", "Timestamp"],
                Expanded = true,
            };

            _messagesTreeView.DataStore = _rootItem;

            var buildLogger = context.MessageLogger;
            buildLogger.OnSuccessLog += msg => AddMessage(msg, Icons.CheckIcon);
            buildLogger.OnErrorLog += msg => AddMessage(msg, Icons.FileIcon);
            buildLogger.OnInfoLog += msg => AddMessage(msg, Icons.InfoIcon);
            buildLogger.OnWarnLog += msg => AddMessage(msg, Icons.CheckIcon);

            // Wrap the tree view in a titled GroupBox for clearer visual separation
            Content = new GroupBox
            {
                Text = "Build Output",
                Font = SystemFonts.Bold(),
                Padding = new Padding(Theme.PADDING),
                Content = _messagesTreeView,
            };
        }

        private void ClearMessages()
        {
            _rootItem.Children.Clear();
            // Refresh data store so newly added rows are visible.
            _messagesTreeView.ReloadData();
        }
        
        internal void Refresh()
        {
            _messagesTreeView.ReloadData();
        }

        /// <summary>
        /// Adds a message to the build output tree view.
        /// </summary>
        /// <param name="message">The message to add.</param>
        /// <param name="icon">An optional icon to display alongside the message.</param>
        public void AddMessage(string message, Icon? icon = null)
        {
            var time = DateTime.Now.ToString("HH:mm:ss");
            _rootItem.Children.Add(new TreeGridItem
            {
                Values = [icon, message, time],
                Expanded = true,
            });

            // Refresh data store so newly added rows are visible.
            _messagesTreeView.ReloadData();

            // Scroll to the bottom of the tree view.            
            _messagesTreeView.ScrollToRow(_rootItem.Children.Count - 1);
        }
    }
}