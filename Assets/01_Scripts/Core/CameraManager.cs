using System.Collections;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoSingleton<CameraManager>
{
    private Camera mainCam;

    private CinemachineVirtualCamera virtualCam;

    private CinemachineBasicMultiChannelPerlin noise = null;

    public override void Init()
    {
        mainCam = Camera.main;
        virtualCam = mainCam.GetComponentInChildren<CinemachineVirtualCamera>();
        noise = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>(); 
    }

    private void Start()
    {
        // noise.m_AmplitudeGain = 0;

        // 제일 기본적인 쉐이킹을 일으키는 매개변수 세팅
        // CameraShake(5f, 1f);
    }

    public void CameraShake(float amplitude, float duration)
    {
        StartCoroutine(ShakeCoroutine(amplitude, duration));
    }

    private IEnumerator ShakeCoroutine(float amplitude, float duration)
    {
        float time = duration;

        while (time > 0)
        {
            noise.m_AmplitudeGain = Mathf.Lerp(0, amplitude, time / duration);
            time -= Time.deltaTime;
            yield return null;
        }

        noise.m_AmplitudeGain = 0;
    }

}
