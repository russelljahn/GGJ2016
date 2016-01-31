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
        [Min(0)] public int InitialPoints = 50;
		[Min(0)] public int PointsToLevel1 = 100;
		[Min(0)] public int PointsToLevel2 = 200;
		[Min(0)] public int PointsToLevel3 = 300;
		[Min(0)] public int PointsToLevel4 = 400;
		[Min(0)] public int PointsToLevel5 = 500;
        
        [Min(0f)] public float PointLossSpeed = 1.0f;
    }
}
