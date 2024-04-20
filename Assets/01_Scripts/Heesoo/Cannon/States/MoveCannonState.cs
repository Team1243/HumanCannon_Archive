using DG.Tweening.Core.Easing;
using System.Collections;
using UnityEngine;

public class MoveCannonState : CannonState
{
    private float moveOffset;
    private float moveDurationTime;

    private Vector3 startPos;
    private Vector3 targetPos;
    private float currentTime;

    public override void SetUp(CannonController controller)
    {
        base.SetUp(controller);
        moveOffset = controller.MoveOffset;
        moveDurationTime = controller.MoveDurationTime;  
    }

    public override void Enter()
    {
        isEntered = true;

        targetPos = transform.position;
        targetPos.z += moveOffset;

        startPos = transform.position;
        currentTime = 0;
    }

    public override void Exit()
    {
        transform.position = targetPos;
        controller.levelController.ChangeState(GameLevelState.Appear);

        isEntered = false;
    }

    private void Update()
    {
        if (isEntered)
        {
            currentTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, currentTime / 2);

            if (currentTime >= moveDurationTime)
            {
                controller.Idle();
            }
        }
    }
}
