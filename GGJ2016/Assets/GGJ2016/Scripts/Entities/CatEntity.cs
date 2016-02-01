using System.Runtime.CompilerServices;
using Assets.OutOfTheBox.Scripts.Extensions;
using Assets.OutOfTheBox.Scripts.Inputs;
using Assets.OutOfTheBox.Scripts.Utils;
using Sense.Injection;
using Sense.PropertyAttributes;
using UnityEngine;
using Zenject;
using AssemblyCSharp;
using Assets.OutOfTheBox.Scripts.Navigation;
using Assets.OutOfTheBox.Scripts.Audio;
using Assets.OutOfTheBox.Scripts;
using Sense.Extensions;

namespace Assets.GGJ2016.Scripts.Entities
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CatEntity : InjectableBehaviour
    {
		[Inject] private AudioManager _audioManager;
        [Inject] private Controller _controller;
		[Inject] private CatStats _stats;
		[Inject] private Navigator _navigator;

        [SerializeField] private float _runMultiplier = 1.75f;
        [SerializeField] private float _jumpMultiplierX = 1.25f;
        [SerializeField] private float _jumpMultiplierY = 2.0f;

        [SerializeField] private float _timeUntilMaxJump = 1.0f;
        [SerializeField]  private float _gravity = -0.0075f;

        [SerializeField, Readonly] private float _timeInMotion;
        [SerializeField, Readonly] private float _timeInJump;
        [SerializeField, Readonly] private float _speedX;


        private const float WalkMultiplier = 1.0f;
        private const float IdleSpeedThreshold = 0.1f;

        [SerializeField] private LayerMask _mask = 12;
        [SerializeField, Readonly] private bool _isGrounded;

        private Rigidbody2D _rigidbody2D;
		private SpriteRenderer _currentSpriteRenderer;
		private Animator _currentAnimator;
		private string _currentBgClipName;


		[SerializeField] private SpriteRenderer _level0SpriteRenderer;
		[SerializeField] private Animator _level0Animator;
		[SerializeField] private SpriteRenderer _level1SpriteRenderer;
		[SerializeField] private Animator _level1Animator;
		[SerializeField] private SpriteRenderer _level2SpriteRenderer;
		[SerializeField] private Animator _level2Animator;
		[SerializeField] private SpriteRenderer _level3SpriteRenderer;
		[SerializeField] private Animator _level3Animator;
		[SerializeField] private SpriteRenderer _level4SpriteRenderer;
		[SerializeField] private Animator _level4Animator;
		[SerializeField] private SpriteRenderer _level5SpriteRenderer;
		[SerializeField] private Animator _level5Animator;

		public GameObject Poof;

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
			_stats.LevelChanged += StatsOnLevelChanged;
			_navigator.AppStateChanged += NavigatorOnAppStateChanged;

			StatsOnLevelChanged(new StateChange<int>(_stats.Level, _stats.Level));
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

		private void NavigatorOnAppStateChanged(StateChange<AppStates> stateChange)
		{
			switch (stateChange.Previous)
			{
				case AppStates.Gameplay:
					_audioManager.Fade(_currentBgClipName, 0.0f);
					break;
			}

			switch (stateChange.Next)
			{
				case AppStates.Gameplay:
					_rigidbody2D.isKinematic = false;

					_currentBgClipName = AudioClips.BgLevel0;
					_audioManager.LoadClip(_currentBgClipName, 1.0f, 1.0f, true);
					_audioManager.PlayTrack(_currentBgClipName);
					break;

				default:
					_rigidbody2D.isKinematic = true;
					break;
			}
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

			if (_controller.MoveX.IsApproximatelyZero()) {
				_currentAnimator.speed = 0;
				_currentAnimator.SetTime(0f);
			}
			else {
				_currentAnimator.speed = 1.0f;
			}


            if (_controller.MoveX > 0)
				transform.rotation = Quaternion.Euler(0,-180,0);
            if (_controller.MoveX < 0)
				transform.rotation = Quaternion.Euler(0,0,0);

            //addGravity
            _rigidbody2D.AddForce(Vector2.up * _gravity);

            //jump
            bool canJump = _controller.IsJumping;
            if (canJump && _isGrounded)
            {
                _rigidbody2D.AddForce(Vector2.up * _jumpMultiplierY);
				var jumpSoundName = AudioClips.SfxJump1;
				float jumpVolume = 0.5f;
				switch(_stats.Level) {
					case 0:
					case 1:
						jumpSoundName = AudioClips.SfxJump1;
						break;
					case 2:
						jumpSoundName = AudioClips.SfxJump2;
						break;

					case 3:
						jumpSoundName = AudioClips.SfxJump3;
						break;

					case 4:
					case 5:
						jumpSoundName = AudioClips.SfxJump4;
						break;
				}
				_audioManager.PlayTrackOneShot(jumpSoundName, jumpVolume);
            }
            //air movement
            if (!_isGrounded)
			{				

                _rigidbody2D.drag = 1f;
                _rigidbody2D.AddForce(Vector2.right * moveX * _jumpMultiplierX);
                _timeInJump += Time.deltaTime;
            	//get stuck fix
                if (_rigidbody2D.velocity.magnitude < 2)
                    _rigidbody2D.AddForce(Vector2.right * moveX * _runMultiplier);
            }

        }

		private void StatsOnLevelChanged(StateChange<int> stateChange) {

			var clipToFadeOut = "";

			switch (stateChange.Previous) {
				case 0:
					clipToFadeOut = AudioClips.BgLevel0;
					break;

				case 1:
					clipToFadeOut = AudioClips.BgLevel1;
					break;

				case 2:
					clipToFadeOut = AudioClips.BgLevel2;
					break;

				case 3:
					clipToFadeOut = AudioClips.BgLevel3;
					break;

				case 4:
					clipToFadeOut = AudioClips.BgLevel4;
					break;

//				case 5:
//					clipToFadeOut = AudioClips.BgLevel4;
//					
//					break;

				default:
					break;
			}

			switch (stateChange.Next) {
				case 0:
					_currentAnimator = _level0Animator;
					_currentSpriteRenderer = _level0SpriteRenderer;
					_currentBgClipName = AudioClips.BgLevel0;
					break;

				case 1:
					_currentAnimator = _level1Animator;
					_currentSpriteRenderer = _level1SpriteRenderer;
					_currentBgClipName = AudioClips.BgLevel1;

					break;

				case 2:
					_currentAnimator = _level2Animator;
					_currentSpriteRenderer = _level2SpriteRenderer;
					_currentBgClipName = AudioClips.BgLevel2;

					break;

				case 3:
					_currentAnimator = _level3Animator;
					_currentSpriteRenderer = _level3SpriteRenderer;
					_currentBgClipName = AudioClips.BgLevel3;
					break;

				case 4:
					_currentAnimator = _level4Animator;
					_currentSpriteRenderer = _level4SpriteRenderer;
					_currentBgClipName = AudioClips.BgLevel4;
					break;

				case 5:
					_currentAnimator = _level5Animator;
					_currentSpriteRenderer = _level5SpriteRenderer;
					break;

				default:
					break;
			}

			var levelUpVolume = 0.5f;
			if (stateChange.Previous < stateChange.Next) {
				_audioManager.PlayTrackOneShot(AudioClips.SfxLevelUp, levelUpVolume);
			}

			if (stateChange.Previous > stateChange.Next) {
				_audioManager.PlayTrackOneShot(AudioClips.SfxLevelDown, levelUpVolume);
			}

			_level0SpriteRenderer.gameObject.SetActive(false);
			_level1SpriteRenderer.gameObject.SetActive(false);
			_level2SpriteRenderer.gameObject.SetActive(false);
			_level3SpriteRenderer.gameObject.SetActive(false);
			_level4SpriteRenderer.gameObject.SetActive(false);
			_level5SpriteRenderer.gameObject.SetActive(false);
			_currentSpriteRenderer.gameObject.SetActive(true);

			if (_navigator.AppState == AppStates.Gameplay) {
				_audioManager.LoadClip(_currentBgClipName, 0.0f, 1.0f, true);
				_audioManager.PlayTrack(_currentBgClipName);

				if (clipToFadeOut != _currentBgClipName) {
					_audioManager.Crossfade(clipToFadeOut, _currentBgClipName, 0f, 1f);
				}
			}

			var go = (GameObject)Instantiate(Poof,transform.position,Quaternion.identity);
			var destroyScript = go.GetComponent<DestroyAfterTime>();

			if (destroyScript.IsNotNull()) {
				destroyScript.ShouldDestroy = true;
			}
		}
    }
}