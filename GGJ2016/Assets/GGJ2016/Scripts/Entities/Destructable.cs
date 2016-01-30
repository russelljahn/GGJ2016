using System;
using Assets.OutOfTheBox.Scripts.Extensions;
using Sense.Extensions;
using Sense.Injection;
using Sense.PropertyAttributes;
using UnityEngine;

namespace Assets.GGJ2016.Scripts.Entities
{
    public class Destructable : InjectableBehaviour, IInteractable
    {
        [SerializeField] private float _maxHealth = 100.0f;
        [SerializeField, Readonly] private float _health; 
        [SerializeField] private float _points = 1.0f;

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

        protected override void OnPostInject()
        {
            _health = _maxHealth;
            Destroyed += OnDestroyed;
        }

        private void OnDestroyed(Destructable destructable)
        {
            this.InvokeAtEndOfFrame(() =>
            {
                gameObject.SetActive(false);
            });
        }
    }
}
