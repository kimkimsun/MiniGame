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
        Debug.Log("chase 들어옴");
        owner.SpineAnimator.SetBool(owner.SpineRunAnimId, true);
        owner.Agent.speed = 5;
    }

    public override void Exit()
    {
        owner.SpineAnimator.SetBool(owner.SpineRunAnimId, false);
        owner.Agent.speed = 3.5f;
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
        owner.SpineAnimator.SetBool(owner.IdleAnimId, true);
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
        owner.SpineAnimator.SetBool(owner.IdleAnimId, false);
    }
    public override void Update() 
    {
        Vector3 direction = (owner.PlayerTrans.position - owner.transform.position).normalized;
        direction.y = 0; // y축을 고정하여 위아래를 무시하고 좌우만 회전하도록 설정

        // 좌우 회전 처리 (몸체)
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, lookRotation, Time.deltaTime * 50);

        // 위아래 회전을 위한 방향 계산
        Vector3 headDirection = (owner.PlayerTrans.position - owner.HeadTreans.position).normalized;
        float pitch = -Mathf.Asin(headDirection.y) * Mathf.Rad2Deg;  // 위아래 각도 계산

        // 위아래 회전 처리 (머리)
        Quaternion headRotation = Quaternion.Euler(pitch, owner.HeadTreans.localEulerAngles.y, owner.HeadTreans.localEulerAngles.z);
        owner.HeadTreans.localRotation = Quaternion.Slerp(owner.HeadTreans.localRotation, headRotation, Time.deltaTime * 50);
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
        owner.SpineAnimator.SetBool(owner.SpineRunAnimId, true);
        owner.Agent.speed = 5;
    }

    public override void Exit()
    {
        owner.IsKeep = true;
        owner.WeaponAnimator.SetBool(owner.WeaponAnimId, false);
        owner.SpineAnimator.SetBool(owner.SpineRunAnimId, false);
        owner.Agent.speed = 3.5f;

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
            owner.Agent.remainingDistance /* 위치까지 남은거리가 */<= owner.Agent.stoppingDistance/* 도착했다고 판단을하면 */)
        {
            Exit();
        }
    }
}
