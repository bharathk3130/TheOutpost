using Clickbait.Utilities;
using UnityEngine;

public class TrainingManager : Singleton<TrainingManager>
{
    [field: SerializeField] public bool IsTraining { get; private set; } = true;
    [field: SerializeField] public bool IsTesting { get; private set; }
    [SerializeField] float _trainingTimeScale = 20f;

    [SerializeField] GunAudioSO _gunAudio;
    
    void Start()
    {
        if (IsTraining)
        {
            Time.timeScale = _trainingTimeScale;
            MuteAudio();
        } else
        {
            UnmuteAudio();
        }
    }

    void MuteAudio() => _gunAudio.Mute();
    void UnmuteAudio() => _gunAudio.Unmute();
}
