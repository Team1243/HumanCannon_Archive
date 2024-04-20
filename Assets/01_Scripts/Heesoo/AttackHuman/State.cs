using UnityEngine;

public abstract class State<T>
{
    protected StateMachine<T> stateMachine;

    protected T controller;

    protected Transform transform;
    protected Rigidbody ridibBody;
    protected Animator animator;

    public virtual void OnAwake() { }
    public virtual void OnEnter() { }
    public abstract void OnUpdate(float deltaTime);
    public virtual void OnExit() { }

    public void SetMachineWithController(StateMachine<T> stateMachine, T stateMachineController)
    {
        this.stateMachine = stateMachine;
        this.controller = stateMachineController;
        OnAwake();
    }
}