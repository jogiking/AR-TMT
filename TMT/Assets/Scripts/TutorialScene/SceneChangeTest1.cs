using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeTest1 : MonoBehaviour
{
    public void SceneChangeTestClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("03.ScanningScene");
    }
}
