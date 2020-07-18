using UnityEngine;
using System.Collections;

public class csDestroyEffect : MonoBehaviour {

    public float DestroyTime = 5.0f;

    void Start()
    {
        Destroy(gameObject, DestroyTime);
    }
}
