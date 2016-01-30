using System;
using Assets.OutOfTheBox.Scripts.Extensions;
using UnityEngine;
using Zenject;

namespace Assets.OutOfTheBox.Scripts.Timers
{
    public class Ticker : ITickable
    {
        public event Action<float> Ticked;

        public void Tick()
        {
            Ticked.SafelyInvoke(Time.deltaTime);
        }
    }
}
