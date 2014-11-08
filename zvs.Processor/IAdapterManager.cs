using System;
using System.ComponentModel.Composition.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace zvs.Processor
{
    public interface IAdapterManager
    {
        ZvsAdapter GetZvsAdapterByGuid(Guid adapterGuid);
        Task<Result> EnableAdapterAsync(Guid adapterGuid, CancellationToken cancellationToken);
        Task<Result> DisableAdapterAsync(Guid adapterGuid, CancellationToken cancellationToken);
        Task InitializeAdaptersAsync(CancellationToken cancellationToken);
    }
}
