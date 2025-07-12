using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnLokcableArea : MonoBehaviour
{
    public int requiredScore;
    public float newCameraSize;
    public Vector3 newCameraPosition;
    public bool isUnLocked = false;
    public Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    public void UnLock()
    {
        if(isUnLocked) return;
        isUnLocked = true;
        gameObject.SetActive(false); 
        cam.orthographicSize = newCameraSize;
        cam.transform.position = newCameraPosition;
    }
}
