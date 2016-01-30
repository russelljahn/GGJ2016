using System;
using Assets.OutOfTheBox.Scripts.Audio;
using Assets.OutOfTheBox.Scripts.Navigation;
using Assets.PaperGhost.Scripts.Views;
using Sense.Common.Navigation;
using Sense.Extensions;
using Sense.PropertyAttributes;
using UnityEngine;
using Zenject;

namespace Assets.OutOfTheBox.Scripts.Views
{
    public class StartingImmersionPanel : Panel
    {
        [Inject] private Navigator _navigator;
        [Inject] private VideoFeed _videoFeed;
        [Inject] private AudioManager _audioManager;
        [Inject] private AppSettings _appSettings;
        [Inject] private MediaPlayerCtrl _mediaPlayer;

        [SerializeField, Min(0f)] private float _timeUntilFullImmersion = 10.0f;

        protected override void OnPostInject()
        {
            base.OnPostInject();

            _navigator.AppStateChanged += NavigatorOnAppStateChanged;
        }

        private void NavigatorOnAppStateChanged(StateChange<AppStates> stateChange)
        {
            switch (stateChange.Previous)
            {
                case AppStates.StartingImmersion:
                    Hide();
                    break;
            }

            switch (stateChange.Next)
            {
                case AppStates.StartingImmersion:
                    this.InvokeAfterTime(0.5f, () =>
                    {
                        gameObject.SetActive(true);
                        LoadMediaPlayer();
                        Show();
                    });
                    break;
            }
        }

        private void LoadMediaPlayer()
        {
            var currentAmbience = _appSettings.CurrentAmbience;
            _mediaPlayer.m_bAutoPlay = false;
            _mediaPlayer.m_bLoop = true;

            _mediaPlayer.OnReady += MediaPlayerOnReady;
            _mediaPlayer.Load(currentAmbience.VideoPathRelativeToStreamingAssets);

            if (Application.isEditor)
            {
                MediaPlayerOnReady();
            }
        }

        private void MediaPlayerOnReady()
        {
            _mediaPlayer.OnReady -= MediaPlayerOnReady;
            _mediaPlayer.Play();

            var timeUntilFadeInAudio = 0.25f * _timeUntilFullImmersion;
            this.InvokeAfterTime(timeUntilFadeInAudio, () =>
            {
                var clip = _appSettings.BgMusic1;
                _audioManager.LoadClip(clip, 0f, _timeUntilFullImmersion - timeUntilFadeInAudio, true);
                _audioManager.PlayTrack(clip);
                _audioManager.Fade(clip, _appSettings.BgMusic1Volume);
            });

            var timeUntilFadeOutVideoFeed = 0.5f * _timeUntilFullImmersion;

            this.InvokeAfterTime(timeUntilFadeOutVideoFeed, () =>
            {
                _videoFeed.FadeTo(0f, _timeUntilFullImmersion - timeUntilFadeOutVideoFeed);
            });

            this.InvokeAfterTime(_timeUntilFullImmersion, OnFullImmersion);
        }

        private void OnFullImmersion()
        {
            Hide();
            this.InvokeAfterTime(0.5f, () => {
                _navigator.AppState = AppStates.Immersion;
            });
        }
    }
}