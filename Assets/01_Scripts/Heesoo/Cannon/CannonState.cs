using UnityEngine;

public abstract class CannonState : MonoBehaviour
{
    protected CannonController controller;

    // State�� ���Դ���.
    protected bool isEntered;

    // Controller���� ������Ʈ�� �������� 
    // ĳ���� �Ϸ��� ���¿��� �ش� �Լ��� ������Ѿ� �Ѵ�.
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
