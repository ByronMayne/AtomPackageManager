using AtomPackageManager.Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomPackageManager
{
    public struct SerilizationRequest
    {
        public string sourceURL;
        public string serializedDataPath;
        public AtomPackage package;

        public SerilizationRequest(string atomFilePath, string sourceURL)
        {
            this.sourceURL = sourceURL;
            this.serializedDataPath = atomFilePath;
            this.package = null;
        }
    }
}
