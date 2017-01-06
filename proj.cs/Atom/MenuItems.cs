using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace AtomPackageManager
{
    internal static class MenuItems
    {
        private const string MENU_ROOT = "Atom";
        
        [MenuItem(MENU_ROOT + "/Create Package..")]
        private static void CreateAtomPackage()
        {

        }

        [MenuItem(MENU_ROOT + "/Load Package..")]
        private static void LoadPackage()
        {

        }
    }
}
