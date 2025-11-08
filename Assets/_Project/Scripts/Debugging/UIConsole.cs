using System.Collections.Generic;
using Clickbait.Utilities;
using TMPro;
using UnityEngine;

/// <summary>
/// Create a text prefab with a Content Size Fitter component on it and set "Vertical Fit" to "Preferred Size".
/// Assign the prefab to "_consoleTextPrefab". Create an empty GameObject "content" with a vertical layout group
/// component on it and assign it to "_contentTransform".
/// </summary>
namespace Clickbait.Debugging
{
    public class UIConsole : PrivateSingleton<UIConsole>
    {
        [Header("References")]
        [SerializeField] TextMeshProUGUI _consoleTextPrefab;
        [SerializeField] Transform _contentTransform;
    
        [Header("Settings")]
        [SerializeField] int _capacity = 20;

        Queue<GameObject> _logs = new();

        public static void Log(string msg) => _instance.LogHelper(msg);

        void LogHelper(string msg)
        {
            if (_logs.Count >= _capacity)
            {
                Destroy(_logs.Dequeue());
            }
        
            TextMeshProUGUI textInstance = Instantiate(_consoleTextPrefab, _contentTransform);
            textInstance.text = msg;
            _logs.Enqueue(textInstance.gameObject);
        
            print(msg);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                ClearConsole();
            }
        }

        [ContextMenu("Clear Console")]
        void ClearConsole()
        {
            for (int i = 0; i < _logs.Count; i++)
            {
                Destroy(_logs.Dequeue());
            }
        }
    }
}