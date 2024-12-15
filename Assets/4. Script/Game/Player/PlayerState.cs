using InterfaceManager;
using StarterAssets;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerState : State
{
    public Player owner;
    protected float walkSpeed;
    protected float sprintSpeed;
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
        walkSpeed = 2.0f;
        sprintSpeed = 5.335f;
        owner._ThirdPersonController.targetSpeed = owner.Input.sprint ? sprintSpeed : walkSpeed;
        // 함수로 묶어야겠지
        // 사운드 호출하고
    }
    public override void Exit() { }
    public override void Update() { }
}
public class PlayerPainState : PlayerState
{
    public override void Enter() 
    {
        walkSpeed = 1.5f;
        sprintSpeed = 4.000f;
        owner._ThirdPersonController.targetSpeed = owner.Input.sprint ? sprintSpeed : walkSpeed;
    }
    public override void Exit() { }
    public override void Update() { }
}
public class PlayerExhaustionState : PlayerState
{
    public override void Enter() 
    {
        walkSpeed = 1.2f;
        sprintSpeed = 3.035f;
        owner._ThirdPersonController.targetSpeed = owner.Input.sprint ? sprintSpeed : walkSpeed;
    }
    public override void Exit() { }
    public override void Update() { }
}
public class PlayerDyingState : PlayerState
{
    public override void Enter() 
    {
        walkSpeed = 1.0f;
        owner._ThirdPersonController.targetSpeed = walkSpeed;
    }
    public override void Exit() { }
    public override void Update() { }
}