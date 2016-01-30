using UnityEngine;
using System.Collections;

public class ShareApp : MonoBehaviour
{

    string subject = "WORD-O-MAZE";
    string body = "PLAY THIS AWESOME. GET IT ON THE PLAYSTORE";

    public void callShareApp()
    {
        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        currentActivity.Call("shareText", subject, body);
    }

    public void Start()
    {
        callShareApp();
    }
}