using System.Collections.Generic;
using Assets.OutOfTheBox.Scripts.Audio;
using Sense.PropertyAttributes;
using UnityEngine;

namespace Assets.OutOfTheBox.Scripts
{
    public class AppSettings : MonoBehaviour
    {
        [Min(0f)] public float AttackCooldown = 0.25f;
        [Min(0f)] public float ScratchDamage = 10.0f;
        [Min(0)] public int MaxCatLevel = 5;
        [Min(0)] public int InitialPoints = 100;
        public List<int> PointsToEachLevel = new List<int>();
        [Min(0f)] public float PointLossSpeed = 1.0f;

        private void Start()
        {
            if (PointsToEachLevel.Count != MaxCatLevel+1)
            {
                Debug.LogError("Size of PointsToEachLevel != MaxCatLevel+1!");
            }

            if (PointsToEachLevel.Count == 0)
            {
                Debug.LogError("Size of PointsToEachLevel is zero!");
            }
        }
    }
}
