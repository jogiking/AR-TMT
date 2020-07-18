using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarklessAR2 : MonoBehaviour
{

    public Text text;

    private bool gyroCheck = false;

    private Gyroscope gyro;

    private GameObject Container;

    private Quaternion rot;

    IEnumerator InitializeGyro()
    {
        gyro = Input.gyro;
        gyro.enabled = true;
        yield return new WaitForSeconds(1.0f);
        gyroCheck = GyroCheck();
        //yield return null;
    }

    private bool AfterinitTime = false;
    private void OnEnable()
    {
        //Container = GameObject.Find("Container");
        //Container.transform.rotation = Quaternion.Euler(90f, GetCameraRotationParam(), 0f);
        //transform.SetParent(Container.transform);


        //GameObject container = GameObject.Find("Container");
        //Container.transform.rotation = Quaternion.Euler(90f, f_output, 0f);
        ////Container.transform.rotation = container.transform.rotation;
        ////Container.transform.rotation = Quaternion.Euler(90f, GetCameraRotationParam(), 0f);
        //transform.SetParent(container.transform);

        if (AfterinitTime)
        {
            transform.SetParent(Container.transform);

        }
    }

    private void OnDisable()
    {
        GameObject parentObject = GameObject.Find("AR Session Origin");
        transform.SetParent(parentObject.transform);
        // transform.position = new Vector3(0, 0, 0);
        // transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

    }

    void Start()
    {
        StartCoroutine(InitializeGyro());

        //Container = new GameObject("Container");
        //Container.transform.position = transform.position;
        //transform.SetParent(Container.transform);

        Container = GameObject.Find("Container");
        Container.transform.position = transform.position;
        transform.SetParent(Container.transform);
        AfterinitTime = true;
    }

    float f_input = 0.0f;
    float f_output = 0.0f;
    private float GetCameraRotationParam()
    {
        float _input = gyro.attitude.z;
        float _output = 0.0f;
        if (_input >= 0.0f && _input <= 1.0f)
        {
            _output = 180 * (_input - 1);
        }
        else if (_input < 0.0f && _input > -1.0f)
        {
            _output = 180 * (_input + 1);
        }
        else
        {
            Debug.Log("GetCameraRotationParam:: _input Error, " + _input);
            _output = 0.0f;
        }

        Debug.Log("GetCameraRotationParam:: _input : " + _input + ", _output : " + _output);

        f_input = _input;
        f_output = _output;

        return _output;
    }

    private float GetCameraRotationParam2()
    {
        float _input = gyro.attitude.z;
        float _output = 0.0f;
        if (_input >= 0.0f && _input <= 1.0f)
        {
            _output = 180 * (_input - 1);
        }
        else if (_input < 0.0f && _input > -1.0f)
        {
            _output = 180 * (_input + 1);
        }
        else
        {
            Debug.Log("GetCameraRotationParam:: _input Error, " + _input);
            _output = 0.0f;
        }

        Debug.Log("GetCameraRotationParam:: _input : " + _input + ", _output : " + _output);

        return _output;
    }

    private bool GyroCheck()
    {
        if (SystemInfo.supportsGyroscope)
        {
            //  자이로센서는 바닥에 올려놓고 북쪽을 기준으로 설정된다.
            //  바닥이 아닌 정면을 응시하기 위해 X축 기준으로 90도 재설정

            Container.transform.rotation = Quaternion.Euler(90f, GetCameraRotationParam(), 0f);

            /*
                Attitude	0.5 	0.5	    0.5 	0.5         //  gyro.attitude
                Gyro	    0.5	    -0.5	0.5 	-0.5        //  tr.localRotation = gyro.attitude * rot
                Cam	        0.0 	0.0     0.0	    -1.0        //  tr.rotation

                자이로에서 왼쪽인식일 때에는 카메라도 왼쪽으로 이동해야한다
            */
            rot = new Quaternion(0f, 0f, 1f, 0f);

            return true;
        }
        return false;
    }

    private void Update()
    {

        if (gyroCheck)
        {
            float tmp = GetCameraRotationParam2();
            transform.localRotation = gyro.attitude * rot;
            text.text = "tr.lcRotation : " + transform.localRotation.ToString() +
                        "\nCam : " + transform.rotation.ToString() +
                      "\nAttitude : " + gyro.attitude.ToString() +
                      "\nf_input : " + f_input +
                      "\nf_output : " + f_output +
                      "\ncurInput : " + gyro.attitude.z.ToString() +
                      "\ncurOutput : " + tmp;
        }
        else
        {
            text.text = "Gyroscope : No ";
        }
    }

}