using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using zvs.Entities;
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

        public async override Task<ExportResult> ExportAsync(string fileName)
        {
            using (zvsContext context = new zvsContext())
            {
                var existingSTs = await context.ScheduledTasks
                    .Include(o => o.StoredCommand)
                    .ToListAsync();

                var backupSTs = new List<ScheduledTaskBackup>();
                foreach (var t in existingSTs)
                {
                    var task = new ScheduledTaskBackup();
                    task.StoredCommand = await StoredCMDBackup.ConvertToBackupCommand(t.StoredCommand);
                    task.Frequency = (int)t.Frequency;
                    task.Name = t.Name;
                    task.isEnabled = t.isEnabled;
                    task.RecurDay01 = t.RecurDay01;
                    task.RecurDay02 = t.RecurDay02;
                    task.RecurDay03 = t.RecurDay03;
                    task.RecurDay04 = t.RecurDay04;
                    task.RecurDay05 = t.RecurDay05;
                    task.RecurDay06 = t.RecurDay06;
                    task.RecurDay07 = t.RecurDay07;
                    task.RecurDay08 = t.RecurDay08;
                    task.RecurDay09 = t.RecurDay09;
                    task.RecurDay10 = t.RecurDay10;
                    task.RecurDay11 = t.RecurDay11;
                    task.RecurDay12 = t.RecurDay12;
                    task.RecurDay13 = t.RecurDay13;
                    task.RecurDay14 = t.RecurDay14;
                    task.RecurDay15 = t.RecurDay15;
                    task.RecurDay16 = t.RecurDay16;
                    task.RecurDay17 = t.RecurDay17;
                    task.RecurDay18 = t.RecurDay18;
                    task.RecurDay19 = t.RecurDay19;
                    task.RecurDay20 = t.RecurDay20;
                    task.RecurDay21 = t.RecurDay21;
                    task.RecurDay22 = t.RecurDay22;
                    task.RecurDay23 = t.RecurDay23;
                    task.RecurDay24 = t.RecurDay24;
                    task.RecurDay25 = t.RecurDay25;
                    task.RecurDay26 = t.RecurDay26;
                    task.RecurDay27 = t.RecurDay27;
                    task.RecurDay28 = t.RecurDay28;
                    task.RecurDay29 = t.RecurDay29;
                    task.RecurDay30 = t.RecurDay30;
                    task.RecurDay31 = t.RecurDay31;
                    task.RecurDayofMonth = t.RecurDayofMonth;
                    task.RecurDays = t.RecurDays;
                    task.RecurEven = t.RecurEven;
                    task.RecurFriday = t.RecurFriday;
                    task.RecurMonday = t.RecurMonday;
                    task.RecurSaturday = t.RecurSaturday;
                    task.RecurMonth = t.RecurMonth;
                    task.RecurSeconds = t.RecurSeconds;
                    task.RecurSunday = t.RecurSunday;
                    task.RecurThursday = t.RecurThursday;
                    task.RecurTuesday = t.RecurTuesday;
                    task.RecurWednesday = t.RecurWednesday;
                    task.RecurWeeks = t.RecurWeeks;
                    task.sortOrder = t.SortOrder;
                    task.startTime = t.StartTime;
                    backupSTs.Add(task);
                }

                var saveResult = await SaveAsXMLToDiskAsync(backupSTs, fileName);

                if (saveResult.HasError)
                    return new ExportResult(saveResult.Message, saveResult.HasError);

                return new ExportResult(string.Format("Exported {0} scheduled tasks to {1}", backupSTs.Count,
                    Path.GetFileName(fileName)), false);
            }
        }

        public async override Task<RestoreSettingsResult> ImportAsync(string fileName)
        {
            var result = await ReadAsXMLFromDiskAsync<List<ScheduledTaskBackup>>(fileName);

            if (result.HasError)
                return new RestoreSettingsResult(result.Message);

            int SkippedCount = 0;
            var newSTs = new List<ScheduledTask>();

            using (zvsContext context = new zvsContext())
            {
                var existingSTs = await context.ScheduledTasks.ToListAsync();

                var existingDeviceValues = await context.DeviceValues
                    .Include(o => o.Device)
                    .ToListAsync();

                foreach (var backupST in result.Data)
                {
                    if (existingSTs.Any(o => o.Name == backupST.Name))
                    {
                        SkippedCount++;
                        continue;
                    }

                    ScheduledTask task = new ScheduledTask();
                    task.StoredCommand = await StoredCMDBackup.RestoreStoredCommandAsync(context, backupST.StoredCommand);
                    task.Frequency = (TaskFrequency)backupST.Frequency;
                    task.Name = backupST.Name;
                    task.isEnabled = backupST.isEnabled;
                    task.RecurDay01 = backupST.RecurDay01;
                    task.RecurDay02 = backupST.RecurDay02;
                    task.RecurDay03 = backupST.RecurDay03;
                    task.RecurDay04 = backupST.RecurDay04;
                    task.RecurDay05 = backupST.RecurDay05;
                    task.RecurDay06 = backupST.RecurDay06;
                    task.RecurDay07 = backupST.RecurDay07;
                    task.RecurDay08 = backupST.RecurDay08;
                    task.RecurDay09 = backupST.RecurDay09;
                    task.RecurDay10 = backupST.RecurDay10;
                    task.RecurDay11 = backupST.RecurDay11;
                    task.RecurDay12 = backupST.RecurDay12;
                    task.RecurDay13 = backupST.RecurDay13;
                    task.RecurDay14 = backupST.RecurDay14;
                    task.RecurDay15 = backupST.RecurDay15;
                    task.RecurDay16 = backupST.RecurDay16;
                    task.RecurDay17 = backupST.RecurDay17;
                    task.RecurDay18 = backupST.RecurDay18;
                    task.RecurDay19 = backupST.RecurDay19;
                    task.RecurDay20 = backupST.RecurDay20;
                    task.RecurDay21 = backupST.RecurDay21;
                    task.RecurDay22 = backupST.RecurDay22;
                    task.RecurDay23 = backupST.RecurDay23;
                    task.RecurDay24 = backupST.RecurDay24;
                    task.RecurDay25 = backupST.RecurDay25;
                    task.RecurDay26 = backupST.RecurDay26;
                    task.RecurDay27 = backupST.RecurDay27;
                    task.RecurDay28 = backupST.RecurDay28;
                    task.RecurDay29 = backupST.RecurDay29;
                    task.RecurDay30 = backupST.RecurDay30;
                    task.RecurDay31 = backupST.RecurDay31;
                    task.RecurDayofMonth = backupST.RecurDayofMonth;
                    task.RecurDays = backupST.RecurDays;
                    task.RecurEven = backupST.RecurEven;
                    task.RecurFriday = backupST.RecurFriday;
                    task.RecurMonday = backupST.RecurMonday;
                    task.RecurSaturday = backupST.RecurSaturday;
                    task.RecurMonth = backupST.RecurMonth;
                    task.RecurSeconds = backupST.RecurSeconds;
                    task.RecurSunday = backupST.RecurSunday;
                    task.RecurThursday = backupST.RecurThursday;
                    task.RecurTuesday = backupST.RecurTuesday;
                    task.RecurWednesday = backupST.RecurWednesday;
                    task.RecurWeeks = backupST.RecurWeeks;
                    task.SortOrder = backupST.sortOrder;
                    task.StartTime = backupST.startTime;
                    newSTs.Add(task);
                }

                context.ScheduledTasks.AddRange(newSTs);

                if (newSTs.Count > 0)
                {
                    var saveResult = await context.TrySaveChangesAsync();
                    if (saveResult.HasError)
                        return new RestoreSettingsResult(saveResult.Message);
                }
            }
            return new RestoreSettingsResult(string.Format("Imported {0} scheduled tasks, skipped {1} from {2}", newSTs.Count, SkippedCount, Path.GetFileName(fileName)), fileName);
        }

    }
}