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

public class EnemyWanderState : EnemyState // ��ȸ State
{
    int nextPosIndex;
    public override void Enter()
    {
        nextPosIndex = Random.Range(0, owner.NextPos.Length);
        Debug.Log("���� ħ��");
    }

    public override void Exit()
    {
        owner.Agent.ResetPath();
        Debug.Log("���� ����");
    }

    public override void Update()
    {
        // ������������ �Ÿ��� ����� �ָ� �������� �̵�
        if (Vector3.Distance(owner.transform.position, owner.NextPos[nextPosIndex].position) > 1.5f)
        {
            owner.Agent.SetDestination(owner.NextPos[nextPosIndex].transform.position);
        }
        else
        {
            // �������� �����ϸ� ���� �������� �̵�
            nextPosIndex = Random.Range(0, owner.NextPos.Length);
        }
    }
}


public class EnemyChaseState : EnemyState // �߰� State
{
    public override void Enter()
    {
        // �ʱ⼼��? �� �ؾߵɱ� �𸣰ڳ� ��ĸ�
        Debug.Log("chase ����");
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

public class EnemyAttackState : EnemyState // ���� State
{
    public override void Enter()
    {
        Debug.LogWarning("���ÿ� �����°ǰ�");
        owner.Agent.isStopped = true;
        owner.WeaponAnimator.SetBool(owner.WeaponAnimId, true);
        owner.Attack();
    }

    public override void Exit()
    {
        owner.Agent.isStopped = false;
        if (owner.AttackCoroutine != null)
        {
            owner.StopCoroutine(owner.AttackCoroutine); // ���� ���� �ڷ�ƾ �ߴ�
            owner.AttackCoroutine = null; // ���۷��� �ʱ�ȭ
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
public class EnemyKeepAttackState : EnemyState // ���� State
{
    Player player = GameManager.Instance.player;
    public override void Enter()
    {
        owner.IsKeep = false;
        Debug.LogWarning("keepattack�� �����°ǰ�?");
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
            owner.StopCoroutine(owner.AttackCoroutine); // ���� ���� �ڷ�ƾ �ߴ�
            owner.AttackCoroutine = null; // ���۷��� �ʱ�ȭ
        }
    }
    public override void Update() 
    {
        // ��ǥ ���� ���� ���� Ȯ��
        if (!owner.Agent.pathPending && // ������Ʈ�� ���ο� ��θ� ��� ������ �ƴ��ϰ�,
            owner.Agent.remainingDistance /* ��ġ���� �����Ÿ��� */<= owner.Agent.stoppingDistance/* */)
        {
            Exit();
        }
    }
}
