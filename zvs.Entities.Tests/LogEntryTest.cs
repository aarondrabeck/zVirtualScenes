using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using zvs.Fakes;

namespace zvs.DataModel.Tests
{
    [TestClass]
    public class LogEntryTest
    {
        [TestMethod]
        public void LogEntryToStringTest()
        {
            //Arrange 
            var time = new StubITimeProvider {TimeGet = () => new DateTime(2000, 01, 02, 13, 1, 40, 100)};
            var entry = new LogEntry(time) {Source = "UnitTest", Message = "Sample log entry"};

            //Assert
            var result = entry.ToString();

            //Act
            Assert.AreEqual("2000-01-02-01:01:40:100|UnitTest            |Sample log entry", result);
        }
    }
}
