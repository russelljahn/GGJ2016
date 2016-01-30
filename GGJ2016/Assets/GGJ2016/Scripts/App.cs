using Assets.OutOfTheBox.Scripts.Navigation;
using InControl;
using Sense.Common.Navigation;
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
            _navigator.AppState = AppStates.Splash;
            _navigator.ImmersionClockEnabled = false;

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
