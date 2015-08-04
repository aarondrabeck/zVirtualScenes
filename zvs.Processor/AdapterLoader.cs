using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace zvs.Processor
{
    public class AdapterLoader
    {
#pragma warning disable 649
        [ImportMany]
        private IEnumerable<ZvsAdapter> Adapters { get; set; }
#pragma warning restore 649

        public async Task<FindAdaptersResult> FindAdaptersAsync(string directoryName, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                Adapters = new List<ZvsAdapter>();
                var catalog = new SafeDirectoryCatalog(directoryName);
                var compositionContainer = new CompositionContainer(catalog);

                try
                {
                    compositionContainer.ComposeParts(this);
                }
                catch (CompositionException compositionException)
                {
                    return new FindAdaptersResult(compositionException.Message);
                }

                var msg = "All adapters loaded.";
                if (catalog.LoadErrors.Count > 0)
                    msg =
                        $@"The following plug-ins could not be loaded: {
                            string.Join(", " + Environment.NewLine, catalog.LoadErrors)}";
                return new FindAdaptersResult(Adapters, msg);
            }, cancellationToken);
        }

        public class FindAdaptersResult : Result
        {
            public IEnumerable<ZvsAdapter> Adapters { get; private set; }

            public FindAdaptersResult(string errorMessage) : base(true, errorMessage) { }

            public FindAdaptersResult(IEnumerable<ZvsAdapter> adapters, string message)
                : base(false, message)
            {
                Adapters = adapters;
            }
        }
    }
}
