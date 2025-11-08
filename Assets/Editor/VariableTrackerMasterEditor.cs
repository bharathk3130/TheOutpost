using Clickbait.Debugging;
using UnityEditor;

/// <summary>
/// Displays the list of all variables being tracked in the editor under the VariableTrackerMaster component with
/// checkboxes to toggle on/off the tracking of each variable.
/// </summary>
namespace Clickbait.Editor
{
    [CustomEditor(typeof(VariableTrackerMaster))]
    public class VariableTrackerMasterEditor : UnityEditor.Editor
    {
        SerializedProperty trackersProperty;

        void OnEnable()
        {
            // Cache the SerializedProperty for trackers
            trackersProperty = serializedObject.FindProperty("Trackers");
        }

        public override void OnInspectorGUI()
        {
            // Get the instance of the VariableTrackerMaster
            VariableTrackerMaster trackerMaster = (VariableTrackerMaster)target;

            // Draw the default inspector first to show other serialized fields
            DrawDefaultInspector();

            // Ensure trackersProperty is not null
            if (trackersProperty != null)
            {
                // Loop through each element in the list of trackers and display a checkbox for the Track property
                for (int i = 0; i < trackersProperty.arraySize; i++)
                {
                    var tracker = trackersProperty.GetArrayElementAtIndex(i);

                    // Access the Track and VariableName properties directly from TrackerBase
                    var trackField = tracker.FindPropertyRelative("Track");
                    var variableNameField = tracker.FindPropertyRelative("VariableName");

                    // Draw the checkbox for Track property
                    if (trackField != null)
                    {
                        trackField.boolValue =
                            EditorGUILayout.Toggle(variableNameField.stringValue, trackField.boolValue);
                    }
                }

                // Mark the object as dirty to apply changes made in the inspector
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}