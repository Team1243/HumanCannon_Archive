using System.Collections;
using UnityEngine;

public class ShootCannonState : CannonState
{
    private Animator cannonAnimator;

    private readonly int isShootHash = Animator.StringToHash("isShoot");

    public override void SetUp(CannonController controller)
    {
        base.SetUp(controller);
        if (cannonAnimator == null)
        {
            cannonAnimator = controller.Animator;
        }
    }

    public override void Enter()
    {
        isEntered = true;

        cannonAnimator.SetBool(isShootHash, true);

        StartCoroutine(ShootRoop());
    }

    public override void Exit()
    {
        cannonAnimator.SetBool(isShootHash, false);

        isEntered = false;
    }

    private void Shooting()
    {
        // effect
        ParticleManager.Instance.PlayParticle("ShootingEffect", controller.ShootPosTransform);
        SoundManager.Instance.Play(controller.ShootAudioClip, false);
        
        PoolableHuman human = PoolManager.Instance.Pop("Human") as PoolableHuman;
        human.transform.position = controller.ShootPosTransform.position;
        controller.PoolableHumans.Add(human);
    }

    private IEnumerator ShootRoop()
    {
        while (isEntered)
        {
            Shooting();
            yield return new WaitForSeconds(controller.ShootSpeed);

            if (!controller.IsCanShoot)
            {
                Exit();
                yield break;
            }
        }
    }
}
