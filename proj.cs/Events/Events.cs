﻿using System;
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
        public const int ON_PACKAGE_MANAGER_LOADED = 2;

        // Source Control
        public const int GIT_CLONE_REQUESTED = 50;
        public const int ON_CLONE_PROGRESS_UPDATED = 51;
        public const int ON_CLONE_COMPLETE = 52;

        // Error Codes
        public const int ERROR_ATOM_YAML_NOT_FOUND = 100;
        public const int ERROR_INVALID_GIT_URL = 101;
        public const int ERROR_PACKING_WAS_NULL = 102;
        public const int ERROR_INVALID_CAST_FOR_COMPILER = 103;
		public const int ERROR_CANT_PARSE_ATOM_FILE = 104;

        // Compiling
        public const int COMPILE_PACKAGE_REQUEST = 150;
        public const int COMPILE_COMPLETE = 151;

        // Serialization 
        public const int SERIALIZATION_REQUEST = 200;
        public const int SERIALIZATION_COMPLETE = 201;
        public const int DESERIALIZATION_REQUEST = 202;
        public const int DESERIALIZATION_COMPLETE = 203;

        // Importing 
        public const int PLUGIN_IMPORTED = 250;
        public const int PLUGIN_SET_IMPORTER_SETTINGS_REQUEST = 251;
    }
}
