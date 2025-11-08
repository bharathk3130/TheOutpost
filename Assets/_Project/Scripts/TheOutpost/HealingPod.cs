using System.Collections;
using UnityEngine;

public class HealingPod : MonoBehaviour
{
    [SerializeField] Health _playerHealth;
    [SerializeField] float _timeBetweenHeals = 0.5f;

    bool _playerInPod;
    
    GameManager _gameManager;

    void Start()
    {
        _gameManager = GameManager.Instance;
    }

    IEnumerator HealPlayer()
    {
        while (!_playerHealth.IsFullHealth && _playerInPod && !_gameManager.IsGameOver)
        {
            _playerHealth.Heal(1);
            yield return new WaitForSeconds(_timeBetweenHeals);
        }
    }
    
    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            _playerInPod = true;
            StartCoroutine(HealPlayer());
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            _playerInPod = false;
        }
    }
}
