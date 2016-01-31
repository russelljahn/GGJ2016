using System;
using Assets.OutOfTheBox.Scripts.Extensions;
using FlexiTweening;
using FlexiTweening.Extensions;
using Sense.Injection;
using UnityEngine;

namespace Assets.GGJ2016.Scripts.Views
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    public abstract class Panel : InjectableBehaviour
    {
        [SerializeField] protected TweenSettings ToggleTweenSettings;

        protected ITween ToggleTween;

        private CanvasGroup _canvasGroup;
        protected CanvasGroup CanvasGroup
        {
            get { return _canvasGroup ?? (_canvasGroup = GetComponent<CanvasGroup>()); }
        }

        private RectTransform _rectTransform;
        protected RectTransform RectTransform
        {
            get { return _rectTransform ?? (_rectTransform = GetComponent<RectTransform>()); }
        }

        protected override void OnPostInject()
        {
            Hide(false);
            gameObject.SetActive(true);
        }

        protected virtual void Show(bool animate = true, Action onComplete = null)
        {
            gameObject.SetActive(true);

            ToggleTween.SafelyAbort();
            var duration = animate ? ToggleTweenSettings.Duration : 0f;
            ToggleTween = CanvasGroup.TweenAlpha()
                .To(1f, duration)
                .Easing(ToggleTweenSettings.Easing)
                .OnComplete(onComplete)
                .Start();

            CanvasGroup.blocksRaycasts = true;
            CanvasGroup.interactable = true;
        }

        protected virtual void Hide(bool animate = true, Action onComplete = null)
        {
            ToggleTween.SafelyAbort();
            var duration = animate ? ToggleTweenSettings.Duration : 0f;
            ToggleTween = CanvasGroup.TweenAlpha()
                .To(0f, duration)
                .Easing(ToggleTweenSettings.Easing)
                .OnComplete(() =>
                {
                    //gameObject.SetActive(false);
                    onComplete.SafelyInvoke();
                })
                .Start();

            CanvasGroup.blocksRaycasts = false;
            CanvasGroup.interactable = false;
        }
    }
}
