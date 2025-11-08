using System;
using UnityEngine;

public abstract class DeathBase : MonoBehaviour, IDeath
{
    [SerializeField] MonoBehaviour[] _componentToBeDisabledOnDeath;
    [SerializeField] CharacterAnimationLayerManager _animationLayerManager;
    [SerializeField] RigManager _rigManager;
    [SerializeField] Animator _anim;
    
    public bool IsDead { get; private set; }
    public event Action OnDeath = delegate { };
    
    static int _dieHash;

    protected virtual void Awake()
    {
        _dieHash = Animator.StringToHash("Die");
    }

    void Start()
    {
        GameManager.Instance.OnGameOver += DisableSelf;
    }

    void DisableSelf()
    {
        GameManager.Instance.OnGameOver -= DisableSelf;
        IsDead = true;
        gameObject.layer = LayerMask.NameToLayer("Dead");
        OnDeath.Invoke();
        
        Array.ForEach(_componentToBeDisabledOnDeath, component => component.enabled = false);
    }

    public void Die()
    {
        DisableSelf();
        PlayDeathAnimation();

        Reset();
    }

    void PlayDeathAnimation()
    {
        _anim.SetTrigger(_dieHash);
        _animationLayerManager.SetLayerWeightSmoothed(CharacterAnimationLayerManager.AnimationLayerType.Die, 1);
        _rigManager.SetWeightSmoothed(0);
    }

    protected abstract void Reset();
}