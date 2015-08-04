using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace zvs.DataModel
{
    public class DatabaseFeedback : IFeedback<LogEntry>
    {
        private IEntityContextConnection EntityContextConnection { get; }

        public DatabaseFeedback(IEntityContextConnection entityContextConnection)
        {
            EntityContextConnection = entityContextConnection;
        }

        public async Task ReportAsync(LogEntry value, CancellationToken ct)
        {
            using (var context = new ZvsContext(EntityContextConnection))
            {
                context.LogEntries.Add(value);

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
        public string Source { get; set; }
    }
}
