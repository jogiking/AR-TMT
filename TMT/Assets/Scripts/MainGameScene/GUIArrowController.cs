using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIArrowController : MonoBehaviour
{
    public Material Lamp = null;
    public GameObject[] Arrows = new GameObject[12];

    public void StartFlicker()
    {
        Debug.Log("StartFlicker");
        StartCoroutine(FlickerCoroutine());
    }

    IEnumerator FlickerCoroutine()
    {
        Color changeColor = Lamp.color;
        Color originColor = Arrows[0].GetComponent<SpriteRenderer>().color;

        while (true)
        {
            float flicker = Mathf.Abs(Mathf.Sin(Time.time * 2));
            for (int i = 0; i < Arrows.Length; i++)
            {
                //Arrows[i].GetComponent<SpriteRenderer>().material.color = originColor * changeColor * flicker;

                Arrows[i].GetComponent<SpriteRenderer>().material.color = new Color(originColor.r, originColor.g, originColor.b, flicker + 0.5f);
            }

            yield return null;
        }

        //gameObject.GetComponent<SpriteRenderer>().color = originColor;
    }
}