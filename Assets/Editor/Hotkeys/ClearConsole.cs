using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Clickbait.Editor
{
    public class ClearConsole
    {
        [MenuItem("Tools/MyShortcuts/Clear console")]
        static void ClearConsoleFn()
        {
            // Try to find the Console window
            Type logEntriesType = Type.GetType("UnityEditor.LogEntries, UnityEditor");
            if (logEntriesType != null)
            {
                MethodInfo clearMethod = logEntriesType.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);

                if (clearMethod != null)
                {
                    clearMethod.Invoke(null, null);
                } else
                {
                    Debug.LogWarning("Failed to find the Clear method in LogEntries. Console may not be cleared.");
                }
            } else
            {
                Debug.LogWarning("Failed to find the LogEntries type. Console may not be cleared.");
            }
        }
    }
}
