using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidNative
{
    private static AndroidNative instance = null;
    public static AndroidNative Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AndroidNative();
            }
            return instance;
        }
    }

    private string userName = null;
    public string UserName
    {
        get { return userName; }
        set { userName = value; }
    }

    private int gameType = -1;
    public int GameType // 1~7 days
    {
        get { return gameType; }
        set { gameType = value; }
    }

    private AndroidJavaObject androidJavaObject = null; 

    private AndroidNative()
    {
        if (Application.platform == RuntimePlatform.Android) {
            if (SceneManager.game_mode.Equals(SceneManager.GAME_MODE.NOMAL)) {
                androidJavaObject = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                userName = androidJavaObject.Call<string>("getUserName");
                gameType = androidJavaObject.Call<int>("getGameType");
            } else {
                Debug.Log("Test Mode");
            }
        }
    }

    public void CheckPermission() {
        if (Application.platform == RuntimePlatform.Android) {
            androidJavaObject.Call("checkPermission");
        }
    }

    public void SetReultTime(string resultTime) {
        if (Application.platform == RuntimePlatform.Android) {
            androidJavaObject.Call("setResultTime", resultTime);
        }
    }

    public void Println()
    {
        Debug.Log("Android Native ID : " + userName + " Day: " + gameType.ToString());
    }
}
