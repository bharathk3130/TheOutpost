using UnityEditor;
using UnityEngine;

namespace Clickbait.Editor {
    public class CloseWindowTab : UnityEditor.Editor {
        [MenuItem("Tools/MyShortcuts/Close Window Tab")]
        static void CloseTab() {
            var focusedWindow = EditorWindow.focusedWindow;
            if (focusedWindow != null) {
                CloseTab(focusedWindow);
            } else {
                Debug.LogWarning("Found no focused window to close");
            }
        }

        static void CloseTab(EditorWindow editorWindow) {
            editorWindow.Close();
        }
    }
}
