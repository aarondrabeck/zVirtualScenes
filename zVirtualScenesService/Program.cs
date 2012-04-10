using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Configuration.Install;
using System.Reflection;
using System.Security.Principal;
using System.Diagnostics;
using System.ComponentModel;

namespace zVirtualScenesService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (System.Environment.UserInteractive == false)
            {
                // run as service
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
                        { 
                            new CoreService() 
                        };
                ServiceBase.Run(ServicesToRun);
                return;
            }
            String command = "";
            if (args.Count() != 0)
            {
                command = args[0].ToUpper();
            }
            if (IsCurrentlyRunningAsAdmin() == false)
            {
                StartAsAdmin(command);
                return;
            }
            try
            {
                switch (command)
                {
                    case "/REGISTER":                        
                        Console.WriteLine("Registering service");
                        ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                        Console.WriteLine("Service registered .. press any key");
                        Console.ReadKey();
                        break;
                    case "/UNREGISTER":
                        Console.WriteLine("Unregister service");
                        ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                        Console.WriteLine("Service unregistered .. press any key");
                        Console.ReadKey();
                        break;
                    case "/CONSOLE":
                        CoreService core = new CoreService();
                        core.Start();
                        Console.ReadLine();
                        core.Stop();
                        break;
                    default:
                        Console.WriteLine("Commands:");
                        Console.WriteLine("/register: Register service");
                        Console.WriteLine("/unregister: Unregister service");
                        Console.WriteLine("/console: Run in console mode");
                        Console.WriteLine("");
                        Console.WriteLine("Press any key to exit");
                        Console.ReadLine();
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
        }

        private static void StartAsAdmin(string parameter)
        {
            var processStartInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().Location);
            processStartInfo.Arguments = parameter;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                processStartInfo.Verb = "runas";
            }
            try
            {
                Process.Start(processStartInfo);
            }
            catch (Win32Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static bool IsCurrentlyRunningAsAdmin()
        {
            bool isAdmin = false;
            WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();
            if (currentIdentity != null)
            {
                WindowsPrincipal pricipal = new WindowsPrincipal(currentIdentity);
                isAdmin = pricipal.IsInRole(WindowsBuiltInRole.Administrator);
                pricipal = null;
            }
            return isAdmin;
        }
    }
}
