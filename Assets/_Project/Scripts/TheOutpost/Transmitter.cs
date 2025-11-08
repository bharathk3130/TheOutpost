using Clickbait.Utilities;
using UnityEngine;

public class Transmitter : MonoBehaviour
{
    [SerializeField] float _uploadDurationMinutes = 5;
    [SerializeField] TransmissionZone _transmissionZone;
    
    CountDownTimer _timer;
    GameManager _gameManager;

    public float TimeLeft => _timer.TimeLeft;

    void Awake()
    {
        _timer = new CountDownTimer(_uploadDurationMinutes * 60);
        _timer.OnTimerStop += OnTransmissionComplete;
    }

    void Start()
    {
        _gameManager = GameManager.Instance;
        _transmissionZone.OnPlayerEnter += PlayerEntersZoneForFirstTime;
    }

    void PlayerEntersZoneForFirstTime()
    {
        _transmissionZone.OnPlayerEnter -= PlayerEntersZoneForFirstTime;
        _timer.Start();
    }

    void Update()
    {
        if (_gameManager.IsGameOver) return;
        
        _timer.Tick(Time.deltaTime);
    }

    void OnTransmissionComplete()
    {
        _gameManager.Victory();
    }
}
