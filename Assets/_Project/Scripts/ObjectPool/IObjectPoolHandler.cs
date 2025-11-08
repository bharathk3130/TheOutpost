using UnityEngine;

public interface IObjectPoolHandler
{
    T InstantiateGameObject<T>(T go) where T : Component;
    void DestroyGameObject(GameObject go);
}