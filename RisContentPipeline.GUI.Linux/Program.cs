using Eto.Forms;
using RisContentPipeline.GUI;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        var app = new Application(Eto.Platform.Detect);
        app.BadgeLabel = Constants.APP_NAME;
        app.Run(new MainForm());
    }
}