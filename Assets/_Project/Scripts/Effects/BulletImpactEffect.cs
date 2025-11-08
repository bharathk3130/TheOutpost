using UnityEngine;

public class BulletImpactEffect : MonoBehaviour, IPoolable
{
    [SerializeField] ParticleSystem _dustParticleEffect;
    [SerializeField, Range(0, 1)] float _darkenAmount = 0.5f;
    [SerializeField] float _lifespan = 11;
    
    DefaultObjectPool<BulletImpactEffect> _pool;
    
    ParticleSystem _particleSystem;
    ParticleSystem.MainModule _dustParticleMain;

    void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _dustParticleMain = _dustParticleEffect.main;
    }
    
    public void SetObjectPool<T>(DefaultObjectPool<T> pool) where T : Component, IPoolable
    {
        _pool = pool as DefaultObjectPool<BulletImpactEffect>;
    }

    public void SetColour(Color hitObjectColour)
    {
        Color dustColour = hitObjectColour * (1f - _darkenAmount);
        _dustParticleMain.startColor = dustColour;
    }

    void OnEnable()
    {
        _particleSystem.Emit(1);
        Invoke(nameof(ReleaseToPool), _lifespan);
    }

    void ReleaseToPool() => _pool.Release(this);
}
