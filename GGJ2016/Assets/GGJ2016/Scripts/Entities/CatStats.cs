using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.OutOfTheBox.Scripts;
using Assets.OutOfTheBox.Scripts.Extensions;
using Assets.OutOfTheBox.Scripts.Navigation;
using Sense.Injection;
using Sense.PropertyAttributes;
using UnityEngine;
using Zenject;

namespace Assets.GGJ2016.Scripts.Entities
{
    public class CatStats : InjectableBehaviour
    {
        [Inject] private AppSettings _appSettings;
        [Inject] private Navigator _navigator;

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
				UpdateLevel();
                if (_points.IsApproximatelyZero())
                {
                    Destroyed.SafelyInvoke(this);
                }
            }
        }

		[SerializeField] private int _level;
        public int Level
        {
            get
            {
				return _level;
            }
			private set 
			{
				_level = value;
			}
        }

        public event Action<CatStats> Destroyed;
		public event Action<StateChange<int>> LevelChanged;

        protected override void OnPostInject()
        {
            Destroyed += OnDestroyed;
            _navigator.AppStateChanged += NavigatorOnAppStateChanged;
        }

        private void NavigatorOnAppStateChanged(StateChange<AppStates> stateChange)
        {
            switch (stateChange.Next)
            {
                case AppStates.Gameplay:
                    Points = _appSettings.InitialPoints;
                    break;
            }
        }

        private void OnDestroyed(CatStats catStats)
        {
            _navigator.AppState = AppStates.GameOver;
        }

        private void Update()
        {
            if (_navigator.AppState == AppStates.Gameplay)
            {
                Points -= _appSettings.PointLossSpeed*Time.deltaTime;
                if (Points <= 0f)
                {
                    Destroyed.SafelyInvoke(this);
                }
            }

        }

		private void UpdateLevel() {
			var previousLevel = Level;
			if (_points <= _appSettings.PointsToLevel1) {
				Level = 0;
			} 
			else if (_points <= _appSettings.PointsToLevel2) {
				Level = 0;
			}
			else if (_points <= _appSettings.PointsToLevel3) {
				Level = 0;
			}
			else if (_points <= _appSettings.PointsToLevel4) {
				Level = 0;
			}
			else {
				Level = 5;
			}

			if (previousLevel != Level) {
				LevelChanged.SafelyInvoke(new StateChange<int>(previousLevel, Level));
			}
		}
    }
}
