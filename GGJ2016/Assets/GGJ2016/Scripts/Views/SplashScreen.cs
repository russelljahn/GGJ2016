using System;
using System.Collections;
using FlexiTweening.Extensions;
using Sense.Common.Navigation;
using Sense.Extensions;
using Sense.Injection;
using UnityEngine;
using Zenject;
using Assets.PaperGhost.Scripts.Views;
using Assets.OutOfTheBox.Scripts.Navigation;
using FlexiTweening;
using UnityEngine.UI;

namespace Sense.Views
{
    public class SplashScreen : Panel
    {
        [Inject] private Navigator _navigator;

        [SerializeField] private TweenSettings _fadeSettings;
        [SerializeField] private float _waitTime = 0.6f;

        [SerializeField] private CanvasGroup _companyPanel;
        [SerializeField] private CanvasGroup _sensePanel;

        private ITween _fadeTween;

        protected override void OnPostInject()
        {
            base.OnPostInject();
            _navigator.AppStateChanged += NavigatorOnAppStateChanged;
        }

        private void NavigatorOnAppStateChanged(StateChange<AppStates> stateChange)
        {
            if (stateChange.Previous == AppStates.Splash)
            {
                Hide();
            }

            if (stateChange.Next == AppStates.Splash)
            {
                StartCoroutine(Co_PlaySplash());
            }
        }

        private IEnumerator Co_PlaySplash()
        {
            Show(false);

            _sensePanel.gameObject.SetActive(true);
            _companyPanel.gameObject.SetActive(true);
            _sensePanel.alpha = 0f;
            _companyPanel.alpha = 0f;
            yield return new WaitForSeconds(_waitTime);

            _fadeTween.SafelyAbort();
            _fadeTween = _companyPanel.TweenAlpha()
                .To(1f, _fadeSettings)
                .Start();
            yield return new WaitForSeconds(_fadeSettings.Duration + _waitTime);

            _fadeTween.SafelyAbort();
            _fadeTween = _companyPanel.TweenAlpha()
               .To(0f, _fadeSettings)
               .Start();
            yield return new WaitForSeconds(_fadeSettings.Duration + _waitTime);

            _fadeTween.SafelyAbort();
            _fadeTween = _sensePanel.TweenAlpha()
                .To(1f, _fadeSettings)
                .Start();
            yield return new WaitForSeconds(_fadeSettings.Duration + 2.0f);

            _fadeTween.SafelyAbort();
            _fadeTween = _sensePanel.TweenAlpha()
                .To(0f, _fadeSettings)
                .Start();
            yield return new WaitForSeconds(_waitTime);

            _navigator.AppState = AppStates.MainMenu;
        }
    }
}
