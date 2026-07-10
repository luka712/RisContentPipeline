using Eto.Drawing;
using Eto.Forms;
using RisContentPipeline.GUI.Data;

namespace RisContentPipeline.GUI.Modals
{
    /// <summary>
    /// The dialog that allows users to select one or more scripts from the
    /// list of available scripts in the context to be used when building assets.
    /// </summary>
    internal class SelectScriptsModal : Dialog<Script[]?>
    {
        private readonly Context _context;
        private readonly ListBox _listBox = new();
        private readonly Button _okButton;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="context">The <see cref="Context"/>.</param>
        internal SelectScriptsModal(Context context)
        {
            _context = context;
            Title = "Select Build Script";
            Resizable = true;
            ClientSize = new Size(460, 360);
            MinimumSize = new Size(360, 280);
            Padding = new Padding(Theme.PADDING * 2);

            _listBox.DataStore = _context.InternalScripts;
            _listBox.MouseDoubleClick += (sender, e) => SelectAndClose();
            _listBox.SelectedIndexChanged += (sender, e) => UpdateOkEnabled();

            var header = new Label
            {
                Text = "Choose a build script to add to the build process:",
                Wrap = WrapMode.Word,
            };

            _okButton = new Button { Text = "Add", Width = 90, Enabled = false };
            _okButton.Click += (sender, e) => SelectAndClose();

            var cancelButton = new Button { Text = "Cancel", Width = 90 };
            cancelButton.Click += (sender, e) => Close(null);

            DefaultButton = _okButton;
            AbortButton = cancelButton;

            var buttonRow = new StackLayout
            {
                Orientation = Orientation.Horizontal,
                Spacing = Theme.CONTROL_SPACING,
                Items =
                {
                    new StackLayoutItem(null, expand: true),
                    _okButton,
                    cancelButton,
                }
            };

            Content = new TableLayout
            {
                Spacing = new Size(0, Theme.CONTROL_SPACING),
                Rows =
                {
                    new TableRow(header),
                    new TableRow(_listBox) { ScaleHeight = true },
                    new TableRow(buttonRow),
                }
            };

            UpdateOkEnabled();
        }

        private void UpdateOkEnabled()
        {
            _okButton.Enabled = _listBox.SelectedValue is Script;
        }

        private void SelectAndClose()
        {
            if (_listBox.SelectedValue is Script selectedScript)
            {
                Close([selectedScript]);
                return;
            }

            if (_listBox.SelectedValue is Script[] scripts)
            {
                Close(scripts);
            }
        }
    }
}
