using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;

namespace zvs.Processor.ImportExport
{
    public class DeviceBackupRestore : BackupRestore
    {
        public DeviceBackupRestore(IEntityContextConnection entityContextConnection) : base(entityContextConnection) { }

        public class DeviceBackup
        {
            public string Name { get; set; }
            public string Location { get; set; }
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
            using (var context = new ZvsContext(EntityContextConnection))
            {
                var backupDevices = await context.Devices
                    .OrderBy(o => o.Name)
                    .Select(o => new DeviceBackup
                    {
                        NodeNumber = o.NodeNumber,
                        Name = o.Name,
                        Location = o.Location
                    })
                    .ToListAsync(cancellationToken);

                var saveResult = await SaveAsXmlToDiskAsync(backupDevices, fileName);

                return saveResult.HasError ? Result.ReportError(saveResult.Message) : Result.ReportSuccessFormat("Exported {0} device names to {1}", backupDevices.Count, Path.GetFileName(fileName));
            }
        }

        public async override Task<Result> ImportAsync(string fileName, CancellationToken cancellationToken)
        {
            var result = await ReadAsXmlFromDiskAsync<List<DeviceBackup>>(fileName);

            if (result.HasError)
                return Result.ReportError(result.Message);

            var backupDevices = result.Data;
            var importedCount = 0;

            using (var context = new ZvsContext(EntityContextConnection))
            {
                foreach (var d in await context.Devices.ToListAsync(cancellationToken))
                {
                    var dev = backupDevices.FirstOrDefault(o => o.NodeNumber == d.NodeNumber);
                    if (dev == null) continue;
                    
                    d.Name = dev.Name;
                    d.Location = dev.Location;
                    importedCount++;
                }

                var saveResult = await context.TrySaveChangesAsync(cancellationToken);

                if (saveResult.HasError)
                    return Result.ReportError(saveResult.Message);
            }

            return Result.ReportSuccess(string.Format("Restored {0} device names. File: '{1}'", importedCount, Path.GetFileName(fileName)));
        }

        
    }
}
