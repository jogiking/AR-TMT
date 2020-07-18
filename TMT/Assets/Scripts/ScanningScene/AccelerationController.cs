using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccelerationController : MonoBehaviour
{
    private Text TestText;

    // Start is called before the first frame update
    void Start()
    {
        TestText = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 angleAcceler = Input.acceleration;
        float rot = Camera.main.transform.localRotation.eulerAngles.y;
        if (rot > 180.0f)
        {
            rot -= 360f;
        }
        TestText.text = "Acc.z : " + (angleAcceler.z * 90f).ToString() + "\nrot :" + rot.ToString();
    }
}
