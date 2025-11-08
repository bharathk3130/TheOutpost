using Clickbait.Utilities;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerDamageEffect : MonoBehaviour
{
    [SerializeField] Health _playerHealth;
    [SerializeField] float _effectDuration = 0.3f;
    Volume _volume;

    bool _playDamageEffect;
    bool _gameLost;

    CountDownTimer _timer;

    void Awake()
    {
        _volume = GetComponent<Volume>();
        _timer = new CountDownTimer(_effectDuration);
        _timer.OnTimerStop += () =>
        {
            _volume.weight = 0;
            _playDamageEffect = false;
        };
    }

    void Start()
    {
        _playerHealth.OnTakeDamage += StartBleedEffect;
        GameManager.Instance.OnLoss += () =>
        {
            _gameLost = true;
            StartBleedEffect();
        };
    }

    void StartBleedEffect()
    {
        _playDamageEffect = true;
            
        if (_timer.IsRunning)
        {
            _timer.ContinueFrom(_volume.weight * (_effectDuration / 2f));
        } else
        {
            _timer.Start();
        }
    }

    void Update()
    {
        if (!_playDamageEffect) return;
        
        _timer.Tick(Time.deltaTime);

        if (_timer.Progress < 0.5f)
        {
            _volume.weight = Mathf.Lerp(0, 1, _timer.Progress * 2);
        }
        else
        {
            if (_gameLost)
            {
                _timer.Abort();
            } else
            {
                _volume.weight = Mathf.Lerp(1, 0, (_timer.Progress - 0.5f) * 2);
            }
        }
    }
}