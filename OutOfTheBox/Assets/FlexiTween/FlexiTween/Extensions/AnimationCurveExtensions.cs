using System.Linq;
using UnityEngine;

namespace FlexiTweening.Extensions
{
    public static class AnimationCurveExtensions
    {
        public static float GetEndTime(this AnimationCurve curve, float fallbackTimeIfNoKeyframes = 0f)
        {
            return curve.keys.Length > 0 ? curve.keys.Last().time : fallbackTimeIfNoKeyframes;
        }

        /// <param name="curve"></param>
        /// <param name="time">A value between 0.0f and 1.0f</param>
        public static float EvaluateAtNormalizedTime(this AnimationCurve curve, float time)
        {
            time = Mathf.Clamp01(time);
            return curve.Evaluate(time * curve.GetEndTime());
        }
    }
}
