using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using zvs.Entities;
using System.Data.Entity;

namespace zvs.Processor.Backup
{
    public class GroupBackupRestore : BackupRestore
    {
        public class GroupBackup
        {
            public string Name { get; set; }
            public List<int> NodeNumbers = new List<int>();
        }

        public override string Name
        {
            get { return "Groups"; }
        }

        public override string FileName
        {
            get { return "GroupsBackup.zvs"; }
        }

        public async override Task<ExportResult> ExportAsync(string fileName)
        {
            using (zvsContext context = new zvsContext())
            {
                var backupGroups = await context.Groups
                    .Include(o => o.Devices)
                    .Select(o => new GroupBackup()
                    {
                        Name = o.Name,
                        NodeNumbers = o.Devices.Select(d => d.NodeNumber).ToList()
                    })
                    .ToListAsync();

                var saveResult = await SaveAsXMLToDiskAsync(backupGroups, fileName);

                if (saveResult.HasError)
                    return new ExportResult(saveResult.Message, saveResult.HasError);

                return new ExportResult(string.Format("Exported {0} groups to {1}", backupGroups.Count,
                    Path.GetFileName(fileName)), false);
            }
        }

        public async override Task<RestoreSettingsResult> ImportAsync(string fileName)
        {
            var result = await ReadAsXMLFromDiskAsync<List<GroupBackup>>(fileName);

            if (result.HasError)
                return new RestoreSettingsResult(result.Message);

            int SkippedCount = 0;
            var newGroups = new List<Group>();

            using (zvsContext context = new zvsContext())
            {
                var existingGroups = await context.Groups.ToListAsync();
                var existingDevice = await context.Devices.ToListAsync();

                foreach (var backupGroup in result.Data)
                {
                    if (existingGroups.Any(o => o.Name == backupGroup.Name))
                    {
                        SkippedCount++;
                        continue;
                    }

                    Group group = new Group();
                    group.Name = backupGroup.Name;
                    var devices = existingDevice.Where(o => backupGroup.NodeNumbers.Contains(o.NodeNumber));

                    foreach (var device in devices)
                        group.Devices.Add(device);

                    newGroups.Add(group);
                }

                context.Groups.AddRange(newGroups);

                if (newGroups.Count > 0)
                {
                    var saveResult = await context.TrySaveChangesAsync();
                    if (saveResult.HasError)
                        return new RestoreSettingsResult(saveResult.Message);
                }
            }
            return new RestoreSettingsResult(string.Format("Imported {0} groups, skipped {1} from {2}", newGroups.Count, SkippedCount, Path.GetFileName(fileName)), fileName);
        }
    }
}