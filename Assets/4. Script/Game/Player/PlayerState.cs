using InterfaceManager;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerState : State
{
    public Player owner;
    public override void Init(IStateMachine sm)
    {
        this.sm = sm;
        owner = (Player)sm.GetOwner();
    }
    public override void Enter() { }
    public override void Exit() { }
    public override void Update() { }
}

public class PlayerIdleState : PlayerState
{
    public override void Enter() 
    {
        Debug.Log("Idle");
    }
    public override void Exit() { }
    public override void Update() { }
}
public class PlayerPainState : PlayerState
{
    public override void Enter() 
    {
        Debug.Log("Pain");
    }
    public override void Exit() { }
    public override void Update() { }
}
public class PlayerExhaustionState : PlayerState
{
    public override void Enter() 
    {
        Debug.Log("Exhaustion");
    }
    public override void Exit() { }
    public override void Update() { }
}
public class PlayerDyingState : PlayerState
{
    public override void Enter() 
    {
        Debug.Log("Dying");
    }
    public override void Exit() { }
    public override void Update() { }
}