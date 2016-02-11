using System;
using Assets.OutOfTheBox.Scripts.Extensions;
using UnityEngine;

namespace Assets.OutOfTheBox.Scripts.Timers
{
    public class Timer
    {
        public bool IsRunning { get; private set; }
        public bool IsFinished { get; private set; }
        public bool IsPaused { get; private set; }

        public float TimeElapsed { get; private set; }

        public float TimeRemaining
        {
            get { return TimeMaximum - TimeElapsed; }
        }

        public float NormalizedTimeElapsed
        {
            get { return TimeElapsed/TimeMaximum; }
        }

        public float NormalizedTimeRemaining
        {
            get { return 1.0f - NormalizedTimeElapsed; }
        }

        public TimeSpan TimeSpanElapsed
        {
            get { return TimeSpan.FromSeconds(TimeElapsed); }
        }

        public TimeSpan TimeSpanRemaining
        {
            get { return TimeSpan.FromSeconds(TimeRemaining); }
        }

        //TODO: Determine a more understandable name.
        private float _timeMaximum;

        public float TimeMaximum
        {
            get { return _timeMaximum; }
            private set { _timeMaximum = Mathf.Max(0f, value); }
        }

        public event Action Ticked;
        public event Action Finished;


        public Timer(Ticker ticker)
        {
            ticker.Ticked += TickerOnTicked;
        }

        private void TickerOnTicked(float deltaTime)
        {
            if (IsRunning && !IsPaused && !IsFinished)
            {
                TimeElapsed += deltaTime;
                Ticked.SafelyInvoke();

                HandleFinished();
            }
        }

        private void HandleFinished()
        {
            if (TimeElapsed >= TimeMaximum)
            {
                TimeElapsed = TimeMaximum;
                IsFinished = true;
                IsRunning = false;
                Finished.SafelyInvoke();
            }
        }

        public void Start()
        {
            IsRunning = true;
            IsPaused = false;
        }

        public void Pause()
        {
            IsRunning = false;
            IsPaused = true;
        }

        public void Stop()
        {
            IsRunning = false;
            IsPaused = false;
            IsFinished = false;
            TimeElapsed = 0f;
        }

        public void Set(float timeMaximum)
        {
            Stop();
            TimeMaximum = timeMaximum;
        }

        public override string ToString()
        {
            return
                string.Format("[Timer, IsRunning: {0}, IsPaused: {1}, IsFinished: {2}, TimeElapsed: {3}/{4}",
                    IsRunning,
                    IsPaused,
                    IsFinished,
                    TimeElapsed,
                    TimeMaximum
                    );
        }
    }
}
