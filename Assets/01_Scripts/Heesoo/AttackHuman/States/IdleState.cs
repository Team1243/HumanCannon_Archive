using UnityEngine;

public class IdleState : State<AttackHumanController>
{
    public override void OnAwake()
    {
        transform = controller.transform;
        ridibBody = controller.GetComponent<Rigidbody>();
    }

    public override void OnEnter()
    {
        ridibBody.MovePosition(transform.position);
    }

    public override void OnUpdate(float deltaTime)
    {
        if (false == controller.IsCanAttack) return;

        Transform target = controller.SearchEnemy();
        if (target)
        {
            if (controller.GetDistanceToTarget < controller.AttackRange)
            {
                stateMachine.ChangeState<AttackState>();
            }
            else
            {
                stateMachine.ChangeState<ChaseState>();
            }
        }
    }
}
