using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Assets.OutOfTheBox.Scripts.Utils
{
    public static class MathfUtils
    {

        public static bool IsZero(float value)
        {
            return Mathf.Approximately(value, 0f);
        }


        public static bool IsNonzero(float value)
        {
            return !IsZero(value);
        }


        public static bool IsLessThanOrEqualTo(float lhs, float rhs)
        {
            return lhs < rhs || Mathf.Approximately(lhs, rhs);
        }


        public static bool IsGreaterThanOrEqualTo(float lhs, float rhs)
        {
            return lhs > rhs || Mathf.Approximately(lhs, rhs);
        }


        public static bool IsLessThanOrEqualToZero(float value)
        {
            return value < 0 || IsZero(value);
        }


        public static bool IsGreaterThanOrEqualToZero(float value)
        {
            return value > 0 || IsZero(value);
        }


        public static bool IsBetweenRange(int value, int minInclusive, int maxInclusive)
        {
            return minInclusive <= value && value <= maxInclusive;
        }


        public static bool IsBetweenRange(float value, float minInclusive, float maxInclusive)
        {
            return IsLessThanOrEqualTo(minInclusive, value) && IsGreaterThanOrEqualTo(maxInclusive, value);
        }


        public static bool IsBetween01(float value)
        {
            return IsBetweenRange(value, 0f, 1f);
        }


        public static float Wrap(float value, float startInclusive, float endInclusive)
        {
            var range = endInclusive - startInclusive;
            var offset = value - startInclusive;
            return startInclusive + offset - range*Mathf.Floor(offset/range);
        }


        public static int Wrap(int value, int startInclusive, int endInclusive)
        {
            var range = endInclusive - startInclusive + 1;
            value = (value - startInclusive)%range + range;
            return value%range + startInclusive;
        }


        public static float WrapAngleRadians(float radians)
        {
            return Wrap(radians, 0f, 2f*Mathf.PI);
        }


        public static float WrapAngle360(float degrees)
        {
            return Wrap(degrees, 0f, 360f);
        }


        /// <param name="value"></param>
        /// <returns>1 if value is greater than or equal to zero, -1 otherwise.</returns>
        public static int IntSign(int value)
        {
            return value >= 0 ? 1 : -1;
        }

        /// <param name="value"></param>
        /// <returns>1 if value is greater than or equal to zero, -1 otherwise.</returns>
        public static int IntSign(float value)
        {
            return IsGreaterThanOrEqualToZero(value) ? 1 : -1;
        }


        public static int Lerp(int from, int to, float t)
        {
            return Mathf.RoundToInt(Mathf.Lerp(from, to, t));
        }


        /// <summary>
        /// Lerps between from and to, mirroring at t=0.5
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="t"></param>
        /// <returns>t=0 returns from, t=0.5 returns to, t=1 returns from</returns>
        public static float MirrorLerp(float from, float to, float t)
        {
            t = 1.0f - Mathf.Abs(1.0f - 2.0f*t);
            return Mathf.Lerp(from, to, t);
        }


        /// <summary>
        /// Lerps between from and to, mirroring at t=0.5
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="t"></param>
        /// <returns>t=0 returns from, t=0.5 returns to, t=1 returns from</returns>
        [SuppressMessage("ReSharper", "RedundantCast")]
        public static int MirrorLerp(int from, int to, float t)
        {
            return Mathf.RoundToInt(MirrorLerp((float) from, (float) to, t));
        }

    }
}
