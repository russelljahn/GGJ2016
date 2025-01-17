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

		public const string BgBabyRoom = "BgBabyRoom";
		public const string BgGameOver = "BgGameOver";
		public const string BgWin = "BgWin";
		public const string BgLevel0 = "BgLevel0";
		public const string BgLevel1 = "BgLevel1";
		public const string BgLevel2 = "BgLevel2";
		public const string BgLevel3 = "BgLevel3";
		public const string BgLevel4 = "BgLevel4";

        public const string SfxClick = "SfxClick";
		public const string SfxLevelUp = "SfxLevelUp";
		public const string SfxLevelDown = "SfxLevelDown";
		public const string SfxJump1 = "jumplvl1";
		public const string SfxJump2 = "jumplvl2";
		public const string SfxJump3 = "jumplvl3";
		public const string SfxJump4 = "jumplvl4";
		public const string SfxAttack1 = "scratchlvl1";
		public const string SfxAttack2 = "scratchlvl2";
		public const string SfxAttack3 = "scratchlvl3";
		public const string SfxAttack4 = "scratchlvl4";
		public const string SfxWalk1 = "steplvl1";
		public const string SfxWalk2 = "steplvl2";
		public const string SfxWalk3 = "steplvl3";
		public const string SfxWalk4 = "steplvl4";

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
				case BgLevel0:
				case BgLevel2:
				case BgLevel4:
					return Bg1TrackId;
	                    
				case BgLevel1:
				case BgLevel3:
                    return Bg2TrackId;

	            case SfxClick:
				case SfxLevelUp:
				case SfxLevelDown:
                    return Sfx1TrackId;

				case BgGameOver:
				case BgWin:
				case SfxJump1:
				case SfxJump2:
				case SfxJump3:
				case SfxJump4:
				case SfxAttack1:
				case SfxAttack2:
				case SfxAttack3:
				case SfxAttack4:
					return Sfx2TrackId;

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
			return filename.EndsWith(".mp3") || filename.EndsWith(".wav") || filename.EndsWith(".ogg");
        }
#endif
    }
}