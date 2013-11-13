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
    public class TriggerBackupRestore : BackupRestore
    {
        [Serializable]
        public class TriggerBackup
        {
            public string Name { get; set; }
            public bool isEnabled { get; set; }
            public int? Operator { get; set; }
            public StoredCMDBackup StoredCommand { get; set; }
            public int trigger_type { get; set; }
            public string trigger_script { get; set; }
            public string DeviceValueName { get; set; }
            public string Value { get; set; }
            public int NodeNumber { get; set; }
        }

        public override string Name
        {
            get { return "Triggers"; }
        }

        public override string FileName
        {
            get { return "TriggersBackup.zvs"; }
        }

        public async override Task<ExportResult> ExportAsync(string fileName)
        {
            using (zvsContext context = new zvsContext())
            {
                var existingTriggers = await context.DeviceValueTriggers
                    .Include(o => o.StoredCommand)
                    .Include(o => o.DeviceValue)
                    .ToListAsync();

                var backupTriggers = new List<TriggerBackup>();
                foreach (var o in existingTriggers)
                {
                    var trigger = new TriggerBackup();
                    trigger.Name = o.Name;
                    trigger.isEnabled = o.isEnabled;
                    trigger.DeviceValueName = o.DeviceValue.Name;
                    trigger.NodeNumber = o.DeviceValue.Device.NodeNumber;
                    trigger.StoredCommand = await StoredCMDBackup.ConvertToBackupCommand(o.StoredCommand);
                    trigger.Operator = (int?)o.Operator;
                    trigger.Value = o.Value;
                    backupTriggers.Add(trigger);
                }

                var saveResult = await SaveAsXMLToDiskAsync(backupTriggers, fileName);

                if (saveResult.HasError)
                    return new ExportResult(saveResult.Message, saveResult.HasError);

                return new ExportResult(string.Format("Exported {0} triggers to {1}", backupTriggers.Count,
                    Path.GetFileName(fileName)), false);
            }
        }

        public async override Task<RestoreSettingsResult> ImportAsync(string fileName)
        {
            var result = await ReadAsXMLFromDiskAsync<List<TriggerBackup>>(fileName);

            if (result.HasError)
                return new RestoreSettingsResult(result.Message);

            int SkippedCount = 0;
            var newTriggers = new List<DeviceValueTrigger>();

            using (zvsContext context = new zvsContext())
            {
                var existingTriggers = await context.DeviceValueTriggers.ToListAsync();

                var existingDeviceValues = await context.DeviceValues
                    .Include(o => o.Device)
                    .ToListAsync();

                foreach (var backupTrigger in result.Data)
                {
                    if (existingTriggers.Any(o => o.Name == backupTrigger.Name))
                    {
                        SkippedCount++;
                        continue;
                    }

                    var dv = existingDeviceValues.FirstOrDefault(o => o.Device.NodeNumber == backupTrigger.NodeNumber
                            && o.Name == backupTrigger.DeviceValueName);

                    if (dv != null)
                    {
                        var dvTrigger = new DeviceValueTrigger();
                        dvTrigger.Name = backupTrigger.Name;
                        dvTrigger.DeviceValue = dv;
                        dvTrigger.isEnabled = backupTrigger.isEnabled;
                        dvTrigger.Name = backupTrigger.Name;
                        dvTrigger.StoredCommand = await StoredCMDBackup.RestoreStoredCommandAsync(context, backupTrigger.StoredCommand);
                        dvTrigger.Operator = (TriggerOperator)backupTrigger.Operator;
                        dvTrigger.Value = backupTrigger.Value;
                        newTriggers.Add(dvTrigger);
                    }
                }

                context.DeviceValueTriggers.AddRange(newTriggers);

                if (newTriggers.Count > 0)
                {
                    var saveResult = await context.TrySaveChangesAsync();
                    if (saveResult.HasError)
                        return new RestoreSettingsResult(saveResult.Message);
                }
            }
            return new RestoreSettingsResult(string.Format("Imported {0} triggers, skipped {1} from {2}", newTriggers.Count, SkippedCount, Path.GetFileName(fileName)), fileName);
        }


    }


}