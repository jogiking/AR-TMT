using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class S2_SceneManager : MonoBehaviour
{    
    public GameObject[] TutorialGames;
    public GameObject StepTextUI = null;
    public GameObject FinishPopup = null;

    private int CurrentGameIdx = 0;

    void Start()
    {
        S2_Game1Controller.CurrentGame1Finish_EventHandler += GameFinishHandler;
        S2_Game2Controller.CurrentGame2Finish_EventHandler += GameFinishHandler;
        S2_Game3Controller.CurrentGame3Finish_EventHandler += GameFinishHandler;
        S2_Game4Controller.CurrentGame4Finish_EventHandler += GameFinishHandler;

        InitGamesAndStart();
    }

    public void InitGamesAndStart()
    {
        InitGames();

        CurrentGameIdx = 0;
        StartGame(CurrentGameIdx);   //  첫번째 게임 시작
    }

    public void InitGames()
    {
        //  모든 게임 끄기
        for (int i = 0; i < TutorialGames.Length; i++)
        {
            TutorialGames[i].SetActive(false);
        }
    }

    public void StartGame(int n)
    {
        //  StepTextUI 바꾸기
        StepTextUI.GetComponent<Text>().text = "Step " + (n+1);

        //  n번인덱스의 게임 오브젝트 켜기(시작)
        TutorialGames[n].SetActive(true);
        TutorialGames[n].SendMessage("StartGame", SendMessageOptions.DontRequireReceiver);
    }

    private void GoNextScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("03.ScanningScene"); //  ScanningScene으로 이동
    }

    public void FinishPopupContinueClick()
    {
        //FinishPopup.SetActive(false);
        
        //InitGamesAndStart();
        
        //UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void FinishPopupExitClick()
    {
        FinishPopup.SetActive(false);
        GoNextScene();
    }

    //  Event Handler
    public void GameFinishHandler(object sender, EventArgs e)
    {
        CurrentGameIdx++;

        //  마자막 게임이 끝난 경우
        if (CurrentGameIdx >= TutorialGames.Length)
        {
            FinishPopup.SetActive(true);    //  게임 종료 여부 팝업 띄우기
            return;
        }

        StartGame(CurrentGameIdx);  //  다음 게임 시작
    }
}