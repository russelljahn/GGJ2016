using System;
using UnityEngine;

namespace FlexiTweening
{
    public interface ITween
    {
        bool IsFinished { get; }
        float NormalizedTime { get; }
        void Finish();
        void Abort();
        ITween Start();
    }

    public interface ITween<T> : ITween
    {
        ITween<T> To(T value, float duration);
        ITween<T> Easing(AnimationCurve curve);
        ITween<T> OnUpdate(Action<T> action);
        ITween<T> OnComplete(Action action);
    }
}