using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace NB21_logger
{
    internal static class Program
    {
        public static bool LaunchedViaStartup { get; private set; }

        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (Process.GetProcessesByName("NB21_Logger").Length > 1)
            {
                MessageBox.Show("NB21 Logger is already running");
                return;
            }

            LaunchedViaStartup = args != null && args.Any(arg => arg.Equals("startup", StringComparison.CurrentCultureIgnoreCase));
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            Console.WriteLine($"NB21 Logger program startup (with LaunchedViaStartup = {LaunchedViaStartup})");

            Application.Run(new NB21LoggerHostForm(LaunchedViaStartup));
        }
    }
}
