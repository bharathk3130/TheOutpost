using UnityEngine;

public class AgentDeath : DeathBase
{
    [SerializeField] float _deathDelay = 5;
    
    protected override void Reset()
    {
        Invoke(nameof(SelfDestruct), _deathDelay);
    }

    void SelfDestruct()
    {
        Destroy(gameObject);
    }
}