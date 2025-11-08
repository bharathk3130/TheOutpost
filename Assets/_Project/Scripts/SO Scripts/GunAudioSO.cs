using UnityEngine;

[CreateAssetMenu(fileName = "GunAudioSO", menuName = "ScriptableObjects/GunAudioSO")]
public class GunAudioSO : ScriptableObject
{
    [Range(0, 1)] public float Volume = 1f;

    public AudioClip[] FireClips;
    public AudioClip ReloadClip;

    bool _isMuted;
    
    public void Mute() => _isMuted = true;
    public void Unmute() => _isMuted = false;

    public void PlayFireClip(AudioSource audioSource)
    {
        if (_isMuted) return;
        
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(FireClips[Random.Range(0, FireClips.Length)], Volume);
    }

    public void PlayReloadClip(AudioSource audioSource)
    {
        if (_isMuted) return;
        
        audioSource.PlayOneShot(ReloadClip);
    }
}
