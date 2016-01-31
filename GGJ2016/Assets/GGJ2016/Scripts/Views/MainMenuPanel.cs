using Assets.OutOfTheBox.Scripts.Inputs;
using Assets.OutOfTheBox.Scripts.Navigation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Assets.GGJ2016.Scripts.Views
{
    public class MainMenuPanel : Panel
    {
        [Inject] private Controller _controller;
        [Inject] private EventSystem _eventSystem;
        [Inject] private Navigator _navigator;
        [SerializeField] private Button _startButton;


        protected override void OnPostInject()
        {
            _navigator.AppStateChanged += NavigatorOnAppStateChanged;
            _startButton.onClick.AddListener(StartButtonOnPressed);
        }

        private void StartButtonOnPressed()
        {
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