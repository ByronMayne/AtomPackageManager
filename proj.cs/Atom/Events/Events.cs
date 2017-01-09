using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomPackageManager
{
    public static class Events
    {
        // Package adding and removing
        public const int ON_PACKAGE_ADDED = 0;
        public const int ON_PACAKGE_REMOVED = 1;

        // Source Control
        public const int GIT_CLONE_REQUESTED = 50;
        public const int ON_CLONE_PROGRESS_UPDATED = 51;
        public const int ON_CLONE_COMPLETE = 52;

        // Error Codes
        public const int ERROR_ATOM_YAML_NOT_FOUND = 100;
        public const int ERROR_INVALID_GIT_URL = 101;
        public const int ERROR_PACKING_WAS_NULL = 102;
        public const int ERROR_INVALID_CAST_FOR_COMPILER = 103;

        // Compiling
        public const int COMPILE_PACKAGE_REQUEST = 150;
        public const int COMPILE_COMPLETE = 151;
    }
}
