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
        // �ִϸ��̼�
        animator.SetBool(isAttackHash, true);

        // ������ ó��
        Transform target = controller.Target;
        if (target.TryGetComponent(out IDamageable damageableCmp))
        {
            damageableCmp.OnDamage(controller.AttackDamage);
        }

        // ��Ÿ��
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
