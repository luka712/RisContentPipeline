using Eto.Drawing;
using Eto.Forms;

namespace RisContentPipeline.GUI.Views
{
    internal class InspectorView
    {
        private readonly Context _context;
        private readonly TreeGridItem _rootItem;

        /// <summary>
        /// Gets the panel control containing the action buttons.
        /// </summary>
        public DynamicLayout Content { get; }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="context">The context for the build view, which provides access to the messanger for receiving build messages.</param>
        public InspectorView(Form mainForm, Context context)
        {
            _context = context;

            TabControl tabs = new();
            tabs.Pages.Add(new TabPage
            {
                Text = "Converter",
                Content = new Label { Text = "Converter UI here" }
            });

            tabs.Pages.Add(new TabPage
            {
                Text = "Batch Queue",
                Content = new Label { Text = "Queue UI here" }
            });

            tabs.Pages.Add(new TabPage
            {
                Text = "Settings",
                Content = new Label { Text = "Settings UI here" }
            });

            Content = new DynamicLayout()
            {
                Padding = new Padding(Theme.PADDING),
                Width = Theme.CLIENT_WIDTH - Theme.SIDE_PANELS_WIDTH * 2 - Theme.PADDING * 2,
            };
            Content.BeginVertical();
            Content.Add(tabs, true, true);
            Content.AddRow(null);
            Content.EndVertical();
        }
    }
}