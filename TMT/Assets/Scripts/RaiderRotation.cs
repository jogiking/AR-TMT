using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiderRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main == null) {
            return;
        }

        float rot = Camera.main.transform.localRotation.eulerAngles.y;
        if (rot > 180.0f) {
            rot -= 360.0f;
        }

        //rot /= 3.0f; 
        // Debug.Log("RaiderRotation " + rot.ToString());

        transform.localEulerAngles = new Vector3(0.0f, 0.0f, -rot);
    }
}

/*
 * 
 * 
 *오일러 각도를 직접 잡아 당기면 안됩니다. 나는 이것과 같은 것을 추천 할 것이다 :

Quaternion q = Input.gyro.attitude.rotation;
오일러 각이 아닌 쿼터니언을 사용하면 짐벌 잠금 장치를 피할 수 있으므로 360, 0 문제를 피할 수 있습니다.

y 방향으로 향하는 각도를 단순히 표시하려면 다음과 같이 권장 할 수 있습니다. 이는 y 각도를 0-180 도로 줄입니다.

/// <summary>
/// This method normalizes the y euler angle between 0 and 180. When the y euler
/// angle crosses the 180 degree threshold if then starts to count back down to zero
/// </summary>
/// <param name="q">Some Quaternion</param>
/// <returns>normalized Y euler angle</returns>
private float normalizedYAngle(Quaternion q)
{
    Vector3 eulers = q.eulerAngles;
    float yAngle = eulers.y;
    if(yAngle >= 180f)
    {
        //ex: 182 = 182 - 360 = -178
        yAngle -= 360;
    }
    return Mathf.Abs(yAngle);
}


    */