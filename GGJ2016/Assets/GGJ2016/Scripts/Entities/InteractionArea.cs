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

namespace Assets.GGJ2016.Scripts.Entities
{
    public class InteractionArea : InjectableBehaviour
    {
        [Inject] private Controller _controller;
        [Inject] private AppSettings _appSettings;

        [SerializeField] private float _attackCooldown = 0.25f;

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
            _currentAttackCooldown = Mathf.Max(0f, _currentAttackCooldown - Time.deltaTime);
            if (_controller.IsAttacking && _currentAttackCooldown.IsApproximatelyZero())
            {
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
