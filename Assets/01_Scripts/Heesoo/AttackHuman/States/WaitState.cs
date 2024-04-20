using UnityEngine;

public class WaitState : State<AttackHumanController>
{
    public override void OnAwake()
    {
        ridibBody = controller.GetComponent<Rigidbody>();
    }

    public override void OnEnter()
    {
        ridibBody.isKinematic = true;

        controller.ActionAfterCoolTime(2f, () =>
        {
            stateMachine.ChangeState<IdleState>();
        });
    }

    public override void OnUpdate(float deltaTime)
    {

    }

    public override void OnExit()
    {
        ridibBody.isKinematic = false;
    }
}
