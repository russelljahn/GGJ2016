using UnityEngine;

namespace Assets.OutOfTheBox.Scripts.Extensions
{
    public static class FloatExtensions
    {
        public static bool IsApproximatelyZero(this float value)
        {
            return Mathf.Approximately(value, 0f);
        }
    }
}
