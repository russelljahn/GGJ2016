using System;
using Assets.OutOfTheBox.Scripts;
using Assets.OutOfTheBox.Scripts.Extensions;

namespace Assets.OutOfTheBox.Scripts.Navigation
{
    public class Navigator
    {
        public event Action<StateChange<AppStates>> AppStateChanged;
        public event Action<StateChange<bool>> ImmersionClockEnabledChanged;

        private AppStates _appState;
        public AppStates AppState
        {
            get { return _appState; }
            set
            {
                if (value == _appState)
                {
                    return;
                }
                var previousState = _appState;
                _appState = value;

                UnityEngine.Debug.Log("AppState changed: " + new StateChange<AppStates>(previousState, value));
                AppStateChanged.SafelyInvoke(new StateChange<AppStates>(previousState, _appState));
            }
        }
    }
}
