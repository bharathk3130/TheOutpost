using UnityEngine;

namespace Clickbait.Utilities
{
    public class PrivateSingleton<T> : MonoBehaviour where T : Component
    {
        /// <summary>
        /// Write your own class that derives PrivateSingleton<YourClass>
        /// Make sure you call base.Awake() if you write your own Awake method
        /// </summary>
        
        protected static T _instance;

        protected virtual void Awake()
        {
            InitializeSingleton();
        }

        protected virtual void InitializeSingleton()
        {
            if (!Application.isPlaying) return;

            _instance = this as T;
        }
    }
}