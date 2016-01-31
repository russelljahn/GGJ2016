using Assets.GGJ2016.Scripts;
using Assets.OutOfTheBox.Scripts.Navigation;
using InControl;
using Sense.Injection;
using UnityEngine;
using Zenject;

namespace Assets.OutOfTheBox.Scripts
{
    public class App : InjectableBehaviour
    {
        [Inject] private readonly Navigator _navigator;

        protected override void OnPostInject()
        {
            _navigator.AppState = AppStates.MainMenu;

            _navigator.AppStateChanged += NavigatorOnAppStateChanged;
        }

        private static void NavigatorOnAppStateChanged(StateChange<AppStates> stateChange)
        {
            if (stateChange.Next == AppStates.Quit)
            {
                Quit();
            }
        }

        private static void Quit()
        {
            Application.Quit();
        }
    }
}
