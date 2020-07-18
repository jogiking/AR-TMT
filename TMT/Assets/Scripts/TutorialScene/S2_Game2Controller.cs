using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S2_Game2Controller : MonoBehaviour
{
    private float NextGameTimeGap = 3f;

    public GameObject DiscriptionUI = null;
    public GameObject StartButtonUI = null;
    public static event EventHandler CurrentGame2Finish_EventHandler;

    //Effect
    public Transform Effect = null;

    //Arrow
    public GameObject Arrow = null;

    //Game2
    public GameObject TouchObject = null;
    private string DiscriptionText = "다음 도형을 기억하셨으면 확인버튼을 눌러보세요";

    private void Start()
    {
        InitGame();
    }

    public void InitGame()
    {
        TouchObject.SetActive(true);

        DiscriptionUI.GetComponent<Text>().text = DiscriptionText;  //  설명 텍스트 변경

        DiscriptionUI.SetActive(true);

        StartButtonUI.SetActive(false);

        Arrow.SetActive(false);

        Invoke("TurnOnStartButtonUI", 0.5f);    //  몇 초뒤 StartButton 켜기

        Debug.Log("Finish InitGame!");
    }

    public void TurnOnStartButtonUI()
    {
        StartButtonUI.SetActive(true);
    }
    
    public void FinishText()
    {
        DiscriptionUI.GetComponent<Text>().text = "잘하셨습니다!";
    }

    public void StartButtonClick()
    {
        StartButtonUI.SetActive(false);
        Arrow.SetActive(true);
        //  누르면 오브젝트 이동후
        StartCoroutine(MoveObjectCoroutine());

        //  방향도 보여주고
        StartCoroutine(ShowArrowCoroutine());

        //  텍스트도 바꿔주고
        DiscriptionUI.GetComponent<Text>().text = "화면을 화살표 방향으로 옮겨 조금 전 보았던 도형을 찾아 터치해보세요";

        //  터치하면 터지게해야 함
        StartCoroutine(PlayerCoroutine());
    }

    IEnumerator PlayerCoroutine()
    {
        //  터치하면 터지게 해야하고
        //  터치하면 끝내고 다음 게임으로 넘어감

        while(TouchObject.activeSelf == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                
                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log("Hit  1 !");
                    if (System.Object.ReferenceEquals(hit.transform.gameObject, TouchObject.gameObject))
                    {
                        Debug.Log("Hit  2 !");

                        Instantiate(Effect, TouchObject.transform.position, Quaternion.identity);    //  오브젝트 터지는 이펙트
                        TouchObject.SetActive(false);
                    }
                }
            }
            yield return null;
        }

        Invoke("CurrentGame2Finish", NextGameTimeGap);    //  Call Event

        yield return new WaitForSeconds(0.3f);
        DiscriptionUI.GetComponent<Text>().text = "잘하셨습니다!";
    }

    IEnumerator MoveObjectCoroutine()
    {
        Vector3 toPlace = new Vector3(-5, 5, 12);

        float smoothing = 7;

        while (Vector3.Distance(TouchObject.transform.position, toPlace) > 0.05f)
        {
            Debug.Log("MoveObject Loop");
            TouchObject.transform.position = Vector3.Lerp(TouchObject.transform.position,
                toPlace, smoothing * Time.deltaTime);
            yield return null;
        }

        TouchObject.transform.position = toPlace;
    }
    
    IEnumerator ShowArrowCoroutine()
    {
        Arrow.GetComponent<GUIArrowController>().SendMessage("StartFlicker", SendMessageOptions.DontRequireReceiver);

        while (TouchObject.activeSelf == true)
        {
            Vector3 camPos = Camera.main.transform.position;
            Arrow.transform.position = Camera.main.transform.forward * 6.0f + new Vector3(camPos.x, -1.5f, camPos.z);

            Vector3 toDirection = (TouchObject.transform.position - Arrow.transform.position).normalized;

            Arrow.transform.rotation = Quaternion.LookRotation(toDirection, Vector3.up);
            
            Arrow.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));

            yield return null;
        }

        Arrow.SetActive(false);
    }

    //  Event Handler
    public void CurrentGame2Finish()
    {

        CurrentGame2Finish_EventHandler(this, EventArgs.Empty);

        DiscriptionUI.SetActive(false);
        StartButtonUI.SetActive(false);
        gameObject.SetActive(false);
        Arrow.SetActive(false);
    }
}
