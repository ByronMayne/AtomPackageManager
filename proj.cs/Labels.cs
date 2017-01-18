
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
            packageCompileButton = new GUIContent("Compile Package");
            packageEditorSettingsButton = new GUIContent("*");
            packageEditorSaveButton = new GUIContent("Save");
            addExistingPackageButton = new GUIContent("Add Existing Package...");
            clonePackageButton = new GUIContent("Clone Package...");
            createNewPackageButton = new GUIContent("Create New Package...");
            closeAtomButton = new GUIContent("Close Atom");
            menuButton = new GUIContent("Atom");
        }

        public static readonly GUIContent packageEditorAddButton;
        public static readonly GUIContent packageEditorSaveButton;
        public static readonly GUIContent packageEditorRemoveButton;
        public static readonly GUIContent packageEditorSettingsButton;
        public static readonly GUIContent packageEditorTitle;
        public static readonly GUIContent packageCompileButton;
        public static readonly GUIContent addExistingPackageButton;
        public static readonly GUIContent clonePackageButton;
        public static readonly GUIContent closeAtomButton;
        public static readonly GUIContent createNewPackageButton;
        public static readonly GUIContent menuButton;
    }
}
