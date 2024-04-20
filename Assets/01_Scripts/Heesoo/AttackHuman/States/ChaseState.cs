using UnityEngine;

public class ChaseState : State<AttackHumanController>
{
    private int isMoveHash = Animator.StringToHash("isMove");

    private Vector3 moveDirection;

    public override void OnAwake()
    {
        transform = controller.transform;
        ridibBody = controller.GetComponent<Rigidbody>();
        animator = controller.GetComponentInChildren<Animator>();
    }

    public override void OnEnter()
    {
        animator.SetBool(isMoveHash, true);
    }

    public override void OnUpdate(float deltaTime)
    {
        Transform target = controller.SearchEnemy();
        if (target != null)
        {
            moveDirection = controller.GetDirectionToTarget;
        }

        if (controller.GetDistanceToTarget > controller.AttackRange)
        {
            Vector3 moveVector = moveDirection.normalized * controller.MoveSpeed * deltaTime;
            ridibBody.MovePosition(transform.position + moveVector);
            moveDirection.y = 0;
            transform.rotation = Quaternion.LookRotation(moveDirection).normalized;
        }
        else
        {
            stateMachine.ChangeState<IdleState>();
        }
    }

    public override void OnExit()
    {
        animator.SetBool(isMoveHash, false);
    }
}
