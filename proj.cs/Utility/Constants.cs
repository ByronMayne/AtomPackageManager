using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

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
        public const string PACKAGE_MANAGER_LOCATION = "/ProjectSettings/PackageManager.asset";

        /// <summary>
        /// The extension of a solution file. 
        /// </summary>
        public const string SOLUTION_EXTENSION = ".sln";

        public const string GENERATED_SOLUTIONS_DIRECTORY_NAME = "Generated Solutions";

        public const float MIN_WINDOW_SIZE = 580f;
    }
}
