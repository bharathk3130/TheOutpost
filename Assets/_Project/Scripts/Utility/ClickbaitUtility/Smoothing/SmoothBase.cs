using System;

namespace Clickbait.Utilities
{
    public abstract class SmoothBase<T>
    {
        protected float _smoothing;
        protected float _threshold;
        
        protected T _targetVal;
        protected T _currentVal;
        public bool IsSmoothing { get; protected set; }

        public event Action OnComplete = delegate { };

        public SmoothBase(float smoothing) =>  _smoothing = smoothing;

        public void SmoothTo(T target)
        {
            _targetVal = target;
            IsSmoothing = true;
        }

        public void SmoothTo(T currentVal, T target)
        {
            _currentVal = currentVal;
            _targetVal = target;
            IsSmoothing = true;
        }
        
        public void SetCurrentVal(T currentVal) => _currentVal = currentVal;

        public abstract T Step(float deltaTime);
        
        protected void RaiseOnComplete() => OnComplete.Invoke();
        public void Cancel() => IsSmoothing = false;
    }
}