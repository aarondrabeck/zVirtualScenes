using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using zvs.Entities;
using System.Data.Entity;

namespace zvs.Processor.Backup
{
    public partial class Backup
    {
        [Serializable]
        public class DeviceBackup
        {
            public string Name;
            public int NodeNumber;
        }

        public static void ExportDevicesAsync(string PathFileName, Action<string> Callback)
        {
            List<DeviceBackup> devices = new List<DeviceBackup>();
            using (zvsContext context = new zvsContext())
            {
                foreach (Device d in context.Devices)
                {
                    devices.Add(new DeviceBackup()
                    {
                        NodeNumber = d.NodeNumber,
                        Name = d.Name
                    });
                }
            }

            Stream stream = null;
            try
            {
                stream = File.Open(PathFileName, FileMode.Create);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<DeviceBackup>));
                xmlSerializer.Serialize(stream, devices);
                Callback(string.Format("Exported {0} device names to '{1}'", devices.Count, Path.GetFileName(PathFileName)));
            }
            catch (Exception e)
            {
                Callback("Error saving " + PathFileName + ": (" + e.Message + ")");
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        public async static void ImportDevicesAsync(string PathFileName, Action<string> Callback)
        {
            List<DeviceBackup> devices = new List<DeviceBackup>();
            int ImportedCount = 0;

            FileStream myFileStream = null;
            try
            {
                //TODO: ASYNC THIS
                if (File.Exists(PathFileName))
                {
                    //TODO: ASYNC THIS
                    //Open the file written above and read values from it.       
                    XmlSerializer ScenesSerializer = new XmlSerializer(typeof(List<DeviceBackup>));
                    myFileStream = new FileStream(PathFileName, FileMode.Open);
                    devices = (List<DeviceBackup>)ScenesSerializer.Deserialize(myFileStream);
                   
                    using (zvsContext context = new zvsContext())
                    {
                        foreach (Device d in await context.Devices.ToListAsync())
                        {
                            DeviceBackup dev = devices.FirstOrDefault(o => o.NodeNumber == d.NodeNumber);
                            if (dev != null)
                            {
                                d.Name = dev.Name;
                                ImportedCount++;
                            }
                        }
                        await context.SaveChangesAsync();
                    }
                    Callback(string.Format("Imported {0} device names from '{1}'", ImportedCount, Path.GetFileName(PathFileName)));
                }
                else
                    Callback(PathFileName + " not found.");

            }
            catch (Exception e)
            {
                Callback("Error importing " + PathFileName + ": (" + e.Message + ")");
            }
            finally
            {

                if (myFileStream != null)
                    myFileStream.Close();
            }
        }
    }


}
