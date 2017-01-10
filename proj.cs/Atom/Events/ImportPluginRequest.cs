using AtomPackageManager.Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace AtomPackageManager
{
    public struct ImportPluginRequest
    {
        public readonly PluginImporter importer;
        public readonly AtomAssembly assembly;

        public ImportPluginRequest(PluginImporter importer, AtomAssembly assembly)
        {
            this.importer = importer;
            this.assembly = assembly;
        }
    }
}
