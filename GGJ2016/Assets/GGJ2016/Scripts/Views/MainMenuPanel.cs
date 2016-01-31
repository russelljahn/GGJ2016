using Assets.OutOfTheBox.Scripts.Inputs;
using Assets.OutOfTheBox.Scripts.Navigation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using Assets.OutOfTheBox.Scripts.Audio;

namespace Assets.GGJ2016.Scripts.Views
{
    public class MainMenuPanel : Panel
    {
		[Inject] private AudioManager _audioManager;
        [Inject] private Controller _controller;
        [Inject] private EventSystem _eventSystem;
        [Inject] private Navigator _navigator;
        [SerializeField] private Button _startButton;


        protected override void OnPostInject()
        {
            _navigator.AppStateChanged += NavigatorOnAppStateChanged;
            _startButton.onClick.AddListener(StartButtonOnPressed);

			_audioManager.LoadClip(AudioClips.BgLevel1, 1.0f, 1.0f, true);
			_audioManager.PlayTrack(AudioClips.BgLevel1);
        }

        private void StartButtonOnPressed()
        {
			_audioManager.LoadClip(AudioClips.BgLevel1, 0.0f);
            _navigator.AppState = AppStates.Gameplay;
        }

        private void NavigatorOnAppStateChanged(StateChange<AppStates> stateChange)
        {
            switch (stateChange.Previous)
            {
                case AppStates.MainMenu:
                    Hide();
                    break;
            }

            switch (stateChange.Next)
            {
                case AppStates.MainMenu:
                    Show();
                    _eventSystem.SetSelectedGameObject(_startButton.gameObject);
                    break;
            }
        }
    }
}