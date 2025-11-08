using System;
using System.Collections.Generic;
using Clickbait.Utilities;
using UnityEngine;

public class TransmissionZone : Singleton<TransmissionZone>
{
    public event Action OnPlayerEnter = delegate { };
    public event Action OnPlayerExit = delegate { };
    
    List<GameObject> _soldiersInZone = new();
    
    public bool IsPlayerInZone {get; private set;}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") ||
            other.gameObject.layer == LayerMask.NameToLayer("Agent"))
        {
            _soldiersInZone.Add(other.gameObject);
            
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                OnPlayerEnter.Invoke();
                IsPlayerInZone = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") ||
            other.gameObject.layer == LayerMask.NameToLayer("Agent"))
        {
            _soldiersInZone.Remove(other.gameObject);
            
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                OnPlayerExit.Invoke();
                IsPlayerInZone = false;
            }
        }
    }
    
    public bool IsInZone(GameObject soldier) => _soldiersInZone.Contains(soldier);
}
