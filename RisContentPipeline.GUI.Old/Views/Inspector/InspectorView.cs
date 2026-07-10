using Eto.Drawing;
using Eto.Forms;

namespace RisContentPipeline.GUI.Views.Inspector
{
    /// <summary>
    /// Hosts the central inspector tabs (Converter, Batch Queue, Settings).
    /// </summary>
    internal class InspectorView
    {
        private readonly Context _context;
        private readonly BuildView _buildView; 

        /// <summary>
        /// The titled panel that hosts the inspector tab control.
        /// </summary>
        public Control Content { get; }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="mainForm">The owning <see cref="Form"/>.</param>
        /// <param name="context">The application <see cref="Context"/>.</param>
        public InspectorView(Form mainForm, Context context)
        {
            _context = context;

            var tabs = new TabControl();
            tabs.Pages.Add(new TabPage
            {
                Text = "Build",
                Padding = new Padding(Theme.PADDING),
                Content = new BuildView(context).Content
            });
            // tabs.Pages.Add(new TabPage
            // {
            //     Text = "Batch Queue",
            //     Padding = new Padding(Theme.PADDING),
            //     Content = CreatePlaceholder("Queue UI here"),
            // });
            // tabs.Pages.Add(new TabPage
            // {
            //     Text = "Settings",
            //     Padding = new Padding(Theme.PADDING),
            //     Content = CreatePlaceholder("Settings UI here"),
            // });

            Content = new GroupBox
            {
                Text = "Inspector",
                Font = SystemFonts.Bold(),
                Padding = new Padding(Theme.PADDING),
                Content = tabs,
            };
        }

        private static Control CreatePlaceholder(string text) => new Label
        {
            Text = text,
            TextColor = SystemColors.DisabledText,
            VerticalAlignment = VerticalAlignment.Center,
            TextAlignment = TextAlignment.Center,
        };
    }
}
