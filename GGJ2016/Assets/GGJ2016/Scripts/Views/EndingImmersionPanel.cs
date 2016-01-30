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
    public class EndingImmersionPanel : Panel
    {
        [Inject] private AudioManager _audioManager;
        [Inject] private AppSettings _appSettings;
        [Inject] private MediaPlayerCtrl _mediaPlayer;
        [Inject] private Navigator _navigator;
        [Inject] private VideoFeed _videoFeed;

        [SerializeField, Min(0f)] private float _timeUntilNoImmersion = 10.0f;

        protected override void OnPostInject()
        {
            base.OnPostInject();

            _navigator.AppStateChanged += NavigatorOnAppStateChanged;
        }

        private void NavigatorOnAppStateChanged(StateChange<AppStates> stateChange)
        {
            switch (stateChange.Previous)
            {
                case AppStates.EndingImmersion:
                    Hide();
                    break;
            }

            switch (stateChange.Next)
            {
                case AppStates.EndingImmersion:
                    this.InvokeAfterTime(0.5f, () =>
                    {
                        Show();

                        var timeUntilFadeOutAudio = 0.25f*_timeUntilNoImmersion;
                        this.InvokeAfterTime(timeUntilFadeOutAudio, () =>
                        {
                            var clip = _appSettings.BgMusic1;
                            _audioManager.Fade(clip, 0f);
                        });

                        var timeUntilFadeInVideoFeed = 0.5f*_timeUntilNoImmersion;

                        this.InvokeAfterTime(timeUntilFadeInVideoFeed, () =>
                        {
                            _videoFeed.FadeTo(1f, _timeUntilNoImmersion - timeUntilFadeInVideoFeed);
                        });

                        this.InvokeAfterTime(_timeUntilNoImmersion, OnNoImmersion);
                    });
                    break;
            }
        }

        private void OnNoImmersion()
        {
            _mediaPlayer.Stop();
            Hide();
            this.InvokeAfterTime(0.5f, () =>
            {
                _navigator.AppState = AppStates.MainMenu;
            });
        }
    }
}