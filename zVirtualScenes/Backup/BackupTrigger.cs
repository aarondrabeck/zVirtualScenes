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
        public class BackupTrigger
        {
            public string Name;
            public bool Enabled;
            public int? trigger_operator;
            public string sceneName;
            public int trigger_type;
            public string trigger_script = string.Empty;
            public string DeviceValueName;
            public string value;
            public int NodeID;
        }

        public static void ExportTriggerAsyc(string PathFileName, Action<string> Callback)
        {
            List<BackupTrigger> triggers = new List<BackupTrigger>();
            using (zvsLocalDBEntities context = new zvsLocalDBEntities())
            {
                foreach (device_value_triggers trigger in context.device_value_triggers)
                {
                    BackupTrigger triggerBackup = new BackupTrigger();
                    triggerBackup.Name = trigger.Name;
                    triggerBackup.Enabled = trigger.enabled;
                    triggerBackup.DeviceValueName = trigger.device_values.label_name;
                    triggerBackup.NodeID = trigger.device_values.device.node_id;
                    triggerBackup.sceneName = trigger.scene.friendly_name;
                    triggerBackup.trigger_operator = trigger.trigger_operator;
                    triggerBackup.trigger_type = trigger.trigger_type;
                    triggerBackup.trigger_script = trigger.trigger_script;
                    triggerBackup.value = trigger.trigger_value;
                    triggers.Add(triggerBackup);
                }
            }

            Stream stream = null;
            try
            {
                stream = File.Open(PathFileName, FileMode.Create);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<BackupTrigger>));
                xmlSerializer.Serialize(stream, triggers);
                stream.Close();
                Callback(string.Format("Exported {0} triggers to '{1}'", triggers.Count, Path.GetFileName(PathFileName)));
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

        public static void ImportTriggersAsyn(string PathFileName, Action<string> Callback)
        {
            List<BackupTrigger> triggers = new List<BackupTrigger>();
            int ImportedCount = 0;

            FileStream myFileStream = null;
            try
            {
                if (File.Exists(PathFileName))
                {
                    //Open the file written above and read values from it.       
                    XmlSerializer ScenesSerializer = new XmlSerializer(typeof(List<BackupTrigger>));
                    myFileStream = new FileStream(PathFileName, FileMode.Open);
                    triggers = (List<BackupTrigger>)ScenesSerializer.Deserialize(myFileStream);
                   

                    using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                    {
                        foreach (BackupTrigger backupTrigger in triggers)
                        {
                            scene s = context.scenes.FirstOrDefault(o => o.friendly_name == backupTrigger.sceneName);
                            device d = context.devices.FirstOrDefault(o => o.node_id == backupTrigger.NodeID);
                            if (d != null && s != null)
                            {
                                device_values dv = d.device_values.FirstOrDefault(o => o.label_name == backupTrigger.DeviceValueName);
                                if (dv != null)
                                {
                                    device_value_triggers trigger = new device_value_triggers();
                                    trigger.device_value_id = dv.id;
                                    trigger.enabled = backupTrigger.Enabled;
                                    trigger.Name = backupTrigger.Name;
                                    trigger.scene_id = s.id;
                                    trigger.trigger_operator = backupTrigger.trigger_operator;
                                    trigger.trigger_script = backupTrigger.trigger_script;
                                    trigger.trigger_type = backupTrigger.trigger_type;
                                    trigger.trigger_value = backupTrigger.value;
                                    context.device_value_triggers.Add(trigger);
                                    ImportedCount++; 
                                }
                            }
                        }
                        context.SaveChanges();
                    }
                    Callback(string.Format("Imported {0} triggers from '{1}'", ImportedCount, Path.GetFileName(PathFileName)));
                }
                else
                    Callback(string.Format("File '{0}' not found.", PathFileName));

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