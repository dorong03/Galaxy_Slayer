using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float cameraY;
    public float cameraX;

    public Camera cam;

    void Start()
    {
        cam = Camera.main;
        cam.orthographic = true;
        
    }
    
    void Update()
    {
        cam.orthographicSize = cameraY;
    }
}
