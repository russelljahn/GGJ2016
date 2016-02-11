using System;
using FlexiTweening;
using FlexiTweening.Extensions;
using UnityEngine;
using Sense.Injection;
using Sense.PropertyAttributes;
using Assets.OutOfTheBox.Scripts.Extensions;

namespace Assets.PaperGhost.Scripts.Components
{
    [RequireComponent(typeof (CanvasGroup))]
    public class Fader : InjectableBehaviour
    {
        private CanvasGroup _canvasGroup;
        private float _targetAlpha;
        private ITween _tween;
        private bool _isDestroyed;

        [SerializeField] private AnimationCurve _fadeEasing = AnimationCurveHelper.GetLinearCurve();
        [SerializeField, Min(0f)] private float _fadeTime = 0.5f;
        [SerializeField, Readonly] private FadingStates _fadingState = FadingStates.NotFading;
        [SerializeField, Readonly] private bool _isVisible;
        [SerializeField] private bool _preventInteractionWhenFading = true;

        public event Action StartedFadingIn;
        public event Action StartedFadingOut;
        public event Action EndedFadingIn;
        public event Action EndedFadingOut;

        public float FadeTime
        {
            get { return _fadeTime; }
            set { _fadeTime = Mathf.Max(0f, value); }
        }

        public AnimationCurve FadeEasing
        {
            get { return _fadeEasing; }
            set { _fadeEasing = value; }
        }

        public bool PreventInteractionWhenFading
        {
            get { return _preventInteractionWhenFading; }
            set { _preventInteractionWhenFading = value; }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            protected set { _isVisible = value; }
        }

        private bool IsFading
        {
            get { return _fadingState != FadingStates.NotFading; }
        }

        private CanvasGroup CanvasGroup
        {
            get { return _canvasGroup ?? (_canvasGroup = GetComponent<CanvasGroup>()); }
        }

        public void SetFadeParameters(TweenSettings faderSettings)
        {
            FadeTime = faderSettings.Duration;
            FadeEasing = faderSettings.Easing;
        }

        public TweenSettings GetFadeParameters()
        {
            return new TweenSettings
            {
                Duration = FadeTime,
                Easing = FadeEasing
            };
        }

        public void FadeIn(bool animate = true)
        {
            if (_isDestroyed)
            {
                return;
            }
            if (animate)
            {
                FadeInWithAnimation();
            }
            else
            {
                FadeInWithoutAnimation();
            }
        }

        private void FadeInWithoutAnimation()
        {
            if (IsFading)
            {
                _fadingState = FadingStates.FadingIn;
                _tween.SafelyAbort();
            }
            else
            {
                OnEndedFadingIn();
            }

            CanvasGroup.alpha = 1.0f;
        }

        private void FadeInWithAnimation()
        {
            _targetAlpha = 1.0f;

            if (PreventInteractionWhenFading)
            {
                CanvasGroup.blocksRaycasts = false;
                CanvasGroup.interactable = false;
            }

            _tween.SafelyAbort();

            _tween = CanvasGroup.TweenAlpha()
                .To(_targetAlpha, FadeTime)
                .Easing(FadeEasing)
                .OnComplete(OnEndedFade)
                .Start();

            _fadingState = FadingStates.FadingIn;

            StartedFadingIn.SafelyInvoke();
        }

        public void FadeOut(bool animate = true)
        {
            if (_isDestroyed)
            {
                return;
            }
            if (animate)
            {
                FadeOutWithAnimation();
            }
            else
            {
                FadeOutWithoutAnimation();
            }
        }

        private void FadeOutWithoutAnimation()
        {
            if (IsFading)
            {
                _fadingState = FadingStates.FadingOut;
                _tween.SafelyAbort();
            }
            else
            {
                OnEndedFadingOut();
            }

            CanvasGroup.alpha = 0.0f;
        }

        private void FadeOutWithAnimation()
        {
            _targetAlpha = 0.0f;

            if (PreventInteractionWhenFading)
            {
                CanvasGroup.blocksRaycasts = false;
                CanvasGroup.interactable = false;
            }

            _tween.SafelyAbort();

            _tween = CanvasGroup.TweenAlpha()
                .To(_targetAlpha, FadeTime)
                .Easing(FadeEasing)
                .OnComplete(OnEndedFade)
                .Start();

            _fadingState = FadingStates.FadingOut;

            StartedFadingOut.SafelyInvoke();
        }

        private void OnEndedFade()
        {
            switch (_fadingState)
            {
                case FadingStates.FadingIn:
                    OnEndedFadingIn();
                    break;

                case FadingStates.FadingOut:
                    OnEndedFadingOut();
                    break;

                default:
                    throw new Exception(gameObject + ", Invalid state at the end of the fade: " + _fadingState);
            }
        }

        private void OnEndedFadingIn()
        {
            if (_isDestroyed)
            {
                return;
            }
            if (PreventInteractionWhenFading)
            {
                CanvasGroup.blocksRaycasts = true;
                CanvasGroup.interactable = true;
            }
            _fadingState = FadingStates.NotFading;
            IsVisible = true;

            EndedFadingIn.SafelyInvoke();
        }

        private void OnEndedFadingOut()
        {
            if (_isDestroyed)
            {
                return;
            }
            if (PreventInteractionWhenFading)
            {
                CanvasGroup.blocksRaycasts = false;
                CanvasGroup.interactable = false;
            }
            _fadingState = FadingStates.NotFading;
            IsVisible = false;

            EndedFadingOut.SafelyInvoke();
        }

        protected override void OnDestroyed()
        {
            _tween.SafelyAbort();
            _isDestroyed = true;
        }

        private enum FadingStates
        {
            NotFading = 0,
            FadingIn = 1,
            FadingOut = 2
        }
    }
}