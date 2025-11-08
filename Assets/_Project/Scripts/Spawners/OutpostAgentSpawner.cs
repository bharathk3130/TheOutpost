using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class OutpostAgentSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] OutpostAgent _outpostAgentPrefab;
    [SerializeField] Transform _playerTransform;
    [SerializeField] TransmissionZone _transmissionZone;
    [SerializeField] Transform _agentsParent;

    [Header("Settings")]
    [SerializeField] int _minAgents = 2;
    [SerializeField] int _minHordeSize = 2;
    [SerializeField] int _maxHordeSize = 4;
    [SerializeField] float _spawnDelay = 2;

    int _liveAgents;
    bool _spawning;

    GameManager _gameManager;
    
    void Start()
    {
        _gameManager = GameManager.Instance;
        
        StartCoroutine(SpawnHorde());
    }

    IEnumerator SpawnHorde()
    {
        _spawning = true;
        int agentsToSpawn = Random.Range(_minHordeSize, _maxHordeSize + 1);
        
        for (int i = 0; i < agentsToSpawn; i++)
        {
            Spawn();
            yield return new WaitForSeconds(_spawnDelay);
        }
        
        _spawning = false;

        if (_liveAgents < _minAgents && !_gameManager.IsGameOver)
        {
            StartCoroutine(SpawnHorde());
        }
    }

    void Spawn()
    {
        OutpostAgent agent = Instantiate(_outpostAgentPrefab, transform.position, transform.rotation, _agentsParent);
        agent.Initialize(this, _playerTransform, _transmissionZone);
        _liveAgents++;
    }

    public void OnAgentDeath()
    {
        _liveAgents--;
        if (!_gameManager.IsGameOver && !_spawning && _liveAgents < _minAgents)
        {
            StartCoroutine(SpawnHorde());
        }
    }
}
