using System.Collections.Generic;
using UnityEngine;

namespace Assets.OutOfTheBox.Scripts.Components
{

    public class SpeedTimeKey : MonoBehaviour
    {
        public List<KeyCode> Keys = new List<KeyCode> {KeyCode.Z};
        public float NormalTimeScale = 1.0f;
        public float SpeededTimeScale = 5.0f;

        private bool _speededLastFrame;


        private void Update()
        {
            var speeding = IsSpeedTimeKeyDown();

            if (!speeding && _speededLastFrame)
            {
                Time.timeScale = NormalTimeScale;
            }
            else if (speeding && !_speededLastFrame)
            {
                Time.timeScale = SpeededTimeScale;
            }
            _speededLastFrame = speeding;
        }


        private bool IsSpeedTimeKeyDown()
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < Keys.Count; ++i)
            {
                if (Input.GetKey(Keys[i]))
                {
                    return true;
                }
            }
            return false;
        }


        private void OnDestroy()
        {
            Time.timeScale = NormalTimeScale;
        }
    }
}