using Sense.Common.Navigation;
using Sense.HoloSet.Navigation;
using Sense.Injection;
using UnityEngine;
using UnityEngine.VR;
using Zenject;

namespace Sense.Common
{
    public class AppCamera : InjectableBehaviour
    {
        //[Inject] private AppCameraViewModel _viewModel;
        [Inject] private Transform _parent;
       // [SerializeField] private Cardboard _cardboard;

        
        [SerializeField] private Camera _centerCamera;
        public Camera CenterCamera { get { return _centerCamera; } }

        [SerializeField] private Camera _leftEyeCamera;
        public Camera LeftEyeCamera { get { return _leftEyeCamera; } }

        [SerializeField] private Camera _rightEyeCamera;
        public Camera RightEyeCamera { get { return _rightEyeCamera; } }


        protected override void OnPostInject()
        {
            base.OnPostInject();

            DisableCardboard();
            DisableUnityNative();
            transform.SetParent(_parent, false);

            //_viewModel.VrStateChanged += VrStateChanged;
        }


        private void VrStateChanged(StateChange<VrStates> stateChange)
        {
            switch (stateChange.Previous)
            {
                case VrStates.Disabled:
                    break;

                case VrStates.Cardboard:
                    DisableCardboard();
                    break;

                case VrStates.UnityNative:
                    DisableUnityNative();
                    break;
            }

            switch (stateChange.Next)
            {
                case VrStates.Disabled:
                    DisableCardboard();
                    DisableUnityNative();
                    break;

                case VrStates.Cardboard:
                    EnableCardboard();
                    break;

                case VrStates.UnityNative:
                    EnableCardboard();
                    break;
            }
        }

        private void EnableCardboard()
        {
            //_cardboard.VRModeEnabled = true;
        }

        private void DisableCardboard()
        {
           // _cardboard.VRModeEnabled = false;
        }

        private void EnableUnityNative()
        {
            VRSettings.enabled = true;
        }

        private void DisableUnityNative()
        {
            VRSettings.enabled = false;
        }


    }
}
