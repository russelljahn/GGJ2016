using UnityEngine;

namespace Assets.OutOfTheBox.Scripts
{
    public class AmbiencePreset : MonoBehaviour
    {
        [SerializeField] private string _name = "";

        public string Name
        {
            get { return _name; }
        }

        [SerializeField] private string _videoPathRelativeToStreamingAssets = "";

        public string VideoPathRelativeToStreamingAssets
        {
            get { return _videoPathRelativeToStreamingAssets; }
        }
    }
}