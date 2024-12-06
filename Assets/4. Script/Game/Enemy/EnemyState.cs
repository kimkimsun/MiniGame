using InterfaceManager;
using UnityEngine;

public class EnemyState : State
{
    public Enemy owner;
    public override void Init(IStateMachine sm)
    {
        this.sm = sm;
        owner = (Enemy)sm.GetOwner();
    }
    public override void Enter() { }
    public override void Exit() { }
    public override void Update() { }
}

public class EnemyWanderState : EnemyState // 배회 State
{
    int nextPosIndex;
    public override void Enter()
    {
        nextPosIndex = Random.Range(0, owner.NextPos.Length);
        owner.Agent.isStopped = false;
    }

    public override void Exit()
    {
        owner.Agent.isStopped = true;
    }

    public override void Update()
    {
        // 목적지까지의 거리가 충분히 멀면 목적지로 이동
        if (Vector3.Distance(owner.transform.position, owner.NextPos[nextPosIndex].position) > 1.5f)
        {
            owner.Agent.SetDestination(owner.NextPos[nextPosIndex].transform.position);
        }
        else
        {
            // 목적지에 도달하면 다음 목적지로 이동
            nextPosIndex = Random.Range(0, owner.NextPos.Length);
        }
    }
}


public class EnemyChaseState : EnemyState // 추격 State
{
    public override void Enter()
    {
        // 초기세팅? 뭘 해야될까 모르겠네 흠냐링
        Debug.Log("chase 들어옴");
        owner.Agent.isStopped = false;
    }

    public override void Exit()
    {
        owner.Agent.isStopped = true;
    }

    public override void Update()
    {
        owner.Agent.SetDestination(owner.SoundTrans.transform.position);
        if (Vector3.Distance(owner.transform.position, owner.SoundTrans.transform.position) > 1.5f)
        {
            owner.Agent.SetDestination(owner.SoundTrans.transform.position);
        }
        else
        {
            owner.SoundTrans = null;
            sm.SetState("Wander");
        }
    }

    public class EnemyAttackState : EnemyState // 공격 State
    {
        public override void Enter()
        {
            owner.Attack();
        }

        public override void Exit()
        {
            Debug.Log("어택해제");
            owner.StopCoroutine(owner.AttackCoroutine);
        }

        public override void Update()
        {
        }
    }
}