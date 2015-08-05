using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;
using zvs.DataModel.Tasks;

namespace zvs.Processor.ImportExport
{
    public class ScheduledTaskBackupRestore : BackupRestore
    {
        public ScheduledTaskBackupRestore(IEntityContextConnection entityContextConnection) : base(entityContextConnection) { }
        public class ScheduledTaskBackup
        {
            public int? Frequency { get; set; }
            public string Name { get; set; }
            public StoredCmdBackup StoredCommand { get; set; }
            public bool isEnabled { get; set; }
            public int? RecurDays { get; set; }
            public int? RecurMonth { get; set; }
            public int? RecurWeeks { get; set; }
            public double? RecurSeconds { get; set; }
            public int? sortOrder { get; set; }
            public DateTime? startTime { get; set; }
            public int? DaysOfMonthToActivate { get; set; }
            public int? DaysOfWeekToActivate { get; set; }
        }

        public override string Name
        {
            get { return "Scheduled Task"; }
        }

        public override string FileName
        {
            get { return "ScheduledTaskBackup.zvs"; }
        }

        public async override Task<Result> ExportAsync(string fileName, CancellationToken cancellationToken)
        {
            using (var context = new ZvsContext(EntityContextConnection))
            {
                var existingSTs = await context.ScheduledTasks
                    .ToListAsync(cancellationToken);

                var backupSTs = new List<ScheduledTaskBackup>();
                foreach (var t in existingSTs)
                {
                    var task = new ScheduledTaskBackup
                    {
                        StoredCommand = await StoredCmdBackup.ConvertToBackupCommand(t),
                        startTime = t.StartTime,
                        Frequency = (int)t.TaskType,
                        Name = t.Name,
                        isEnabled = t.IsEnabled,

                        DaysOfMonthToActivate = (int)t.DaysOfMonthToActivate,
                        DaysOfWeekToActivate = (int)t.DaysOfWeekToActivate,

                        RecurDays = t.RepeatIntervalInDays,
                        RecurMonth = t.RepeatIntervalInMonths,
                        RecurSeconds = t.RepeatIntervalInSeconds,
                        RecurWeeks = t.RepeatIntervalInWeeks,

                        sortOrder = t.SortOrder

                    };
                    backupSTs.Add(task);
                }

                var saveResult = await SaveAsXmlToDiskAsync(backupSTs, fileName);

                if (saveResult.HasError)
                    return Result.ReportError(saveResult.Message);

                return Result.ReportSuccessFormat("Exported {0} scheduled tasks to {1}", backupSTs.Count,
                    Path.GetFileName(fileName));
            }
        }

        public async override Task<Result> ImportAsync(string fileName, CancellationToken cancellationToken)
        {
            var result = await ReadAsXmlFromDiskAsync<List<ScheduledTaskBackup>>(fileName);

            if (result.HasError)
                return Result.ReportError(result.Message);

            var skippedCount = 0;
            var newSTs = new List<DataModel.ScheduledTask>();

            using (var context = new ZvsContext(EntityContextConnection))
            {
                var existingScheduledTasks = await context.ScheduledTasks
                    .ToListAsync(cancellationToken);

                foreach (var scheduledTaskBackup in result.Data)
                {
                    if (existingScheduledTasks.Any(o => o.Name == scheduledTaskBackup.Name))
                    {
                        skippedCount++;
                        continue;
                    }

                    var cmd =
                        await
                            StoredCmdBackup.RestoreStoredCommandAsync(context, scheduledTaskBackup.StoredCommand,
                                cancellationToken);
                    if (cmd == null) continue;

                    var task = new DataModel.ScheduledTask
                    {
                        Argument = cmd.Argument,
                        Argument2 = cmd.Argument2,
                        CommandId = cmd.CommandId,

                        Name = scheduledTaskBackup.Name,
                        IsEnabled = scheduledTaskBackup.isEnabled,

                        RepeatIntervalInDays = scheduledTaskBackup.RecurDays ?? 0,
                        RepeatIntervalInMonths = scheduledTaskBackup.RecurMonth ?? 0,
                        RepeatIntervalInSeconds = scheduledTaskBackup.RecurSeconds ?? 0,
                        RepeatIntervalInWeeks = scheduledTaskBackup.RecurWeeks ?? 0,
                        SortOrder = scheduledTaskBackup.sortOrder,
                        StartTime = scheduledTaskBackup.startTime ?? DateTime.Now
                    };

                    if (scheduledTaskBackup.Frequency != null)
                        task.TaskType = (ScheduledTaskType)scheduledTaskBackup.Frequency;
                    if (scheduledTaskBackup.DaysOfMonthToActivate != null)
                        task.DaysOfMonthToActivate = (DaysOfMonth)scheduledTaskBackup.DaysOfMonthToActivate;
                    if (scheduledTaskBackup.DaysOfWeekToActivate != null)
                        task.DaysOfWeekToActivate = (DaysOfWeek)scheduledTaskBackup.DaysOfWeekToActivate;

                    
                    newSTs.Add(task);
                }

                context.ScheduledTasks.AddRange(newSTs);

                if (newSTs.Count > 0)
                {
                    var saveResult = await context.TrySaveChangesAsync(cancellationToken);
                    if (saveResult.HasError)
                        return Result.ReportError(saveResult.Message);

                    foreach (var task in newSTs)
                    {
                        task.SetDescription();
                        await task.SetTargetObjectNameAsync(context);
                    }

                    var r = await context.TrySaveChangesAsync(cancellationToken);
                    if (r.HasError)
                        return Result.ReportError(r.Message);
                }

               
            }
            return Result.ReportSuccess(
                $"Imported {newSTs.Count} scheduled tasks, skipped {skippedCount} from {Path.GetFileName(fileName)}");
        }

    }
}