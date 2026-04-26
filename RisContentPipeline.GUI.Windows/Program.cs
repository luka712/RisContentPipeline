using Eto.Forms;
using RisContentPipeline.GUI;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        new Application(Eto.Platform.Detect).Run(new MainForm());
    }
}