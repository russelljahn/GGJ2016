using System;
using System.Collections;
using UnityEngine;

namespace Sense.Extensions
{
    public static class MonoBehaviourExtensions
    {
        #region Coroutine extensions

        public static void InvokeAfterTime(this MonoBehaviour behaviour, float time, params Action[] onComplete)
        {
            InvokeAfterCoroutine(behaviour, Co_YieldForTime(time), onComplete);
        }

        public static void InvokeAtEndOfFrame(this MonoBehaviour behaviour, params Action[] onComplete)
        {
            InvokeAfterCoroutine(behaviour, Co_WaitForEndOfFrame(), onComplete);
        }

        public static void InvokeNextFrame(this MonoBehaviour behaviour, params Action[] onComplete)
        {
            InvokeAfterFrames(behaviour, 1, onComplete);
        }

        public static void InvokeAfterFrames(this MonoBehaviour behaviour, int frames, params Action[] onComplete)
        {
            if (frames < 0)
            {
                throw new ArgumentException("Frames cannot be negative!", "frames");
            }
            behaviour.StartCoroutine(Co_InvokeAfterFramesHelper(frames, onComplete));
        }

        public static void InvokeAfterCoroutine(this MonoBehaviour behaviour, IEnumerator coroutine, params Action[] onComplete)
        {
            behaviour.StartCoroutine(Co_InvokeAfterCoroutineHelper(behaviour, coroutine, onComplete));
        }

        private static IEnumerator Co_WaitForEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
        }

        private static IEnumerator Co_YieldForTime(float time)
        {
            yield return new WaitForSeconds(time);
        }

        private static IEnumerator Co_InvokeAfterCoroutineHelper(this MonoBehaviour behaviour, IEnumerator coroutine, params Action[] onComplete)
        {
            yield return behaviour.StartCoroutine(coroutine);
            foreach (var callback in onComplete)
            {
                if (callback != null)
                {
                    callback();
                }
            }
        }

        private static IEnumerator Co_InvokeAfterFramesHelper(int frames, params Action[] onComplete)
        {
            while (frames > 0)
            {
                yield return null;
                --frames;
            }
            foreach (var callback in onComplete)
            {
                if (callback != null)
                {
                    callback();
                }
            }
        }

        #endregion
    }
}
