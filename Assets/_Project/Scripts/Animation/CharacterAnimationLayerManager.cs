using System;
using Clickbait.Utilities;
using UnityEngine;

public class CharacterAnimationLayerManager : MonoBehaviour
{
    [SerializeField] float _layerWeightSmoothing = 10f;
    [SerializeField] Animator _anim;
    
    SmoothFloat[] _layerLerps;
    int _numLayer;

    public enum AnimationLayerType
    {
        Gun,
        Die
    }

    void Awake()
    {
        InitializeLerpArray();
    }

    void InitializeLerpArray()
    {
        _numLayer = Enum.GetValues(typeof(AnimationLayerType)).Length;
        _layerLerps = new SmoothFloat[_numLayer];
        for (int i = 0; i < _numLayer; i++)
        {
            _layerLerps[i] = new SmoothFloat(_layerWeightSmoothing);
        }
    }

    void Update()
    {
        for (int i = 0; i < _numLayer; i++)
        {
            if (_layerLerps[i].IsSmoothing)
            {
                _anim.SetLayerWeight(i + 1, _layerLerps[i].Step(Time.deltaTime));
            }
        }
    }

    public void SetLayerWeightSmoothed(AnimationLayerType layerType, float weight)
    {
        int layer = (int)layerType;
        _layerLerps[layer].SmoothTo(weight);
    }

    public void SetLayerWeight(AnimationLayerType layerType, float weight)
    {
        int layer = (int)layerType;
        _layerLerps[layer].Cancel();
        _anim.SetLayerWeight(layer + 1, weight);
    }
}
