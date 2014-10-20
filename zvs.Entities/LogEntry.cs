using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.DataModel
{
    public class LogEntry
    {
        public LogEntry(ITimeProvider provider)
        {
            Datetime = provider.Time;
        }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime Datetime { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public LogEntryLevel Level { get; set; }

        public override string ToString()
        {
            return String.Format("{0:yyyy-MM-dd-hh:mm:ss:fff}|{1,-20}|{2}", Datetime, Source, Message);
        }
    }
}
