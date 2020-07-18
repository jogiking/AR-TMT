using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameSceneController : MonoBehaviour
{
    public GameObject OpeningUI = null;
    public GameObject Games = null;
    public GameObject RadarUI = null;
    public GameObject NavigationBarUI = null;
    public GameObject TimerUI = null;

    private float time = 0.0f;

    private void Awake()
    {
        OpeningUI.SetActive(true);
    }

    void Start()
    {
        GamesController.AllGameFinish_EventHandler += FinishSceneHandler;
        MainGameGameController.ProgramExit_EventHandler += FinishSceneHandler;

        MainGameGameController.CountdownFinish_EventHandler += TurnOnUIHandler;

        MainGameGameController.PlayerAllObjectsFind_EventHandler += TurnOffUIHandler;
    }

    public void OpengingUIButtonClick()
    {
        // 스타트 게임 버튼 눌리면 진짜 게임 시작함
        OpeningUI.SetActive(false);

        Games.SetActive(true);
    }

    public void TurnOnUIHandler(object sender, EventArgs e)
    {
        RadarUI.SetActive(true);
        NavigationBarUI.SetActive(true);

        TimerUI.SetActive(true);
        StartCoroutine(TimerStartCoroutine());
    }

    public void TurnOffUIHandler(object sender, EventArgs e)
    {
        RadarUI.SetActive(false);
        NavigationBarUI.SetActive(false);

        TimerUI.SetActive(false);
    }

    public void FinishSceneHandler(object sender, EventArgs e)
    {
        Application.Quit();
    }
    
    public float GetFinishTime()
    {
        return time;
    }

    IEnumerator TimerStartCoroutine()
    {
        Text textLabel = TimerUI.GetComponent<Text>();
        time = 0.0f;
        while(TimerUI.activeSelf == true)
        {
            time += Time.deltaTime;
            textLabel.text = "Time\n" + string.Format("{0:N2}", time);

            yield return null;
        }
    }
}