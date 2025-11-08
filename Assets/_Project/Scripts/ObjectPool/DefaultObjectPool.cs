using UnityEngine;
using UnityEngine.Pool;

public class DefaultObjectPool<T> where T : Component, IPoolable
{
    T _prefab;
    IObjectPool<T> _pool;

    IObjectPoolHandler _poolHandler;
    Transform _poolParent;

    /// <summary>
    /// Prefab: The prefab to spawn
    /// poolHandler: The script spawning the instances of the prefab (It should implement IObjectPoolHandler)
    /// collectionCheck: Checks if you're returning an object that's already in the pool, back into the pool and gives a warning
    /// defaultCapacity: How much is initialized at first. It grows as the pool grows - avoid making it grow often
    /// maxCapacity: Max capacity of the pool. If more objects are added into the pool, they're destroyed
    /// </summary>
    public DefaultObjectPool(T prefab, IObjectPoolHandler poolHandler, Transform poolParent, bool collectionCheck = true, 
        int defaultCapacity = 10, int maxCapacity = 1000)
    {
        _prefab = prefab;
        _poolHandler = poolHandler;
        _poolParent = poolParent;
        
        _pool = new ObjectPool<T>(CreateInstance, OnGetFromPool, OnRelease, OnDestroyPooledObject, collectionCheck, 
            defaultCapacity, maxCapacity);
    }
    
    public T Spawn() => _pool.Get();

    public T Spawn(Vector3 position, Quaternion rotation)
    {
        T instance = Spawn();
        instance.transform.SetPositionAndRotation(position, rotation);
        return instance;
    }

    public T Spawn(Vector3 position)
    {
        T instance = Spawn();
        instance.transform.position = position;
        return instance;
    }
    
    public void Release(T obj) => _pool.Release(obj);

    T CreateInstance()
    {
        T instance = _poolHandler.InstantiateGameObject(_prefab);
        instance.SetObjectPool(this);
        instance.transform.SetParent(_poolParent);
        return instance;
    }
    
    void OnGetFromPool(T instance) => instance.gameObject.SetActive(true);
    void OnRelease(T instance) => instance.gameObject.SetActive(false);
    
    // Invoked when we exceed the number of pooled objects
    void OnDestroyPooledObject(T instance) => _poolHandler.DestroyGameObject(instance.gameObject);
}