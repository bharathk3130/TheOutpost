using UnityEngine;
using UnityEngine.UI;

public class ReloadSceneButton : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(SceneHandler.Instance.ReloadScene);
    }
}