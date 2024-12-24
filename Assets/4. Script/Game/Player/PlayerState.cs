using InterfaceManager;

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
        owner.IsHurt = false;
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
        owner.IsHurt = true;
        owner.Speed = walkSpeed;
        owner.SprintSpeed = sprintSpeed;
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
        owner.IsHurt = true;
        owner.Speed = walkSpeed;
        owner.SprintSpeed = sprintSpeed;
    }
    public override void Exit() { }
    public override void Update() { }
}
public class PlayerDyingState : PlayerState
{
    public override void Enter() 
    {
        walkSpeed = 1.0f;
        owner.IsHurt = true;
        owner.Speed = walkSpeed;
        owner.SprintSpeed = walkSpeed;
    }
    public override void Exit() { }
    public override void Update() { }
}