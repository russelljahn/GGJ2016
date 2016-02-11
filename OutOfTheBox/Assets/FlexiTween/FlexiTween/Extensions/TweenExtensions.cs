namespace FlexiTweening.Extensions
{
    public static class TweenExtensions
    {
        public static void SafelyAbort(this ITween tween)
        {
            if (tween != null)
                tween.Abort();
        }

        public static void SafelyFinish(this ITween tween)
        {
            if (tween != null)
                tween.Finish();
        }

        public static ITween<T> To<T>(this ITween<T> tween, T value, TweenSettings settings)
        {
            return tween.To(value, settings.Duration).Easing(settings.Easing);
        }
    }
}
