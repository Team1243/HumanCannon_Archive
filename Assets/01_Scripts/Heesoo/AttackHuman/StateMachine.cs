using System.Collections.Generic;
using System;

public sealed class StateMachine<T>
{
    private T stateMachineController;

    private State<T> currentState;
    public State<T> CurrentState => currentState;

    private State<T> beforeState;
    public State<T> BeforeState => beforeState;

    private float stateDurationTime = 0.0f;
    public float StateDurationTime => stateDurationTime;

    private Dictionary<Type, State<T>> stateList = new Dictionary<Type, State<T>>();

    public StateMachine(T stateMachineController, State<T> initState)
    {
        this.stateMachineController = stateMachineController;

        AddStateList(initState);
        currentState = initState;
        currentState.OnEnter();
    }

    public void AddStateList(State<T> addState)
    {
        addState.SetMachineWithController(this, stateMachineController);
        stateList[addState.GetType()] = addState;
    }

    public void Update(float deltaTime)
    {
        stateDurationTime += deltaTime;
        currentState.OnUpdate(deltaTime);
    }

    public S ChangeState<S>() where S : State<T>
    {
        var newType = typeof(S);
        if (currentState.GetType() == newType)
        {
            return currentState as S;
        }

        if (currentState != null)
        {
            currentState.OnExit();
        }
        beforeState = currentState;

        currentState = stateList[newType];
        currentState.OnEnter();
        stateDurationTime = 0.0f;

        return currentState as S;
    }
}