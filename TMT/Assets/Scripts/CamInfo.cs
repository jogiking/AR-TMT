using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamInfo : MonoBehaviour
{
    public Camera cam;
    public GameObject text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = cam.transform.localRotation * Vector3.forward;
        text.GetComponent<Text>().text = "Cam : " + dir.ToString();
    }
}
