using System;
using Cinemachine;
using Clickbait.Utilities;
using DG.Tweening;
using StarterAssets;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] PlayerDeath _playerDeath;
    [SerializeField] GameObject _playerAimCamera;
    [SerializeField] GameObject _gameOverUI;
    [SerializeField] GameObject _victoryUI;
    [SerializeField] AudioSource _wastedAudioSource;
    [SerializeField] AudioSource _successAudioSource;
    [SerializeField] CinemachineVirtualCamera _playerFollowCam;

    [SerializeField] float _gameOverTimeScale = 0.4f;
    
    public bool IsGameOver { get; private set; }
    public event Action OnGameOver = delegate { };
    public event Action OnLoss = delegate { };

    protected override void Awake()
    {
        base.Awake();

        Time.timeScale = 1;
    }

    void Start()
    {
        _playerDeath.OnDeath += Lost;
    }

    public void Lost()
    {
        GameComplete();
        GameOverAnimation();
        
        OnLoss.Invoke();

        Invoke(nameof(ShowGameOverUI), 1.5f);
    }
    
    public void Victory()
    {
        GameComplete();
        GameOverAnimation();
        
        _successAudioSource.Play();
        Invoke(nameof(ShowVictoryUI), 1.5f);
    }

    void GameComplete()
    {
        _playerDeath.OnDeath -= Lost;
        
        IsGameOver = true;
        OnGameOver.Invoke();
        
        ThirdPersonController.UnlockCursor();
    }

    void GameOverAnimation()
    {
        _playerAimCamera.SetActive(false);
        Time.timeScale = _gameOverTimeScale;

        var lens = _playerFollowCam.m_Lens;
        float startFOV = lens.FieldOfView;
        float targetFOV = startFOV + 20f; // how far to zoom out

        DOTween.To(() => _playerFollowCam.m_Lens.FieldOfView,
                x => _playerFollowCam.m_Lens.FieldOfView = x,
                targetFOV, 1.5f)
            .SetEase(Ease.OutExpo) // starts fast, ends slow
            .SetUpdate(true); // ignore Time.timeScale
    }

    void ShowGameOverUI()
    {
        _wastedAudioSource.Play();
        _gameOverUI.SetActive(true);
    }

    void ShowVictoryUI()
    {
        _victoryUI.SetActive(true);
    }
}
