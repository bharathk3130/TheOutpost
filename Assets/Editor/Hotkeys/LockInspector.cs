using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Clickbait.Editor {
    class LockInspector : MonoBehaviour {
        [MenuItem("Tools/MyShortcuts/Toggle Inspector Lock")]
        public static void Lock () {
            ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
            
            foreach (var activeEditor in ActiveEditorTracker.sharedTracker.activeEditors) {
                if (activeEditor.target is not Transform) continue;
                
                var transform = (Transform)activeEditor.target;

                var propInfo = transform.GetType().GetProperty("constrainProportionsScale", BindingFlags.NonPublic | BindingFlags.Instance);
                if (propInfo == null) continue;
                    
                var currentValue = (bool)propInfo.GetValue(transform, null);
                propInfo.SetValue(transform, !currentValue, null);
            }
            
            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }

        [MenuItem("Edit/Toggle Inspector Lock", true)]
        public static bool Valid () {
            return ActiveEditorTracker.sharedTracker.activeEditors.Length != 0;
        }
    }
}