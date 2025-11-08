using System;
using UnityEngine;

public class TrainingDeath : MonoBehaviour, IDeath
{
    public bool IsDead {get; private set;}
    
    public event Action OnDeath = delegate { };
    Health _health;

    void Awake()
    {
        _health = GetComponent<Health>();
    }
    
    public void Die()
    {
        IsDead = true;
        OnDeath.Invoke();
        _health.ResetHealth();
    }
}