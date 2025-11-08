using System.Collections.Generic;
using UnityEngine;

public class AgentSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform _agentsParent;
    [SerializeField] Transform _dummiesParent;
    [SerializeField] GameObject _agentPrefab;
    [SerializeField] GameObject _dummyPrefab;
    [SerializeField] Transform _spawnPointsParent;
    [SerializeField] CameraSystem _tpsAgentCameraSystem;
    [SerializeField] CameraSystem _fpsAgentCameraSystem;
    
    [Header("Settings")]
    [SerializeField] int _agentAmount = 1;
    [SerializeField] int _dummyAmount = 0;
    [SerializeField] bool _followAgentWithCam;
    [SerializeField] bool _isTPS = true;

    List<Transform> _availableSpawnPoints = new();
        
    void Start()
    {
        int total = _agentAmount + _dummyAmount;
        if (_spawnPointsParent.childCount < total)
        {
            Debug.LogError("Not enough spawn points");
            return;
        }

        for (int i = 0; i < _spawnPointsParent.childCount; i++)
            _availableSpawnPoints.Add(_spawnPointsParent.GetChild(i));

        for (int i = 0; i < _agentAmount; i++)
        {
            Transform spawnPoint = _availableSpawnPoints[Random.Range(0, _availableSpawnPoints.Count)];
            GameObject agentInstance = Instantiate(_agentPrefab, spawnPoint.position, spawnPoint.rotation, _agentsParent);
            WeaponManager weaponManager = agentInstance.GetComponent<ThirdPersonShooterController>().WeaponManager;
            
            _availableSpawnPoints.Remove(spawnPoint);

            if (i == 0 && _followAgentWithCam)
            {
                ThirdPersonShooterController thirdPersonController = agentInstance.GetComponent<ThirdPersonShooterController>();
                Transform target = _isTPS ? thirdPersonController.TPSCamTarget : thirdPersonController.FPSCamTarget;
                CameraSystem agentCameraSystem = _isTPS ? _tpsAgentCameraSystem : _fpsAgentCameraSystem;
                agentCameraSystem.EnableAndSetTarget(target);
            }
        }

        for (int i = 0; i < _dummyAmount; i++)
        {
            Transform spawnPoint = _availableSpawnPoints[Random.Range(0, _availableSpawnPoints.Count)];
            Instantiate(_dummyPrefab, spawnPoint.position, spawnPoint.rotation, _dummiesParent);
            
            _availableSpawnPoints.Remove(spawnPoint);
        }
    }
}
