using System;
using UnityEngine;

namespace FlexiTweening.Extensions
{
    [Serializable]
    public class TweenSettings
    {
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private AnimationCurve _easing = AnimationCurveHelper.GetLinearCurve();

        public float Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        public AnimationCurve Easing
        {
            get { return _easing; }
            set { _easing = value; }
        }
    }
}
