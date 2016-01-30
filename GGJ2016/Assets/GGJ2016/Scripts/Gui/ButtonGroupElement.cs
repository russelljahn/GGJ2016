using System;
using FlexiTweening;
using FlexiTweening.Extensions;
using Sense.Injection;
using Sense.PropertyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Assets.OutOfTheBox.Scripts.Gui
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ButtonGroupElement : InjectableBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler
    {
        [Inject] private EventSystem _eventSystem;

        [SerializeField, Clamp01] private float _selectedAlpha = 1.0f;
        [SerializeField, Clamp01] private float _deselectedAlpha = 0.35f;

        [SerializeField] private TweenSettings _fadeTweenSettings;

        private CanvasGroup _canvasGroup;
        private ITween _fadeTween;

        public bool Selected { get; private set; }

        protected override void OnPostInject()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            Selected = _eventSystem.currentSelectedGameObject == gameObject;
        }

        public void OnSelect(BaseEventData eventData)
        {
            Selected = true;
            FadeTo(_selectedAlpha);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            Selected = false;
            FadeTo(_deselectedAlpha);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _eventSystem.SetSelectedGameObject(gameObject);
        }

        private void FadeTo(float alpha)
        {
            _fadeTween.SafelyAbort();

            _fadeTween = _canvasGroup.TweenAlpha()
                .To(alpha, _fadeTweenSettings)
                .Start();
        }
    }
}
