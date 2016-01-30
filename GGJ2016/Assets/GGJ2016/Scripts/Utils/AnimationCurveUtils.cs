using UnityEngine;

namespace Assets.OutOfTheBox.Scripts.Utils 
{
    public static class AnimationCurveUtils 
    {
        public static AnimationCurve GetLinearCurve()
        {
            return AnimationCurve.Linear(0f, 0f, 1f, 1f);
        }


        public static AnimationCurve GetConcaveParabolaCurve()
        {
            var key1 = new Keyframe(0f, 0.0f);
            var key2 = new Keyframe(0.5f, 1.0f);
            var key3 = new Keyframe(1.0f, 0.0f);
            return new AnimationCurve(key1, key2, key3) { preWrapMode = WrapMode.Once, postWrapMode = WrapMode.Once };
        }
    }
}
