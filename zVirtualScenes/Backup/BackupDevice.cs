using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using zVirtualScenesModel;

namespace zVirtualScenes.Backup
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
            using (zvsLocalDBEntities context = new zvsLocalDBEntities())
            {
                foreach (device d in context.devices)
                {
                    devices.Add(new BackupDevice()
                    {
                        NodeID = d.node_id,
                        FreindlyName = d.friendly_name
                    });
                }
            }

            try
            {
                Stream stream = File.Open(PathFileName, FileMode.Create);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<BackupDevice>));
                xmlSerializer.Serialize(stream, devices);
                stream.Close();
                Callback(PathFileName + " saved to disk.");
            }
            catch (Exception e)
            {
                Callback("Error saving " + PathFileName + ": (" + e.Message + ")");
            }
        }

        public static void ImportDevicesAsyn(string PathFileName, Action<string> Callback)
        {
            List<BackupDevice> devices = new List<BackupDevice>();
            try
            {
                if (File.Exists(PathFileName))
                {
                    //Open the file written above and read values from it.       
                    XmlSerializer ScenesSerializer = new XmlSerializer(typeof(List<BackupDevice>));
                    FileStream myFileStream = new FileStream(PathFileName, FileMode.Open);
                    devices = (List<BackupDevice>)ScenesSerializer.Deserialize(myFileStream);
                    myFileStream.Close();

                    using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                    {
                        foreach (device d in context.devices)
                        {
                            BackupDevice dev = devices.FirstOrDefault(o => o.NodeID == d.node_id);
                            if (dev != null)
                                d.friendly_name = dev.FreindlyName;
                        }
                        context.SaveChanges();
                    }
                    Callback(PathFileName + " imported and names applied.");
                }
                else
                    Callback(PathFileName + " not found.");

            }
            catch (Exception e)
            {
                Callback("Error importing " + PathFileName + ": (" + e.Message + ")");
            }
        }
    }

    
}
