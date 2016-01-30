using UnityEngine;

namespace FlexiTweening
{
    public static class AnimationCurveHelper
    {
        public static AnimationCurve GetLinearCurve(float time = 1f, float magnitude = 1f)
        {
            return AnimationCurve.Linear(0f, 0f, time, magnitude);
        }

        public static AnimationCurve GetSmoothConcaveCurve(float time = 1f, float magnitude = 1f)
        {
            return new AnimationCurve(
                new Keyframe(0f, 0f),
                new Keyframe(0.5f * time, magnitude),
                new Keyframe(time, 0f));
        }
    }
}