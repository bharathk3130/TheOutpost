using UnityEditor;
using UnityEngine;

namespace Clickbait.Editor
{
    [InitializeOnLoad]
    public class SceneViewFollowObject
    {
        static GameObject _followObject; // The object to follow
        static bool _isFollowing; // Track if we are currently following the object
        static GameObject _topmostParent; // The topmost parent of the followed object

        // For delaying stop-following after the game ends
        static int _framesSinceGameEnded = 0; // Counter for frames after game ends
        static bool _waitingToStopFollowing = false; // Flag to track if we are waiting to stop following

        static SceneViewFollowObject()
        {
            // Subscribe to the SceneView event
            SceneView.duringSceneGui += OnSceneGUI;

            // Subscribe to the update event to check when the game stops
            EditorApplication.update += OnEditorUpdate;

            // Listen for when the game starts/stops
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            // Listen for selection changes
            Selection.selectionChanged += OnSelectionChanged;

            // Load persisted following state
            _isFollowing = EditorPrefs.GetBool("FollowObject_IsFollowing", false);
        }

        // Method to start following the GameObject
        static void StartFollowing(GameObject target)
        {
            if (target == null) return;

            // Find the topmost parent of the selected object
            _topmostParent = GetTopmostParent(target);

            _followObject = target;
            _isFollowing = true;

            // Persist the following state
            EditorPrefs.SetBool("FollowObject_IsFollowing", true);

            // Reset frame counter and flag when starting to follow
            _framesSinceGameEnded = 0;
            _waitingToStopFollowing = false;

            // Repaint the Scene view immediately
            SceneView.RepaintAll();
        }

        // Method to stop following the GameObject
        static void StopFollowing()
        {
            _followObject = null;
            _topmostParent = null;
            _isFollowing = false;

            // Persist the following state
            EditorPrefs.SetBool("FollowObject_IsFollowing", false);

            // Reset the frame counter and flag
            _framesSinceGameEnded = 0;
            _waitingToStopFollowing = false;

            // Repaint the Scene view to reset the view
            SceneView.RepaintAll();
        }

        // Method to handle Scene view updates
        static void OnSceneGUI(SceneView sceneView)
        {
            if (_followObject != null && _isFollowing)
            {
                // Focus on the GameObject's position
                Vector3 objectPosition = _followObject.transform.position;

                // Get the camera's current distance and direction from the target object
                Camera sceneCamera = sceneView.camera;

                // Adjust the camera's position to follow the target while allowing rotations and pans
                sceneView.LookAt(objectPosition, sceneCamera.transform.rotation);

                // Repaint the Scene view to update the focus
                sceneView.Repaint();
            }
        }

        // Method to handle selection change
        static void OnSelectionChanged()
        {
            // If there's no follow object, no need to check
            if (_followObject == null) return;

            // If the selected object or one of its children is not part of the hierarchy of the topmost parent, stop following
            if (!IsInSameHierarchy(_topmostParent, Selection.activeGameObject))
            {
                StopFollowing();
            }
        }

        // Method to check if the selected object or its ancestors match the topmost parent
        static bool IsInSameHierarchy(GameObject topmostParent, GameObject selectedObject)
        {
            if (selectedObject == null) return false;

            // Traverse up the hierarchy of the selected object
            Transform parent = selectedObject.transform;
            while (parent != null)
            {
                if (parent.gameObject == topmostParent)
                {
                    return true; // The selected object or one of its ancestors is in the same hierarchy
                }
                parent = parent.parent;
            }

            return false;
        }

        // Method to get the topmost parent of the object
        static GameObject GetTopmostParent(GameObject target)
        {
            Transform parent = target.transform;
            while (parent.parent != null)
            {
                parent = parent.parent;
            }
            return parent.gameObject;
        }

        // Add menu item "Focus & Follow" to the Unity Editor
        [MenuItem("Tools/MyShortcuts/Focus and Follow")]
        static void FocusAndFollowSelectedObject()
        {
            GameObject selectedObject = Selection.activeGameObject;

            // If the currently selected object is the same as the one we are following, stop following
            if (_isFollowing && _followObject == selectedObject)
            {
                StopFollowing();
            }
            else if (selectedObject != null)
            {
                // Start following the selected object if it's not already being followed
                StartFollowing(selectedObject);
            }
        }

        // Method to handle play mode state changes
        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                // Start waiting to stop following after the game ends
                _waitingToStopFollowing = true;
                _framesSinceGameEnded = 0;
            }
            else if (state == PlayModeStateChange.EnteredPlayMode)
            {
                // Reset everything when entering play mode
                _waitingToStopFollowing = false;
                _framesSinceGameEnded = 0;

                if (_isFollowing)
                {
                    // If we're following before entering play mode, call follow again after entering play mode
                    _isFollowing = false;
                    FocusAndFollowSelectedObject();
                }
            }
        }

        // Method to stop following after a specified number of frames when the game stops
        static void OnEditorUpdate()
        {
            if (!EditorApplication.isPlaying && _waitingToStopFollowing)
            {
                // Increment frame counter when game is not playing
                _framesSinceGameEnded++;

                if (_framesSinceGameEnded >= 30)
                {
                    StopFollowing(); // Stop following after 30 frames
                }
            }
        }
    }
}