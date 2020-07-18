using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MainGameGameController : MonoBehaviour
{
    public static event EventHandler CurrentGameFinish_EventHandler;
    public static event EventHandler CountdownFinish_EventHandler;
    public static event EventHandler ProgramExit_EventHandler;
    public static event EventHandler PlayerAllObjectsFind_EventHandler;

    public GameObject MainGameSceneManager = null;
    
    public int NumberOfObjects = 0;
    public bool IsHaveNumbers = false;
    public bool IsHaveSameObjects = false;
    public bool IsHaveSameColors = false;

    public Material Lamp = null;

    //Effect
    public Transform Effect = null;

    // UI
    public GameObject StartButton = null;
    public GameObject FinishPopup = null;
    public GameObject DiscriptionPopup = null;
    public GameObject WrongTouchPopup = null;
    public GameObject Countdown = null;
    public GameObject NavigationBarUI = null;

    public GameObject Contents = null;
    public GameObject Arrows = null;
    public GameObject ObjectFrame = null;
    public GameObject Arrow = null;

    public GameObject[] ObjectPrefabs = null;
    public GameObject[] NumberPrefabs = null;

    private List<GameObject> Objects = new List<GameObject>();
    private List<Vector3> MovePositions = new List<Vector3>();
    private List<Vector3> InitPositions = new List<Vector3>();
    private float data = 0.0f;


    private void Awake()
    {
        data = ScanningValue.getData();
        if(data < 90f)
        {
            data = 90f;
        }
        else if(data > 180f)
        {
            data = 180;
        }
    }

    void Start()
    {
        // 게임 설명 팝업 띄우기
        DiscriptionPopup.SetActive(true);
    }

    public void DescriptionPopupButtonOnClick()
    {
        //  팝업 없어지면, 게임 시작
        DiscriptionPopup.SetActive(false);

        StartCoroutine(GameLogicCoroutine());
    }

    IEnumerator GameLogicCoroutine()
    {
        SetGame();
        yield return StartCoroutine(GameInitCoroutine());
        yield return StartCoroutine(ObjectsHighLightRoutine());

        StartCoroutine(MoveObjectsCoroutine());
        yield return StartCoroutine(GameStartCountdown(3));  //  Count down

        yield return StartCoroutine(PlayerCoroutine()); //  플레이어 타임
        yield return StartCoroutine(FinishPopupCoroutine());
    }

    IEnumerator FinishPopupCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        FinishPopup.SetActive(true);
        GameObject TimeLabel = FinishPopup.transform.Find("Time").gameObject;
        float time = MainGameSceneManager.GetComponent<MainGameSceneController>().GetFinishTime();
        TimeLabel.GetComponent<Text>().text = string.Format("{0:N2}초", time);

    }
    
    IEnumerator CheckHitCoroutine()
    {
        RaycastHit hit;
        GameObject hitObject = null;
        bool isHit = false;

        var screenCenter = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);
       
        while (Objects.Count > 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenCenter);

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("CheckHit");

                hitObject = hit.transform.gameObject;
                //hit.transform.SendMessage("onScale", SendMessageOptions.DontRequireReceiver);

                if (isHit)
                {
                    if (hitObject != null)
                    {
                        if (!hit.transform.gameObject.Equals(hitObject))
                        {
                            hitObject.GetComponent<Animator>().SetBool("isHited", false);
                            hitObject = hit.transform.gameObject;
                            hit.transform.GetComponent<Animator>().SetBool("isHited", true);
                        }
                    }
                }
                else
                {
                    hitObject = hit.transform.gameObject;
                    isHit = true;
                    hit.transform.GetComponent<Animator>().SetBool("isHited", true);
                }
            }
            else
            {
                if (isHit == true)
                {
                    try
                    {
                        if (hitObject != null) hitObject.GetComponent<Animator>().SetBool("isHited", false);
                        hitObject = null;
                        isHit = false;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log("AnimationTest:: try-catch error");
                    }
                }
            }

            yield return null;
        }
    }

    IEnumerator PlayerCoroutine()
    {
        //  네비게이션바 시작하는 부분
        StartCoroutine(NavigationBarUICoroutine());

        //  확대 애니메이션 처리 시작 부분
        if (IsHaveNumbers == true)
        {
            StartCoroutine(CheckHitCoroutine());
        }

        //  터치 입력 처리하는 부분
        while (Objects.Count > 0)
        {
            if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject() && (WrongTouchPopup.activeSelf == false))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (System.Object.ReferenceEquals(hit.transform.gameObject, Objects[0].gameObject))
                    {
                        Debug.Log("Hit " + hit.transform.gameObject.transform.position +", Remove " + Objects[0].transform.position);
                        Instantiate(Effect, Objects[0].transform.position, Quaternion.identity);    //  오브젝트 터지는 이펙트
                        Objects.RemoveAt(0);

                        DestroyImmediate(hit.transform.gameObject);
                    }
                    else
                    {
                        Handheld.Vibrate();
                        WrongTouchPopup.SetActive(true);
                    }
                }
            }

            yield return null;
        }

        PlayerAllObjectsFound(); //  Call Event
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

    IEnumerator NavigationBarUICoroutine()
    {
        bool left = false, right = false, top = false, bottom = false;

        NavigationBarUI.SetActive(true);

        while(Objects.Count > 0)
        {
            Vector3 camVec = Camera.main.transform.forward;
            left = false;
            right = false;
            top = false;
            bottom = false;

            for (int i = 0; i < Objects.Count; i++)
            {
                if ((left && right && top && bottom) == true)
                {
                    break;
                }

                Vector3 objVec = Objects[i].transform.localPosition;
                objVec.Normalize();

                Vector3 pos = Camera.main.WorldToViewportPoint(Objects[i].transform.position);

                if (pos.x < 0f || pos.x > 1f)
                {
                    //  물체가 x범위에 없다
                    //  없으면, 각도 계산을 한다. 그리고 그 방향을 표시한다.
                    float diff = objVec.x - camVec.x;
                    if (diff > 0) right = true;
                    else left = true;
                }

                if (pos.y < 0f || pos.y > 1f)
                {
                    //  물체가 y범위에 없다
                    //  없으면, 각도 계산을 한다. 그리고 그 방향을 표시한다.
                    float diff = objVec.y - camVec.y;
                    if (diff > 0) top = true;
                    else bottom = true;
                }
            }

            //  네비게이션바 표시여부 결정
            NavigationBarUI.transform.Find("Left").gameObject.SetActive(left);
            NavigationBarUI.transform.Find("Right").gameObject.SetActive(right);
            NavigationBarUI.transform.Find("Top").gameObject.SetActive(top);
            NavigationBarUI.transform.Find("Bottom").gameObject.SetActive(bottom);

            //Debug.Log("Navi : " + left + ", " + right + ", " + top + ", " + bottom);

            yield return null;
        }

        NavigationBarUI.SetActive(false);
    }

    IEnumerator MoveObjectsCoroutine()
    {
        for(int i = 0; i < NumberOfObjects; i++)
        {
            yield return new WaitForSeconds(Random.Range(0.1f, 0.25f));
            StartCoroutine(MoveObjectsSubCoroutine(i));
        }
    }

    IEnumerator MoveObjectsSubCoroutine(int objectIndex)
    {
        float smoothing = 7f;

        while (Vector3.Distance(Objects[objectIndex].transform.position, MovePositions[objectIndex]) > 0.05f)
        {
            Objects[objectIndex].transform.position = Vector3.Lerp(Objects[objectIndex].transform.position, MovePositions[objectIndex], smoothing * Time.deltaTime);
            yield return null;
        }

        Objects[objectIndex].transform.position = MovePositions[objectIndex];
    }

    private void GenerateInitPosition()
    {
        float R = 3.0f;
        float InfrontOf = 4.0f;
        float Interval = 360.0f / NumberOfObjects;

        for(int i = 0; i < NumberOfObjects; i++)
        {
            Vector3 pos = new Vector3(Mathf.Cos(Interval * i * Mathf.PI / 180.0f), Mathf.Sin(Interval * i * Mathf.PI / 180.0f), InfrontOf);
            Debug.Log("Pos : " + pos.ToString());

            pos = R * pos;
            InitPositions.Add(pos);
        }
    }

    private void SettOffArrow()
    {
        Destroy(Arrows);

        //Arrows.SetActive(false);
    }

    IEnumerator GameStartCountdown(int time)
    {
        Countdown.SetActive(true);

        //  객체 이동 화살표 해제 부분
        SettOffArrow();

        for (int i = 0; i < time; i++)
        {
            Countdown.GetComponent<Text>().text = (time - i).ToString();
            yield return new WaitForSeconds(1.0f);
        }
        
        Countdown.GetComponent<Text>().text = "시작!";

        yield return new WaitForSeconds(1.0f);
        Countdown.SetActive(false);
        
        CountdownFinish();  //  Call Event
    }

    IEnumerator GameInitCoroutine()
    {
        yield return new WaitForSeconds(3.0f);
    }

    IEnumerator ObjectsHighLightRoutine()
    {
        for(int i = 0; i < NumberOfObjects; i++)
        {
            StartButton.SetActive(false);
            
            yield return StartCoroutine(FlickerCoroutine(Objects[i], 7f, 1.5f));

            //  화살표 켜는 부분   --> 메인 게임에서 화살표는 사용하지 않는것으로 변경되었음(2020.07.03)
            //SetOnArrow(i);

            yield return new WaitForSeconds(1.0f);

            // 버튼을 표시 후 대기 -> 삭제(2020.07.03)
        }

        StartButton.SetActive(true);
        yield return new WaitUntil(() => StartButton.activeSelf == false);
    }

    private void SetOnArrow(int currentIdx)
    {
        //  기본적으로 y축을 가리키고 있는 형태임
        GameObject obj = Instantiate(Arrow);
        
        //  Help Code
        //GameObject To = Instantiate(Arrow);
        //To.transform.position = MovePositions[currentIdx];
        //To.transform.localScale = new Vector3(5.0f, 5.0f, 5.0f);
        //

        Vector3 direction = (MovePositions[currentIdx] - InitPositions[currentIdx]).normalized;
        Vector3 pos = direction * 0.8f + InitPositions[currentIdx]; //  놓을 위치
        Debug.Log("FROM:" + InitPositions[currentIdx] + ", TO:" + pos);

        obj.transform.position = pos;
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        obj.transform.rotation = rotation;
        obj.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));

        obj.transform.parent = Arrows.transform;

        obj.transform.SendMessage("StartFlicker", SendMessageOptions.DontRequireReceiver);

        //  StartCoroutine(FlickerCoroutine(obj, 20.0f));
    }
       
    IEnumerator FlickerCoroutine(GameObject gameObject, float value, float duration)
    {
        //Color changeColor = Lamp.color;
        Color originColor = gameObject.GetComponent<SpriteRenderer>().color;

        float time = 0f;

        while(time < duration)
        {
            float flicker = Mathf.Abs(Mathf.Sin(time * value));
            gameObject.GetComponent<SpriteRenderer>().color = originColor * flicker;
            
            //t = t + Time.deltaTime * value;

            time += Time.deltaTime;
            yield return null;
        }

        gameObject.GetComponent<SpriteRenderer>().color = originColor;


        /*
         * 
         * 
          Color changeColor = Lamp.color;
        Color originColor = gameObject.GetComponent<SpriteRenderer>().color;

        float time = 0f;

        while(time < duration)
        {
            float flicker = Mathf.Abs(Mathf.Sin(Time.time * duration));
            gameObject.GetComponent<SpriteRenderer>().color = originColor * changeColor * flicker;

            time += Time.deltaTime;
            yield return null;
        }

        gameObject.GetComponent<SpriteRenderer>().color = originColor;

         */
    }

    private int[] GetRandomRange(bool isHaveSameST)
    {
        //  중복되지 않는 난수 생성
        //  중복되는 난수 생성
        int ranLength = NumberOfObjects;
        int[] ranArr = Enumerable.Range(0, ranLength).ToArray();

        if (isHaveSameST == true)
        {
            ranArr = new int[ranLength];

            for(int i = 0; i < ranLength; i++)
            {
                ranArr[i] = Random.Range(0, ranLength);
            }
        }

        else
        {
            for (int i = 0; i < ranLength; i++)
            {
                int ranIdx = Random.Range(i, ranLength);
                int tmp = ranArr[ranIdx];
                ranArr[ranIdx] = ranArr[i];
                ranArr[i] = tmp;
            }
        }

        return ranArr;
    }

    private Color[] GetColorPallet()
    {
        Color[] ranArr = new Color[8];

        ranArr[0] = new Color(1, 0, 0); //red
        ranArr[1] = new Color(0, 1, 0); //  blue
        ranArr[2] = new Color(0.3f, 0.3f, 1);   // 옅은파랑
        ranArr[3] = new Color(1, 0, 1); //  자홍magenta
        ranArr[4] = new Color(1, 1, 1); //  white
        ranArr[5] = new Color(1, 0.92f, 0.016f); // Color.yellow;   //1 0.92 0.016
        ranArr[6] = new Color(1, 0.5f, 0.6f);   //분홍;
        ranArr[7] = new Color(0, 1, 1); // Color.cyan;

        return ranArr;
    }

    public void SetGame()
    {
        //  맨 처음에 보여줄 위치를 생성
        GenerateInitPosition();

        int []objectsIdxArray = GetRandomRange(IsHaveSameObjects);
        int []colorsIdxArray = GetRandomRange(IsHaveSameColors);
        int []numberIdxArray = GetRandomRange(false);   //  중복없이 하기위해서 false를 넣음
        Color[] ranColorArr = GetColorPallet();

        for (int i = 0; i < NumberOfObjects; i++)
        {
            int objectIndex = objectsIdxArray[i];
            GameObject obj = Instantiate(ObjectFrame);
            obj.GetComponent<SpriteMask>().sprite = ObjectPrefabs[objectIndex].GetComponent<SpriteRenderer>().sprite;   //  오브젝트 형태 지정

            obj.transform.localPosition = InitPositions[i];
            MovePositions.Add(GenerateRandomPosition());

            //  숫자 텍스쳐가 있는 경우
            if (IsHaveNumbers == true)
            {
                obj.GetComponent<SpriteRenderer>().sprite = NumberPrefabs[numberIdxArray[i]].GetComponent<SpriteRenderer>().sprite;
            }

            //  색상 입히기
            int colorIndex = colorsIdxArray[i];
            obj.GetComponent<SpriteRenderer>().color = ranColorArr[colorIndex];

            //  카메라 바라보게 하기
            //obj.transform.LookAt(obj.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
            obj.transform.parent = Contents.transform;

            //obj.GetComponent<Renderer>().enabled = false;

            Objects.Add(obj);
        }

    }
    //private Vector3 generatePosition()
    //{
    //    float x = 0.0f, y = 0.0f, z = 0.0f, r = 0.0f;
    //    float angle = data;
    //    float angleRadian = angle / 2.0f * 3.14f / 180.0f;
    //    float angleTan = Mathf.Tan(3.14f / 2.0f - angleRadian);
    //    float range = 7.0f;
    //    float gap = 5.0f;

    //    bool flag = true;
    //    while (flag)
    //    {
    //        x = Random.Range(-range, range);
    //        r = Random.Range(gap, range);
    //        z = Mathf.Sqrt(Mathf.Abs(r * r - x * x));
    //        float v1 = Mathf.Abs(angleTan);
    //        float v2 = Mathf.Abs(x / z);
    //        if (v1 > v2)
    //        {
    //            float yRange = z * Mathf.Abs(angleTan);
    //            y = Random.Range(-yRange, yRange);
    //            flag = false;
    //        }
    //    }
    //    Debug.Log("New Pos: " + x + y + z);
    //    return new Vector3(x, y, z);
    //}

    public Vector3 GenerateRandomPosition()
    {
        // 실제로 이동될 위치를 생성하는 부분

        float x, y, z;
        float rangeDistance = 4f / 90f * (data - 90) + 5;
        x = Random.Range(-rangeDistance, rangeDistance);
        y = Random.Range(-rangeDistance, rangeDistance);
        z = Random.Range(12.0f, 15.0f);

        //x = Random.Range(-7.0f, 7.0f);
        //y = Random.Range(-7.0f, 7.0f);
        //z = Random.Range(12.0f, 15.0f);

        return new Vector3(x, y, z);
    }

    public void StartButtonClick()
    {
        StartButton.SetActive(false);
        //goNext = false;
    }

    public void FinishPopupContinueClick()
    {
        FinishPopup.SetActive(false);
        CurrentGameFinish();    //  Call Event
    }

    public void FinishPopupExitClick()
    {
        FinishPopup.SetActive(false);
        ProgramExit();  //  Call Event
    }

    public void WrongTouchPopupClick()
    {
        WrongTouchPopup.SetActive(false);
    }

    //  Event Handler
    public void CurrentGameFinish()
    {
        CurrentGameFinish_EventHandler(this, EventArgs.Empty);
    }

    //  Event Handler
    public void CountdownFinish()
    {
        CountdownFinish_EventHandler(this, EventArgs.Empty);
    }

    //  Event Handler
    public void ProgramExit()
    {
        ProgramExit_EventHandler(this, EventArgs.Empty);
    }

    //  Event Handler
    public void PlayerAllObjectsFound()
    {
        PlayerAllObjectsFind_EventHandler(this, EventArgs.Empty);
    }
}