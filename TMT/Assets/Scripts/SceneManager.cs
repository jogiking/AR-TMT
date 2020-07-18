using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public enum GAME_MODE
    {
        NOMAL = 0,
        TEST = 1
    }

    public static GAME_MODE game_mode = GAME_MODE.TEST;

    public GameObject splashScreen = null;

    public GameObject warningPopup = null;

    public GameObject waitingScreen = null;

    public GameObject helperPopup = null;

    public GameObject scanningUI = null;

    public GameObject arObject = null;

    public GameObject resultScreen = null;

    public Text day = null;

    public Text record = null;

    public GameObject baseUI = null;

    public GameObject baseUI2 = null;

    public GameObject testUI = null;

    public GameObject warningPopup2 = null;

    public GameController gameController = null;
    //IEnumerator AllowARScene()
    //{
    //    while (true)
    //    {
            
    //        while (ARSubsystemManager.systemState == ARSystemState.CheckingAvailability ||
    //            ARSubsystemManager.systemState == ARSystemState.None)
    //        {
    //            Debug.Log("Waiting...");
    //            yield return null;
    //        }
    //        if (ARSubsystemManager.systemState == ARSystemState.Unsupported)
    //        {
    //            Debug.Log("AR unsupported");
    //            yield break;
    //        }
    //        if (ARSubsystemManager.systemState > ARSystemState.CheckingAvailability)
    //        {
    //            Debug.Log("AR supported");
    //            yield break;
    //        }
    //    }
    //}
    //private void OnEnable()
    //{
    //    ARSubsystemManager.CreateSubsystems();
    //    StartCoroutine(ARSubsystemManager.CheckAvailability());
    //    StartCoroutine(AllowARScene());
    //}
    // Start is called before the first frame update
    void Start()
    {

        //  2020.02.06. note 8를 위한 임시 코드
        int SetWidth = 16;
        int SetHeight = 9;
        bool fullscreen = Screen.fullScreen;
        Screen.SetResolution(Screen.width, Screen.width * SetWidth / SetHeight, fullscreen);


        splashScreen.SetActive(true);
        StartCoroutine(SplashScreen());

        // AndroidNative.Instance.Println();
    }

    IEnumerator SplashScreen()
    {
        yield return new WaitForSeconds(2.0f);  //  특정 시간 뒤에 함수 호출하기
        warningPopup.SetActive(true);   // 안전을 위해 이동 중 플레이를 금지하며~ 부분
    }

    public void WarningPopupButton()
    {
        splashScreen.SetActive(false);  //  스플레시 화면을 닫고
        warningPopup.SetActive(false);  //  경고팝업도 닫고
        waitingScreen.SetActive(true);  //  대기 스크린을 띄운다
    }

    public void WaitingSreenButton()
    {
        waitingScreen.SetActive(false);

        if (game_mode == GAME_MODE.TEST)
        {
            testUI.SetActive(true);
            gameController.SetGame(1);
        }
        else
        {
            baseUI.SetActive(true);
            AndroidNative.Instance.CheckPermission();
        }

        arObject.SetActive(true);
        StartCoroutine(ScanningUI());
    }

    bool isScanningFinished = false;
    IEnumerator ScanningUI()
    {
        isScanningFinished = false;
        Debug.Log("New Test::scanning...");
        scanningUI.SetActive(true);
        // TODO : use sprite sheet animation
        Text text = scanningUI.transform.Find("Number").GetComponent<Text>();
        RectTransform rectTransform = scanningUI.transform.Find("Bar").transform.Find("ColorBar").GetComponent<RectTransform>();

        float d = 0.0f , rot = 0.0f;
        float ANGLE_MAX = 90.0f, ANGLE_MIN = -90.0f;
        float max = 0.0f, min = 0.0f;
        float angle = 0.0f;

        while (scanningUI.activeSelf == true)
        {
            if (Camera.main == null){ yield return new WaitForSeconds(0.1f); }
            rot = Camera.main.transform.localRotation.eulerAngles.y;
            if (rot > 180.0f)  rot -= 360.0f;

            if(rot > 0.0f)
            {
                if(rot < ANGLE_MAX && rot > max)
                {
                    max = rot;
                }
            }
            else
            {
                if(rot > ANGLE_MIN && rot < min)
                {
                    min = rot;
                }
            }
            
            //  rot is theta
            if (rot < 0)    
            {
                d = rot * (5.0f / 9.0f) + 50; //    left side
                if (rot < 90 && rot > -90)                {                   
                    rectTransform.offsetMin = new Vector2(d, rectTransform.offsetMin.y);
                    rectTransform.offsetMax = new Vector2(-50, rectTransform.offsetMax.y);
                }
            }
            else
            {
                d = (-1) * rot * (5.0f / 9.0f) + 50; // right side
                if (rot < 90 && rot > -90)
                {
                    rectTransform.offsetMax = new Vector2(-d, rectTransform.offsetMax.y);    //  +일 때는 오른쪽만 수정 = right
                    rectTransform.offsetMin = new Vector2(50, rectTransform.offsetMin.y);
                }
            }

            //rot /= 3.0f; 

            angle = Mathf.Abs(max) + Mathf.Abs(min);
            angle = angle < 90.0f ? 90.0f : angle;

            text.text = "angle1 :  " + rot.ToString() + "\nangle2 : " + angle.ToString() + "\npadding : " + d.ToString();
            yield return null;
        }
 
        gameController.ChangeObjectPosition(angle); //  활동각도 측정을 다하고나서 오브젝트들의 위치를 재배치한다

        isScanningFinished = true;
        ScanningUIButton();

        baseUI2.SetActive(true);
    }

    public void ScanningUIButton()
    {
        scanningUI.SetActive(false);
        isInGame = true;
    }

    public void SetWarningPopup2(bool flag)
    {
        warningPopup2.SetActive(flag);
    }

    public void SetBaseUI2(int numOfRemains)
    {
        Text text = baseUI2.transform.Find("remains").GetComponent<Text>();
        text.text = "남은 갯수 : " + numOfRemains.ToString();
    }
    
    public bool gameover = false;
    public bool isInGame = false;
    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Update Test::SceneManager");

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameExit();
        }

        if (Camera.main == null)
        {
            return;
        }

        if (isScanningFinished == false)
        {
            return;
        }

        if (gameover == false)
        {
            if (gameController.NextIsReady())   //  현재 게임에서 남은 도형이 없을 때(첫번째 게임이 끝났을 때)
            {
                Debug.Log("Generating ok");
                if (!gameController.NextStep()) //  남은 게임이 없는경우라면 false반환되고, 내부 if문 실행함.
                {
                    //  남아있는 게임이 없다면 실행
                    resultScreen.SetActive(true);
                    arObject.SetActive(false);

                    day.text = AndroidNative.Instance.GameType.ToString() + " " + day.text;
                    record.text += " " + string.Format("{0:000.00}", gameController.time);

                    AndroidNative.Instance.SetReultTime(gameController.time.ToString());

                    gameover = true;
                }
            }
        }
    }

    public void updateBaseGuide(bool left, bool right, bool top, bool bottom)
    {
        baseUI2.transform.Find("left").gameObject.SetActive(left);
        baseUI2.transform.Find("right").gameObject.SetActive(right);
        baseUI2.transform.Find("top").gameObject.SetActive(top);
        baseUI2.transform.Find("bottom").gameObject.SetActive(bottom);
    }

    public void GameExit()
    {
        Application.Quit();
    }

    public void OpenHelper()
    {
        helperPopup.SetActive(true);
    }

    public void CloseHelper()
    {
        helperPopup.SetActive(false);
    }
}
