using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using zvs.Entities;


namespace zvs.Processor.Backup
{
    public partial class Backup
    {
        [Serializable]
        public class BackupDevice
        {
            public string FreindlyName;
            public int NodeID;
        }

        public static void ExportDevicesAsyc(string PathFileName, Action<string> Callback)
        {
            List<BackupDevice> devices = new List<BackupDevice>();
            using (zvsContext context = new zvsContext())
            {
                foreach (Device d in context.Devices)
                {
                    devices.Add(new BackupDevice()
                    {
                        NodeID = d.NodeNumber,
                        FreindlyName = d.Name
                    });
                }
            }

            Stream stream = null;
            try
            {
                stream = File.Open(PathFileName, FileMode.Create);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<BackupDevice>));
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

        public static void ImportDevicesAsyn(string PathFileName, Action<string> Callback)
        {
            List<BackupDevice> devices = new List<BackupDevice>();
            int ImportedCount = 0;

            FileStream myFileStream = null;
            try
            {
                if (File.Exists(PathFileName))
                {
                    //Open the file written above and read values from it.       
                    XmlSerializer ScenesSerializer = new XmlSerializer(typeof(List<BackupDevice>));
                    myFileStream = new FileStream(PathFileName, FileMode.Open);
                    devices = (List<BackupDevice>)ScenesSerializer.Deserialize(myFileStream);
                   
                    using (zvsContext context = new zvsContext())
                    {
                        foreach (Device d in context.Devices)
                        {
                            BackupDevice dev = devices.FirstOrDefault(o => o.NodeID == d.NodeNumber);
                            if (dev != null)
                            {
                                d.Name = dev.FreindlyName;
                                ImportedCount++;
                            }
                        }
                        context.SaveChanges();
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
