using Cinemachine;
using UnityEngine;

public class CinemachineCameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin noise;
    private float startingIntensity;
    private float timer;
    private float timerTotal;

    public float intensity = 3.0f;

    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        GameManager.Instance.cameraShake = this;
    }

    public void Shake(float time)
    {
        noise.m_AmplitudeGain = intensity;
        startingIntensity = intensity;
        timer = time;
        timerTotal = time;
    }

    private void Update()
    {
        if (timer >= 0.0f)
        {
            timer -= Time.deltaTime;
            noise.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0.0f, 1 - (timer / timerTotal));
        }
    }
}
