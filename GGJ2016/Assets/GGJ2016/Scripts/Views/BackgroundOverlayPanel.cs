using System;
using Assets.PaperGhost.Scripts.Views;
using Assets.OutOfTheBox.Scripts.Navigation;
using FlexiTweening;
using FlexiTweening.Extensions;
using Sense.Common.Navigation;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using Zenject;

namespace Sense.Views
{
    public class BackgroundOverlayPanel : Panel
    {
        [Inject] private Navigator _navigator;
        [SerializeField] private Image _tintImage;
        [SerializeField] private TweenSettings _tintTweenSettings;
        [SerializeField] private DepthOfField _blur;

        private ITween _tintTween;

        protected override void OnPostInject()
        {
            base.OnPostInject();
            _navigator.AppStateChanged += NavigatorOnAppStateChanged;
            _navigator.ImmersionClockEnabledChanged += NavigatorOnImmersionClockEnabledChanged;
        }

        private void NavigatorOnImmersionClockEnabledChanged(StateChange<bool> stateChange)
        {
            if (stateChange.Next)
            {
                Show();
            }
            else if (_navigator.AppState == AppStates.Immersion)
            {
                Hide();
            }
        }

        private void NavigatorOnAppStateChanged(StateChange<AppStates> stateChange)
        {
            switch (stateChange.Next)
            {
                case AppStates.Splash:
                    Show();
                    break;

                case AppStates.MainMenu:
                    Show();
                    break;

                case AppStates.StartingImmersion:
                    Show();
                    break;

                case AppStates.Immersion:
                    Hide();
                    break;

                case AppStates.EndingImmersion:
                    Show();
                    break;

                default:
                    Hide();
                    break;
            }
        }

        protected override void Hide(bool animate = true, Action onComplete = null)
        {
            base.Hide(animate, onComplete);
            //_blur.enabled = false;
            //if (animate)
            //{
            //    _tintTween.SafelyAbort();
            //    _tintTween = FlexiTween
            //        .From(_blur.)
            //        .Start();
            //}
        }

        protected override void Show(bool animate = true, Action onComplete = null)
        {
            base.Show(animate, onComplete);
            //if (animate)
            //{
            //    _tintTween.SafelyAbort();
            //    _tintTween = _tintImage.TweenColor()
            //        .To(_initialTint, _tintTweenSettings)
            //        .Start();
            //}
            //_blur.enabled = true;
        }
    }
}