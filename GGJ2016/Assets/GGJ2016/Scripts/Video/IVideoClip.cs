using System;
using UnityEngine;

namespace Sense 
{
    public interface IVideoClip
    {
        float Duration { get; }
        float PlaybackTime { get; }
        float RemainingTime { get; }
        bool IsPlaying { get; }
        Texture Texture { get; }

        event Action Finished;

        void Play();
        void Pause();
        void Stop();
    }
}
