using UnityEngine;

namespace Clickbait.Utilities
{
    public class SmoothFloat : SmoothBase<float>
    {
        public SmoothFloat(float smoothing) : base(smoothing)
        {
            _threshold = 0.01f;
        }
        
        public override float Step(float deltaTime)
        {
            if (IsSmoothing)
            {
                _currentVal = Mathf.Lerp(_currentVal, _targetVal, deltaTime * _smoothing);

                if (Mathf.Abs(_currentVal - _targetVal) < _threshold)
                {
                    IsSmoothing = false;
                    _currentVal = _targetVal;
                    RaiseOnComplete();
                }
            }

            return _currentVal;
        }
    }
}