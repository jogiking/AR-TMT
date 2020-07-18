using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHitController : MonoBehaviour
{

    //  충돌 처음 한번만 호출되는 함수
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        onScale();
    }

    //  충돌중 계속 호출되는 함수
    private void OnTriggerStay2D(Collider2D collision)
    {
        
    }

    //  벗어날 때 한번 호출되는 함수
    private void OnTriggerExit2D(Collider2D collision)
    {
        ExitScale();
    }

    public void onScale()
    {
        Debug.Log("onScale");
        transform.GetComponent<Animator>().SetBool("isHited", true);
    }

    public void ExitScale()
    {
        Debug.Log("ExitScale");
        transform.GetComponent<Animator>().SetBool("isHited", false);
    }
}
