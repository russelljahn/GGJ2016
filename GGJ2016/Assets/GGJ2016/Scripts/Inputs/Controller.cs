using System;
using Assets.OutOfTheBox.Scripts.Extensions;
using InControl;
using Sense.Injection;
using UnityEngine;
using Zenject;

namespace Assets.OutOfTheBox.Scripts.Inputs
{
    public class Controller : InjectableBehaviour
    {
        private class GearVrActionSet : PlayerActionSet
        {
            public readonly PlayerAction Submit;
            public readonly PlayerAction Cancel;
            public readonly PlayerAction Run;
            public readonly PlayerAction Jump;
            public readonly PlayerAction Attack;

            public readonly PlayerAction MoveLeft;
            public readonly PlayerAction MoveRight;
            public readonly PlayerAction MoveUp;
            public readonly PlayerAction MoveDown;
            public readonly PlayerTwoAxisAction Move;
            public readonly PlayerAction CameraLeft;
            public readonly PlayerAction CameraRight;
            public readonly PlayerAction CameraUp;
            public readonly PlayerAction CameraDown;
            public readonly PlayerTwoAxisAction Camera;

            public GearVrActionSet()
            {
                Submit = CreatePlayerAction("IsSubmitting");
                Cancel = CreatePlayerAction("IsCancelling");

                Run = CreatePlayerAction("IsRunning");
                Jump = CreatePlayerAction("IsJumping");
                Attack = CreatePlayerAction("IsAttacking");

                MoveLeft = CreatePlayerAction("MoveLeft");
                MoveRight = CreatePlayerAction("MoveRight");
                MoveUp = CreatePlayerAction("MoveUp");
                MoveDown = CreatePlayerAction("MoveDown");
                Move = CreateTwoAxisPlayerAction(MoveLeft, MoveRight, MoveDown, MoveUp);

                CameraLeft = CreatePlayerAction("CameraLeft");
                CameraRight = CreatePlayerAction("CameraRight");
                CameraUp = CreatePlayerAction("CameraUp");
                CameraDown = CreatePlayerAction("CameraDown");
                Camera = CreateTwoAxisPlayerAction(CameraLeft, CameraRight, CameraDown, CameraUp);
            }
        }


        [Inject] private InControlInputModule _inControlInputModule;
        private GearVrActionSet _actionSet;

        public bool IsSubmitting
        {
            get { return _actionSet.Submit; }
        }

        public bool IsCancelling
        {
            get { return _actionSet.Cancel; }
        }

        public bool IsRunning
        {
            get { return _actionSet.Run; }
        }

        public bool IsAttacking
        {
            get { return _actionSet.Attack; }
        }

        public bool IsJumping
        {
            get { return _actionSet.Jump; }
        }

        public float MoveX
        {
            get { return _actionSet.Move.X; }
        }

        public float MoveY
        {
            get { return _actionSet.Move.Y; }
        }

        public float CameraX
        {
            get { return _actionSet.Camera.X; }
        }

        public float CameraY
        {
            get { return _actionSet.Camera.Y; }
        }

        public Action ReleasedSubmit;

        private void OnEnable()
        {
            BindActions();

            _inControlInputModule.SubmitAction = _actionSet.Submit;
            _inControlInputModule.CancelAction = _actionSet.Cancel;
            _inControlInputModule.MoveAction = _actionSet.Move;
        }

        protected override void OnDestroyed()
        {
            DestroyActions();
        }

        private void BindActions()
        {
            _actionSet = new GearVrActionSet();

            _actionSet.Attack.AddDefaultBinding(InputControlType.RightTrigger);
            _actionSet.Attack.AddDefaultBinding(Key.K);

            _actionSet.Jump.AddDefaultBinding(InputControlType.Action1);
            _actionSet.Jump.AddDefaultBinding(Key.J);
            _actionSet.Jump.AddDefaultBinding(Key.Space);
            
            _actionSet.Run.AddDefaultBinding(InputControlType.Action3);
            _actionSet.Run.AddDefaultBinding(Key.Shift);

            _actionSet.Submit.AddDefaultBinding(InputControlType.Action1);
            _actionSet.Submit.AddDefaultBinding(InputControlType.Start);
            _actionSet.Submit.AddDefaultBinding(Mouse.LeftButton);

            _actionSet.Cancel.AddDefaultBinding(InputControlType.Action2);
            _actionSet.Cancel.AddDefaultBinding(Key.Escape);
            _actionSet.Cancel.AddDefaultBinding(Mouse.RightButton);

            _actionSet.MoveLeft.AddDefaultBinding(InputControlType.LeftStickLeft);
            _actionSet.MoveLeft.AddDefaultBinding(InputControlType.DPadLeft);
            _actionSet.MoveLeft.AddDefaultBinding(Key.LeftArrow);
            _actionSet.MoveLeft.AddDefaultBinding(Key.A);

            _actionSet.MoveRight.AddDefaultBinding(InputControlType.LeftStickRight);
            _actionSet.MoveRight.AddDefaultBinding(InputControlType.DPadRight);
            _actionSet.MoveRight.AddDefaultBinding(Key.RightArrow);
            _actionSet.MoveRight.AddDefaultBinding(Key.D);

            _actionSet.MoveUp.AddDefaultBinding(InputControlType.LeftStickUp);
            _actionSet.MoveUp.AddDefaultBinding(InputControlType.DPadUp);
            _actionSet.MoveUp.AddDefaultBinding(Key.UpArrow);
            _actionSet.MoveUp.AddDefaultBinding(Key.W);

            _actionSet.MoveDown.AddDefaultBinding(InputControlType.LeftStickDown);
            _actionSet.MoveDown.AddDefaultBinding(InputControlType.DPadDown);
            _actionSet.MoveDown.AddDefaultBinding(Key.DownArrow);
            _actionSet.MoveDown.AddDefaultBinding(Key.S);

            _actionSet.CameraLeft.AddDefaultBinding(InputControlType.RightStickLeft);
            _actionSet.CameraRight.AddDefaultBinding(InputControlType.RightStickRight);

            _actionSet.CameraUp.AddDefaultBinding(InputControlType.RightStickUp);
            _actionSet.CameraUp.AddDefaultBinding(Mouse.PositiveScrollWheel);

            _actionSet.CameraDown.AddDefaultBinding(InputControlType.RightStickDown);
            _actionSet.CameraDown.AddDefaultBinding(Mouse.NegativeScrollWheel);
        }

        private void DestroyActions()
        {
            _actionSet.Destroy();
        }

        private void Update()
        {
            if (_actionSet.Submit.WasReleased)
            {
                ReleasedSubmit.SafelyInvoke();
            }
        }
    }
}