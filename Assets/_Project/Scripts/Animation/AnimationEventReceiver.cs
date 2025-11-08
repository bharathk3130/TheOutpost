using StarterAssets;
using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    [SerializeField] ThirdPersonController _controller;
    
    void OnFootstep(AnimationEvent animationEvent) => _controller.OnFootstep(animationEvent);
    void OnLand(AnimationEvent animationEvent) => _controller.OnLand(animationEvent);
}
