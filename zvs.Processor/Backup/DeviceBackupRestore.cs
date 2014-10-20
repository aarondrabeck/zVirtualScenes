using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;
using System.Data.Entity;

namespace zvs.Processor.Backup
{
    public class DeviceBackupRestore : BackupRestore
    {
        public class DeviceBackup
        {
            public string Name { get; set; }
            public int NodeNumber { get; set; }
        }

        public override string Name
        {
            get { return "Device Names"; }
        }

        public override string FileName
        {
            get { return "DevicesBackup.zvs"; }
        }

        public async override Task<Result> ExportAsync(string fileName, CancellationToken cancellationToken)
        {
            using (var context = new ZvsContext())
            {
                var backupDevices = await context.Devices
                    .OrderBy(o => o.Name)
                    .Select(o => new DeviceBackup
                    {
                        NodeNumber = o.NodeNumber,
                        Name = o.Name
                    })
                    .ToListAsync(cancellationToken);

                var saveResult = await SaveAsXMLToDiskAsync(backupDevices, fileName);

                return saveResult.HasError ? Result.ReportError(saveResult.Message) : Result.ReportErrorFormat("Exported {0} device names to {1}", backupDevices.Count, Path.GetFileName(fileName));
            }
        }

        public async override Task<RestoreSettingsResult> ImportAsync(string fileName, CancellationToken cancellationToken)
        {
            var result = await ReadAsXMLFromDiskAsync<List<DeviceBackup>>(fileName);

            if (result.HasError)
                return RestoreSettingsResult.ReportError(result.Message);

            var backupDevices = result.Data;
            var importedCount = 0;

            using (var context = new ZvsContext())
            {
                foreach (var d in await context.Devices.ToListAsync())
                {
                    var dev = backupDevices.FirstOrDefault(o => o.NodeNumber == d.NodeNumber);
                    if (dev != null)
                    {
                        d.Name = dev.Name;
                        importedCount++;
                    }
                }

                var saveResult = await context.TrySaveChangesAsync(cancellationToken);

                if (saveResult.HasError)
                    return RestoreSettingsResult.ReportError(saveResult.Message);
            }

            return RestoreSettingsResult.ReportSuccess(string.Format("Restored {0} device names. File: '{1}'", importedCount, Path.GetFileName(fileName)));
        }

        
    }
}
