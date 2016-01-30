using System.Runtime.CompilerServices;
using Assets.OutOfTheBox.Scripts.Extensions;
using Assets.OutOfTheBox.Scripts.Inputs;
using Sense.Common.Navigation;
using Sense.Injection;
using Sense.PropertyAttributes;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Assets.GGJ2016.Scripts.Entities
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CatEntity : InjectableBehaviour
    {
        [Inject] private Controller _controller;

        [SerializeField] private SpriteRenderer _spriteRenderer;

        [SerializeField] private float _walkSpeed = 10.0f;
        [SerializeField] private float _runMultiplier = 1.75f;
        [SerializeField] private float _frictionMultiplier = 0.975f;

        [SerializeField] private float _timeUntilMaxSpeed = 1.0f;

        [SerializeField] private float _timeInMotion;

        private float WalkMultiplier = 1.0f;
        [SerializeField, Readonly] private float _speedX;

        private const float IdleSpeedThreshold = 0.1f;

        private Rigidbody2D _rigidbody2D;

        private enum StateType
        {
            Initial,
            Idle,
            Moving,
            Jumping,
            Falling
        }

        [SerializeField, Readonly] private StateType _state = StateType.Initial;
        private StateType State
        {
            get { return _state; }
            set
            {
                if (_state == value)
                {
                    return;
                }
                var previous = _state;
                _state = value;
                OnStateChanged(new StateChange<StateType>(previous, _state));
            }
        }

        private float MaxSpeedX
        {
            get { return _walkSpeed*_runMultiplier; }
        }

        protected override void OnPostInject()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();

            State = StateType.Idle;
        }

        private void Update()
        {
            if (State != StateType.Idle)
            {
                _timeInMotion += Time.deltaTime;
            }

            switch (State)
            {
                case StateType.Initial:
                    InInitial();
                    break;

                case StateType.Idle:
                    InIdle();
                    break;

                case StateType.Moving:
                    InMoving();
                    break;

                case StateType.Jumping:
                    InJumping();
                    break;

                case StateType.Falling:
                    InFalling();
                    break;
            }
           
            //Debug.Log("_controller: " + new Vector2(_controller.MoveX, _controller.MoveY));
        }

        private void OnStateChanged(StateChange<StateType> stateChange)
        {
            switch (State)
            {
                case StateType.Initial:
                    OnInitial();
                    break;

                case StateType.Idle:
                    OnIdle();
                    break;

                case StateType.Moving:
                    OnMoving();
                    break;

                case StateType.Jumping:
                    OnJumping();
                    break;

                case StateType.Falling:
                    OnFalling();
                    break;
            }
        }

        private void OnInitial()
        {
            
        }

        private void OnIdle()
        {
            _timeInMotion = 0f;
        }

        private void OnMoving()
        {
            
        }

        private void OnJumping()
        {
            
        }

        private void OnFalling()
        {
            
        }

        private void InInitial()
        {
            
        }

        private void InIdle()
        {
            var movement = _controller.MoveX;
            if (!movement.IsApproximatelyZero())
            {
                State = StateType.Moving;
            }

        }

        private void InMoving()
        {
            var moveX = _controller.MoveX;
            var baseSpeedMultiplier = _controller.Run ? _runMultiplier : WalkMultiplier;
            var speedMultiplier = Mathf.Lerp(WalkMultiplier, baseSpeedMultiplier, _timeInMotion/_timeUntilMaxSpeed);

            var fasterThanWalking = Mathf.Abs(_speedX) > _walkSpeed;
            if (_controller.Run || !fasterThanWalking)
            {
                _speedX += moveX*speedMultiplier;
            }
            if (moveX.IsApproximatelyZero() || fasterThanWalking)
            {
                _speedX *= _frictionMultiplier;
            }
            else
            {
                _spriteRenderer.flipX = _controller.MoveX > 0f;
            }

            _speedX = Mathf.Clamp(_speedX, -MaxSpeedX, MaxSpeedX);
            var displacementX = new Vector3(_speedX * Time.deltaTime, 0f, 0f);
            _rigidbody2D.MovePosition(transform.position + displacementX);

            if (Mathf.Abs(_speedX) <= IdleSpeedThreshold)
            {
                State = StateType.Idle;
            }
            Debug.Log("_speedX: " + _speedX + ", speedMultiplier: " + speedMultiplier);
        }

        private void InJumping()
        {
            
        }

        private void InFalling()
        {
            
        }
    }
}
