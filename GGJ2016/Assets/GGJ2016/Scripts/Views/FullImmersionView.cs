using System;
using System.Collections;
using System.Linq;
using Assets.OutOfTheBox.Scripts.Audio;
using Assets.OutOfTheBox.Scripts.Inputs;
using Assets.OutOfTheBox.Scripts.Navigation;
using Assets.OutOfTheBox.Scripts.Timers;
using Assets.PaperGhost.Scripts.Views;
using Assets.OutOfTheBox.Scripts.Extensions;
using FlexiTweening;
using FlexiTweening.Extensions;
using Sense.Common.Navigation;
using Sense.Extensions;
using Sense.Injection;
using Sense.PropertyAttributes;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.OutOfTheBox.Scripts.Views
{
    public class FullImmersionView : InjectableBehaviour
    {
        [Inject] private AppSettings _appSettings;
        [Inject] private AudioManager _audioManager;
        [Inject] private Controller _controller;
        [Inject] private Navigator _navigator;
        [Inject] private Timer _timer;
        [Inject] private VideoFeed _videoFeed;

        [SerializeField] private CanvasGroup _clockMenuRoot;
        [SerializeField] private Image _cancelImmersionRing;
        [SerializeField] private Image _elapsedTimeRing;
        [SerializeField] private Text _timeRemainingText;
        [SerializeField] private RectTransform _contextDialogIdle;
        [SerializeField] private Text _contextDialogCancelling;

        [SerializeField] private TweenSettings _toggleClockMenuTweenSettings;

        private bool IsClockMenuActive
        {
            get { return _navigator.ImmersionClockEnabled; }
            set { _navigator.ImmersionClockEnabled = value; }
        }

        private bool IsCancellingImmersion { get; set; }

        private float _remainingTimeUntilHideClockMenu;
        private float _remainingTimeUntilCancelImmersion;
        private IEnumerator _co_clockMenuHideCooldown;
        private IEnumerator _co_cancelImmersionPolling;
        private IEnumerator _co_ShowClock;

        private ITween _toggleClockMenuTween;


        protected override void OnPostInject()
        {
            _navigator.AppStateChanged += NavigatorOnAppStateChanged;
            HideClockMenu(false);
        }

        private void NavigatorOnAppStateChanged(StateChange<AppStates> stateChange)
        {
            switch (stateChange.Previous)
            {
                case AppStates.Immersion:
                    StopCancelImmersionPolling();
                    HideClockMenu();
                    break;
            }

            switch (stateChange.Next)
            {
                case AppStates.Immersion:
                    gameObject.SetActive(true);
                    TimerOnStart();
                    break;
            }
        }

        private void TimerOnStart()
        {
            _timer.Finished += TimerOnFinished;
            _timer.Ticked += TimerOnTick;

            UpdateRemainingTimeText();
            _elapsedTimeRing.fillAmount = 0.0f;
            _timer.Set(_appSettings.ImmersionTime);
            _timer.Start();
        }

        private void TimerOnTick()
        {
            UpdateRemainingTimeText();
            _elapsedTimeRing.fillAmount = _timer.NormalizedTimeElapsed;

            if (ActivatingClockMenu() && !IsClockMenuActive)
            {
                _remainingTimeUntilCancelImmersion = _appSettings.RingMenuCooldown;
                UpdateCancelImmersionVisuals();
                ShowClockMenu();
            }
        }

        private void TimerOnFinished()
        {
            _timer.Finished -= TimerOnFinished;
            _timer.Ticked -= TimerOnTick;

            UpdateRemainingTimeText();
            _timer.Stop();
            _elapsedTimeRing.fillAmount = 1.0f;

            _navigator.ImmersionClockEnabled = false;
            _navigator.AppState = AppStates.EndingImmersion;
        }

        private void UpdateRemainingTimeText()
        {
            var remainingTimeSpan = _timer.TimeSpanRemaining;
            var seconds = remainingTimeSpan.Seconds == 0 && remainingTimeSpan.Milliseconds == 0
                ? 0
                : remainingTimeSpan.Seconds + 1;
            _timeRemainingText.text = string.Format("{0:00}:{1:00}", remainingTimeSpan.Minutes, seconds);
        }

        private void ShowClockMenu(bool animate = true)
        {
            _clockMenuRoot.gameObject.SetActive(true);
            IsClockMenuActive = true;

            if (_co_ShowClock.IsNotNull())
            {
                StopCoroutine(_co_ShowClock);
            }
            _co_ShowClock = Co_ShowClock(animate);
            StartCoroutine(_co_ShowClock);  
        }

        private void HideClockMenu(bool animate = true)
        {
            if (_co_ShowClock.IsNotNull())
            {
                StopCoroutine(_co_ShowClock);
                _co_ShowClock = null;
            }

            _toggleClockMenuTween.SafelyAbort();
            var duration = animate ? _toggleClockMenuTweenSettings.Duration : 0f;
            _toggleClockMenuTween = _clockMenuRoot.TweenAlpha()
                .To(0f, duration)
                .OnComplete(() =>
                {
                    _clockMenuRoot.gameObject.SetActive(false);
                })
                .Start();

            IsClockMenuActive = false;
            StopClockMenuHideCooldown();
            StopCancelImmersionPolling();
        }

        private IEnumerator Co_ShowClock(bool animate = true)
        {
            _toggleClockMenuTween.SafelyAbort();
            yield return new WaitForSeconds(1.00f);
            var duration = animate ? _toggleClockMenuTweenSettings.Duration : 0f;
            _toggleClockMenuTween = _clockMenuRoot.TweenAlpha()
                .To(1f, duration)
                .Easing(_toggleClockMenuTweenSettings.Easing)
                .OnComplete(StartClockMenuHideCooldown)
                .Start();

            StartCancelImmersionPolling();
        }

        private bool ActivatingClockMenu()
        {
            return _controller.Submit || _controller.Cancel;
        }

        private bool ActivatedCancel()
        {
            return _controller.Cancel;
        }

        private void StartClockMenuHideCooldown()
        {
            StopClockMenuHideCooldown();
            _co_clockMenuHideCooldown = Co_ClockMenuHideCooldown();
            StartCoroutine(_co_clockMenuHideCooldown);
        }

        private void StopClockMenuHideCooldown()
        {
            if (_co_clockMenuHideCooldown != null)
            {
                StopCoroutine(_co_clockMenuHideCooldown);
            }
            _co_clockMenuHideCooldown = null;
        }

        private IEnumerator Co_ClockMenuHideCooldown()
        {
            _remainingTimeUntilHideClockMenu = _appSettings.ClockMenuHideCooldown;
            while (_remainingTimeUntilHideClockMenu > 0f)
            {
                if (ActivatingClockMenu())
                {
                    _remainingTimeUntilHideClockMenu = _appSettings.ClockMenuHideCooldown;
                }
                else
                {
                    _remainingTimeUntilHideClockMenu -= Time.deltaTime;
                }
                yield return null;
            }
            HideClockMenu();
        }

        private void StartCancelImmersionPolling()
        {
            StopCancelImmersionPolling();
            _co_cancelImmersionPolling = Co_CancelImmersionPolling();

            StartCoroutine(_co_cancelImmersionPolling);
        }

        private void StopCancelImmersionPolling()
        {
            if (_co_cancelImmersionPolling != null)
            {
                StopCoroutine(_co_cancelImmersionPolling);
            }
            _co_cancelImmersionPolling = null;
            IsCancellingImmersion = false;
        }

        private IEnumerator Co_CancelImmersionPolling()
        {
            _remainingTimeUntilCancelImmersion = _appSettings.RingMenuCooldown;
            while (_remainingTimeUntilCancelImmersion > 0f)
            {
                IsCancellingImmersion = ActivatedCancel();
                _contextDialogIdle.gameObject.SetActive(!IsCancellingImmersion);
                _contextDialogCancelling.gameObject.SetActive(IsCancellingImmersion);

                if (IsCancellingImmersion)
                {
                    _remainingTimeUntilCancelImmersion -= Time.deltaTime;
                }
                else
                {
                    _remainingTimeUntilCancelImmersion += 2.0f*Time.deltaTime;
                }
                _remainingTimeUntilCancelImmersion = Mathf.Clamp(_remainingTimeUntilCancelImmersion, 0f, _appSettings.RingMenuCooldown);
                UpdateCancelImmersionVisuals();

                yield return null;
            }
            _timer.Finished -= TimerOnFinished;
            _timer.Ticked -= TimerOnTick;
            _timer.Pause();
            _navigator.AppState = AppStates.EndingImmersion;
        }

        private void UpdateCancelImmersionVisuals()
        {
            var normalizedTimeCancellingImmersion = (_appSettings.RingMenuCooldown - _remainingTimeUntilCancelImmersion) / _appSettings.RingMenuCooldown;
            _cancelImmersionRing.fillAmount = normalizedTimeCancellingImmersion;

            if (_contextDialogCancelling.gameObject.activeInHierarchy)
            {
                _contextDialogCancelling.text = string.Format("Returning to reality...\n{0}%", (int)(normalizedTimeCancellingImmersion*100));
            }
        }
    }
}