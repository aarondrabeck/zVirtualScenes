using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using zvs.Processor.Backup;

namespace zvs.Processor.Tests
{
    public class FileIOTests
    {

        [Fact]
        public async Task ReadWriteRoundTripTest()
        {
            var samples = new List<SampleClass>();
            samples.Add(new SampleClass() { Number = 5312, Text = "Hello World.", Field ="abc123" });
            samples.Add(new SampleClass() { Number = 4587, Text = "Testing 12345", Field = "fieldtest" });

            //Write to disk
            var file = Path.Combine(AssemblyDirectory, "ReadWriteTestUnitTest.xml");
            var result = await BackFileIO.SaveAsXMLToDiskAsync(samples, file);
            Assert.False(result.HasError);

            //Read to disk
            var readResult = await BackFileIO.ReadAsXMLFromDiskAsync<List<SampleClass>>(file);

            Assert.False(readResult.HasError);
            Assert.Equal(2, readResult.Data.Count());

            Assert.Equal(samples[0].Number, readResult.Data[0].Number);
            Assert.Equal(samples[0].Text, readResult.Data[0].Text);
            Assert.Equal(samples[0].Field, readResult.Data[0].Field);

            Assert.Equal(samples[1].Number, readResult.Data[1].Number);
            Assert.Equal(samples[1].Text, readResult.Data[1].Text);
            Assert.Equal(samples[1].Field, readResult.Data[1].Field);
        }

        public class SampleClass
        {
            public int Number { get; set; }
            public string Text { get; set; }

            public string Field = string.Empty;
        }

        static public string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
