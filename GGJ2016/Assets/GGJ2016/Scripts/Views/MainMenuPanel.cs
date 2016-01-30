using System;
using System.Collections;
using Assets.OutOfTheBox.Scripts;
using Assets.OutOfTheBox.Scripts.Gui;
using Assets.OutOfTheBox.Scripts.Inputs;
using Assets.OutOfTheBox.Scripts.Navigation;
using FlexiTweening;
using FlexiTweening.Extensions;
using Sense.Common.Navigation;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Zenject;

namespace Sense.Views
{
    public class MainMenuPanel : Assets.PaperGhost.Scripts.Views.Panel
    {
        [Inject] private AppSettings _appSettings;
        [Inject] private EventSystem _eventSystem;
        [Inject] private Controller _controller;
        [Inject] private Navigator _navigator;
        [Inject] private VideoFeed _videoFeed;

        [SerializeField] private Button _startButton;
        [SerializeField] private CanvasGroup _ringMenuRoot;
        [SerializeField] private TweenSettings _toggleRingMenuTweenSettings;

        [SerializeField] private Image _startImmersionRing;
        [SerializeField] private RectTransform _contextDialogIdle;
        [SerializeField] private RectTransform _contextDialogInitializing;
        [SerializeField] private Text _contextDialogInitializingText;
        [SerializeField] private AmbienceSelector _ambienceSelector;

        private bool IsRingMenuActive { get; set; }
        private bool IsStartingImmersion { get; set; }

        private IEnumerator _co_clockMenuHideCooldown;
        private IEnumerator _co_cancelImmersionPolling;

        private float _remainingTimeUntilHideRingMenu;
        private float _remainingTimeUntilStartImmersion;

        private float _timeSincePressed;

        private IEnumerator _co_ringMenuHideCooldown;
        private IEnumerator _co_startImmersionPolling;
        private ITween _toggleRingMenuTween;

        private const float TapTimeThreshold = 0.5f;


        protected override void OnPostInject()
        {
            base.OnPostInject();

            _navigator.AppStateChanged += NavigatorOnAppStateChanged;
            _controller.ReleasedSubmit += ReleasedSubmit;

            _eventSystem.SetSelectedGameObject(_startButton.gameObject);
        }

        private void ReleasedSubmit()
        {
            if (_timeSincePressed <= TapTimeThreshold)
            {
                _ambienceSelector.SelectNext();
            }
        }

        private void Update()
        {
            if (_controller.Submit || _controller.Cancel)
            {
                _timeSincePressed += Time.deltaTime;
            }
            else
            {
                _timeSincePressed = 0f;
            }
        }

        private void NavigatorOnAppStateChanged(StateChange<AppStates> stateChange)
        {
            switch (stateChange.Previous)
            {
                case AppStates.MainMenu:
                    Hide();
                    StopImmersionPolling();
                    break;
            }

            switch (stateChange.Next)
            {
                case AppStates.MainMenu:
                    Show();
                    _videoFeed.Start();
                    StartImmersionPolling();
                    break;
            }
        }

        private void StartImmersionPolling()
        {
            StopImmersionPolling();
            _co_cancelImmersionPolling = Co_StartImmersionPolling();

            StartCoroutine(_co_cancelImmersionPolling);
        }

        private void StopImmersionPolling()
        {
            if (_co_cancelImmersionPolling != null)
            {
                StopCoroutine(_co_cancelImmersionPolling);
            }
            _co_cancelImmersionPolling = null;
            IsStartingImmersion = false;
        }

        private IEnumerator Co_StartImmersionPolling()
        {
            _remainingTimeUntilStartImmersion = _appSettings.RingMenuCooldown;
            while (_remainingTimeUntilStartImmersion > 0f)
            {
                IsStartingImmersion = (_controller.Submit || _controller.Cancel) && _timeSincePressed > TapTimeThreshold;

                _contextDialogIdle.gameObject.SetActive(!IsStartingImmersion);
                _contextDialogInitializing.gameObject.SetActive(IsStartingImmersion);

                if (IsStartingImmersion)
                {
                    _remainingTimeUntilStartImmersion -= Time.deltaTime;
                }
                else
                {
                    _remainingTimeUntilStartImmersion += 2.0f * Time.deltaTime;
                }
                _remainingTimeUntilStartImmersion = Mathf.Clamp(_remainingTimeUntilStartImmersion, 0f, _appSettings.RingMenuCooldown);
                UpdateStartImmersionVisuals();

                yield return null;
            }

            _navigator.AppState = AppStates.StartingImmersion;
        }

        private void UpdateStartImmersionVisuals()
        {
            var normalizedTimeCancellingImmersion = (_appSettings.RingMenuCooldown - _remainingTimeUntilStartImmersion) / _appSettings.RingMenuCooldown;
            _startImmersionRing.fillAmount = normalizedTimeCancellingImmersion;

            if (_contextDialogInitializing.gameObject.activeInHierarchy)
            {
                _contextDialogInitializingText.text = string.Format("Initializing...\n{0}%", (int)(normalizedTimeCancellingImmersion * 100));
            }
        }
    }
}