using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomPackageManager
{
    public struct GitCloneRequest
    {
        public readonly string sourceURL;
        public readonly string workingDirectory;

        public GitCloneRequest(string sourceURL, string workingDirectory)
        {
            this.sourceURL = sourceURL;
            this.workingDirectory = workingDirectory;
        }
    }
}
