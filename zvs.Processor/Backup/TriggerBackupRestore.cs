using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;
using System.Data.Entity;

namespace zvs.Processor.Backup
{
    public class TriggerBackupRestore : BackupRestore
    {
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

        public async override Task<Result> ExportAsync(string fileName, CancellationToken cancellationToken)
        {
            using (var context = new ZvsContext())
            {
                var existingTriggers = await context.DeviceValueTriggers
                    .Include(o => o.StoredCommand)
                    .Include(o => o.DeviceValue)
                    .ToListAsync(cancellationToken);

                var backupTriggers = new List<TriggerBackup>();
                foreach (var o in existingTriggers)
                {
                    var trigger = new TriggerBackup
                    {
                        Name = o.Name,
                        isEnabled = o.isEnabled,
                        DeviceValueName = o.DeviceValue.Name,
                        NodeNumber = o.DeviceValue.Device.NodeNumber,
                        StoredCommand = await StoredCMDBackup.ConvertToBackupCommand(o.StoredCommand),
                        Operator = (int?) o.Operator,
                        Value = o.Value
                    };
                    backupTriggers.Add(trigger);
                }

                var saveResult = await SaveAsXMLToDiskAsync(backupTriggers, fileName);

                if (saveResult.HasError)
                    return Result.ReportError(saveResult.Message);

                return Result.ReportSuccessFormat("Exported {0} triggers to {1}", backupTriggers.Count,
                    Path.GetFileName(fileName));
            }
        }

        public async override Task<RestoreSettingsResult> ImportAsync(string fileName, CancellationToken cancellationToken)
        {
            var result = await ReadAsXMLFromDiskAsync<List<TriggerBackup>>(fileName);

            if (result.HasError)
                return RestoreSettingsResult.ReportError(result.Message);

            var skippedCount = 0;
            var newTriggers = new List<DeviceValueTrigger>();

            using (var context = new ZvsContext())
            {
                var existingTriggers = await context.DeviceValueTriggers.ToListAsync();

                var existingDeviceValues = await context.DeviceValues
                    .Include(o => o.Device)
                    .ToListAsync();

                foreach (var backupTrigger in result.Data)
                {
                    if (existingTriggers.Any(o => o.Name == backupTrigger.Name))
                    {
                        skippedCount++;
                        continue;
                    }

                    var dv = existingDeviceValues.FirstOrDefault(o => o.Device.NodeNumber == backupTrigger.NodeNumber
                            && o.Name == backupTrigger.DeviceValueName);

                    if (dv != null)
                    {
                        var dvTrigger = new DeviceValueTrigger
                        {
                            Name = backupTrigger.Name,
                            DeviceValue = dv,
                            isEnabled = backupTrigger.isEnabled
                        };
                        dvTrigger.Name = backupTrigger.Name;
                        dvTrigger.StoredCommand = await StoredCMDBackup.RestoreStoredCommandAsync(context, backupTrigger.StoredCommand, cancellationToken);
                        dvTrigger.Operator = (TriggerOperator)backupTrigger.Operator;
                        dvTrigger.Value = backupTrigger.Value;
                        newTriggers.Add(dvTrigger);
                    }
                }

                context.DeviceValueTriggers.AddRange(newTriggers);

                if (newTriggers.Count > 0)
                {
                    var saveResult = await context.TrySaveChangesAsync(cancellationToken);
                    if (saveResult.HasError)
                        return RestoreSettingsResult.ReportError(saveResult.Message);
                }
            }
            return RestoreSettingsResult.ReportSuccess(string.Format("Imported {0} triggers, skipped {1} from {2}", newTriggers.Count, skippedCount, Path.GetFileName(fileName)));
        }


    }


}