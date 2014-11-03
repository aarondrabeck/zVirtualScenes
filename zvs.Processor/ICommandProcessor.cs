using System.Threading;
using System.Threading.Tasks;

namespace zvs.Processor
{
    public interface ICommandProcessor
    {
        Task<Result> RunCommandAsync(int? commandId, string argument, string argument2, CancellationToken cancellationToken);
    }
}
