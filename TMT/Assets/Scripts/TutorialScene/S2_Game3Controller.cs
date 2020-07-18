using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class S2_Game3Controller : MonoBehaviour
{
    private float NextGameTimeGap = 3f;

    public GameObject DiscriptionUI = null;
    public GameObject StartButtonUI = null;
    public Material Lamp = null;

    public static event EventHandler CurrentGame3Finish_EventHandler;

    //Effect
    public Transform Effect = null;

    //Game3
    public GameObject TouchObject = null;
    public GameObject TouchObject2 = null;
    public GameObject WrongTouchPopup = null;

    private string DiscriptionText = "화면에 제시된 도형이 깜빡이는 순서대로 터치해주세요";

    private void Start()
    {
        InitGame(); //  게임 오브젝트 초기화

        StartCoroutine(StartGameCoroutine());
    }

    public void InitGame()
    {
        TouchObject.SetActive(true);
        TouchObject2.SetActive(true);

        DiscriptionUI.GetComponent<Text>().text = DiscriptionText;  //  설명 텍스트 변경
        DiscriptionUI.SetActive(true);

        StartButtonUI.SetActive(false);

        WrongTouchPopup.SetActive(false);
    }

    public void FinishText()
    {
        DiscriptionUI.GetComponent<Text>().text = "잘하셨습니다!";
    }

    IEnumerator StartGameCoroutine()
    {

        // 순서대로 깜빡이는 작업
        yield return StartCoroutine(FlickerCoroutine());
        StartCoroutine(PlayerCoroutine());
    }

    IEnumerator FlickerCoroutine()
    {
        StartButtonUI.SetActive(false);
        StartCoroutine(FlickerSubCoroutine(TouchObject, 5f));
        //  화살표 켜는 부분(현재 없음)
        yield return new WaitForSeconds(1.0f);
        StartButtonUI.SetActive(true);
        yield return new WaitUntil(() => StartButtonUI.activeSelf == false);


        StartButtonUI.SetActive(false);
        StartCoroutine(FlickerSubCoroutine(TouchObject2, 5f));
        //  화살표 켜는 부분(현재 없음)
        yield return new WaitForSeconds(1.0f);
        StartButtonUI.SetActive(true);
        yield return new WaitUntil(() => StartButtonUI.activeSelf == false);

    }

    private bool goNext = true;
    IEnumerator FlickerSubCoroutine(GameObject gameObject, float value)
    {
        Color changeColor = Lamp.color;
        Color originColor = gameObject.GetComponent<SpriteRenderer>().color;
        goNext = true;

        while (goNext == true)
        {
            float flicker = Mathf.Abs(Mathf.Sin(Time.time * value));
            gameObject.GetComponent<SpriteRenderer>().color = originColor * changeColor * flicker;

            yield return null;
        }

        gameObject.GetComponent<SpriteRenderer>().color = originColor;
    }


    IEnumerator PlayerCoroutine()
    {
        //  터치하면 터지게 해야하고
        //  터치하면 끝내고 다음 게임으로 넘어감

        while ((TouchObject.activeSelf == true) || (TouchObject2.activeSelf == true))
        {
            if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject() && (WrongTouchPopup.activeSelf ==  false))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (ReferenceEquals(hit.transform.gameObject, TouchObject.gameObject))
                    {
                        Instantiate(Effect, TouchObject.transform.position, Quaternion.identity);    //  오브젝트 터지는 이펙트
                        TouchObject.SetActive(false);
                    }
                    if(ReferenceEquals(hit.transform.gameObject, TouchObject2.gameObject))
                    {
                        Debug.Log("Touch Object 2 hit!");
                        if(TouchObject.activeSelf == false)
                        {
                            Instantiate(Effect, TouchObject2.transform.position, Quaternion.identity);    //  오브젝트 터지는 이펙트
                            TouchObject2.SetActive(false);
                        }
                        else
                        {
                            Handheld.Vibrate();
                            WrongTouchPopup.SetActive(true);
                        }
                    }

                }
            }
            yield return null;
            Debug.Log("while test");
        }

        Invoke("CurrentGame3Finish", NextGameTimeGap);    //  Call Event

        yield return new WaitForSeconds(0.3f);
        DiscriptionUI.GetComponent<Text>().text = "잘하셨습니다!";
    }

    // UI터치 시 GameObject 터치 무시하는 코드
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public void WrongTouchPopupClick()
    {
        WrongTouchPopup.SetActive(false);
    }

    public void StartButtonClick()
    {
        StartButtonUI.SetActive(false);
        goNext = false;
    }

    //  Event Handler
    public void CurrentGame3Finish()
    {
        DiscriptionUI.SetActive(false);
        StartButtonUI.SetActive(false);
        gameObject.SetActive(false);

        CurrentGame3Finish_EventHandler(this, EventArgs.Empty);
    }
}
