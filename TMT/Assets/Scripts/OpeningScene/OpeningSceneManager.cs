using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OpeningSceneManager : MonoBehaviour
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

    public GameObject selectPage = null;

    public Text day = null;

    public Text record = null;


    private void Awake()
    {
        //  2020.02.06. note 8를 위한 임시 코드
        int widthRatio = 16;
        int heightRatio = 9;
        bool fullscreen = Screen.fullScreen;
        Screen.SetResolution(Screen.width, Screen.width * widthRatio / heightRatio, fullscreen);
    }

    void Start()
    {
        //  1. Splash Screen
        prepareSplashScreen();
        splashScreen.SetActive(true);
        StartCoroutine(SplashScreen());

        //  2. Warning Popup
    }

    private void prepareSplashScreen()
    {
        Text splashScreenLabel = splashScreen.transform.Find("Text").GetComponent<Text>();
        string today = System.DateTime.Now.ToString("yyyy-MM-dd");
        splashScreenLabel.text = "AR 인지게임\n" + today;
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

    public void WaitingScreenButton()
    {
        waitingScreen.SetActive(false);

        if (game_mode == GAME_MODE.TEST)
        {

        }
        else
        {
            AndroidNative.Instance.CheckPermission();
        }

        selectPage.SetActive(true);
    }

    public void TutorialButtonClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("02.TutorialScene");
    }

    public void MainGameButtonClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("03.ScanningScene");
    }
}
