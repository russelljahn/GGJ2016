using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.OutOfTheBox.Scripts;
using Assets.OutOfTheBox.Scripts.Extensions;
using Assets.OutOfTheBox.Scripts.Inputs;
using Sense.Extensions;
using Sense.Injection;
using UnityEngine;
using Zenject;
using Assets.OutOfTheBox.Scripts.Audio;
using Assets.OutOfTheBox.Scripts.Navigation;

namespace Assets.OutOfTheBox.Scripts.Entities
{
	public class InteractionArea : InjectableBehaviour
    {
        [Inject] private Controller _controller;
        [Inject] private AppSettings _appSettings;
		[Inject] private AudioManager _audioManager;
		[Inject] private Navigator _navigator;
		[Inject] private CatStats _stats;

        [SerializeField] private float _attackCooldown = 0.25f;

		public ParticleSystem CatScratch;

        private readonly List<Destructable> _destructablesInRange = new List<Destructable>();
        private float _currentAttackCooldown;


        private void OnTriggerEnter2D(Collider2D collider)
        {
            var destructables = collider.GetComponents<Destructable>();
            foreach (var destructable in destructables)
            {
                _destructablesInRange.Add(destructable);
                destructable.Destroyed += DestructableOnDestroyed;
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            var destructables = collider.GetComponents<Destructable>();
            foreach (var destructable in destructables)
            {
                _destructablesInRange.Remove(destructable);
                destructable.Destroyed -= DestructableOnDestroyed;
            }
        }

        private void DestructableOnDestroyed(Destructable destructable)
        {
            _destructablesInRange.Remove(destructable);
            destructable.Destroyed -= DestructableOnDestroyed;
        }

        private void Update()
        {
            if (_navigator.AppState != AppStates.Gameplay)
            {
                return;
            }
            _currentAttackCooldown = Mathf.Max(0f, _currentAttackCooldown - Time.deltaTime);
            if (_controller.IsAttacking && _currentAttackCooldown.IsApproximatelyZero())
            {
				if (_destructablesInRange.Count > 0) {
					var attackSound = AudioClips.SfxAttack1;
					switch (_stats.Level) {
						case 0:
						case 1:
						attackSound = AudioClips.SfxAttack1;
							break;

						case 2:
						attackSound = AudioClips.SfxAttack2;
							break;

						case 3:
						attackSound = AudioClips.SfxAttack3;
							break;

						case 4:
						case 5:
						attackSound = AudioClips.SfxAttack4;
							break;

						default:
							break;
					}
					_audioManager.PlayTrackOneShot(attackSound, 0.5f);
				}
                var newPos = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1f);
                var particles = (ParticleSystem)Instantiate(CatScratch, newPos, Quaternion.identity);
                particles.gameObject.SetActive(true);
                var destroyScript = particles.GetComponent<DestroyAfterTime>();

                if (destroyScript.IsNotNull())
                {
                    destroyScript.ShouldDestroy = true;
                }

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < _destructablesInRange.Count; ++i)
                {
                    var destructable = _destructablesInRange[i];
                    destructable.Health -= _appSettings.ScratchDamage;
                }
                _currentAttackCooldown = _attackCooldown;
            }
        }
    }
}
