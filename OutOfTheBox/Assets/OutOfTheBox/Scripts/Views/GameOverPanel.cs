using Assets.OutOfTheBox.Scripts.Inputs;
using Assets.OutOfTheBox.Scripts.Navigation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using Assets.OutOfTheBox.Scripts;
using UnityEngine.SceneManagement;
using Assets.OutOfTheBox.Scripts.Audio;
using Assets.OutOfTheBox.Scripts.Entities;
using Sense.Extensions;

namespace Assets.OutOfTheBox.Scripts.Views
{
    public class GameOverPanel : Panel
    {
		[Inject] private AppSettings _appSettings;
		[Inject] private AudioManager _audioManager;
        [Inject] private Controller _controller;
		[Inject] private CatStats _catStats;
        [Inject] private EventSystem _eventSystem;
        [Inject] private Navigator _navigator;

		[SerializeField] private Image _gameOverScreen;
		[SerializeField] private Image _winScreen;

        [SerializeField] private Button _replayButton;


        protected override void OnPostInject()
        {
            _navigator.AppStateChanged += NavigatorOnAppStateChanged;
            _replayButton.onClick.AddListener(ReplayButtonOnPressed);
			Hide();
        }

        private void ReplayButtonOnPressed()
        {
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
					if (_catStats.Points >= _appSettings.PointsToLevel5) {
						_winScreen.gameObject.SetActive(true);
						_gameOverScreen.gameObject.SetActive(false);

						_audioManager.PlayTrackOneShot(AudioClips.BgWin);
					}
					else {
						_winScreen.gameObject.SetActive(false);
						_gameOverScreen.gameObject.SetActive(true);

						_audioManager.PlayTrackOneShot(AudioClips.BgGameOver);
					}

                    _replayButton.onClick.RemoveListener(ReplayButtonOnPressed);

                    this.InvokeAfterTime(1.5f*ToggleTweenSettings.Duration, () =>
                    {
                        _replayButton.onClick.AddListener(ReplayButtonOnPressed);
                        _eventSystem.SetSelectedGameObject(_replayButton.gameObject);
                    });
                    break;
            }
        }
    }
}