using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarklessAR : MonoBehaviour
{

    public GameObject text;

    private bool gyroCheck;

    private Gyroscope gyro;

    private GameObject Container;

    private Quaternion rot;
    
    void Start()
    {
        Container = new GameObject("Container");
        Container.transform.position = transform.position;
        transform.SetParent(Container.transform);
        gyroCheck = GyroCheck();

    }

    private bool GyroCheck()
    {
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;

            Container.transform.rotation = Quaternion.Euler(90f, -90f, 0f);
            //Container.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

            rot = new Quaternion(0f, 0f, 1f, 0);

            return true;
        }
        return false;
    }

    private void Update()
    {

        if (gyroCheck)
        {
            transform.localRotation = gyro.attitude * rot;
            text.GetComponent<Text>().text = "Gyroscope : " + transform.localRotation;
        }
        else
        {
            text.GetComponent<Text>().text = "Gyroscope : No ";
        }
    }

}