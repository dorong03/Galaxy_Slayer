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
    public SpriteRenderer[] blinkSprite;
    public PlayerManager playerManager;

    void Start()
    {
        cam = Camera.main;
    }

    public void UnLock()
    {
        if(isUnLocked) return;
        isUnLocked = true;
        StartCoroutine(CameraChange(newCameraSize, newCameraPosition, 1f));
    }

    IEnumerator CameraChange(float CameraSize, Vector3 CameraPosition, float time)
    {
        float elapsed = 0f;
        float startSize = cam.orthographicSize;
        Vector3 startPos = cam.transform.position;

        Color[] originColors = new Color[blinkSprite.Length];
        for (int i = 0; i < blinkSprite.Length; i++)
        {
            originColors[i] = blinkSprite[i].color;
        }

        while(elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / time;
            t = Mathf.SmoothStep(0, 1, t);

            cam.orthographicSize = Mathf.Lerp(startSize, CameraSize, t);
            cam.transform.position = Vector3.Lerp(startPos, CameraPosition, t);
            for (int i = 0; i < blinkSprite.Length; i++)
            {
                Color c = originColors[i];
                c.a = Mathf.Lerp(1f, 0f, t);
                blinkSprite[i].color = c;
            }
            yield return null;
        }


        for (int i = 0; i < blinkSprite.Length; i++)
        {
            Color c = originColors[i];
            c.a = 0;
            blinkSprite[i].color = c;
        }
        cam.orthographicSize = CameraSize;
        cam.transform.position = CameraPosition;
        gameObject.SetActive(false);
    }
}
