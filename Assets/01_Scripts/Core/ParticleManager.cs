using UnityEngine;

public class ParticleManager : MonoSingleton<ParticleManager>
{
    public override void Init()
    {
        
    }

    public void PlayParticle(string poolId, Transform parent, float duration = -1)
    {
        var particle = PoolManager.Instance.Pop(poolId) as PoolableParticle;
        particle.SetPositionAndRotation(parent.position, parent.rotation);
        particle.Play(duration);
    }
}
