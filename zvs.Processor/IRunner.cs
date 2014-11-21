using System.Threading;
using System.Threading.Tasks;

namespace zvs.Processor
{
    public interface IRunner
    {
        Task StartAsync(CancellationToken ct);
        Task StopAsync(CancellationToken ct);
    }
}
