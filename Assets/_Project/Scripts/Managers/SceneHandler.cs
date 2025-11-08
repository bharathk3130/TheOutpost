using System;
using Clickbait.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : Singleton<SceneHandler>
{
    [SerializeField] InputReader _inputReader;
    
    public event Action OnEndScene = delegate { };
    
    public void ReloadScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void LoadScene(int buildIndex)
    {
        _inputReader.RemoveAllSubscribers();
        OnEndScene.Invoke();
        SceneManager.LoadScene(buildIndex);
    }
}