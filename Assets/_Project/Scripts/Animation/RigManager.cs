using Clickbait.Utilities;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigManager : MonoBehaviour
{
    [SerializeField] float _smoothing = 20f;

    Rig _rig;
    SmoothFloat _lerp;

    void Awake()
    {
        _rig = GetComponent<Rig>();
        
        _lerp = new SmoothFloat(_smoothing);
    }

    void Update()
    {
        if (_lerp.IsSmoothing)
        {
            _rig.weight = _lerp.Step(Time.deltaTime);
        }
    }

    public void SetWeightSmoothed(float weight)
    {
        _lerp.SmoothTo(weight);
    }

    public void SetWeight(float weight)
    {
        _lerp.Cancel();
        _rig.weight = weight;
    }
}
