using System;
using Assets.OutOfTheBox.Scripts.Utils;
using Assets.OutOfTheBox.Scripts.Inputs;
using FlexiTweening;
using FlexiTweening.Extensions;
using Sense.Injection;
using Sense.PropertyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Assets.OutOfTheBox.Scripts.Gui
{
    public class AmbienceSelector : ButtonGroupElement
    {
        [Inject] private AppSettings _appSettings;

        [SerializeField] private Text _currentAmbienceNameText;
        [SerializeField] private RectTransform _leftArrow;
        [SerializeField] private RectTransform _rightArrow;

        [SerializeField, Min(0f)] private float _pulseAmount = 1.25f;
        [SerializeField] private TweenSettings _pulseTweenSettings;

        private int _selectedAmbienceIndex;
        private Vector2 _initialArrowSizeDelta;
        private ITween _pulseTween;

        protected override void OnPostInject()
        {
            base.OnPostInject();
            _appSettings.CurrentAmbience = _appSettings.AmbiencePresets[_selectedAmbienceIndex];
            _initialArrowSizeDelta = _leftArrow.sizeDelta;
            UpdateGui();
        }

        public void SelectPrevious()
        {
            _selectedAmbienceIndex = MathfUtils.Wrap(_selectedAmbienceIndex + 1, 0, _appSettings.AmbiencePresets.Count - 1);
            _appSettings.CurrentAmbience = _appSettings.AmbiencePresets[_selectedAmbienceIndex];

            DoPulseAnimation(_leftArrow);
            UpdateGui();
        }

        public void SelectNext()
        {
            _selectedAmbienceIndex = MathfUtils.Wrap(_selectedAmbienceIndex - 1, 0, _appSettings.AmbiencePresets.Count - 1);
            _appSettings.CurrentAmbience = _appSettings.AmbiencePresets[_selectedAmbienceIndex];

            DoPulseAnimation(_rightArrow);
            UpdateGui();
        }

        private void UpdateGui()
        {
            var currentAmbience = _appSettings.CurrentAmbience;
            _currentAmbienceNameText.text = currentAmbience.Name;
        }

        private void DoPulseAnimation(RectTransform rectTransform)
        {
            _pulseTween.SafelyAbort();
            rectTransform.sizeDelta = _initialArrowSizeDelta;
            var pulseSize = _initialArrowSizeDelta * _pulseAmount;
            _pulseTween = rectTransform.TweenSize()
                .To(pulseSize, _pulseTweenSettings)
                .Start();
        }
    }
}
