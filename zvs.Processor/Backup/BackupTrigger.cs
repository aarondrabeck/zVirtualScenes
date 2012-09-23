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
            using (zvsContext context = new zvsContext())
            {
                foreach (DeviceValueTrigger trigger in context.DeviceValueTriggers)
                {
                    BackupTrigger triggerBackup = new BackupTrigger();
                    triggerBackup.Name = trigger.Name;
                    triggerBackup.Enabled = trigger.isEnabled;
                    triggerBackup.DeviceValueName = trigger.DeviceValue.Name;
                    triggerBackup.NodeID = trigger.DeviceValue.Device.NodeNumber;
                    //TODO: Backup Script
                    //triggerBackup.sceneName = trigger..Name;
                    triggerBackup.trigger_operator = (int?)trigger.Operator;
                    triggerBackup.value = trigger.Value;
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
                   

                    using (zvsContext context = new zvsContext())
                    {
                        foreach (BackupTrigger backupTrigger in triggers)
                        {
                            Scene s = context.Scenes.FirstOrDefault(o => o.Name == backupTrigger.sceneName);
                            Device d = context.Devices.FirstOrDefault(o => o.NodeNumber == backupTrigger.NodeID);
                            if (d != null && s != null)
                            {
                                DeviceValue dv = d.Values.FirstOrDefault(o => o.Name == backupTrigger.DeviceValueName);
                                if (dv != null)
                                {
                                    DeviceValueTrigger trigger = new DeviceValueTrigger();
                                    trigger.DeviceValue = dv;
                                    trigger.isEnabled = backupTrigger.Enabled;
                                    trigger.Name = backupTrigger.Name;
                                    //TODO: Fix
                                   // trigger.Scene = s;
                                    trigger.Operator = (TriggerOperator)backupTrigger.trigger_operator;
                                    trigger.Value = backupTrigger.value;
                                    context.DeviceValueTriggers.Add(trigger);
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