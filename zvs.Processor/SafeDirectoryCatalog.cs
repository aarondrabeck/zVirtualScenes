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
        private AggregateCatalog Catalog { get; set; }
        public List<string> LoadErrors { get; set; }
        private string DirectoryPath { get; set; }

        public SafeDirectoryCatalog(string directoryPath)
        {
            DirectoryPath = directoryPath;
            LoadErrors = new List<string>();
        }

        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                var files = Directory.EnumerateFiles(DirectoryPath, "*.dll", SearchOption.AllDirectories);
                Catalog = new AggregateCatalog();

                foreach (var file in files)
                {
                    try
                    {
                        var asmCat = new AssemblyCatalog(file);

                        //Force MEF to load the plug-in and figure out if there are any exports
                        // good assemblies will not throw the RTLE exception and can be added to the catalog
                        if (asmCat.Parts.ToList().Count > 0)
                            Catalog.Catalogs.Add(asmCat);
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        foreach (var error in ex.LoaderExceptions)
                            LoadErrors.Add(string.Format("Error loading '{0}': {1}", file, error.Message));
                    }
                    catch (Exception ex)
                    {
                        LoadErrors.Add(string.Format("Error loading '{0}': {1}", file, ex.Message));
                    }
                }

                return Catalog.Parts;
            }
        }
    }
}
