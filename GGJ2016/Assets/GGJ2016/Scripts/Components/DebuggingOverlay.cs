using Assets.GGJ2016.Scripts.Entities;
using Assets.OutOfTheBox.Scripts;
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
		[SerializeField] private Image _progress;
        [SerializeField] private Text _text;

        private void Update()
        {
			var points = Mathf.Clamp(Mathf.FloorToInt(_catStats.Points), 0, _appSettings.PointsToLevel5);
			_text.text = string.Format("<b>LV:</b> {0}/{1}\n<b>Cattiness:</b> {2}", _catStats.Level, _appSettings.MaxCatLevel, points);
			_progress.fillAmount = Mathf.Clamp01(_catStats.Points/_appSettings.PointsToLevel5);
        }
    }
}
