using UnityEngine;

public interface IPoolable
{
    void SetObjectPool<T>(DefaultObjectPool<T> pool) where T : Component, IPoolable;
}