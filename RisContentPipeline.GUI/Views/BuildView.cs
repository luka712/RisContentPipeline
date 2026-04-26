using Eto.Drawing;
using Eto.Forms;

namespace RisContentPipeline.GUI.Views
{
    internal class BuildView
    {
        private readonly Context _context;
        private readonly TreeGridItem _rootItem;

        /// <summary>
        /// Gets the panel control containing the action buttons.
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
                Size = new Size(250, -1)
            };

            // Add column to tree view
            BuildOutputTreeView.Columns.Add(new GridColumn
            {
                HeaderText = "Build Output",
                DataCell = new ImageTextCell(0, 1)
            });

            // Create a placeholder icon for the root folder
            _rootItem = new TreeGridItem()
            {
                Values = ["Hello"]
            };


            BuildOutputTreeView.DataStore = _rootItem;

            // Expand the root
            _rootItem.Expanded = true;

            var buildLogger = context.BuildLogger;
            buildLogger.OnSuccessLog += msg => AddMessage(msg, Icons.CheckIcon);
            buildLogger.OnErrorLog += msg => AddMessage(msg);

            _context.OnBuildStarted += () =>
            {
                _rootItem.Children.Clear();
                AddMessage("Build started...");
            };
        }

        /// <summary>
        /// Adds a message to the build output tree view.
        /// This can be used to display build progress, errors, or other relevant information during the asset building process.
        /// </summary>
        /// <param name="message">The message to add.</param>
        /// <param name="icon">An optional icon to display alongside the message.</param>
        public void AddMessage(string message, Icon? icon = null)
        {
            _rootItem.Children.Add(new TreeGridItem
            {
                Values = [icon, message]
            });

            BuildOutputTreeView.DataStore = _rootItem;
        }

    }
}
