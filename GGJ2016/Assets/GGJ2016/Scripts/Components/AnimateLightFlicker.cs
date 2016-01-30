using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OutOfTheBox.Scripts
{
    public class AnimateLightFlicker : MonoBehaviour
    {
        [SerializeField] private Light _light;
        [SerializeField] private float _speed = 1f;
        [SerializeField] private float _minValue = 1f;
        [SerializeField] private float _maxValue = 2f;
        [SerializeField] private Vector2 _seed;

        private void Update()
        {
            _light.intensity = _minValue + (_maxValue-_minValue)*Mathf.PerlinNoise(_seed.x + _speed * Time.time, _seed.y);
        }
    }
}
