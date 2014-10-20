using System;
using System.Collections;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace zvs.Processor.Backup
{
    public abstract class BackupRestore
    {
        public abstract string Name { get; }
        public abstract string FileName { get; }

        public abstract Task<Result> ExportAsync(string fileName, CancellationToken cancellationToken);

        public abstract Task<RestoreSettingsResult> ImportAsync(string fileName, CancellationToken cancellationToken);

        protected async Task<Result> SaveAsXMLToDiskAsync<T>(T objCollection, string fileName) where T : IEnumerable
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(stream, objCollection);
                    stream.Position = 0;

                    using (var reader = new StreamReader(stream))
                    {
                        var output = reader.ReadToEnd();

                        // Write the string to a file.
                        using (var file = new StreamWriter(fileName))
                            await file.WriteLineAsync(output);
                    }
                }
                return Result.ReportSuccess();
            }
            catch (Exception e)
            {
                return Result.ReportErrorFormat("Error saving {0}: {1}", fileName, e.Message);
            }
        }

        protected async Task<ReadAsXmlFromDiskResult<T>> ReadAsXMLFromDiskAsync<T>(string fileName)
        {
            try
            {
                if (!File.Exists(fileName))
                    return ReadAsXmlFromDiskResult<T>.ReportError(string.Format("{0} not found", fileName));

                //Open the file written above and read values from it. 
                //http://stackoverflow.com/questions/1127431/xmlserializer-giving-filenotfoundexception-at-constructor
                var scenesSerializer = new XmlSerializer(typeof(T));

                string fileData;
                using (var streamReader = new StreamReader(fileName))
                    fileData = await streamReader.ReadToEndAsync();

                var data = (T) scenesSerializer.Deserialize(new StringReader(fileData));
                return ReadAsXmlFromDiskResult<T>.ReportSuccess(data);
            }
            catch (Exception e)
            {
                return ReadAsXmlFromDiskResult<T>.ReportError(string.Format("Error reading {0}: {1}", fileName, e.Message));
            }
        }

        public class ReadAsXmlFromDiskResult<T> : Result
        {
            public T Data { get; private set; }

            public static ReadAsXmlFromDiskResult<T> ReportSuccess(T data)
            {
                return new ReadAsXmlFromDiskResult<T>(false, string.Empty, data);
            }

            public new static ReadAsXmlFromDiskResult<T> ReportError(string errorMessage)
            {
                return new ReadAsXmlFromDiskResult<T>(true, errorMessage);
            }

            private ReadAsXmlFromDiskResult(bool hasError, string message, T data)
                : base(hasError, message)
            {
                Data = data;
            }

            private ReadAsXmlFromDiskResult(bool hasError, string message)
                : base(hasError, message)
            {
            }
        }

        public class RestoreSettingsResult : Result
        {
            public string FilePath { get; private set; }

            public static RestoreSettingsResult ReportSuccess(string filePath)
            {
                return new RestoreSettingsResult(false, string.Empty, filePath);
            }

            public new static RestoreSettingsResult ReportError(string errorMessage)
            {
                return new RestoreSettingsResult(true, errorMessage, string.Empty);
            }

            private RestoreSettingsResult(bool hasError, string message, string filePath)
                : base(hasError, message)
            {
                FilePath = filePath;
            }
        }
    }
}
