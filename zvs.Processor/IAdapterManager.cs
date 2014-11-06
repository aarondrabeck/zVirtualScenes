using System;
using System.ComponentModel.Composition.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace zvs.Processor
{
    public interface IAdapterManager
    {
        ZvsAdapter GetZvsAdapterByGuid(Guid adapterGuid);
        Task EnableAdapterAsync(Guid adapterGuid, CancellationToken cancellationToken);
        Task DisableAdapterAsync(Guid adapterGuid, CancellationToken cancellationToken);
        Task InitializeAdaptersAsync(CancellationToken cancellationToken);
    }
}
