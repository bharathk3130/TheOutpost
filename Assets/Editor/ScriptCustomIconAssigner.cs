using UnityEditor;
using UnityEngine;
using System;

namespace Clickbait.Editor
{
    [InitializeOnLoad]
    public static class ScriptCustomIconAssigner
    {
        static ScriptCustomIconAssigner()
        {
            // Load the icons
            Texture2D abstractClassIcon =
                AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Icons/AbstractClassIcon.png");
            Texture2D interfaceIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Icons/InterfaceIcon.png");

            // Find all scripts in the project, including those in subfolders of Assets/_Project/Scripts
            string[] guids = AssetDatabase.FindAssets("t:Script", new[] { "Assets/_Project/Scripts" });

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                Type scriptClass = script.GetClass();

                if (scriptClass == null)
                    continue;

                // Check if the type is an abstract class or an interface
                if (scriptClass.IsAbstract && !scriptClass.IsInterface)
                {
                    EditorGUIUtility.SetIconForObject(script, abstractClassIcon);
                } else if (scriptClass.IsInterface)
                {
                    EditorGUIUtility.SetIconForObject(script, interfaceIcon);
                }
            }

            // Repaint the Project window to apply icon changes
            EditorApplication.RepaintProjectWindow();

            // Refresh the Asset Database to ensure icon changes are reflected
            AssetDatabase.Refresh();
        }
    }
}