using System;
using UnityEngine;
using Sense.PropertyAttributes;
using Sense.Injection;
using Assets.GGJ2016.Scripts.Entities;
using System.Collections.Generic;
using Sense.Extensions;

namespace AssemblyCSharp
{
	[RequireComponent(typeof(Collider2D))]
	public class CollisionDetector : MonoBehaviour
	{
		[SerializeField] private LayerMask _layersToIgnore;

		private readonly List<Destructable> _destructablesInRange = new List<Destructable>();
		[SerializeField, Readonly] private int _numContactedObjects;

		public bool InContact 
		{
			get { return _numContactedObjects > 0; }
		}

		private void OnTriggerEnter2D(Collider2D collider)
		{
			// Should I ignore this layer?
			if (collider.gameObject.layer == _layersToIgnore.value) 
			{
				return;
			}

			var destructable = collider.GetComponent<Destructable>();
			if (destructable.IsNotNull())
			{
				_destructablesInRange.Add(destructable);
				destructable.Destroyed += DestructableOnDestroyed;
			}
			++_numContactedObjects;
		}

		private void OnTriggerExit2D(Collider2D collider)
		{
			// Should I ignore this layer?
			if (collider.gameObject.layer == _layersToIgnore.value) 
			{
				return;
			}

			var destructable = collider.GetComponent<Destructable>();
			if (destructable.IsNotNull())
			{
				_destructablesInRange.Remove(destructable);
				destructable.Destroyed -= DestructableOnDestroyed;
			}
			--_numContactedObjects;
		}

		private void DestructableOnDestroyed(Destructable destructable)
		{
			_destructablesInRange.Remove(destructable);
			destructable.Destroyed -= DestructableOnDestroyed;
			--_numContactedObjects;
		}
	}
}

