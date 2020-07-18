using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class S2_Game4Controller : MonoBehaviour
{
    private float NextGameTimeGap = 3f;

    public GameObject DiscriptionUI = null;
    public GameObject StartButtonUI = null;
    public Material Lamp = null;

    public static event EventHandler CurrentGame4Finish_EventHandler;

    //Effect
    public Transform Effect = null;

    //Game3
    public GameObject TouchObject = null;
    public GameObject TouchObject2 = null;
    public GameObject WrongTouchPopup = null;
    public GameObject Countdown = null;

    public GameObject Arrow1 = null;
    public GameObject Arrow2 = null;
    
    private bool goNext = true;
    private string DiscriptionText = "화면에 제시된 도형이 깜빡이는 순서대로 찾아 터치해주세요";
    private Vector3 toGo1 = new Vector3(5, 5, 14);
    private Vector3 toGo2 = new Vector3(-6, 2, 13);

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

        Countdown.SetActive(false);
    }

    public void FinishText()
    {
        DiscriptionUI.GetComponent<Text>().text = "잘하셨습니다!";
    }

    IEnumerator StartGameCoroutine()
    {
        // 순서대로 깜빡이는 작업
        yield return StartCoroutine(FlickerCoroutine());

        StartCoroutine(MoveObjectsCoroutine());
        yield return StartCoroutine(GameStartCountdown(3)); //  Count down

        StartCoroutine(PlayerCoroutine());
    }

    IEnumerator MoveObjectsCoroutine()
    {
        StartCoroutine(MoveObjectSubCoroutine(TouchObject, toGo1));
        StartCoroutine(MoveObjectSubCoroutine(TouchObject2, toGo2));
        yield return null;
    }

    IEnumerator MoveObjectSubCoroutine(GameObject obj, Vector3 movePosition)
    {
        float smoothing = 4f;

        while (Vector3.Distance(obj.transform.position, movePosition) > 0.05f)
        {
            obj.transform.position = Vector3.Lerp(obj.transform.position, movePosition, smoothing * Time.deltaTime);
            yield return null;
        }

        obj.transform.position = movePosition;
    }


    IEnumerator GameStartCountdown(int time)
    {
        Countdown.SetActive(true);

        for (int i = 0; i < time; i++)
        {
            Countdown.GetComponent<Text>().text = (time - i).ToString();
            yield return new WaitForSeconds(1.0f);
        }

        Countdown.GetComponent<Text>().text = "시작!";

        yield return new WaitForSeconds(1.0f);
        Countdown.SetActive(false);

        //  CountdownFinish();  //  Call Event
    }

    IEnumerator FlickerCoroutine()
    {
        StartButtonUI.SetActive(false);
        StartCoroutine(FlickerSubCoroutine(TouchObject, 5f));
        
        //  화살표 켜는 부분(현재 없음)
        Arrow1.SetActive(true);
        Vector3 direc1 = (toGo1 - TouchObject.transform.position).normalized;
        Arrow1.transform.position = direc1 + TouchObject.transform.position;
        Arrow1.GetComponent<GUIArrowController>().SendMessage("StartFlicker", SendMessageOptions.DontRequireReceiver);
        Quaternion rotation = Quaternion.LookRotation(direc1, Vector3.up);
        Arrow1.transform.rotation = rotation;
        Arrow1.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));

        yield return new WaitForSeconds(1.0f);
        StartButtonUI.SetActive(true);
        yield return new WaitUntil(() => StartButtonUI.activeSelf == false);


        StartButtonUI.SetActive(false);
        StartCoroutine(FlickerSubCoroutine(TouchObject2, 5f));
        //  화살표 켜는 부분(현재 없음)
        Arrow2.SetActive(true);
        Vector3 direc2 = (toGo2 - TouchObject2.transform.position).normalized;
        Arrow2.transform.position = direc2 + TouchObject2.transform.position;
        Arrow2.GetComponent<GUIArrowController>().SendMessage("StartFlicker", SendMessageOptions.DontRequireReceiver);
        Quaternion rotation2 = Quaternion.LookRotation(direc2, Vector3.up);
        Arrow2.transform.rotation = rotation2;
        Arrow2.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));


        yield return new WaitForSeconds(1.0f);
        StartButtonUI.SetActive(true);
        yield return new WaitUntil(() => StartButtonUI.activeSelf == false);


        //  화살표 끄기
        Arrow1.SetActive(false);
        Arrow2.SetActive(false);
    }

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
            if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject() && (WrongTouchPopup.activeSelf == false))
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
                    if (ReferenceEquals(hit.transform.gameObject, TouchObject2.gameObject))
                    {
                        Debug.Log("Touch Object 2 hit!");
                        if (TouchObject.activeSelf == false)
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
        }

        Invoke("CurrentGame4Finish", NextGameTimeGap);    //  Call Event

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
    public void CurrentGame4Finish()
    {
        DiscriptionUI.SetActive(false);
        StartButtonUI.SetActive(false);

        gameObject.SetActive(false);

        WrongTouchPopup.SetActive(false);
        Countdown.SetActive(false);

        CurrentGame4Finish_EventHandler(this, EventArgs.Empty);
    }
}
