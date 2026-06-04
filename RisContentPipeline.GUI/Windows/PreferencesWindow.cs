using Eto.Drawing;
using Eto.Forms;
using RisContentPipeline.GUI.Controls;
using RisContentPipeline.GUI.Settings;

namespace RisContentPipeline.GUI.Windows
{
    /// <summary>
    /// The preferences' dialog. Lets the user edit build settings, general settings,
    /// and appearance settings. Settings are committed to the <see cref="Context"/>
    /// when the user clicks <c>OK</c>; <c>Cancel</c> closes without saving.
    /// </summary>
    internal class PreferencesWindow : Dialog
    {
        private readonly Context _context;

        // Build settings
        private TextBox _buildDirectoryTextBox = null!;

        // KTX2 settings
        private TextureModeDropdown _encodeTargetDropDown = null!;
        private CheckBox _generateMipmapsCheckBox = null!;
        private EncodingQualityDropdown _encodingQualityDropdown = null!;

        private Ktx2Settings _ktx2SettingsPreferences = new();

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
            
            _ktx2SettingsPreferences.EncodeTarget = _context.Preferences.Ktx2GlobalSettings.EncodeTarget;
            _ktx2SettingsPreferences.GenerateMipmaps = _context.Preferences.Ktx2GlobalSettings.GenerateMipmaps;
            _ktx2SettingsPreferences.QualityLevel = _context.Preferences.Ktx2GlobalSettings.QualityLevel;

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
            
            var applyButton = new Button { Text = "Apply", Width = 90 };
            applyButton.Click += (sender, e) => ApplySettings();

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
                    applyButton,
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

        private void ApplySettings()
        {
            _context.BuildDirectory = _buildDirectoryTextBox.Text ?? _context.BuildDirectory;
            _context.Preferences.Ktx2GlobalSettings.EncodeTarget = _ktx2SettingsPreferences.EncodeTarget;
            _context.Preferences.Ktx2GlobalSettings.GenerateMipmaps = _ktx2SettingsPreferences.GenerateMipmaps;
            _context.Preferences.Ktx2GlobalSettings.QualityLevel = _ktx2SettingsPreferences.QualityLevel;
            _ = _context.SavePreferencesAsync();
        }
        
        private void ApplySettingsAndClose()
        {
            ApplySettings();
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
            _encodeTargetDropDown = new(_context.Preferences.Ktx2GlobalSettings.EncodeTarget);
            _encodeTargetDropDown.SelectedIndexChanged += (sender, e) =>
            {
                var selectedTarget = (Ktx2EncodingTarget) _encodeTargetDropDown.SelectedIndex;
                _ktx2SettingsPreferences.EncodeTarget = selectedTarget;
            };

            _encodingQualityDropdown =
                new EncodingQualityDropdown(_ktx2SettingsPreferences.QualityLevel);
            _encodingQualityDropdown.EncodingQualityLevelChanged += (_, qualityLevel) =>
                _ktx2SettingsPreferences.QualityLevel = qualityLevel;
            
            _generateMipmapsCheckBox = new GenerateMipmapsCheckbox(_ktx2SettingsPreferences.GenerateMipmaps);
            _generateMipmapsCheckBox.CheckedChanged += (sender, e) =>
                _ktx2SettingsPreferences.GenerateMipmaps = _generateMipmapsCheckBox.Checked == true;

            var ktx2GroupBox = new GroupBox { Text = "KTX2 Settings" };
            var ktx2Layout = new TableLayout()
            {
                Padding = new Padding(Theme.PADDING * 2),
                Spacing = Theme.FormSpacing,
                Rows =
                {
                    new TableRow(
                        new TextureModeLabel(),
                        new TableCell(_encodeTargetDropDown, scaleWidth: true),
                        null),
                    new TableRow(
                        new EncodingQualityLabel(),
                        new TableCell(_encodingQualityDropdown, scaleWidth: true),
                        null),
                    new TableRow(
                        new Label(),
                        new TableCell(_generateMipmapsCheckBox, scaleWidth: true),
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