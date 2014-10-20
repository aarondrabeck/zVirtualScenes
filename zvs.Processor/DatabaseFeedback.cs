using System;
using System.Diagnostics;
using System.Threading.Tasks;
using zvs.DataModel;

namespace zvs.Processor
{
    public class DatabaseFeedback : IFeedback<LogEntry>
    {
        private int MaxLogSize { get; set; }

        public DatabaseFeedback(int maxLogSize = 1000)
        {
            MaxLogSize = maxLogSize;
        }

        public async Task ReportAsync(LogEntry value, System.Threading.CancellationToken ct)
        {
            using (var context = new ZvsContext())
            {
                context.LogEntries.Add(value);

                //TODO: DELETE OLD LOG ENTRIES
               // if (await context.LogEntries.CountAsync(ct) > MaxLogSize)
                //    context.



                try
                {
                    await context.SaveChangesAsync(ct);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }
        public string Souce { get; set; }
    }
}
