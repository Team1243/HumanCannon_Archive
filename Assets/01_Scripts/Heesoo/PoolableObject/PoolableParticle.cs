using System.Collections;
using UnityEngine;

public class PoolableParticle : PoolableMono
{
    private ParticleSystem myParticleSystem;

    private Coroutine currentCoroutine;

    private void Awake()
    {
        myParticleSystem = GetComponent<ParticleSystem>();
    }

    public override void Init()
    {

    }

    public void SetPositionAndRotation(Vector3 position = default, Quaternion rotation = default)
    {
        myParticleSystem.transform.SetPositionAndRotation(position, rotation);
    }

    public void Play(float durationTime = -1)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        if (durationTime <= 0)
        {
            durationTime = myParticleSystem.main.duration;
        }

        currentCoroutine = StartCoroutine(PlayCoroutine(durationTime));
    }

    private IEnumerator PlayCoroutine(float duration)
    {
        const float offset = 0.1f;

        myParticleSystem.Play();
        yield return new WaitForSeconds(duration + offset);
        myParticleSystem.Stop();

        PoolManager.Instance.Push(this);
        currentCoroutine = null;
    }
}
