using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Clickbait.Editor
{
    public class ToggleInspectorDebug
    {
        [MenuItem("Tools/MyShortcuts/Toggle Inspector Debugging")] // Change the shortcut here
        static void ToggleInspectorDebugging()
        {
            // Iterate through all open EditorWindows to find InspectorWindow
            EditorWindow[] windows = Resources.FindObjectsOfTypeAll<EditorWindow>();
            foreach (var window in windows)
            {
                if (window.GetType().Name == "InspectorWindow")
                {
                    Type type = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.InspectorWindow");
                    FieldInfo field = type.GetField("m_InspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);

                    InspectorMode mode = (InspectorMode)field.GetValue(window);
                    mode = (mode == InspectorMode.Normal ? InspectorMode.Debug : InspectorMode.Normal);

                    MethodInfo method = type.GetMethod("SetMode", BindingFlags.NonPublic | BindingFlags.Instance);
                    method.Invoke(window, new object[] { mode });

                    window.Repaint(); // Refresh inspector
                    break; // Optional: break if you only want to toggle the first InspectorWindow found
                }
            }
        }
    }
}