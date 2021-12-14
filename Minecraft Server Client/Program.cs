using System;
using System.IO;
using System.Windows.Forms;

namespace MSC
{
    static class Program
    {
        private static readonly string AppDir = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf(char.Parse(@"\")));

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            File.WriteAllText($@"{AppDir}\eula.txt", "eula=true");
            if (!File.Exists($@"{AppDir}\runtime\bin\java.exe")) { Application.Run(new Setup()); }
            else if (!File.Exists($@"{AppDir}\server.jar")) { Application.Run(new Setup()); }
            else { Application.Run(new MainUI()); }
        }
    }
}
