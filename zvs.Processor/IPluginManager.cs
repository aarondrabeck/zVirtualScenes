using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace zvs.Processor
{
    public interface IPluginManager
    {
        IReadOnlyList<ZvsPlugin> GetZvsPlugins();
        ZvsPlugin FindZvsPlugin(Guid pluginGuid);
        Task<Result> EnablePluginAsync(Guid pluginGuid, CancellationToken cancellationToken);
        Task<Result> DisablePluginAsync(Guid pluginGuid, CancellationToken cancellationToken);
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}
