using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamesController : MonoBehaviour
{
    public static event EventHandler AllGameFinish_EventHandler;

    private int NumberOfGames = 0;
    private int IndexOfCurrentGame = 0;

    void Start()
    {
        PrepareGames();        

        MainGameGameController.CurrentGameFinish_EventHandler += GameFinishHandler;  
    }

    private void PrepareGames()
    {
        //  게임 갯수를 구하기
        NumberOfGames = transform.childCount;

        //  게임 다 끄고
        for (int i = 0; i < NumberOfGames; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        
        //  첫번째만 켜기
        if(NumberOfGames > 0)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void GameFinishHandler(object sender, EventArgs e)
    {
        Debug.Log("함수 호출 스크립트 = " + this.name);

        //  이벤트 호출 객체인 sender를 MainGameGameController로 다운캐스팅
        MainGameGameController MGGC = sender as MainGameGameController;

        //  현재 게임 끄고
        GameObject child = transform.GetChild(IndexOfCurrentGame).gameObject;
        child.SetActive(false);

        IndexOfCurrentGame++;

        if(IndexOfCurrentGame < NumberOfGames)
        {
            //  다음 게임을 켠다
            child = transform.GetChild(IndexOfCurrentGame).gameObject;
            child.SetActive(true);
        }

        //  마지막 게임까지 다 했다면
        if(IndexOfCurrentGame >= NumberOfGames)
        {
            AllGameFinish_EventHandler(this, EventArgs.Empty);  //  Call AllGameFinish Event
        }
    }
}
