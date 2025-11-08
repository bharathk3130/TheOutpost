using System;
using UnityEngine;

public class BulletTrail : MonoBehaviour, IPoolable
{
    [SerializeField] float _speed = 100;

    DefaultObjectPool<BulletTrail> _objectPool;
    TrailRenderer _trailRenderer;
    
    Vector3 _dir;
    float _distanceToTarget;

    public event Action OnReachTarget = delegate { };

    void Awake()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
    }
    
    public void Initialize(Vector3 spawnPos, Vector3 targetPos)
    {
        transform.position = spawnPos;
        
        ResetSelf();

        _dir = (targetPos - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(_dir);
        _distanceToTarget = Vector3.Distance(transform.position, targetPos);
    }

    void ResetSelf()
    {
        _trailRenderer.Clear();
        OnReachTarget = delegate { };
    }

    void Update()
    {
        float step = _speed * Time.deltaTime;
        
        transform.position += _dir * step;
        _distanceToTarget -= step;

        if (_distanceToTarget <= 0)
        {
            OnReachTarget.Invoke();
            _objectPool.Release(this);
        }
    }

    public void SetObjectPool<T>(DefaultObjectPool<T> pool) where T : Component, IPoolable =>
        _objectPool = pool as DefaultObjectPool<BulletTrail>;
}
