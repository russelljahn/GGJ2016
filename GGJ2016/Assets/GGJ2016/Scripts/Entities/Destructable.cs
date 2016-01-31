using System;
using System.Linq;
using Assets.OutOfTheBox.Scripts.Extensions;
using Assets.OutOfTheBox.Scripts.Utils;
using FlexiTweening;
using FlexiTweening.Extensions;
using Sense.Extensions;
using Sense.Injection;
using Sense.PropertyAttributes;
using SVGImporter;
using UnityEngine;

namespace Assets.GGJ2016.Scripts.Entities
{
    public class Destructable : InjectableBehaviour, IInteractable
    {
        [SerializeField] private float _maxHealth = 100.0f;
        [SerializeField, Readonly] private float _health; 
        [SerializeField] private float _points = 10.0f;
		[SerializeField] private bool _freezeRigidbodyUntilCollision = false;
        [SerializeField] private bool _breakByForce = true;
        [SerializeField] private float _impactVelocityToBreak = 4.5f;

        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private SVGRenderer _svgRenderer;
		[SerializeField] private SpriteRenderer _spriteRenderer;

        [SerializeField] private TweenSettings _fadeSettings;
        private ITween _fadeTween;

        public float MaxHealth
        {
            get { return _maxHealth; }
        }

        public float Health
        {
            get
            {
                return _health;
            }
            set
            {
                if (_health <= 0.0f && value <= 0.0f)
                {
                    return;
                }
                if (_health >= MaxHealth && value >= MaxHealth)
                {
                    return;
                }
                _health = Mathf.Clamp(value, 0f, MaxHealth);
                if (_health.IsApproximatelyZero())
                {
                    Destroyed.SafelyInvoke(this);
                }
            }
        }

        public event Action<Destructable> Destroyed;

        private void Reset()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _svgRenderer = GetComponent<SVGRenderer>();
			_spriteRenderer = GetComponent<SpriteRenderer>();

        }

        protected override void OnPostInject()
        {
            _health = _maxHealth;
            Destroyed += OnDestroyed;

            if (_breakByForce && _rigidbody2D.IsNull())
            {
                Debug.Log(gameObject.name + " needs a Rigidbody2D to break by force!");
            }

			if (_rigidbody2D.IsNotNull() && _freezeRigidbodyUntilCollision) {
				_rigidbody2D.isKinematic = true;
			}
        }

        private void OnDestroyed(Destructable destructable)
        {
			if (_svgRenderer.IsNotNull()) {
	            _fadeTween.SafelyAbort();
	            _fadeTween = _svgRenderer.TweenColor()
	                .To(ColorUtils.Colors.Translucent, _fadeSettings)
	                .OnComplete(
	                    () => {
	                        Destroy(gameObject);
	                    }
	                )
	                .Start();
			}
			else if (_spriteRenderer.IsNotNull()) {
				_fadeTween.SafelyAbort();
				_fadeTween = _spriteRenderer.TweenColor()
					.To(ColorUtils.Colors.Translucent, _fadeSettings)
					.OnComplete(
						() => {
							Destroy(gameObject);
						}
					)
					.Start();
			}
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
			_rigidbody2D.isKinematic = false;
            var impactVelocity = _rigidbody2D.GetPointVelocity(collision.contacts.First().point);
            Debug.Log(impactVelocity);
            if (_breakByForce && impactVelocity.magnitude >= _impactVelocityToBreak)
            {
                Destroyed.SafelyInvoke(this);
            }

        }
    }
}
