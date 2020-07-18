using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScanningSceneManager : MonoBehaviour
{
    public Text ChartAvgText = null;
    public GameObject scanningUI = null;
    public GameObject Gauge = null;
    public GameObject GaugeText = null;

    public GameObject HoldOnUI = null;

    //public GameObject WhichArea = null;

    private float angle = 0.0f;
    private int[] ChartData = new int[40];
    private float ChartAvg = 0f;

    private float CompensationAngle = 0f;

    private void Awake()
    {
        scanningUI.SetActive(false);
    }

    void Start()
    {
        for (int i = 0; i < ChartData.Length; i++)
        {
            ChartData[i] = 0;
        }


        StartCoroutine(ScanningLogicCoroutine());
    }

    IEnumerator ScanningLogicCoroutine()
    {

        yield return StartCoroutine(HoldOnUICoroutine());

        StartCoroutine(ScanningUI());
    }

    IEnumerator HoldOnUICoroutine()
    {
        HoldOnUI.SetActive(true);
        int cnt = 0;
        float sum = 0f;
        while (cnt < 5)
        {
            cnt++;
            float tmp = Input.acceleration.z * 90f;
            Debug.Log("HoldOnUI, cnt =" + cnt);
            sum += tmp;
            yield return new WaitForSeconds(0.1f);
        }
        CompensationAngle = (-1)* sum / cnt;
        HoldOnUI.transform.Find("AngleText").transform.GetComponent<Text>().text =
            "<color=#ff0000>" + "측정된 각도 : " + (CompensationAngle * (-1)).ToString() + "</color>";
        yield return new WaitForSeconds(1.2f);
        HoldOnUI.SetActive(false);
    }

    IEnumerator ScanningUI()
    {
        scanningUI.SetActive(true);
        //Text scanningUIText = scanningUI.transform.Find("Number").GetComponent<Text>();
        //RectTransform rectTransform = scanningUI.transform.Find("Bar").transform.Find("ColorBar").GetComponent<RectTransform>();

        float d = 0.0f, rot = 0.0f;
        float ANGLE_MAX = 90.0f, ANGLE_MIN = -90.0f;
        float max = 0.0f, min = 0.0f;
        angle = 0.0f;

        while (scanningUI.activeSelf == true)
        {
            if (Camera.main == null)
            {
                yield return new WaitForSeconds(0.1f);
            }

            rot = Camera.main.transform.localRotation.eulerAngles.y;

            if (rot > 180.0f)
            {
                rot -= 360.0f;
            }

            if(rot < 92f && rot > -92f)
            {
                if (Convert.ToInt32(rot) % Convert.ToInt32(90f / (ChartData.Length / 4)) == 0)
                {
                    Debug.Log("Data Checking!");
                    //RecortData(rot, Input.acceleration.z * 90f);
                    StartCoroutine(RecordDataCoroutine(rot, Input.acceleration.z * 90f + CompensationAngle));
                }
            }

            if (rot > 0.0f)
            {
                if (rot < ANGLE_MAX && rot > max)
                {
                    max = rot;
                }
            }
            else
            {
                if (rot > ANGLE_MIN && rot < min)
                {
                    min = rot;
                }
            }

            //  rot is theta
            //if (rot < 0)
            //{
            //    d = rot * (5.0f / 9.0f) + 50; //    left side
            //   if (rot < 90 && rot > -90)
            //    {
            //        rectTransform.offsetMin = new Vector2(d, rectTransform.offsetMin.y);
            //       rectTransform.offsetMax = new Vector2(-50, rectTransform.offsetMax.y);
            //    }
           // }
            // right side
            //else
            //{
            //    d = (-1) * rot * (5.0f / 9.0f) + 50;
             //   if (rot < 90 && rot > -90)
             //   {
             //       rectTransform.offsetMax = new Vector2(-d, rectTransform.offsetMax.y);    //  +일 때는 오른쪽만 수정 = right
             //       rectTransform.offsetMin = new Vector2(50, rectTransform.offsetMin.y);
             //   }
            //}

            angle = Mathf.Abs(max) + Mathf.Abs(min);
            angle = angle < 90.0f ? 90.0f : angle;
            //scanningUIText.text = "angle1 :  " + rot.ToString() + "\nangle2 : " + angle.ToString() + "\npadding : " + d.ToString();

            float sum = 0f;
            for (int i = 0; i < 4; i++)
            {
                float tmp = 0f;
                for(int j = 0; j < ChartData.Length / 4; j++)
                {
                    tmp += ChartData[i * ChartData.Length / 4 + j];
                }
                tmp = Mathf.Sqrt(tmp / (ChartData.Length / 4)); //  한 사분면의 평균의 변환값
                if(tmp > 50f)
                {
                    tmp = 50f;
                }
                sum += tmp;
            }

            ChartAvg = sum / 4f;// Mathf.Sqrt(sum / ChartData.Length);

            ChartAvgText.text = "ChartAvg. : "+ChartAvg.ToString() + "\ny'' :"+(Input.acceleration.z * 90f + CompensationAngle).ToString();

            /*-----------------게이지 그리기-----------------------*/
            // ChartAvg = [0, 50]
            float currentPercentage;
            if(ChartAvg > 50f)
            {
                currentPercentage = 100f;
            }
            else if(ChartAvg < 0f)
            {
                currentPercentage = 0f;
            }
            else
            {
                currentPercentage = ChartAvg * 2;
            }
            StartCoroutine(GaugeCoroutine(currentPercentage));//
            GaugeText.GetComponent<Text>().text = "avg : " + ((int)currentPercentage).ToString();

            yield return null;
        }

        /*-----------------값 변환 및 전달-----------------------*/
        angle = 90f / 50f * ChartAvg + 90f;
        if (ChartAvg > 50f) //  현재 50이 최대라고 생각하고 있음
        {
            angle = 180f;
        }
        if(angle > 180f)
        {
            angle = 180f;
        }
        else if(angle < 90f)
        {
            angle = 90f;
        }

        ScanningValue.setData(angle);

        UnityEngine.SceneManagement.SceneManager.LoadScene("04.MainGameScene");
    }

    IEnumerator GaugeCoroutine(float scaleValue)
    {
        // 0 -> 50%(0.5), 100 -> 100%(1.0)
        float tmp = scaleValue / 100 * 0.5f;
        Gauge.transform.localScale = new Vector3(0.5f + tmp, 0.5f + tmp, 0.5f + tmp);

        yield return null;
    }

    private float Fx(float v)
    {
        //  y = 90/(45**4)*(x**4)
        float t1 = v / 45f;
        float t2 = 90f * t1 * t1 * t1 * t1;
        return t2;
    }

    IEnumerator RecordDataCoroutine(float x, float y)
    {
        //ChartData 10 * 4

        if (Convert.ToInt32(x) == 0 || Convert.ToInt32(y) == 0)
        {
            yield break;
        }

        float y2 = y;
        if(Mathf.Abs(y2) > 45f)
        {
            y2 = 45f;
        }
        y2 = Fx(Mathf.Abs(y2)) * Mathf.Sign(y) * 0.5f;
        y = y2;
        
        float dist1 = x * x + y * y;
                
        //  1사분면
        if (x > 0 && y > 0)
        {
            int idx = Math.Abs(Convert.ToInt32(x)) / (90 / (ChartData.Length / 4)) - 1;
            float dist2 = ChartData[idx];
            if (dist1 > dist2)
            {
                ChartData[idx] = (int)dist1;
                Debug.Log("1사분면");
            }
        }
        // 2사분면
        else if (x < 0 && y > 0)
        {
            int idx = ChartData.Length / 4 + Math.Abs(Convert.ToInt32(x)) / (90 / (ChartData.Length / 4)) - 1;
            float dist2 = ChartData[idx];

            if (dist1 > dist2)
            {
                ChartData[idx] = (int)dist1;
                Debug.Log("2사분면");
            }

        }
        // 3사분면
        else if (x < 0 && y < 0)
        {
            int idx = ChartData.Length / 4 * 2 + Math.Abs(Convert.ToInt32(x)) / (90 / (ChartData.Length / 4)) - 1;
            float dist2 = ChartData[idx];
            if (dist1 > dist2)
            {
                ChartData[idx] = (int)dist1;
                Debug.Log("3사분면");
            }
        }
        //  4사분면
        else if(x > 0 && y < 0)
        {
            int idx = ChartData.Length / 4 * 3 + Math.Abs(Convert.ToInt32(x)) / (90 / (ChartData.Length / 4)) - 1;
            float dist2 = ChartData[idx];
            if (dist1 > dist2)
            {
                ChartData[idx] = (int)dist1;
                Debug.Log("4사분면");
            }
        }

        yield return null;
    }

    public void OnButtonClick()
    {
        scanningUI.SetActive(false);
    }
}
