using Assets.GGJ2016.Scripts.Entities;
using Assets.OutOfTheBox.Scripts;
using Assets.OutOfTheBox.Scripts.Navigation;
using Sense.Injection;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.GGJ2016.Scripts.Components
{
    public class DebuggingOverlay : InjectableBehaviour
    {
        [Inject] private AppSettings _appSettings;
        [Inject] private CatStats _catStats;

        [SerializeField] private Text _text;

        private void Update()
        {
            _text.text = string.Format("LV: {0}/{1}, Points: {2}", _catStats.Level, _appSettings.MaxCatLevel, _catStats.Points);
        }
    }
}
