﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomPackageManager
{
    public static class Constants
    {
        /// <summary>
        /// The Root Menu Item path where all of the atom options go. 
        /// </summary>
        public const string MENU_ITEM_ROOT = "Atom";

        /// <summary>
        /// The location on disk starting form the root project path where we
        /// save the package manager on disk.
        /// </summary>
        public const string PACKAGE_MANAGER_LOCATION = "../ProjectSettings/PackageManager.asset";

        /// <summary>
        /// The location on disk where we store the repositories for our Atom packages.
        /// </summary>
        public const string SCRIPT_IMPORT_DIRECTORY = "/Library/Atom/";
    }
}