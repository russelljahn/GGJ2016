using System;
using Sense.PropertyAttributes;
using UnityEngine;

namespace Sense.Video 
{
    public class DummyClip : MonoBehaviour, IVideoClip
    {
        [SerializeField, Min(0f)] private float _duration = 5.0f;
        [SerializeField, Readonly] private bool _isPlaying;
        [SerializeField] private Texture _texture;

        public float Duration
        {
            get { return _duration; }
        }

        public float PlaybackTime { get; set; }

        public float RemainingTime
        {
            get { return Math.Max(0f, Duration - PlaybackTime); }
        }

        public bool IsPlaying
        {
            get { return _isPlaying; }
            private set { _isPlaying = value; }
        }

        public Texture Texture
        {
            get { return _texture; }
        }

        public event Action Finished;

        public void Play()
        {
            IsPlaying = true;
        }

        public void Pause()
        {
            IsPlaying = false;
        }

        public void Stop()
        {
            PlaybackTime = 0f;
            IsPlaying = false;
        }

        private void Update()
        {
            if (!IsPlaying)
            {
                return;
            }

            if (PlaybackTime >= Duration)
            {
                PlaybackTime = Duration;
                IsPlaying = false;

                var handler = Finished;
                if (handler != null)
                {
                    handler.Invoke();
                }
            }
            else
            {
                PlaybackTime += Time.deltaTime;
            }
        }
    }
}
