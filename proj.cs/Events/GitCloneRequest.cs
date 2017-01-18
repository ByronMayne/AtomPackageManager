using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomPackageManager
{
    public struct GitCloneRequest
    {
        public readonly string name;
        public readonly string sourceURL;
        public readonly string workingDirectory;

        public GitCloneRequest(string name, string sourceURL)
        {
            this.name = name;
            this.sourceURL = sourceURL;
            workingDirectory = null;// Constants.scriptImportLocation + name + '/';
        }
    }
}
