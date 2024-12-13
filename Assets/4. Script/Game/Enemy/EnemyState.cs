using InterfaceManager;
using System.Collections;
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
        Debug.Log("원더 침입");
    }

    public override void Exit()
    {
        owner.Agent.ResetPath();
        Debug.Log("원더 나옴");
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
    }

    public override void Exit()
    {
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
}

public class EnemyAttackState : EnemyState // 공격 State
{
    public override void Enter()
    {
        Debug.LogWarning("어택에 들어오는건가");
        owner.Agent.isStopped = true;
        owner.WeaponAnimator.SetBool(owner.WeaponAnimId, true);
        owner.Attack();
    }

    public override void Exit()
    {
        owner.Agent.isStopped = false;
        if (owner.AttackCoroutine != null)
        {
            owner.StopCoroutine(owner.AttackCoroutine); // 실행 중인 코루틴 중단
            owner.AttackCoroutine = null; // 레퍼런스 초기화
        }
        owner.WeaponAnimator.SetBool(owner.WeaponAnimId, false);
    }
    public override void Update() 
    {
        Vector3 direction = (owner.PlayerTrans.position - owner.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, lookRotation, Time.deltaTime * 50);
    }
}
public class EnemyKeepAttackState : EnemyState // 공격 State
{
    Player player = GameManager.Instance.player;
    public override void Enter()
    {
        owner.IsKeep = false;
        Debug.LogWarning("keepattack에 들어오는건가?");
        owner.Attack();
        owner.Agent.SetDestination(player.transform.position);
        owner.WeaponAnimator.SetBool(owner.WeaponAnimId, true);
    }

    public override void Exit()
    {
        owner.IsKeep = true;
        owner.WeaponAnimator.SetBool(owner.WeaponAnimId, false);
        if (owner.AttackCoroutine != null)
        {
            owner.StopCoroutine(owner.AttackCoroutine); // 실행 중인 코루틴 중단
            owner.AttackCoroutine = null; // 레퍼런스 초기화
        }
    }
    public override void Update() 
    {
        // 목표 지점 도달 여부 확인
        if (!owner.Agent.pathPending && // 에이전트가 새로운 경로를 계산 중이지 아니하고,
            owner.Agent.remainingDistance /* 위치까지 남은거리가 */<= owner.Agent.stoppingDistance/* */)
        {
            Exit();
        }
    }
}
