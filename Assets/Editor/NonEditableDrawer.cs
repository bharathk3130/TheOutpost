using UnityEditor;
using UnityEngine;

namespace Clickbait.Utilities
{
    [CustomPropertyDrawer(typeof(NonEditableAttribute))]
    public class NonEditableDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false; // Disable editing
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true; // Enable editing back
        }
    }
}