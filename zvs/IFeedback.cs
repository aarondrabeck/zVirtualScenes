using System.Threading;
using System.Threading.Tasks;

namespace zvs
{
    public interface IFeedback<in TLogItem>
    {
        Task ReportAsync(TLogItem value, CancellationToken ct);
        string Source { get; set; }
    }
}
