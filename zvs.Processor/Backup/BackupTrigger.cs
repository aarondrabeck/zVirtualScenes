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
        public class TriggerBackup
        {
            public string Name;
            public bool isEnabled;
            public int? Operator;
            public StoredCMDBackup StoredCommand;
            public int trigger_type;
            public string trigger_script = string.Empty;
            public string DeviceValueName;
            public string Value;
            public int NodeNumber;
        }

        public static void ExportTriggerAsync(string PathFileName, Action<string> Callback)
        {
            List<TriggerBackup> triggers = new List<TriggerBackup>();
            using (zvsContext context = new zvsContext())
            {
                foreach (DeviceValueTrigger trigger in context.DeviceValueTriggers)
                {
                    TriggerBackup triggerBackup = new TriggerBackup();
                    triggerBackup.Name = trigger.Name;
                    triggerBackup.isEnabled = trigger.isEnabled;
                    triggerBackup.DeviceValueName = trigger.DeviceValue.Name;
                    triggerBackup.NodeNumber = trigger.DeviceValue.Device.NodeNumber;
                    triggerBackup.StoredCommand = (StoredCMDBackup)trigger.StoredCommand;
                    triggerBackup.Operator = (int?)trigger.Operator;
                    triggerBackup.Value = trigger.Value;
                    triggers.Add(triggerBackup);
                }
            }

            Stream stream = null;
            try
            {
                stream = File.Open(PathFileName, FileMode.Create);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<TriggerBackup>));
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

        public static void ImportTriggersAsync(string PathFileName, Action<string> Callback)
        {
            List<TriggerBackup> triggers = new List<TriggerBackup>();
            int ImportedCount = 0;

            FileStream myFileStream = null;
            try
            {
                if (File.Exists(PathFileName))
                {
                    //Open the file written above and read values from it.       
                    XmlSerializer ScenesSerializer = new XmlSerializer(typeof(List<TriggerBackup>));
                    myFileStream = new FileStream(PathFileName, FileMode.Open);
                    triggers = (List<TriggerBackup>)ScenesSerializer.Deserialize(myFileStream);
                   

                    using (zvsContext context = new zvsContext())
                    {
                        foreach (TriggerBackup backupTrigger in triggers)
                        {
                            Device d = context.Devices.FirstOrDefault(o => o.NodeNumber == backupTrigger.NodeNumber);
                            if (d != null )
                            {
                                DeviceValue dv = d.Values.FirstOrDefault(o => o.Name == backupTrigger.DeviceValueName);
                                if (dv != null)
                                {
                                    DeviceValueTrigger trigger = new DeviceValueTrigger();
                                    trigger.DeviceValue = dv;
                                    trigger.isEnabled = backupTrigger.isEnabled;
                                    trigger.Name = backupTrigger.Name;
                                    trigger.StoredCommand = StoredCMDBackup.RestoreStoredCommand(context, backupTrigger.StoredCommand);
                                    trigger.Operator = (TriggerOperator)backupTrigger.Operator;
                                    trigger.Value = backupTrigger.Value;
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