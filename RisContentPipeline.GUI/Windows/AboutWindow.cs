using Eto.Drawing;
using Eto.Forms;

namespace RisContentPipeline.GUI.Windows;

/// <summary>
/// The About dialog window that displays application information,
/// version, and credits.
/// </summary>
public class AboutWindow : Dialog
{
    public AboutWindow()
    {
        Title = "About";
        Resizable = false;
        Icon = Constants.MAIN_ICON;
        ClientSize = new Size(480, 360);
        Padding = new Padding(Theme.PADDING * 2);

        // Application icon
        ImageView? appIcon = null;
        try
        {
            appIcon = new ImageView
            {
                Image = new Bitmap(Constants.PNG_ICON_FILE_PATH),
                Size = new Size(64, 64),
            };
        }
        catch
        {
            // If icon fails to load, continue without it
        }

        // Application name
        var appNameLabel = new Label
        {
            Text = Constants.APP_NAME,
            Font = new Font(SystemFont.Bold, 18),
            TextAlignment = TextAlignment.Center,
        };

        // Version information
        var versionLabel = new Label
        {
            Text = "Version 0.2.0",
            Font = new Font(SystemFont.Default, 11),
            TextAlignment = TextAlignment.Center,
        };

        // Description
        var descriptionLabel = new Label
        {
            Text = "A content pipeline tool for processing and converting\ngame assets, with support for texture compression\nand Python scripting.",
            TextAlignment = TextAlignment.Center,
            Wrap = WrapMode.Word,
        };

        // Copyright
        var copyrightLabel = new Label
        {
            Text = $"© {DateTime.Now.Year} luka712",
            TextAlignment = TextAlignment.Center,
        };

        // GitHub link
        var githubLinkButton = new LinkButton
        {
            Text = "View on GitHub",
        };
        githubLinkButton.Click += (sender, e) =>
        {
            try
            {
                Application.Instance.Open("https://github.com/luka712/RisContentPipeline");
            }
            catch
            {
                // Silently fail if unable to open browser
            }
        };

        // Email contact
        var emailLinkButton = new LinkButton
        {
            Text = "erkapic.luka.dev@gmail.com",
        };
        emailLinkButton.Click += (sender, e) =>
        {
            try
            {
                Application.Instance.Open("mailto:erkapic.luka.dev@gmail.com");
            }
            catch
            {
                // Silently fail if unable to open email client
            }
        };

        // Close button
        var closeButton = new Button
        {
            Text = "Close",
            Width = 100,
        };
        closeButton.Click += (sender, e) => Close();

        DefaultButton = closeButton;
        AbortButton = closeButton;

        // Layout
        var contentLayout = new StackLayout
        {
            Orientation = Orientation.Vertical,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Spacing = Theme.CONTROL_SPACING,
            Items =
            {
                new StackLayoutItem(null, expand: true),
            }
        };

        // Add icon if available
        if (appIcon != null)
        {
            contentLayout.Items.Add(appIcon);
        }

        contentLayout.Items.Add(appNameLabel);
        contentLayout.Items.Add(versionLabel);
        contentLayout.Items.Add(new StackLayoutItem(null) { Expand = false });
        contentLayout.Items.Add(descriptionLabel);
        contentLayout.Items.Add(new StackLayoutItem(null) { Expand = false });
        contentLayout.Items.Add(copyrightLabel);
        contentLayout.Items.Add(githubLinkButton);
        contentLayout.Items.Add(emailLinkButton);
        contentLayout.Items.Add(new StackLayoutItem(null, expand: true));

        var buttonLayout = new StackLayout
        {
            Orientation = Orientation.Horizontal,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Items = { closeButton }
        };

        var rootLayout = new TableLayout
        {
            Spacing = new Size(0, Theme.CONTROL_SPACING),
            Rows =
            {
                new TableRow(contentLayout) { ScaleHeight = true },
                new TableRow(buttonLayout),
            }
        };

        Content = rootLayout;
    }
}
