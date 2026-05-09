using Eto.Drawing;
using Eto.Forms;

namespace RisContentPipeline.GUI.Views
{
    /// <summary>
    /// Renders build output messages produced by the <see cref="Services.BuildLogger"/>.
    /// </summary>
    internal class BuildView
    {
        private readonly Context _context;
        private readonly TreeGridItem _rootItem;

        /// <summary>
        /// Gets the panel control containing the build output tree view.
        /// </summary>
        public Panel Content { get; }

        /// <summary>
        /// Gets the underlying tree view for advanced styling.
        /// </summary>
        public TreeGridView BuildOutputTreeView { get; }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="context">The context for the build view, which provides access to the messanger for receiving build messages.</param>
        public BuildView(Context context)
        {
            _context = context;
            BuildOutputTreeView = new TreeGridView
            {
                ShowHeader = true,
                AllowMultipleSelection = false,
                Border = BorderType.None,
            };

            // Add a column to the tree view
            BuildOutputTreeView.Columns.Add(new GridColumn
            {
                HeaderText = "Build Output",
                AutoSize = true,
                DataCell = new ImageTextCell(0, 1),
            });

            // Create a placeholder root item that hosts every log entry as a child.
            _rootItem = new TreeGridItem
            {
                Values = [Icons.FolderIcon, "Build Output"],
                Expanded = true,
            };

            BuildOutputTreeView.DataStore = _rootItem;

            var buildLogger = context.BuildLogger;
            buildLogger.OnSuccessLog += msg => AddMessage(msg, Icons.CheckIcon);
            buildLogger.OnErrorLog += msg => AddMessage(msg, Icons.FileIcon);
            buildLogger.OnInfoLog += msg => AddMessage(msg, Icons.InfoIcon);

            _context.OnBuildStarted += () =>
            {
                _rootItem.Children.Clear();
            };

            // Wrap the tree view in a titled GroupBox for clearer visual separation
            Content = new GroupBox
            {
                Text = "Build Output",
                Font = SystemFonts.Bold(),
                Padding = new Padding(Theme.PADDING),
                Content = BuildOutputTreeView,
            };
        }

        /// <summary>
        /// Adds a message to the build output tree view.
        /// </summary>
        /// <param name="message">The message to add.</param>
        /// <param name="icon">An optional icon to display alongside the message.</param>
        public void AddMessage(string message, Icon? icon = null)
        {
            _rootItem.Children.Add(new TreeGridItem
            {
                Values = [icon, message]
            });

            // Refresh data store so newly-added rows are visible.
            BuildOutputTreeView.ReloadData();
        }
    }
}
