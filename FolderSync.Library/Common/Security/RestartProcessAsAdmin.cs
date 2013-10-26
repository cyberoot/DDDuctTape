using System;
using System.Security.Principal;
using System.Diagnostics;
using System.Reflection;

namespace FolderSync.Library.Common.Security
{
    public class RestartProcessAsAdmin
    {
        public static bool RestartAsAdmin(string arguments)
        {
            if (IsAdministrator == false && arguments != "1")
            {               
                Process oProcess = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.FileName = Assembly.GetEntryAssembly().GetName().Name;
                startInfo.Verb = "runas";
                startInfo.Arguments = "\"1\"";
                Process p = Process.Start(startInfo);
                return true;
            }
            return false;
        }

        public static bool IsAdministrator
        {
            get
            {
                WindowsIdentity wi = WindowsIdentity.GetCurrent();
                WindowsPrincipal wp = new WindowsPrincipal(wi);

                return wp.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

    }
}
/* In Windows Form Application Program.cs write code as like this :
 * The argument is the argument of the application, if it is 1, it means that the application 
 * is running before previous instance is restarted
            
            string argument = string.Empty;
            if (args.Length >= 1) argument = args[0];
            if (RestartProcessAsAdmin.RestartAsAdmin(argument) == false)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain());
            }
*/