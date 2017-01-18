using UnityEngine;
using UnityEditor;

namespace AtomPackageManager.Editors
{
    [CustomPropertyDrawer(typeof(AssemblyNameAttribute))]
    public class AssemblyNamePropertyDrawer : PropertyDrawer
    {
        public const float BUTTON_WIDTH = 40;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect textRect = position;
            textRect.width -= BUTTON_WIDTH;
            EditorGUI.DelayedTextField(textRect, property, label);

            Rect buttonRect = position;
            buttonRect.x += textRect.width;
            buttonRect.width = BUTTON_WIDTH;

            if(GUI.Button(buttonRect, "Find", EditorStyles.miniButtonRight))
            {

            }
        }
    }
}
