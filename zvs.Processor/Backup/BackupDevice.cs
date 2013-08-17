using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using zvs.Entities;
using System.Data.Entity;
using System.Diagnostics;

namespace zvs.Processor.Backup
{
    public partial class Backup
    {
        [Serializable]
        public class DeviceBackup
        {
            public string Name;
            public int NodeNumber;
        }

        public async static Task<BackFileIO.ExportResult> ExportDevicesAsync(string fileName)
        {
            List<DeviceBackup> devices = new List<DeviceBackup>();
            using (zvsContext context = new zvsContext())
            {
                var backupDevices = await context.Devices
                    .Select(o => new DeviceBackup()
                    {
                        NodeNumber = o.NodeNumber,
                        Name = o.Name
                    })
                    .ToListAsync();

                var saveResult = await BackFileIO.SaveAsXMLToDiskAsync(backupDevices, fileName);

                if (saveResult.HasError)
                    return new BackFileIO.ExportResult(saveResult.Message, saveResult.HasError);

                return new BackFileIO.ExportResult(string.Format("Exported {0} device names to '{1}'", devices.Count, Path.GetFileName(fileName)), false);
            }
        }

        public async static Task<RestoreSettingsResult> ImportDevicesAsync(string fileName)
        {
            var result = await BackFileIO.ReadAsXMLFromDiskAsync<List<DeviceBackup>>(fileName);

            if (result.HasError)
                return new RestoreSettingsResult(result.Message);

            var backupDevices = result.Data;
            int ImportedCount = 0;

            using (zvsContext context = new zvsContext())
            {
                foreach (Device d in await context.Devices.ToListAsync())
                {
                    DeviceBackup dev = backupDevices.FirstOrDefault(o => o.NodeNumber == d.NodeNumber);
                    if (dev != null)
                    {
                        d.Name = dev.Name;
                        ImportedCount++;
                    }
                }
                await context.SaveChangesAsync();
            }

            return new RestoreSettingsResult(string.Format("Imported {0} device names from '{1}'", ImportedCount, Path.GetFileName(fileName)), fileName);
        }
    }

    public class RestoreSettingsResult : Result
    {
        public string Summary { get; private set; }
        public string FilePath { get; private set; }

        public RestoreSettingsResult(string summary, string filePath)
            : base()
        {
            Summary = summary;
            FilePath = filePath;
        }

        public RestoreSettingsResult(string error)
            : base(error)
        {
        }
    }
}
