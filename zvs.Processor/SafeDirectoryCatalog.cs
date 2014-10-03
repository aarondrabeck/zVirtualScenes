using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;

namespace zvs.Processor
{
    public class SafeDirectoryCatalog : ComposablePartCatalog
    {
        private readonly AggregateCatalog _catalog;
        public readonly List<string> LoadErrors;

        public SafeDirectoryCatalog(string directory)
        {
            LoadErrors = new List<string>();
            var files = Directory.EnumerateFiles(directory, "*.dll", SearchOption.AllDirectories);
            _catalog = new AggregateCatalog();

            foreach (var file in files)
            {
                try
                {
                    var asmCat = new AssemblyCatalog(file);

                    //Force MEF to load the plug-in and figure out if there are any exports
                    // good assemblies will not throw the RTLE exception and can be added to the catalog
                    if (asmCat.Parts.ToList().Count > 0)
                        _catalog.Catalogs.Add(asmCat);
                }
                catch (ReflectionTypeLoadException ex)
                {
                    foreach(var error in ex.LoaderExceptions)
                        LoadErrors.Add(string.Format("Error loading '{0}': {1}", file, error.Message));
                }
                catch (Exception ex)
                {
                    LoadErrors.Add(string.Format("Error loading '{0}': {1}", file, ex.Message));
                }
            }
        }
        public override IQueryable<ComposablePartDefinition> Parts
        {
            get { return _catalog.Parts; }
        }
    }
}
