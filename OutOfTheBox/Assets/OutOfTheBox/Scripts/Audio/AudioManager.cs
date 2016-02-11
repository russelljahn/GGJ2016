using System.Collections.Generic;
using System.Linq;
using Assets.OutOfTheBox.Scripts.Utils;
using FlexiTweening.Extensions;
using Sense.Extensions;
using Sense.Injection;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace Assets.OutOfTheBox.Scripts.Audio
{
    public class AudioManager : InjectableBehaviour
    {
        [Inject] private AudioClips _audioClips;

        private readonly List<AudioSource> _tracks = new List<AudioSource>();
        private const int NumTracks = 5;

        [SerializeField] private AnimationCurve _fadeInEasing = AnimationCurveUtils.GetLinearCurve();
        [SerializeField] private AnimationCurve _fadeOutEasing = AnimationCurveUtils.GetLinearCurve();

        [SerializeField] private float _fadeTime = 1.0f;

        protected override void OnPostInject()
        {
            SetupTracks();
        }

        private void SetupTracks()
        {
            var sources = GetComponents<AudioSource>();
            int trackId = 0;
            for (; trackId < NumTracks; ++trackId)
            {
                if (trackId >= sources.Count() || trackId >= NumTracks)
                {
                    break;
                }
                var source = sources[trackId];
                SetDefaultSourceSettings(source);
                _tracks.Add(source);
            }

            while (trackId < NumTracks)
            {
                var source = gameObject.AddComponent<AudioSource>();
                SetDefaultSourceSettings(source);
                _tracks.Add(source);
                ++trackId;
            }
        }


        private void SetDefaultSourceSettings(AudioSource source)
        {
            source.loop = false;
            source.playOnAwake = false;
            source.volume = 1f;
            source.pitch = 1f;
            source.spatialBlend = 0f;
        }


        public void LoadClip(string clipName, float volume = 1.0f, float pitch = 1.0f, bool loop = false)
        {
            var trackId = _audioClips.GetClipTrackId(clipName);
            var clip = _audioClips.GetClip(clipName);
            LoadClip(trackId, clip, volume, pitch, loop);
        }


        private void LoadClip(int trackId, AudioClip clip, float volume = 1.0f, float pitch = 1.0f, bool loop = false)
        {
            var track = _tracks[trackId];
            track.time = 0f;
            track.clip = clip;
            track.volume = volume;
            track.pitch = pitch;
            track.loop = loop;
        }


        public void PlayTrack(string clipName)
        {
            var trackId = _audioClips.GetClipTrackId(clipName);
            PlayTrack(trackId);
        }


        private void PlayTrack(int trackId)
        {
            var track = _tracks[trackId];
            track.Play();
        }


        public void PlayTrackOneShot(string clipName, float volume = 1.0f, float pitch = 1.0f)
        {
            var clip = _audioClips.GetClip(clipName);
            var trackId = _audioClips.GetClipTrackId(clipName);
            LoadClip(trackId, clip, volume, pitch);
            PlayTrackOneShot(trackId);
        }


        private void PlayTrackOneShot(int trackId)
        {
            var track = _tracks[trackId];
            track.PlayOneShot(track.clip);
        }


        public void Crossfade(string fadeOutClipName, string fadeInClipName, float fadeOutVolume = 0.0f,
            float fadeInVolume = 1.0f)
        {
            var fadeOutTrackId = _audioClips.GetClipTrackId(fadeOutClipName);
            var fadeInTrackId = _audioClips.GetClipTrackId(fadeInClipName);

            if (fadeInTrackId == fadeOutTrackId)
            {
                Debug.LogError(string.Format("'{0}' and '{1}' share the same track id of '{2}'!", fadeOutClipName,
                    fadeInClipName, fadeInTrackId));
                return;
            }

            Crossfade(fadeOutTrackId, fadeInTrackId, fadeOutVolume, fadeInVolume);
        }

        public float GetTime(string clipName)
        {
            var trackId = _audioClips.GetClipTrackId(clipName);
            var track = _tracks[trackId];

            return track.time;
        }

        public void SetTime(string clipName, float time)
        {
            time = Mathf.Max(0f, time);
            var trackId = _audioClips.GetClipTrackId(clipName);
            var track = _tracks[trackId];

            track.time = time;
        }

        private void Crossfade(int fadeOutTrackId, int fadeInTrackId, float fadeOutVolume = 0.0f,
            float fadeInVolume = 1.0f)
        {
            Fade(fadeOutTrackId, fadeOutVolume, _fadeOutEasing);
            Fade(fadeInTrackId, fadeInVolume, _fadeInEasing);
        }


        public void Fade(string clipName, float volume = 1.0f, AnimationCurve easing = null)
        {
            easing = easing ?? AnimationCurveUtils.GetLinearCurve();
            var trackId = _audioClips.GetClipTrackId(clipName);
            var fadeTrack = _tracks[trackId];

            //TODO: Abort audio if already tweening
            fadeTrack.TweenVolume()
                .To(volume, _fadeTime)
                .Easing(easing)
                .Start();
        }


        private void Fade(int trackId, float volume = 1.0f, AnimationCurve easing = null)
        {
            easing = easing ?? AnimationCurveUtils.GetLinearCurve();
            var fadeTrack = _tracks[trackId];

            //TODO: Abort audio if already tweening
            fadeTrack.TweenVolume()
                .To(volume, _fadeTime)
                .Easing(easing)
                .Start();
        }

    }
}