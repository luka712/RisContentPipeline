using Eto.Drawing;
using Eto.Forms;
using RisContentPipeline.GUI.Data;

namespace RisContentPipeline.GUI.Modals
{
    /// <summary>
    /// The dialog that allows users to select a script from the list of available scripts in the context
    /// to be used when building assets.
    /// </summary>
    internal class SelectScriptsModal : Dialog<Script[]?>
    {
        private readonly Context _context;
        private readonly ListBox _listBox = new();

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="context">The <see cref="Context"/>.</param>
        internal SelectScriptsModal(Context context)
        {
            _context = context;
            Title = "Select Scripts";
        
            _listBox.Size = new Size(400, 300);
            _listBox.DataStore = _context.InternalScripts;

            var okButton = new Button { Text = "OK" };
            okButton.Click += (sender, e) =>
            {
                if (_listBox.SelectedValue != null)
                {
                    if(_listBox.SelectedValue is Script selectedScript)
                    {
                        Close([ selectedScript ]);
                        return;
                    };

                    Close(_listBox.SelectedValue as Script[]);
                }
            };

            var cancelButton = new Button { Text = "Cancel" };
            cancelButton.Click += (sender, e) => Close(null);

            Content = new StackLayout
            {
                Padding = 10,
                Spacing = 5,
                Items =
                {
                    _listBox,
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Spacing = 5,
                        Items = { okButton, cancelButton }
                    }
                }
            };
        }
    }
}
