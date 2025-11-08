using System;
using System.Collections;
using Clickbait.Utilities;
using UnityEngine;

public class Health : MonoBehaviour, IAgentInitializable
{
    public Observer<float> CurrentHealthPercent = new(1f);
    [SerializeField] int _maxHealth = 100;
    
    [Header("Flashing effect")]
    [SerializeField] Renderer[] _renderers;
    [SerializeField] Material _flashMaterial;
    [SerializeField] float _flashDuration = 0.1f;

    int _currentHealth;
    public int CurrentHealth => _currentHealth;

    public event Action OnTakeDamage = delegate { };
    public IDeath Death { get; private set; }
    Material _defaultMaterial;
    
    public bool IsFullHealth => _currentHealth >= _maxHealth;
    
    void Awake()
    {
        Death = GetComponent<IDeath>();
        _defaultMaterial = _renderers[0].material;
        _currentHealth = _maxHealth;
    }
    
    public void OnEpisodeStart()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int damage)
    {
        OnTakeDamage.Invoke();
        
        IncrementHealth(-damage);
        StartCoroutine(Flash());
        
        if (CurrentHealthPercent.Value <= 0)
        {
            Death.Die();
        }
    }

    public void Heal(int amount)
    {
        IncrementHealth(amount);
    }

    IEnumerator Flash()
    {
        SetRendererMaterial(_flashMaterial);
        yield return new WaitForSeconds(_flashDuration);
        SetRendererMaterial(_defaultMaterial);
    }

    void SetRendererMaterial(Material mat) => Array.ForEach(_renderers, r => r.material = mat);

    [ContextMenu("Die")]
    void TakeFullDamage() => TakeDamage(100);
    [ContextMenu("Take 20 Damage")]
    void TakeDamage20() => TakeDamage(20);

    public void ResetHealth() => SetHealth(100);

    void IncrementHealth(int increment) => SetHealth(_currentHealth + increment);
    
    void SetHealth(int health)
    {
        _currentHealth = Mathf.Clamp(health, 0, _maxHealth);
        CurrentHealthPercent.Value = (float) _currentHealth / _maxHealth;
    }
    
    public void OnInitialize() { }
}
