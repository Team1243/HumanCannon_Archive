// CannonState들의 상태 흐름 관리
using UnityEngine;

public class CannonStateContext
{
    public CannonStateContext(CannonState state)
    {
        this.CurrentState = state;
    }

    public CannonState CurrentState { get; set; }

    public void Transition(CannonState state)
    {
        if (CurrentState != state)
        {
            CurrentState.Exit();
            CurrentState = state;
            CurrentState.Enter();
        }
    }
}
