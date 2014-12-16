using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace zvs.DataModel
{
    public class InMemoryFeedback : IFeedback<LogEntry>
    {
        public InMemoryFeedback()
        {
            LogEntries = new ObservableCollection<LogEntry>();
        }

        public ObservableCollection<LogEntry> LogEntries { get; set; }

        public Task ReportAsync(LogEntry value, CancellationToken ct)
        {
            LogEntries.Add(value);
            return Task.FromResult(0);
        }
        public string Source { get; set; }
    }
}
