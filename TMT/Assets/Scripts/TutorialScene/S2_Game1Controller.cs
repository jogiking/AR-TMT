using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S2_Game1Controller : MonoBehaviour
{
    private float NextGameTimeGap = 3f;

    public GameObject DiscriptionUI = null;
    //public GameObject StartButtonUI = null;
    public static event EventHandler CurrentGame1Finish_EventHandler;

    //Effect
    public Transform Effect = null;

    //Game1
    public GameObject TouchObject = null;
    private string DiscriptionText = "화면 중앙에 보이는 도형을 터치해 보세요";

    private void Start()
    {
        InitGame(); //  게임 오브젝트 초기화
    }

    public void OnObjectTouch()
    {
        //  터치면서 사라지는 애니메이션
        Debug.Log("Pos:" + TouchObject.transform.position);
        //Instantiate(Effect, Camera.main.transform.forward * 4.0f + Camera.main.transform.position, Quaternion.identity);
        Instantiate(Effect, new Vector3(0,0,4f), Quaternion.identity);
   
        TouchObject.SetActive(false);
        
        //  몇 초 뒤에
        Invoke("FinishText", 0.3f);

        //  몇 초 뒤에
        Invoke("CurrentGame1Finish", NextGameTimeGap);  //  Call Event
    }

    public void FinishText()
    {
        DiscriptionUI.GetComponent<Text>().text = "잘하셨습니다!";
    }

    public void InitGame()
    {
        TouchObject.SetActive(true);

        DiscriptionUI.GetComponent<Text>().text = DiscriptionText;  //  설명 텍스트 변경
        DiscriptionUI.SetActive(true);

    }

    //  Event Handler
    public void CurrentGame1Finish()
    {
        gameObject.SetActive(false);
        DiscriptionUI.SetActive(false);

        CurrentGame1Finish_EventHandler(this, EventArgs.Empty);
    }
}
