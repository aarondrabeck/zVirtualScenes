using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zvs.Entities;

namespace TestingCodeFirstConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDataPath);

            using (zvsContext context = new zvsContext())
            {
                Plugin p = new Plugin()
                {
                    Description = "test",
                    isEnabled = true,
                    Name = "TEST3",
                    FriendlyName = "test"
                };

                DeviceType dt = new DeviceType();
                dt.Name = "New Device Type";
                dt.Plugin = p;

                DeviceTypeCommand dtc = new DeviceTypeCommand();
                dtc.FriendlyName = "new device type command";
                dt.Commands.Add(dtc);

                Device d = new Device()
                {
                    DeviceType = dt,
                    FriendlyName = "New Device"
                };

                DeviceCommand dc = new DeviceCommand()
                {
                    FriendlyName = "new device command"
                };

                d.Commands.Add(dc);
                context.Plugins.Add(p);

                context.QueuedCommands.Add(new QueuedDeviceCommand()
                {
                    Device = d,
                    Command = dc,
                    Argument = "12"

                });

                context.QueuedCommands.Add(new QueuedDeviceTypeCommand()
                {
                    Device = d,
                    Command = dtc,
                    Argument = "54"

                });
 

               
                context.SaveChanges();


                foreach (Device dev in context.Devices)
                {
                    Console.WriteLine("{0} {1}", dev.FriendlyName, dev.DeviceType.Plugin.Name);

                    foreach (QueuedCommand q in dev.QueuedDeviceCommands)
                    {
                        Console.WriteLine("    {0} {1}", q.Argument, q.GetType().ToString());
                    }
                }
                Console.ReadKey(true);
            }
        }
        public static string AppDataPath
        {
            get
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string path = Path.Combine(appData, @"zVirtualScenes");
                if (!Directory.Exists(path))
                {
                    try { Directory.CreateDirectory(path); }
                    catch { }
                }
                return path + "\\";
            }
        }
    }
}
