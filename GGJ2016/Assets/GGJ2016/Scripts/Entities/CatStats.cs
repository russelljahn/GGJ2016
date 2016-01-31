using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.OutOfTheBox.Scripts;
using Assets.OutOfTheBox.Scripts.Extensions;
using Sense.Injection;
using Sense.PropertyAttributes;
using UnityEngine;
using Zenject;

namespace Assets.GGJ2016.Scripts.Entities
{
    public class CatStats : InjectableBehaviour
    {
        [Inject] private AppSettings _appSettings;

        [SerializeField, Readonly] private float _points;
        public float Points
        {
            get
            {
                return _points;
            }
            set
            {
                if (_points <= 0.0f && value <= 0.0f)
                {
                    return;
                }
                _points = Mathf.Max(0f, value);
                if (_points.IsApproximatelyZero())
                {
                    Destroyed.SafelyInvoke(this);
                }
            }
        }

        public int Level
        {
            get
            {
                var health = Points;
                if (health.IsApproximatelyZero())
                {
                    return 0;
                }
                var level = 1;
                for (; level < _appSettings.MaxCatLevel; ++level)
                {
                    health -= _appSettings.PointsToEachLevel[level];
                    if (health < 0)
                    {
                        return level;
                    }
                }
                return level;
            }
        }

        public event Action<CatStats> Destroyed;


        protected override void OnPostInject()
        {
            Points = _appSettings.InitialPoints;
        }

        private void Update()
        {
            Points -= _appSettings.PointLossSpeed*Time.deltaTime;
        }
    }
}
