using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.OutOfTheBox.Scripts.Editor
{
#if UNITY_5_3
    
    [InitializeOnLoad]
    internal static class AutosaveOnPlay
    {
        static AutosaveOnPlay()
        {
            EditorApplication.playmodeStateChanged = HandleSave;
        }

        private static void HandleSave()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPlaying)
            {
                return;
            }
            Debug.Log(string.Format("Autosaving scene before entering Play mode: '{0}'", SceneManager.GetActiveScene()));
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
            EditorApplication.SaveAssets();
        }
    }
#else
    [InitializeOnLoad]
    internal static class AutosaveOnPlay
    {
        static AutosaveOnPlay()
        {
            EditorApplication.playmodeStateChanged = HandleSave;
        }

        private static void HandleSave()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPlaying)
            {
                return;
            }
            Debug.Log(string.Format("Autosaving scene before entering Play mode: '{0}'", EditorApplication.currentScene));
            EditorApplication.SaveScene();
            EditorApplication.SaveAssets();
        }
    }
#endif
}