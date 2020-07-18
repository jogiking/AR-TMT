using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameSceneTest : MonoBehaviour
{
    public Text text = null;

    void Start()
    {
        text.text = ScanningValue.getData().ToString();

     
    }

}
