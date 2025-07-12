using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeDuration = 0.3f; // 흔들림 지속 시간
    public float shakeMagnitude = 0.2f; // 흔들림 강도

    public Vector3 originalPos; // 카메라의 원래 위치

    void Start()
    {
        originalPos = transform.position; // 초기 위치 저장
    }

    public void Shake()
    {
        StopAllCoroutines(); // 기존 코루틴 중지
        StartCoroutine(ShakeCoroutine()); // 새로운 흔들림 코루틴 시작
    }

    IEnumerator ShakeCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            // 카메라의 현재 위치를 기준으로 오프셋을 계산
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            // 오프셋을 적용하여 카메라 위치 업데이트
            transform.position = originalPos + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 흔들림이 끝난 후 카메라를 원래 위치로 복원
        transform.position = originalPos;
    }
}