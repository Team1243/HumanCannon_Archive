using UnityEngine;

public class AttackState : State<AttackHumanController>
{
    private int isAttackHash = Animator.StringToHash("isAttack");

    public override void OnAwake()
    {
        animator = controller.GetComponentInChildren<Animator>();
    }

    public override void OnEnter()
    {
        Attack();
    }

    public override void OnUpdate(float deltaTime)
    {

    }

    private void Attack()
    {
        // 애니메이션
        animator.SetBool(isAttackHash, true);

        // 데미지 처리
        Transform target = controller.Target;
        if (target.TryGetComponent(out IDamageable damageableCmp))
        {
            damageableCmp.OnDamage(controller.AttackDamage);
        }

        // 쿨타임
        controller.ActionAfterCoolTime(controller.AttackCoolTime, () =>
        {
            stateMachine.ChangeState<IdleState>();
        });
    }

    public override void OnExit()
    {
        animator.SetBool(isAttackHash, false);
    }
}
