using Eto.Drawing;
using Eto.Forms;
using RisContentPipeline.GUI.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace RisContentPipeline.GUI.Windows
{
    internal class PreferencesWindow : Dialog
    {
        private readonly Context _context;

        private TextBox _buildDirectoryTextBox;

        // Ktx2 settings
        private DropDown _encodeTargetDropDown;
        private CheckBox _useUastcCheckBox;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="context">The <see cref="Context"/>.</param>
        internal PreferencesWindow(Context context)
        {
            _context = context;
            Title = "Preferences";

            Resizable = true;
            ClientSize = new Size(800, 600);

            var buildSettingsTab = AddBuildSettings();  

            // --- General Tab ---
            var autoSaveCheckBox = new CheckBox { Text = "Enable Auto Save" };
            var usernameTextBox = new TextBox { PlaceholderText = "Enter username" };

            var generalLayout = new DynamicLayout { Padding = 10, Spacing = new Size(5, 5) };
            generalLayout.AddRow(autoSaveCheckBox);
            generalLayout.AddRow(new Label { Text = "Username:" }, usernameTextBox);

            var generalTab = new TabPage
            {
                Text = "General",
                Content = generalLayout
            };

            // --- Appearance Tab ---
            var themeDropDown = new DropDown
            {
                DataStore = new string[] { "Light", "Dark", "System" },
                SelectedIndex = 2
            };

            var fontSizeStepper = new NumericStepper
            {
                MinValue = 8,
                MaxValue = 24,
                Value = 12
            };

            var appearanceLayout = new DynamicLayout { Padding = 10, Spacing = new Size(5, 5) };
            appearanceLayout.AddRow(new Label { Text = "Theme:" }, themeDropDown);
            appearanceLayout.AddRow(new Label { Text = "Font Size:" }, fontSizeStepper);

            var appearanceTab = new TabPage
            {
                Text = "Appearance",
                Content = appearanceLayout
            };

            // --- Tab Control ---
            var tabs = new TabControl
            {
                Pages = { buildSettingsTab, generalTab, appearanceTab }
            };

            // --- Buttons ---
            var okButton = new Button { Text = "OK" };
            okButton.Click += (sender, e) => ApplySettingsAndClose();

            var cancelButton = new Button { Text = "Cancel" };
            cancelButton.Click += (sender, e) => Close();

            DefaultButton = okButton;
            AbortButton = cancelButton;

            // --- Main Layout ---
            Content = new DynamicLayout
            {
                Padding = 10,
                Spacing = new Size(5, 5),
                Rows =
            {
                tabs,
                new StackLayout
                {
                    Orientation = Orientation.Horizontal,
                    VerticalContentAlignment = VerticalAlignment.Bottom,
                    HorizontalContentAlignment = HorizontalAlignment.Right,
                    Items = { okButton, cancelButton }
                }
            }
            };
        }

        private void ApplySettingsAndClose()
        {
            // Here you would apply the settings from the UI to the context or configuration
            _context.BuildDirectory = _buildDirectoryTextBox.Text;
            _context.Ktx2Settings.UseUastc = _useUastcCheckBox.Checked == true;
            _context.Ktx2Settings.EncodeTarget = (Ktx2EncodingTarget)_encodeTargetDropDown.SelectedIndex;
            Close();
        }   

        private TabPage AddBuildSettings()
        {
            var generalLayout = new DynamicLayout { Spacing = new Size(5, 5) };

            // Build Directory
            _buildDirectoryTextBox = new TextBox { Text = _context.BuildDirectory, PlaceholderText = "Build directory ..." };
            generalLayout.AddRow([new Label { Text = "Build Directory" }, _buildDirectoryTextBox]);

            // KTX Settings
            _encodeTargetDropDown = new DropDown 
            { 
                AllowDrop= true,
                DataStore = Enum.GetNames(typeof(Ktx2EncodingTarget)),
                SelectedIndex = (int)_context.Ktx2Settings.EncodeTarget,
                ToolTip = "Select the target encoding format for KTX2 textures. Basis will encode textures to Basis format during the build process, while NoEncoding will process textures as-is.",
            };
            generalLayout.AddRow(new Label { Text = "KTX2 Encoding Target" }, _encodeTargetDropDown);
            _encodeTargetDropDown.SelectedIndexChanged += (sender, e) =>
            {
                var selectedTarget = (Ktx2EncodingTarget)_encodeTargetDropDown.SelectedIndex;
                _useUastcCheckBox.Enabled = selectedTarget == Ktx2EncodingTarget.Basis;
            };

            _useUastcCheckBox = new CheckBox 
            { 
                Checked = _context.Ktx2Settings.UseUastc,
                ToolTip = "Check to use UASTC base, uncheck to use ETC1S base."
            };
            generalLayout.AddRow([new Label { Text = "Use UASTC Base" }, _useUastcCheckBox]);

            return new TabPage
            {
                Text = "Build Settings",
                Content = generalLayout,
            };
        }
    }
}
