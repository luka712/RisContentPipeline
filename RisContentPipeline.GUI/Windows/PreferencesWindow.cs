using Eto.Drawing;
using Eto.Forms;
using RisContentPipeline.GUI.Settings;

namespace RisContentPipeline.GUI.Windows
{
    /// <summary>
    /// The preferences dialog. Lets the user edit build settings, general settings,
    /// and appearance settings. Settings are committed to the <see cref="Context"/>
    /// when the user clicks <c>OK</c>; <c>Cancel</c> closes without saving.
    /// </summary>
    internal class PreferencesWindow : Dialog
    {
        private readonly Context _context;

        // Build settings
        private TextBox _buildDirectoryTextBox = null!;

        // KTX2 settings
        private DropDown _encodeTargetDropDown = null!;
        private CheckBox _useUastcCheckBox = null!;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="context">The <see cref="Context"/>.</param>
        internal PreferencesWindow(Context context)
        {
            _context = context;

            Title = "Preferences";
            Resizable = true;
            ClientSize = new Size(640, 420);
            MinimumSize = new Size(480, 320);
            Padding = new Padding(Theme.PADDING * 2);

            // ---- Tabs ---------------------------------------------------------
            var buildSettingsTab = AddBuildSettings();
            var generalTab = CreateGeneralTab();
            var appearanceTab = CreateAppearanceTab();

            var tabs = new TabControl
            {
                Pages = { buildSettingsTab, generalTab, appearanceTab }
            };

            // ---- Buttons ------------------------------------------------------
            var okButton = new Button { Text = "OK", Width = 90 };
            okButton.Click += (sender, e) => ApplySettingsAndClose();

            var cancelButton = new Button { Text = "Cancel", Width = 90 };
            cancelButton.Click += (sender, e) => Close();

            DefaultButton = okButton;
            AbortButton = cancelButton;

            var buttonRow = new StackLayout
            {
                Orientation = Orientation.Horizontal,
                Spacing = Theme.CONTROL_SPACING,
                HorizontalContentAlignment = HorizontalAlignment.Right,
                Items =
                {
                    new StackLayoutItem(null, expand: true),
                    okButton,
                    cancelButton,
                }
            };

            // ---- Root layout: tabs fill, button row pinned to bottom ----------
            var root = new TableLayout
            {
                Spacing = new Size(0, Theme.CONTROL_SPACING),
                Rows =
                {
                    new TableRow(tabs) { ScaleHeight = true },
                    new TableRow(buttonRow),
                }
            };

            Content = root;
        }

        private void ApplySettingsAndClose()
        {
            _context.BuildDirectory = _buildDirectoryTextBox.Text ?? _context.BuildDirectory;
            _context.Ktx2GlobalSettings = new()
            {
                UseUastc = _useUastcCheckBox.Checked == true,
                EncodeTarget = (Ktx2EncodingTarget)_encodeTargetDropDown.SelectedIndex,
            };

            Close();
        }

        private TabPage AddBuildSettings()
        {
            // Build Directory
            _buildDirectoryTextBox = new TextBox
            {
                Text = _context.BuildDirectory,
                PlaceholderText = "Build directory ...",
            };
            var browseBuildDirButton = new Button { Text = "Browse..." };
            browseBuildDirButton.Click += (_, _) =>
            {
                using var dialog = new SelectFolderDialog { Title = "Select Build Directory" };
                if (dialog.ShowDialog(this) == DialogResult.Ok)
                {
                    _buildDirectoryTextBox.Text = dialog.Directory;
                }
            };

            // KTX Settings
            _encodeTargetDropDown = new DropDown
            {
                DataStore = [ "Basis ETC1S", "Basis UASTC"],
                SelectedIndex = (int)_context.Ktx2GlobalSettings.EncodeTarget,
                ToolTip = "Select the target encoding format for KTX2 textures. " +
                          "Basis will encode textures to Basis format during the build process, " +
                          "while NoEncoding will process textures as-is.",
            };
            _encodeTargetDropDown.SelectedIndexChanged += (sender, e) =>
            {
                var selectedTarget = (Ktx2EncodingTarget)_encodeTargetDropDown.SelectedIndex;
                _useUastcCheckBox.Enabled = selectedTarget == Ktx2EncodingTarget.Basis;
            };

            _useUastcCheckBox = new CheckBox
            {
                Checked = _context.Ktx2GlobalSettings.UseUastc,
                ToolTip = "Check to use UASTC base, uncheck to use ETC1S base.",
                Enabled = _context.Ktx2GlobalSettings.EncodeTarget == Ktx2EncodingTarget.Basis,
            };

            var ktx2GroupBox = new GroupBox { Text = "KTX2 Settings" };
            var ktx2Layout = new TableLayout()
            {
                Padding = new Padding(Theme.PADDING * 2),
                Spacing = Theme.FormSpacing,
                Rows =
                {
                    new TableRow(
                        new Label { Text = "Texture Mode"},
                        new TableCell(_encodeTargetDropDown, scaleWidth: true),
                        null),
                    new TableRow(
                        new Label { Text = "Use UASTC Base" },
                        new TableCell(_useUastcCheckBox, scaleWidth: true),
                        null),
                }
            };
            ktx2GroupBox.Content = ktx2Layout;

            var layout = new DynamicLayout()
            {
                Padding = new Padding(Theme.PADDING * 2),
                Spacing = Theme.FormSpacing,
                Rows =
                {
                    new DynamicRow(new GroupBox()
                    {
                        Text = "Build Directory",
                        Content = new DynamicLayout()
                        {
                            Padding = new Padding(Theme.PADDING * 2),
                            Spacing = Theme.FormSpacing,
                            Rows = {
                                new DynamicRow(_buildDirectoryTextBox, browseBuildDirButton),
                            }
                        }
                    }),
                    new DynamicRow([ktx2GroupBox], xscale: true),
                    new DynamicRow(null, yscale: true),
                }
            };

            return new TabPage
            {
                Text = "Build Settings",
                Padding = new Padding(Theme.PADDING),
                Content = layout,
            };
        }

        private static TabPage CreateGeneralTab()
        {
            var autoSaveCheckBox = new CheckBox { Text = "Enable Auto Save" };
            var usernameTextBox = new TextBox { PlaceholderText = "Enter username" };

            var layout = new TableLayout
            {
                Padding = new Padding(Theme.PADDING * 2),
                Spacing = Theme.FormSpacing,
                Rows =
                {
                    new TableRow(autoSaveCheckBox, null),
                    new TableRow(
                        new Label { Text = "Username:" },
                        new TableCell(usernameTextBox, scaleWidth: true)),
                    new TableRow { ScaleHeight = true },
                }
            };

            return new TabPage
            {
                Text = "General",
                Padding = new Padding(Theme.PADDING),
                Content = layout,
            };
        }

        private static TabPage CreateAppearanceTab()
        {
            var themeDropDown = new DropDown
            {
                DataStore = new[] { "Light", "Dark", "System" },
                SelectedIndex = 2,
            };

            var fontSizeStepper = new NumericStepper
            {
                MinValue = 8,
                MaxValue = 24,
                Value = 12,
            };

            var layout = new TableLayout
            {
                Padding = new Padding(Theme.PADDING * 2),
                Spacing = Theme.FormSpacing,
                Rows =
                {
                    new TableRow(
                        new Label { Text = "Theme:" },
                        new TableCell(themeDropDown, scaleWidth: true)),
                    new TableRow(
                        new Label { Text = "Font Size:" },
                        new TableCell(fontSizeStepper, scaleWidth: true)),
                    new TableRow { ScaleHeight = true },
                }
            };

            return new TabPage
            {
                Text = "Appearance",
                Padding = new Padding(Theme.PADDING),
                Content = layout,
            };
        }
    }
}