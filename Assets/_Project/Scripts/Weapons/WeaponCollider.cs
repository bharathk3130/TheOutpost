using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class WeaponCollider : MonoBehaviour
{
    [SerializeField] ThirdPersonController _controller;
    [SerializeField] LayerMask _obstructionMask;
    
    List<Collider> _obstructions = new();
    
    void OnTriggerEnter(Collider other)
    {
        if ((_obstructionMask.value & (1 << other.gameObject.layer)) == 0) // Checks if other doesn't belong to the layer mask
            return;
        
        if (_obstructions.Count == 0)
        {
            _controller.TuckGun();
        }
        _obstructions.Add(other);
    }

    void OnTriggerExit(Collider other)
    {
        if ((_obstructionMask.value & (1 << other.gameObject.layer)) == 0)
            return;
        
        _obstructions.Remove(other);
        if (_obstructions.Count == 0)
        {
            _controller.UntuckGun();
        }
    }
}
