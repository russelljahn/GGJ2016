using Assets.OutOfTheBox.Scripts.Inputs;
using Assets.OutOfTheBox.Scripts.Navigation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Assets.GGJ2016.Scripts.Views
{
    public class GameOverPanel : Panel
    {
        [Inject] private Controller _controller;
        [Inject] private EventSystem _eventSystem;
        [Inject] private Navigator _navigator;
        [SerializeField] private Button _replayButton;


        protected override void OnPostInject()
        {
            _navigator.AppStateChanged += NavigatorOnAppStateChanged;
            _replayButton.onClick.AddListener(ReplayButtonOnPressed);
        }

        private void ReplayButtonOnPressed()
        {
            _navigator.AppState = AppStates.Gameplay;
        }

        private void NavigatorOnAppStateChanged(StateChange<AppStates> stateChange)
        {
            switch (stateChange.Previous)
            {
                case AppStates.GameOver:
                    Hide();
                    break;
            }

            switch (stateChange.Next)
            {
                case AppStates.GameOver:
                    Show();
                    _eventSystem.SetSelectedGameObject(_replayButton.gameObject);
                    break;
            }
        }
    }
}