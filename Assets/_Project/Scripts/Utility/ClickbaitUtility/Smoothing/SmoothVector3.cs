using UnityEngine;

namespace Clickbait.Utilities
{
    public class SmoothVector3 : SmoothBase<Vector3>
    {
        public SmoothVector3(float smoothing) : base(smoothing)
        {
            _threshold = 0.01f * 0.01f;
        }

        public override Vector3 Step(float deltaTime)
        {
            if (IsSmoothing)
            {
                _currentVal = Vector3.Lerp(_currentVal, _targetVal, deltaTime * _smoothing);

                if ((_currentVal - _targetVal).sqrMagnitude < _threshold)
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