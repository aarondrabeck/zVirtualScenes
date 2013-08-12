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

        // public async static Task<ReadAsXMLFromDisk<T>> SaveAsXMLToDisk<T>(string fileName)
        //{
        //    Debug.WriteLine(string.Format("Saving {0}", typeof(T).Name));

        //    try
        //    {
        //        if (!File.Exists(fileName))
        //        {
        //            return new ReadAsXMLFromDisk<T>(fileName + " not found");
        //        }

        //        //Open the file written above and read values from it. 
        //        //http://stackoverflow.com/questions/1127431/xmlserializer-giving-filenotfoundexception-at-constructor
        //        XmlSerializer ScenesSerializer = new XmlSerializer(typeof(T));

        //        string fileData;
        //        using (StreamReader streamReader = new StreamReader(fileName))
        //            fileData = await streamReader.ReadToEndAsync();

        //        return new ReadAsXMLFromDisk<T>((T)ScenesSerializer.Deserialize(new StringReader(fileData)));
        //    }
        //    catch (Exception e)
        //    {
        //        return new ReadAsXMLFromDisk<T>(string.Format("Error reading {0}: {1}", fileName, e.Message));
        //    }
        //}

        //        public async Task<Result> SaveToDiskAsync()
        //        {
        //            try
        //            {
        //                Type type = this.GetType();

        //                using (MemoryStream stream = new MemoryStream())
        //                {
        //                    XmlSerializer xmlSerializer = new XmlSerializer(type);
        //                    xmlSerializer.Serialize(stream, this);
        //                    stream.Position = 0;

        //                    using (StreamReader reader = new StreamReader(stream))
        //                    {
        //                        var output = string.Empty;
        //#if !(DISABLE_CRYPTO)
        //                        output = Cryptography.EncryptStringAES(reader.ReadToEnd(), await GetEncryptionDecryptionKey());
        //#else                       
        //                            output = reader.ReadToEnd();
        //#endif

        //                        // Write the string to a file.
        //                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(this.FullPath))
        //                            await file.WriteLineAsync(output);
        //                    }
        //                }

        //                Debug.WriteLine(string.Format("<--> DISK I/O SAVE!!! <--> {0}", type.Name));
        //                return new Result();
        //            }
        //            catch (Exception e)
        //            {
        //                Debug.WriteLine("SaveSettingFileAsync Error: " + e.Message);
        //                return new Result(string.Format("Error saving {0}: {1}", this.FileName, e.Message));
        //            }
        //        }

        public async static Task<ReadAsXMLFromDiskResult<T>> ReadAsXMLFromDisk<T>(string fileName)
        {
            Debug.WriteLine(string.Format("Loading {0}", typeof(T).Name));

            try
            {
                if (!File.Exists(fileName))
                    return new ReadAsXMLFromDiskResult<T>(fileName + " not found");

                //Open the file written above and read values from it. 
                //http://stackoverflow.com/questions/1127431/xmlserializer-giving-filenotfoundexception-at-constructor
                XmlSerializer ScenesSerializer = new XmlSerializer(typeof(T));

                string fileData;
                using (StreamReader streamReader = new StreamReader(fileName))
                    fileData = await streamReader.ReadToEndAsync();

                return new ReadAsXMLFromDiskResult<T>((T)ScenesSerializer.Deserialize(new StringReader(fileData)));
            }
            catch (Exception e)
            {
                return new ReadAsXMLFromDiskResult<T>(string.Format("Error reading {0}: {1}", fileName, e.Message));
            }
        }

        public class ReadAsXMLFromDiskResult<T> : Result
        {
            public T Data { get; private set; }

            public ReadAsXMLFromDiskResult(T data)
                : base()
            {
                Data = data;
            }

            public ReadAsXMLFromDiskResult(string message)
                : base(message)
            {
            }
        }

        public static void ExportDevicesAsync(string PathFileName, Action<string> Callback)
        {
            List<DeviceBackup> devices = new List<DeviceBackup>();
            using (zvsContext context = new zvsContext())
            {
                foreach (Device d in context.Devices)
                {
                    devices.Add(new DeviceBackup()
                    {
                        NodeNumber = d.NodeNumber,
                        Name = d.Name
                    });
                }
            }

            Stream stream = null;
            try
            {
                stream = File.Open(PathFileName, FileMode.Create);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<DeviceBackup>));
                xmlSerializer.Serialize(stream, devices);
                Callback(string.Format("Exported {0} device names to '{1}'", devices.Count, Path.GetFileName(PathFileName)));
            }
            catch (Exception e)
            {
                Callback("Error saving " + PathFileName + ": (" + e.Message + ")");
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        public async static Task<RestoreSettingsResult> ImportDevicesAsync(string PathFileName)
        {
            var result = await ReadAsXMLFromDisk<List<DeviceBackup>>(PathFileName);

            if (result.HasError)
                return new RestoreSettingsResult(result.Message);

            List<DeviceBackup> devices = result.Data;
            int ImportedCount = 0;

            using (zvsContext context = new zvsContext())
            {
                foreach (Device d in await context.Devices.ToListAsync())
                {
                    DeviceBackup dev = devices.FirstOrDefault(o => o.NodeNumber == d.NodeNumber);
                    if (dev != null)
                    {
                        d.Name = dev.Name;
                        ImportedCount++;
                    }
                }
                await context.SaveChangesAsync();
            }

            return new RestoreSettingsResult(string.Format("Imported {0} device names from '{1}'", ImportedCount, Path.GetFileName(PathFileName)), PathFileName);
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
