using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;

namespace zvs.Processor.ImportExport
{
    public class TriggerBackupRestore : BackupRestore
    {
        public TriggerBackupRestore(IEntityContextConnection entityContextConnection) : base(entityContextConnection)
        {
        }

        public class TriggerBackup
        {
            public string Name { get; set; }
            public bool isEnabled { get; set; }
            public int? Operator { get; set; }
            public StoredCmdBackup StoredCommand { get; set; }
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

        public override async Task<Result> ExportAsync(string fileName, CancellationToken cancellationToken)
        {
            using (var context = new ZvsContext(EntityContextConnection))
            {
                var existingTriggers = await context.DeviceValueTriggers
                    .ToListAsync(cancellationToken);

                var backupTriggers = new List<TriggerBackup>();
                foreach (var o in existingTriggers)
                {
                    var trigger = new TriggerBackup
                    {
                        Name = o.Name,
                        isEnabled = o.IsEnabled,
                        DeviceValueName = o.DeviceValue.Name,
                        NodeNumber = o.DeviceValue.Device.NodeNumber,
                        StoredCommand = await StoredCmdBackup.ConvertToBackupCommand(o),
                        Operator = (int?) o.Operator,
                        Value = o.Value
                    };
                    backupTriggers.Add(trigger);
                }

                var saveResult = await SaveAsXmlToDiskAsync(backupTriggers, fileName);

                if (saveResult.HasError)
                    return Result.ReportError(saveResult.Message);

                return Result.ReportSuccessFormat("Exported {0} triggers to {1}", backupTriggers.Count,
                    Path.GetFileName(fileName));
            }
        }

        public override async Task<Result> ImportAsync(string fileName, CancellationToken cancellationToken)
        {
            var result = await ReadAsXmlFromDiskAsync<List<TriggerBackup>>(fileName);

            if (result.HasError)
                return Result.ReportError(result.Message);

            var skippedCount = 0;
            var newTriggers = new List<DeviceValueTrigger>();

            using (var context = new ZvsContext(EntityContextConnection))
            {
                var existingTriggers = await context.DeviceValueTriggers.ToListAsync(cancellationToken);

                var existingDeviceValues = await context.DeviceValues
                    .Include(o => o.Device)
                    .ToListAsync(cancellationToken);

                foreach (var backupTrigger in result.Data)
                {
                    if (existingTriggers.Any(o => o.Name == backupTrigger.Name))
                    {
                        skippedCount++;
                        continue;
                    }

                    var dv = existingDeviceValues.FirstOrDefault(o => o.Device.NodeNumber == backupTrigger.NodeNumber
                                                                      && o.Name == backupTrigger.DeviceValueName);

                    var cmd =
                        await
                            StoredCmdBackup.RestoreStoredCommandAsync(context, backupTrigger.StoredCommand,
                                cancellationToken);

                    if (dv == null || cmd == null) continue;
                    var dvTrigger = new DeviceValueTrigger
                    {
                        Name = backupTrigger.Name,
                        DeviceValue = dv,
                        IsEnabled = backupTrigger.isEnabled
                    };
                    dvTrigger.Name = backupTrigger.Name;
                    dvTrigger.Argument = cmd.Argument;
                    dvTrigger.Argument2 = cmd.Argument2;
                    dvTrigger.CommandId = cmd.CommandId;
                    if (backupTrigger.Operator != null)
                        dvTrigger.Operator = (TriggerOperator) backupTrigger.Operator;
                    dvTrigger.Value = backupTrigger.Value;
                    newTriggers.Add(dvTrigger);
                }

                context.DeviceValueTriggers.AddRange(newTriggers);

                if (newTriggers.Count > 0)
                {
                    var saveResult = await context.TrySaveChangesAsync(cancellationToken);
                    if (saveResult.HasError)
                        return Result.ReportError(saveResult.Message);

                    foreach (var cmd in newTriggers)
                    {
                        cmd.SetDescription();
                        await cmd.SetTargetObjectNameAsync(context);
                        cmd.SetTriggerDescription();
                    }

                    var r = await context.TrySaveChangesAsync(cancellationToken);
                    if (r.HasError)
                        return Result.ReportError(r.Message);
                }
            }
            return
                Result.ReportSuccess(string.Format("Imported {0} triggers, skipped {1} from {2}", newTriggers.Count,
                    skippedCount, Path.GetFileName(fileName)));
        }
    }
}