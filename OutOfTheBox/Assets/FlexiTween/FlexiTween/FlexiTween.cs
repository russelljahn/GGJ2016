using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace FlexiTweening
{
    public class FlexiTween : MonoBehaviour
    {
        private static bool _appHasQuitted;
        private static FlexiTween _instance;

        public static FlexiTween Instance
        {
            get
            {
                if (_appHasQuitted)
                {
                    throw new InvalidOperationException(
                        "Application has already quitted but new FlexiTween is trying to be created.");
                }

                if (_instance == null)
                    _instance = FindObjectsOfType<FlexiTween>().SingleOrDefault();

                if (_instance != null)
                    return _instance;

                var go = new GameObject(typeof (FlexiTween).Name)
                {
                    hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector
                };

                DontDestroyOnLoad(go);
                _instance = go.AddComponent<FlexiTween>();

                return _instance;
            }
        }

        [UsedImplicitly]
        private void OnApplicationQuit()
        {
            _appHasQuitted = true;
        }

        /// <summary>
        ///     Stops all tweens. Any complete callbacks are no longer invoked after the tweens are stopped.
        /// </summary>
        public static void SafelyAbortTweens(params ITween[] tweens)
        {
            foreach (var tween in tweens.Where(t => t != null))
            {
                tween.Abort();
            }
        }

        public static ITween<float> From(float startValue)
        {
            return new Tween<float>(Mathf.Lerp, startValue);
        }

        public static ITween<Vector2> From(Vector2 startValue)
        {
            return new Tween<Vector2>(Vector2.Lerp, startValue);
        }

        public static ITween<Vector3> From(Vector3 startValue)
        {
            return new Tween<Vector3>(Vector3.Lerp, startValue);
        }

        public static ITween<Vector4> From(Vector4 startValue)
        {
            return new Tween<Vector4>(Vector4.Lerp, startValue);
        }

        public static ITween<Color> From(Color startValue)
        {
            return new Tween<Color>(Color.Lerp, startValue);
        }

        public static ITween<Color32> From(Color32 startValue)
        {
            return new Tween<Color32>(Color32.Lerp, startValue);
        }

        public static ITween<Quaternion> From(Quaternion startValue)
        {
            return new Tween<Quaternion>(Quaternion.Lerp, startValue);
        }
    }
}