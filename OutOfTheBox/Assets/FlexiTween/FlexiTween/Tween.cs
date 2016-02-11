using System;
using System.Collections;
using FlexiTweening.Extensions;
using JetBrains.Annotations;
using UnityEngine;

namespace FlexiTweening
{
    public delegate T Lerp<T>(T from, T to, float t);

    internal class Tween<T> : ITween<T>
    {
        private readonly Lerp<T> _lerp;
        private readonly T _startValue;
        private Action _complete;
        private float _currentTime;
        private AnimationCurve _curve;
        private float _duration;
        private T _endValue;
        private bool _shouldCallbackCompletion = true;
        private Action<T> _update;

        public Tween(Lerp<T> lerpFunction, T startValue)
        {
            _lerp = lerpFunction;
            _startValue = startValue;
            _curve = AnimationCurveHelper.GetLinearCurve();
        }

        public float NormalizedTime
        {
            get { return _currentTime/_duration; }
        }

        public bool IsFinished { get; private set; }

        /// <summary>
        ///     Stops tween and invokes complete callbacks.
        /// </summary>
        public void Finish()
        {
            _shouldCallbackCompletion = true;
            IsFinished = true;
        }

        /// <summary>
        ///     Stops tween without invoking complete callbacks.
        /// </summary>
        public void Abort()
        {
            _shouldCallbackCompletion = false;
            IsFinished = true;
        }

        public ITween<T> To(T value, float duration)
        {
            if (duration < 0)
                throw new ArgumentException("Duration is negative.", "duration");

            _endValue = value;
            _duration = duration;
            return this;
        }

        public ITween<T> Easing([NotNull] AnimationCurve curve)
        {
            if (curve == null) throw new ArgumentNullException("curve");

            _curve = curve;
            return this;
        }

        public ITween<T> OnComplete(Action action)
        {
            _complete = action;
            return this;
        }

        public ITween Start()
        {
            if (_update == null) throw new NullReferenceException("Update function is not set.");

            var coroutine = GetTweenEnumerator();
            FlexiTween.Instance.StartCoroutine(coroutine);
            return this;
        }

        public ITween<T> OnUpdate(Action<T> action)
        {
            _update = action;
            return this;
        }

        public override string ToString()
        {
            return string.Format("Current time: {0}, duration: {1}, current value: {2}, end value: {3}",
                _currentTime, _duration, GetValue(NormalizedTime), _endValue);
        }

        /// <param name="normalizedTime">Should be float between 0f-1f.</param>
        private T GetValue(float normalizedTime)
        {
            const float fallbackEasingEndTime = 1f;
            var easingLength = _curve.GetEndTime(fallbackEasingEndTime);
            var amount = _curve.Evaluate(normalizedTime*easingLength);

            return _lerp(_startValue, _endValue, amount);
        }

        private IEnumerator GetTweenEnumerator()
        {
            while (IsFinished == false && _currentTime < _duration)
            {
                _update(GetValue(NormalizedTime));

                yield return null;

                _currentTime = Mathf.Clamp(_currentTime + Time.deltaTime, 0f, _duration);
            }

            // Clamp value to end value only if not aborting
            if (IsFinished == false)
            {
                _update(GetValue(1f));
            }

            IsFinished = true;

            if (_shouldCallbackCompletion == false)
                yield break;

            if (_complete != null)
                _complete();
        }
    }
}
