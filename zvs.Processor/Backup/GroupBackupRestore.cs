using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;
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

        public async override Task<Result> ExportAsync(string fileName, CancellationToken cancellationToken)
        {
            using (var context = new ZvsContext())
            {
                var backupGroups = await context.Groups
                    .Include(o => o.Devices)
                    .Select(o => new GroupBackup
                    {
                        Name = o.Name,
                        NodeNumbers = o.Devices.Select(d => d.NodeNumber).ToList()
                    })
                    .ToListAsync(cancellationToken);

                var saveResult = await SaveAsXMLToDiskAsync(backupGroups, fileName);

                if (saveResult.HasError)
                    return Result.ReportError(saveResult.Message);

                return Result.ReportErrorFormat("Exported {0} groups to {1}", backupGroups.Count,
                    Path.GetFileName(fileName));
            }
        }

        public async override Task<RestoreSettingsResult> ImportAsync(string fileName, CancellationToken cancellationToken)
        {
            var result = await ReadAsXMLFromDiskAsync<List<GroupBackup>>(fileName);

            if (result.HasError)
                return RestoreSettingsResult.ReportError(result.Message);

            var skippedCount = 0;
            var newGroups = new List<Group>();

            using (var context = new ZvsContext())
            {
                var existingGroups = await context.Groups.ToListAsync(cancellationToken);
                var existingDevice = await context.Devices.ToListAsync(cancellationToken);

                foreach (var backupGroup in result.Data)
                {
                    if (existingGroups.Any(o => o.Name == backupGroup.Name))
                    {
                        skippedCount++;
                        continue;
                    }

                    var group = new Group();
                    group.Name = backupGroup.Name;
                    var devices = existingDevice.Where(o => backupGroup.NodeNumbers.Contains(o.NodeNumber));

                    foreach (var device in devices)
                        group.Devices.Add(device);

                    newGroups.Add(group);
                }

                context.Groups.AddRange(newGroups);

                if (newGroups.Count > 0)
                {
                    var saveResult = await context.TrySaveChangesAsync(cancellationToken);
                    if (saveResult.HasError)
                        return RestoreSettingsResult.ReportError(saveResult.Message);
                }
            }
            return RestoreSettingsResult.ReportSuccess(string.Format("Imported {0} groups, skipped {1} from {2}", newGroups.Count, skippedCount, Path.GetFileName(fileName)));
        }
    }
}