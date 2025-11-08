using UnityEngine;

namespace Clickbait.Utilities
{
	public class SmoothQuaternion : SmoothBase<Quaternion>
    {
        public SmoothQuaternion(float smoothing) : base(smoothing)
        {
            _threshold = 0.5f; // Degrees
        }

        public override Quaternion Step(float deltaTime)
        {
            if (IsSmoothing)
            {
                _currentVal = Quaternion.Lerp(_currentVal, _targetVal, deltaTime * _smoothing);

                if (Quaternion.Angle(_currentVal, _targetVal) < _threshold)
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