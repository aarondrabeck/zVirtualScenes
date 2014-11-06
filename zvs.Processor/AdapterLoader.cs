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

        private async Task FindAdaptersAsync(string directoryName, CancellationToken cancellationToken)
        {
            var catalog = new SafeDirectoryCatalog(directoryName);
            var compositionContainer = new CompositionContainer(catalog);
            if (catalog.LoadErrors.Count > 0)
            {
                //await Log.ReportWarningFormatAsync(cancellationToken, @"The following plug-ins could not be loaded: {0}",
                //    string.Join(", " + Environment.NewLine, catalog.LoadErrors));
            }

            try
            {
                compositionContainer.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
        }
    }
}
