using UnityEngine;

public abstract class CannonState : MonoBehaviour
{
    protected CannonController controller;

    // State에 들어왔는지.
    protected bool isEntered;

    // Controller에서 컴포넌트를 가져오니 
    // 캐싱을 완료한 상태에서 해당 함수를 실행시켜야 한다.
    public virtual void SetUp(CannonController controller)
    {
        if (controller != null)
        {
            this.controller = controller;
        }
    }

    public abstract void Enter();
    public abstract void Exit();
}
