using UnityEditor;
using UnityEngine;

namespace Clickbait.Editor
{
    public class ResetTransformShortcuts
    {
        [MenuItem("Tools/MyShortcuts/Transform/Reset Position")]
        static void ResetPosition()
        {
            if (Selection.activeGameObject != null)
            {
                Undo.RecordObject(Selection.activeGameObject.transform, "Reset Position");
                Selection.activeGameObject.transform.localPosition = Vector3.zero;
            }
        }

        [MenuItem("Tools/MyShortcuts/Transform/Reset Rotation")]
        static void ResetRotation()
        {
            if (Selection.activeGameObject != null)
            {
                Undo.RecordObject(Selection.activeGameObject.transform, "Reset Rotation");
                Selection.activeGameObject.transform.localRotation = Quaternion.identity;
            }
        }

        [MenuItem("Tools/MyShortcuts/Transform/Reset Scale")]
        static void ResetScale()
        {
            if (Selection.activeGameObject != null)
            {
                Undo.RecordObject(Selection.activeGameObject.transform, "Reset Scale");
                Selection.activeGameObject.transform.localScale = Vector3.one;
            }
        }

        [MenuItem("Tools/MyShortcuts/Transform/Reset Transform")]
        static void ResetTransform()
        {
            if (Selection.activeGameObject != null)
            {
                Undo.RecordObject(Selection.activeGameObject.transform, "Reset Transform");
                ResetPosition();
                ResetRotation();
                ResetScale();
            }
        }
    }
}