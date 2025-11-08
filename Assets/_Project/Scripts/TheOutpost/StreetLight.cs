using Clickbait.Utilities;
using UnityEngine;

public class StreetLight : MonoBehaviour
{
    [SerializeField] GameObject _emissiveCube;
    [SerializeField] Light _spotlight;
    
    [SerializeField] float _minTimeBetweenFlickers = 5f;
    [SerializeField] float _maxTimeBetweenFlickers = 15f;
    [SerializeField] float _flickerDuration = 0.1f;
    [SerializeField] float _minIntensity = 0f;

    float _maxIntensity;
    bool _isFlickering;
    CountDownTimer _timer;

    void Awake()
    {
        _timer = new CountDownTimer(Random.Range(_minTimeBetweenFlickers, _maxTimeBetweenFlickers));
        _timer.OnTimerStop += OnTimerStop;
        _maxIntensity = _spotlight.intensity;
    }
    
    void Start()
    {
        _timer.Start();
    }

    void OnTimerStop()
    {
        if (!_isFlickering)
        {
            // Start a quick flicker
            _isFlickering = true;
            _emissiveCube.SetActive(false);
            _spotlight.intensity = 0f;

            _timer.Reset(_flickerDuration);
            _timer.Start();
        }
        else
        {
            // End flicker, return to normal
            _isFlickering = false;
            _emissiveCube.SetActive(true);
            _spotlight.intensity = Random.Range(_minIntensity, _maxIntensity);

            _timer.Reset(Random.Range(_minTimeBetweenFlickers, _maxTimeBetweenFlickers));
            _timer.Start();
        }
    }

    void Update()
    {
        _timer.Tick(Time.deltaTime);
    }
}