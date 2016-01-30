using System;
using Assets.OutOfTheBox.Scripts.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.OutOfTheBox.Scripts.Events
{
    public class OnSelectDispatcher : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public event Action<GameObject> Selected;
        public event Action<GameObject> Deselected;

        public void OnSelect(BaseEventData eventData)
        {
            Selected.SafelyInvoke(gameObject);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            Deselected.SafelyInvoke(gameObject);
        }
    }
}