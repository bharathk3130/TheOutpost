using UnityEngine;

public class BatteryCharge : MonoBehaviour
{
    [SerializeField] TransmissionZone _zone;
    [SerializeField] float _depletionTime = 45f;
    [SerializeField] float _fillTime = 45f;

    float _batteryLevel = 100f;
    BatteryState _batteryState = BatteryState.Stable;
    
    public float BatteryLevel => _batteryLevel;
    
    GameManager _gameManager;

    enum BatteryState
    {
        Filling,
        Depleting,
        Stable
    }

    void Start()
    {
        _zone.OnPlayerEnter += () => _batteryState = BatteryState.Filling;
        _zone.OnPlayerExit += () => _batteryState = BatteryState.Depleting;

        _gameManager = GameManager.Instance;
    }

    void Update()
    {
        HandleBatteryLevel();
    }

    void HandleBatteryLevel()
    {
        if (_gameManager.IsGameOver) return;
        
        if (_batteryState == BatteryState.Depleting)
        {
            _batteryLevel -= (100f / _depletionTime) * Time.deltaTime;
            if (_batteryLevel <= 0f)
            {
                _batteryLevel = 0f;
                _batteryState = BatteryState.Stable;
                OnBatteryDie();
            }
        }
        else if (_batteryState == BatteryState.Filling)
        {
            _batteryLevel += (100f / _fillTime) * Time.deltaTime;
            if (_batteryLevel >= 100f)
            {
                _batteryLevel = 100f;
                _batteryState = BatteryState.Stable;
            }
        }
    }

    void OnBatteryDie()
    {
        // print("Mission failed - Battery died");
        GameManager.Instance.Lost();
    }
}