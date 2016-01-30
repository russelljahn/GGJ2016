using System.Runtime.CompilerServices;
using Assets.OutOfTheBox.Scripts.Extensions;
using Assets.OutOfTheBox.Scripts.Inputs;
using Assets.OutOfTheBox.Scripts.Utils;
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
        [SerializeField] private float _jumpMultiplierX = 1.25f;
        [SerializeField] private float _jumpMultiplierY = 2.0f;

        [SerializeField] private float _frictionMultiplier = 0.975f;

        [SerializeField] private float _timeUntilMaxSpeed = 1.0f;
        [SerializeField] private float _timeUntilMaxJump = 1.0f;
        [SerializeField] private float _gravity = -0.0075f;

        [SerializeField, Readonly] private float _timeInMotion;
        [SerializeField, Readonly] private float _timeInJump;
        [SerializeField, Readonly] private float _speedX;

        [SerializeField] private AnimationCurve _jumpSpeedCurve = AnimationCurveUtils.GetLinearCurve();

        private const float WalkMultiplier = 1.0f;
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

        private float MaxSpeedY
        {
            get { return _walkSpeed * _jumpMultiplierY; }
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
            switch (stateChange.Previous)
            {
                case StateType.Initial:
                    break;

                case StateType.Idle:
                    break;

                case StateType.Moving:
                    break;

                case StateType.Jumping:
                    _timeInJump = 0.0f;
                    break;

                case StateType.Falling:
                    break;
            }

            switch (stateChange.Next)
            {
                case StateType.Initial:
                    break;

                case StateType.Idle:
                    _timeInMotion = 0f;
                    break;

                case StateType.Moving:
                    break;

                case StateType.Jumping:
                    break;

                case StateType.Falling:
                    break;
            }
        }

        private void InIdle()
        {
            HandleDisplacement();

            var movement = _controller.MoveX;
            if (!movement.IsApproximatelyZero())
            {
                State = StateType.Moving;
            }
            if (_controller.IsJumping)
            {
                State = StateType.Jumping;
            }
        }

        private void InMoving()
        {
            HandleDisplacement();

            if (Mathf.Abs(_speedX) <= IdleSpeedThreshold)
            {
                State = StateType.Idle;
            }
            if (_controller.IsJumping)
            {
                State = StateType.Jumping;
            }
        }

        private void InJumping()
        {
            _timeInJump += Time.deltaTime;

            HandleDisplacement();

            if (!_controller.IsJumping)
            {
                State = StateType.Moving;
            }
        }

        private void InFalling()
        {
            
        }

        private void HandleDisplacement()
        {
            var displacement = Vector3.zero;

            // Horizontal
            var moveX = _controller.MoveX;
            var baseSpeedMultiplier = _controller.IsRunning ? _runMultiplier : WalkMultiplier;
            //BUG: _timeInMotion is still > _timeUntilMaxSpeed if run, let go, then run again
            var speedMultiplier = Mathf.Lerp(WalkMultiplier, baseSpeedMultiplier, _timeInMotion / _timeUntilMaxSpeed);

            if (_controller.IsJumping)
            {
                speedMultiplier *= _jumpMultiplierX;
            }

            var speedIsFasterThanWalking = Mathf.Abs(_speedX) > _walkSpeed;
            if (_controller.IsRunning || !speedIsFasterThanWalking)
            {
                _speedX += moveX * speedMultiplier;
            }
            if (moveX.IsApproximatelyZero() || speedIsFasterThanWalking)
            {
                _speedX *= _frictionMultiplier;
            }
            else
            {
                _spriteRenderer.flipX = _controller.MoveX > 0f;
            }

            _speedX = Mathf.Clamp(_speedX, -MaxSpeedX, MaxSpeedX);
            displacement.x = _speedX * Time.deltaTime;

            Debug.Log("_speedX: " + _speedX + ", speedMultiplier: " + speedMultiplier);

            // Vertical
            var moveY = _controller.IsJumping ? 1.0f : 0.0f;
            if (_timeInJump < _timeUntilMaxJump)
            {
                var speedY = moveY*_jumpMultiplierY*_jumpSpeedCurve.Evaluate(Mathf.Clamp01(_timeInJump/_timeUntilMaxJump));
                speedY = Mathf.Clamp(speedY, -MaxSpeedY, MaxSpeedY);
                displacement.y += speedY * Time.deltaTime;
            }

            displacement.y += _gravity;

            _rigidbody2D.MovePosition(transform.position + displacement);
        }
    }
}
