using System;
using Assets.GGJ2016.Scripts;
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

        private bool _immersionClockEnabled;
        public bool ImmersionClockEnabled
        {
            get { return _immersionClockEnabled; }
            set
            {
                if (value == _immersionClockEnabled)
                {
                    return;
                }
                var previousState = _immersionClockEnabled;
                _immersionClockEnabled = value;

                ImmersionClockEnabledChanged.SafelyInvoke(new StateChange<bool>(previousState, _immersionClockEnabled));
            }
        }

        //private PlaybackStates _playbackState;
        //public PlaybackStates PlaybackState
        //{
        //    get { return _playbackState; }
        //    set
        //    {
        //        if (value == _playbackState)
        //        {
        //            return;
        //        }
        //        var previousState = _playbackState;
        //        _playbackState = value;
        //        OnPropertyChanged("PlaybackState");
        //        OnPlaybackStateChanged(previousState, _playbackState);
        //    }
        //}

        //private VrStates _vrState;
        //public VrStates VrState
        //{
        //    get { return _vrState; }
        //    set
        //    {
        //        if (value == _vrState)
        //        {
        //            return;
        //        }
        //        var previousState = _vrState;
        //        _vrState = value;
        //        OnPropertyChanged("VrState");
        //        OnVrStateChanged(previousState, _vrState);
        //    }
        //}

        //private void OnPlaybackStateChanged(PlaybackStates previous, PlaybackStates next)
        //{
        //    var handler = PlaybackStateChanged;
        //    if (handler != null) handler(new StateChange<PlaybackStates>(previous, next));
        //}

        //private void OnVrStateChanged(VrStates previous, VrStates next)
        //{
        //    var handler = VrStateChanged;
        //    if (handler != null) handler(new StateChange<VrStates>(previous, next));
        //}
    }
}
