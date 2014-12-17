using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.DataModel
{
    [Table("LogEntries", Schema = "ZVS")]
    public class LogEntry
    {
        public LogEntry()
        {
        }

        public LogEntry(ITimeProvider provider)
        {
            Datetime = provider.Time;
        }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime Datetime { get; set; }

        [NotMapped]
        public DateTimeOffset DateTimeOffset
        {
            get { return new DateTimeOffset(Datetime, TimeSpan.FromHours(0)); }
            set { Datetime = value.UtcDateTime; }
        }

        public string Message { get; set; }
        public string Source { get; set; }
        public LogEntryLevel Level { get; set; }

        public override string ToString()
        {
            return String.Format("{0:yyyy-MM-dd-hh:mm:ss:fff}|{1,-20}|{2,-20}|{3}", Datetime, Source, Level, Message);
        }
    }
}
