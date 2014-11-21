using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace zvs.Processor
{
    public class PluginLoader
    {
#pragma warning disable 649
        [ImportMany]
        private IEnumerable<ZvsPlugin> Plugins { get; set; }
#pragma warning restore 649

        public async Task<FindPluginsResult> FindPluginsAsync(string directoryName, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                Plugins = new List<ZvsPlugin>();
                var catalog = new SafeDirectoryCatalog(directoryName);
                var compositionContainer = new CompositionContainer(catalog);

                try
                {
                    compositionContainer.ComposeParts(this);
                }
                catch (CompositionException compositionException)
                {
                    return new FindPluginsResult(compositionException.Message);
                }

                var msg = "All plugins loaded.";
                if (catalog.LoadErrors.Count > 0)
                    msg = string.Format(@"The following plug-ins could not be loaded: {0}", string.Join(", " + Environment.NewLine, catalog.LoadErrors));
                return new FindPluginsResult(Plugins, msg);
            }, cancellationToken);
        }

        public class FindPluginsResult : Result
        {
            public IEnumerable<ZvsPlugin> Plugins { get; private set; }

            public FindPluginsResult(string errorMessage) : base(true, errorMessage) { }

            public FindPluginsResult(IEnumerable<ZvsPlugin> plugins, string message)
                : base(false, message)
            {
                Plugins = plugins;
            }
        }
    }
}
