using System.Collections.Generic;
using System.Linq;
using Sense.Extensions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Assets.OutOfTheBox.Scripts.Audio
{
    public class AudioClips : MonoBehaviour
    {
        [SerializeField] protected List<AudioClip> Clips = new List<AudioClip>();

        public const string BgMusic1 = "BgMusic1";
        public const string BgSeaWaves1 = "BgSeaWaves1";
        public const string BgSeaWaves2 = "BgSeaWaves2";

        public const string SfxClick = "SfxClick";

        private const int Bg1TrackId = 0;
        private const int Bg2TrackId = 1;
        private const int Sfx1TrackId = 2;
        private const int Sfx2TrackId = 2;

        public AudioClip GetClip(string clipName)
        {
            foreach (var clip in Clips)
            {
                if (clip.IsNull())
                {
                    continue;
                }
                if (clip.name.Equals(clipName))
                {
                    return clip;
                }
            }
            Debug.LogError("Could not find clip with name: " + clipName);
            return null;
        }


        public int GetClipTrackId(string clipName)
        {
            switch (clipName)
            {
                case BgMusic1:
                    return Bg1TrackId;
                    
                case BgSeaWaves1:
                case BgSeaWaves2:
                    return Bg2TrackId;

                case SfxClick:
                    return Sfx1TrackId;

                default:
                    Debug.LogError(string.Format("Unknown clip name: " + clipName));
                    return Sfx2TrackId;
            }
        }

#if UNITY_EDITOR
        [MenuItem("Sense/Refresh Audio Clips")]
        private static void RefreshAudioClips()
        {
            AudioClips instance;
            if (Selection.activeGameObject.IsNull() ||
                (instance = Selection.activeGameObject.GetComponent<AudioClips>()).IsNull())
            {
                instance = FindObjectOfType<AudioClips>();
                if (instance.IsNull())
                {
                    return;
                }
            }
            var assetPaths = AssetDatabase.GetAllAssetPaths().Where(IsAudioFile).ToArray();

            instance.Clips.Clear();
            instance.Clips.Capacity = assetPaths.Length;

            foreach (var path in assetPaths)
            {
                var clip = (AudioClip) AssetDatabase.LoadAssetAtPath(path, typeof (AudioClip));
                instance.Clips.Add(clip);
            }

            Debug.Log("Refreshed " + assetPaths.Length + " audio clips!");
        }

        private static bool IsAudioFile(string filename)
        {
            filename = filename.ToLower();
            return filename.EndsWith(".mp3") || filename.EndsWith(".wav");
        }
#endif
    }
}