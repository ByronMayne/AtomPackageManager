using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomPackageManager
{
    public static class Events
    {
        public const int ON_PACKAGE_ADDED = 0;
        public const int ON_PACAKGE_REMOVED = 1;
        public const int GIT_CLONE_REQUESTED = 2;
        public const int ON_CLONE_PROGRESS_UPDATED = 3;
        public const int ON_CLONE_COMPLETE = 4;

        // Error Codes
        public const int ERROR_ATOM_YAML_NOT_FOUND = 100;
        public const int ERROR_INVALID_GIT_URL = 101;
    }
}
