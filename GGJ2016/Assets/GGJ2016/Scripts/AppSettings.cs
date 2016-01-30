using System.Collections.Generic;
using Assets.OutOfTheBox.Scripts.Audio;
using Sense.PropertyAttributes;
using UnityEngine;

namespace Assets.OutOfTheBox.Scripts
{
    public class AppSettings : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _immersionTime = 60.0f;
        public float ImmersionTime
        {
            get { return _immersionTime; }
            set { _immersionTime = Mathf.Max(0f, value); }
        }

        [SerializeField, Min(0f)] private float _clockMenuHideCooldown = 3.0f;
        public float ClockMenuHideCooldown
        {
            get { return _clockMenuHideCooldown; }
            set { _clockMenuHideCooldown = Mathf.Max(0f, value); }
        }

        [SerializeField, Min(0f)]
        private float _ringMenuCooldown = 2.0f;
        public float RingMenuCooldown
        {
            get { return _ringMenuCooldown; }
            set { _ringMenuCooldown = Mathf.Max(0f, value); }
        }

        [SerializeField, Clamp01]
        private float _bgMusic1Volume = 1.0f;
        public float BgMusic1Volume
        {
            get { return _bgMusic1Volume; }
            set { _bgMusic1Volume = Mathf.Clamp01(value); }
        }

        public string BgMusic1
        {
            get { return AudioClips.BgMusic1; }
        }

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        [SerializeField] private List<AmbiencePreset> _ambiencePresets = new List<AmbiencePreset>();

        public List<AmbiencePreset> AmbiencePresets
        {
            get { return _ambiencePresets; }
        }

        public AmbiencePreset CurrentAmbience { get; set; }
    }
}
