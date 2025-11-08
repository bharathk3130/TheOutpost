using System;
using System.Collections.Generic;
using Clickbait.Utilities;
using UnityEngine;

/// <summary>
/// Handles variable tracking logic. VariableTrackerUI displays the tracked variable as UI.
/// The VariableTrackerMasterEditor displays the list of all variables being tracked in the editor under this component
/// with checkboxes to toggle on/off the tracking of each variable.
/// 
/// Run this once to start tracking a variable:
///     AddTracker("CurrentLevel", () => saveLoadController.CurrentLevel);
///
/// Run this to stop tracking a variable:
///     RemoveTracker("CurrentLevel");
/// </summary>
namespace Clickbait.Debugging
{
    public class VariableTrackerMaster : PrivateSingleton<VariableTrackerMaster>
    {
        [Header("Variables to track")] [HideInInspector]
        public List<TrackerBase> Trackers = new();

        void Update()
        {
            Trackers.ForEach(t => t.Update());
        }

        public static void AddTracker<T>(string variableName, Func<T> getter) =>
            _instance?.AddTrackerHelper(variableName, getter);

        void AddTrackerHelper<T>(string variableName, Func<T> getter)
        {
            if (Trackers.Find(t => t.VariableName == variableName) is Tracker<T> matchedTracker)
            {
                matchedTracker.ChangeGetter(getter);
            }

            Tracker<T> tracker = new Tracker<T>();
            tracker.Init(variableName, getter);
            Trackers.Add(tracker);
        }

        public static void RemoveTracker<T>(string variableName) =>
            _instance?.RemoveTrackerHelper<T>(variableName);

        void RemoveTrackerHelper<T>(string variableName)
        {
            if (Trackers.Find(t => t.VariableName == variableName) is Tracker<T> matchedTracker)
            {
                Trackers.Remove(matchedTracker);
                VariableTrackerUI.StopTracking(matchedTracker.VariableName);
            }
        }
    }

    [Serializable]
    public class TrackerBase
    {
        public string VariableName;
        public bool Track = true;

        public virtual void Update() { }
    }

    [Serializable]
    public class Tracker<T> : TrackerBase
    {
        bool trackPrevVal;
        Func<T> variableGetter;

        public void Init(string variableName, Func<T> getter)
        {
            VariableName = variableName;
            variableGetter = getter;
        }

        public void ChangeGetter(Func<T> getter) => variableGetter = getter;

        public override void Update()
        {
            if (Track)
            {
                VariableTrackerUI.Track(VariableName, variableGetter.Invoke());
                trackPrevVal = true;
            }

            if (!Track && trackPrevVal)
            {
                VariableTrackerUI.StopTracking(VariableName);
                trackPrevVal = false;
            }
        }
    }
}