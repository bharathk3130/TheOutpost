using Cinemachine;
using Clickbait.Utilities;
using UnityEngine;

public class CameraShake : Singleton<CameraShake>
{
    [Header("References")]
    [SerializeField] CinemachineVirtualCamera _mainCinemachineCam;
    [SerializeField] CinemachineVirtualCamera _aimCam;

    [Header("Settings")]
    [SerializeField] float _aimingIntensityAmplifier = 0.6f;

    CountDownTimer _stopShakingCountdown;

    CinemachineBasicMultiChannelPerlin _mainCamPerlin;
    CinemachineBasicMultiChannelPerlin _aimCamPerlin;

    void Start()
    {
        if (!enabled) return;
        
        _mainCamPerlin = _mainCinemachineCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _aimCamPerlin = _aimCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        
        _stopShakingCountdown = new CountDownTimer(1);
        _stopShakingCountdown.OnTimerStop += () =>
        {
            StopShaking(_mainCamPerlin);
            StopShaking(_aimCamPerlin);
        };
    }

    void Update()
    {
        _stopShakingCountdown.Tick(Time.deltaTime);
    }

    public void Shake(float intensity, float duration)
    {
        if (!enabled) return;
        
        ShakeCamera(intensity, _mainCamPerlin);
        ShakeCamera(intensity * _aimingIntensityAmplifier, _aimCamPerlin);
        
        _stopShakingCountdown.Abort();
        _stopShakingCountdown.SetNewInitialTime(duration);
        _stopShakingCountdown.Start();
    }

    void ShakeCamera(float intensity, CinemachineBasicMultiChannelPerlin perlin)
    {
        perlin.m_AmplitudeGain = intensity;
    }

    void StopShaking(CinemachineBasicMultiChannelPerlin perlin)
    {
        perlin.m_AmplitudeGain = 0;
    }
}
