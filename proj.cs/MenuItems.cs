using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace AtomPackageManager
{
    internal static class MenuItems
    {
        [MenuItem(Constants.MENU_ITEM_ROOT + "/Package Manager...")]
        private static void ShowPackageEditor()
        {
            EditorWindow.GetWindow<PackageEditor>();
        }
    }
}
