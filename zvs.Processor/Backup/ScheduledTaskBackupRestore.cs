using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;
using System.Data.Entity;

namespace zvs.Processor.Backup
{
    public class ScheduledTaskBackupRestore : BackupRestore
    {
        public class ScheduledTaskBackup
        {
            public int? Frequency { get; set; }
            public string Name { get; set; }
            public StoredCMDBackup StoredCommand { get; set; }
            public bool isEnabled { get; set; }
            public bool? RecurDay01 { get; set; }
            public bool? RecurDay02 { get; set; }
            public bool? RecurDay03 { get; set; }
            public bool? RecurDay04 { get; set; }
            public bool? RecurDay05 { get; set; }
            public bool? RecurDay06 { get; set; }
            public bool? RecurDay07 { get; set; }
            public bool? RecurDay08 { get; set; }
            public bool? RecurDay09 { get; set; }
            public bool? RecurDay10 { get; set; }
            public bool? RecurDay11 { get; set; }
            public bool? RecurDay12 { get; set; }
            public bool? RecurDay13 { get; set; }
            public bool? RecurDay14 { get; set; }
            public bool? RecurDay15 { get; set; }
            public bool? RecurDay16 { get; set; }
            public bool? RecurDay17 { get; set; }
            public bool? RecurDay18 { get; set; }
            public bool? RecurDay19 { get; set; }
            public bool? RecurDay20 { get; set; }
            public bool? RecurDay21 { get; set; }
            public bool? RecurDay22 { get; set; }
            public bool? RecurDay23 { get; set; }
            public bool? RecurDay24 { get; set; }
            public bool? RecurDay25 { get; set; }
            public bool? RecurDay26 { get; set; }
            public bool? RecurDay27 { get; set; }
            public bool? RecurDay28 { get; set; }
            public bool? RecurDay29 { get; set; }
            public bool? RecurDay30 { get; set; }
            public bool? RecurDay31 { get; set; }
            public int? RecurDayofMonth { get; set; }
            public int? RecurDays { get; set; }
            public bool? RecurEven { get; set; }
            public bool? RecurFriday { get; set; }
            public bool? RecurMonday { get; set; }
            public int? RecurMonth { get; set; }
            public bool? RecurSaturday { get; set; }
            public int? RecurSeconds { get; set; }
            public bool? RecurSunday { get; set; }
            public bool? RecurThursday { get; set; }
            public bool? RecurTuesday { get; set; }
            public bool? RecurWednesday { get; set; }
            public int? RecurWeeks { get; set; }
            public int? sortOrder { get; set; }
            public DateTime? startTime { get; set; }
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
            using (var context = new ZvsContext())
            {
                var existingSTs = await context.ScheduledTasks
                    .Include(o => o.StoredCommand)
                    .ToListAsync(cancellationToken);

                var backupSTs = new List<ScheduledTaskBackup>();
                foreach (var t in existingSTs)
                {
                    var task = new ScheduledTaskBackup
                    {
                        StoredCommand = await StoredCMDBackup.ConvertToBackupCommand(t.StoredCommand),
                        Frequency = (int) t.Frequency,
                        Name = t.Name,
                        isEnabled = t.isEnabled,
                        RecurDay01 = t.RecurDay01,
                        RecurDay02 = t.RecurDay02,
                        RecurDay03 = t.RecurDay03,
                        RecurDay04 = t.RecurDay04,
                        RecurDay05 = t.RecurDay05,
                        RecurDay06 = t.RecurDay06,
                        RecurDay07 = t.RecurDay07,
                        RecurDay08 = t.RecurDay08,
                        RecurDay09 = t.RecurDay09,
                        RecurDay10 = t.RecurDay10,
                        RecurDay11 = t.RecurDay11,
                        RecurDay12 = t.RecurDay12,
                        RecurDay13 = t.RecurDay13,
                        RecurDay14 = t.RecurDay14,
                        RecurDay15 = t.RecurDay15,
                        RecurDay16 = t.RecurDay16,
                        RecurDay17 = t.RecurDay17,
                        RecurDay18 = t.RecurDay18,
                        RecurDay19 = t.RecurDay19,
                        RecurDay20 = t.RecurDay20,
                        RecurDay21 = t.RecurDay21,
                        RecurDay22 = t.RecurDay22,
                        RecurDay23 = t.RecurDay23,
                        RecurDay24 = t.RecurDay24,
                        RecurDay25 = t.RecurDay25,
                        RecurDay26 = t.RecurDay26,
                        RecurDay27 = t.RecurDay27,
                        RecurDay28 = t.RecurDay28,
                        RecurDay29 = t.RecurDay29,
                        RecurDay30 = t.RecurDay30,
                        RecurDay31 = t.RecurDay31,
                        RecurDayofMonth = t.RecurDayofMonth,
                        RecurDays = t.RecurDays,
                        RecurEven = t.RecurEven,
                        RecurFriday = t.RecurFriday,
                        RecurMonday = t.RecurMonday,
                        RecurSaturday = t.RecurSaturday,
                        RecurMonth = t.RecurMonth,
                        RecurSeconds = t.RecurSeconds,
                        RecurSunday = t.RecurSunday,
                        RecurThursday = t.RecurThursday,
                        RecurTuesday = t.RecurTuesday,
                        RecurWednesday = t.RecurWednesday,
                        RecurWeeks = t.RecurWeeks,
                        sortOrder = t.SortOrder,
                        startTime = t.StartTime
                    };
                    backupSTs.Add(task);
                }

                var saveResult = await SaveAsXMLToDiskAsync(backupSTs, fileName);

                if (saveResult.HasError)
                    return Result.ReportError(saveResult.Message);

                return Result.ReportSuccessFormat("Exported {0} scheduled tasks to {1}", backupSTs.Count,
                    Path.GetFileName(fileName));
            }
        }

        public async override Task<RestoreSettingsResult> ImportAsync(string fileName, CancellationToken cancellationToken)
        {
            var result = await ReadAsXMLFromDiskAsync<List<ScheduledTaskBackup>>(fileName);

            if (result.HasError)
                return RestoreSettingsResult.ReportError(result.Message);

            var skippedCount = 0;
            var newSTs = new List<ScheduledTask>();

            using (var context = new ZvsContext())
            {
                var existingSTs = await context.ScheduledTasks.ToListAsync(cancellationToken);

                var existingDeviceValues = await context.DeviceValues
                    .Include(o => o.Device)
                    .ToListAsync(cancellationToken);

                foreach (var backupSt in result.Data)
                {
                    if (existingSTs.Any(o => o.Name == backupSt.Name))
                    {
                        skippedCount++;
                        continue;
                    }

                    var task = new ScheduledTask
                    {
                        StoredCommand = await StoredCMDBackup.RestoreStoredCommandAsync(context, backupSt.StoredCommand,cancellationToken),
                        Frequency = (TaskFrequency) backupSt.Frequency,
                        Name = backupSt.Name,
                        isEnabled = backupSt.isEnabled,
                        RecurDay01 = backupSt.RecurDay01,
                        RecurDay02 = backupSt.RecurDay02,
                        RecurDay03 = backupSt.RecurDay03,
                        RecurDay04 = backupSt.RecurDay04,
                        RecurDay05 = backupSt.RecurDay05,
                        RecurDay06 = backupSt.RecurDay06,
                        RecurDay07 = backupSt.RecurDay07,
                        RecurDay08 = backupSt.RecurDay08,
                        RecurDay09 = backupSt.RecurDay09,
                        RecurDay10 = backupSt.RecurDay10,
                        RecurDay11 = backupSt.RecurDay11,
                        RecurDay12 = backupSt.RecurDay12,
                        RecurDay13 = backupSt.RecurDay13,
                        RecurDay14 = backupSt.RecurDay14,
                        RecurDay15 = backupSt.RecurDay15,
                        RecurDay16 = backupSt.RecurDay16,
                        RecurDay17 = backupSt.RecurDay17,
                        RecurDay18 = backupSt.RecurDay18,
                        RecurDay19 = backupSt.RecurDay19,
                        RecurDay20 = backupSt.RecurDay20,
                        RecurDay21 = backupSt.RecurDay21,
                        RecurDay22 = backupSt.RecurDay22,
                        RecurDay23 = backupSt.RecurDay23,
                        RecurDay24 = backupSt.RecurDay24,
                        RecurDay25 = backupSt.RecurDay25,
                        RecurDay26 = backupSt.RecurDay26,
                        RecurDay27 = backupSt.RecurDay27,
                        RecurDay28 = backupSt.RecurDay28,
                        RecurDay29 = backupSt.RecurDay29,
                        RecurDay30 = backupSt.RecurDay30,
                        RecurDay31 = backupSt.RecurDay31,
                        RecurDayofMonth = backupSt.RecurDayofMonth,
                        RecurDays = backupSt.RecurDays,
                        RecurEven = backupSt.RecurEven,
                        RecurFriday = backupSt.RecurFriday,
                        RecurMonday = backupSt.RecurMonday,
                        RecurSaturday = backupSt.RecurSaturday,
                        RecurMonth = backupSt.RecurMonth,
                        RecurSeconds = backupSt.RecurSeconds,
                        RecurSunday = backupSt.RecurSunday,
                        RecurThursday = backupSt.RecurThursday,
                        RecurTuesday = backupSt.RecurTuesday,
                        RecurWednesday = backupSt.RecurWednesday,
                        RecurWeeks = backupSt.RecurWeeks,
                        SortOrder = backupSt.sortOrder,
                        StartTime = backupSt.startTime
                    };
                    newSTs.Add(task);
                }

                context.ScheduledTasks.AddRange(newSTs);

                if (newSTs.Count > 0)
                {
                    var saveResult = await context.TrySaveChangesAsync(cancellationToken);
                    if (saveResult.HasError)
                        return RestoreSettingsResult.ReportError(saveResult.Message);
                }
            }
            return RestoreSettingsResult.ReportSuccess(string.Format("Imported {0} scheduled tasks, skipped {1} from {2}", newSTs.Count, skippedCount, Path.GetFileName(fileName)));
        }

    }
}