
using UnityEngine;

namespace AtomPackageManager
{
    internal class Labels
    {
        static Labels()
        {
            packageEditorTitle = new GUIContent("Package Editor");
            packageEditorAddButton = new GUIContent("Add", "Opens a window to allow you to add a new package based on a url");
            packageEditorRemoveButton = new GUIContent("Remove", "Removes the currently selected package from your project");
            packageEditorSettingsButton = new GUIContent("*");
            packageEditorSaveButton = new GUIContent("Save");
        }

        public static readonly GUIContent packageEditorAddButton;
        public static readonly GUIContent packageEditorSaveButton;
        public static readonly GUIContent packageEditorRemoveButton;
        public static readonly GUIContent packageEditorSettingsButton;
        public static readonly GUIContent packageEditorTitle;
    }
}
