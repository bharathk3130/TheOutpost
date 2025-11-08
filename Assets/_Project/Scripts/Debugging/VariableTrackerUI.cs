using System.Collections.Generic;
using Clickbait.Utilities;
using TMPro;
using UnityEngine;

namespace Clickbait.Debugging
{
    public class VariableTrackerUI : Singleton<VariableTrackerUI>
    {
        [SerializeField] TextMeshProUGUI _logText;
        Dictionary<string, object> _trackedVariables = new();

        void Update()
        {
            string text = "";
            if (_trackedVariables.Count > 0)
            {
                foreach (var kvp in _trackedVariables)
                {
                    text += $"{kvp.Key}: {kvp.Value}\n";
                }
            }

            _logText.text = text;
        }

        public static void Track<T>(string variableName, T variable) => Instance?.TrackHelper(variableName, variable);
        public static void StopTracking(string variableName) => Instance?.StopTrackingHelper(variableName);

        void TrackHelper<T>(string variableName, T variable)
        {
            _trackedVariables[variableName] = variable;
        }

        void StopTrackingHelper(string variableName)
        {
            _trackedVariables.Remove(variableName);
        }
    }
}