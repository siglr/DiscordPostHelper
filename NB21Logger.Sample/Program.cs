using System;
using System.Linq;
using System.Windows.Forms;

namespace NB21Logger.Sample;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        bool launchedViaStartup = args.Any(arg => arg.Equals("startup", StringComparison.OrdinalIgnoreCase));
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        using var form = new MainForm(launchedViaStartup);
        Application.Run(form);
    }
}
