using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;

namespace zvs.Processor
{
    public interface ICommandProcessor
    {
        Task<Result> RunStoredCommandAsync(object sender, int storedCommandId, CancellationToken cancellationToken);

        Task<Result> RunCommandAsync(object sender, Command command, string argument, string argument2, CancellationToken cancellationToken);
    }
}
