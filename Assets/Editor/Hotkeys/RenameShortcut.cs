using UnityEngine;
using UnityEditor;

namespace Clickbait.Editor
{
    public class RenameShortcut
    {
        [MenuItem("Tools/MyShortcuts/Rename Selected Object")]
        public static void RenameSelectedObject()
        {
            // Check if the focused window is the Hierarchy window
            if (EditorWindow.focusedWindow.titleContent.text == "Hierarchy")
            {
                RenameSelectedGameObject();
            }
            // Check if the focused window is the Project window
            else if (EditorWindow.focusedWindow.titleContent.text == "Project")
            {
                RenameSelectedAsset();
            }
        }

        private static void RenameSelectedGameObject()
        {
            GameObject selectedGameObject = Selection.activeGameObject;
            if (selectedGameObject != null)
            {
                EditorWindow.focusedWindow.SendEvent(new Event { keyCode = KeyCode.F2, type = EventType.KeyDown });
            } else
            {
                Debug.LogWarning("No GameObject selected in the Hierarchy to rename.");
            }
        }

        private static void RenameSelectedAsset()
        {
            Object selectedAsset = Selection.activeObject;
            if (selectedAsset != null)
            {
                EditorWindow.focusedWindow.SendEvent(new Event { keyCode = KeyCode.F2, type = EventType.KeyDown });
            } else
            {
                Debug.LogWarning("No asset selected in the Project window to rename.");
            }
        }
    }
}