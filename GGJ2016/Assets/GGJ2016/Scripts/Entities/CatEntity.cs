using System.Runtime.CompilerServices;
using Assets.OutOfTheBox.Scripts.Extensions;
using Assets.OutOfTheBox.Scripts.Inputs;
using Assets.OutOfTheBox.Scripts.Utils;
using Sense.Injection;
using Sense.PropertyAttributes;
using UnityEngine;
using Zenject;
using AssemblyCSharp;

namespace Assets.GGJ2016.Scripts.Entities
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CatEntity : InjectableBehaviour
    {
        [Inject]
        private Controller _controller;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;
		[SerializeField]
		private CollisionDetector _groundDetector;

        [SerializeField]
        private float _runMultiplier = 1.75f;
        [SerializeField]
        private float _jumpMultiplierX = 1.25f;
        [SerializeField]
        private float _jumpMultiplierY = 2.0f;

        [SerializeField]
        private float _timeUntilMaxJump = 1.0f;
        [SerializeField]
        private float _gravity = -0.0075f;

        [SerializeField, Readonly]
        private float _timeInMotion;
        [SerializeField, Readonly]
        private float _timeInJump;
        [SerializeField, Readonly]
        private float _speedX;


        private const float WalkMultiplier = 1.0f;
        private const float IdleSpeedThreshold = 0.1f;

        [SerializeField] private LayerMask _mask = 12;
        [SerializeField, Readonly] private bool _isGrounded;

        private Rigidbody2D _rigidbody2D;

        private enum StateType
        {
            Initial,
            Idle,
            Moving,
            Jumping,
            Falling
        }

        [SerializeField, Readonly]
        private StateType _state = StateType.Initial;
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
            if (!_controller.IsJumping)
            {
                State = StateType.Moving;
            }
        }

        void FixedUpdate()
        {
			//_isGrounded = Physics2D.Raycast(transform.position, -Vector2.up, .15f, _mask.value) || _groundDetector.InContact;
			_isGrounded = Physics2D.Raycast(transform.position, -Vector2.up, .15f, _mask.value);
            
			var moveX = _controller.MoveX;

            // Horizontal
            if (_isGrounded)
            {
                _timeInJump = 0;
                _rigidbody2D.AddForce(Vector2.right * moveX * _runMultiplier, ForceMode2D.Impulse);
                if (_rigidbody2D.velocity.magnitude > 3)
                    _rigidbody2D.drag = 4;
                else
                    _rigidbody2D.drag = 1;

            }
            if (_controller.MoveX > 0)
                _spriteRenderer.flipX = true;
            if (_controller.MoveX < 0)
                _spriteRenderer.flipX = false;

            //addGravity
            _rigidbody2D.AddForce(Vector2.up * _gravity);

            //jump
            bool canJump = _controller.IsJumping;
            if (canJump && _isGrounded)
            {
                _rigidbody2D.AddForce(Vector2.up * _jumpMultiplierY);
            }
            //air movement
            if (!_isGrounded)
			{				

                _rigidbody2D.drag = 1f;
                _rigidbody2D.AddForce(Vector2.right * moveX * _jumpMultiplierX);
                _timeInJump += Time.deltaTime;
                //continue jumping in air
                if (_timeInJump < _timeUntilMaxJump && canJump)
                    _rigidbody2D.AddForce(Vector2.up * _jumpMultiplierY * .2f);
                //get stuck fix
                if (_rigidbody2D.velocity.magnitude < 2)
                    _rigidbody2D.AddForce(Vector2.right * moveX * _runMultiplier);
            }

        }
    }
}