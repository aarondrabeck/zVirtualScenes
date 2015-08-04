using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace zvs.Processor
{
    public interface IAdapterManager
    {
        IReadOnlyList<ZvsAdapter> GetZvsAdapters();
        ZvsAdapter FindZvsAdapter(Guid adapterGuid);
        Task<Result> EnableAdapterAsync(Guid adapterGuid, CancellationToken cancellationToken);
        Task<Result> DisableAdapterAsync(Guid adapterGuid, CancellationToken cancellationToken);
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}
