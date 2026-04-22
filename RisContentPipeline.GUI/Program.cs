using Eto.Forms;
using RisContentPipeline;

namespace RisContentPipeline.GUI;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        new Application(Eto.Platform.Detect).Run(new MainForm());
    }
}

internal sealed class MainForm : Form
{
    public MainForm()
    {
        Title = "RisContentPipeline";
        ClientSize = new Eto.Drawing.Size(680, 180);
        Resizable = false;

        var inputBox = new TextBox { PlaceholderText = "Input image file (.png/.jpg/.bmp/.gif/.tga...)" };
        var outputBox = new TextBox { PlaceholderText = "Output .ktx2 file" };
        var statusLabel = new Label();

        var browseInputButton = new Button { Text = "Browse input" };
        browseInputButton.Click += (_, _) =>
        {
            using var dialog = new OpenFileDialog();
            dialog.Filters.Add(new FileFilter("Images", ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tga", ".webp"));
            if (dialog.ShowDialog(this) == DialogResult.Ok)
            {
                inputBox.Text = dialog.FileName;
            }
        };

        var browseOutputButton = new Button { Text = "Browse output" };
        browseOutputButton.Click += (_, _) =>
        {
            using var dialog = new SaveFileDialog();
            dialog.Filters.Add(new FileFilter("KTX2", ".ktx2"));
            dialog.FileName = "output.ktx2";
            if (dialog.ShowDialog(this) == DialogResult.Ok)
            {
                outputBox.Text = dialog.FileName;
            }
        };

        var convertButton = new Button { Text = "Convert to KTX2" };
        convertButton.Click += (_, _) =>
        {
            try
            {
                Ktx2Converter.ConvertFileToKtx2(inputBox.Text ?? string.Empty, outputBox.Text ?? string.Empty);
                statusLabel.Text = "Conversion complete.";
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Conversion failed: {ex.Message}";
            }
        };

        Content = new TableLayout
        {
            Padding = 12,
            Spacing = new Eto.Drawing.Size(8, 8),
            Rows =
            {
                new TableRow(inputBox, browseInputButton),
                new TableRow(outputBox, browseOutputButton),
                new TableRow(convertButton),
                new TableRow(statusLabel)
            }
        };
    }
}
