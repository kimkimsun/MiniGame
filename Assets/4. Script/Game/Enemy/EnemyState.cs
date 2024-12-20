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
        Debug.Log("chase ����");
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

public class EnemyAttackState : EnemyState // ���� State
{
    public override void Enter()
    {
        Debug.LogWarning("���ÿ� �����°ǰ�");
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
            owner.StopCoroutine(owner.AttackCoroutine); // ���� ���� �ڷ�ƾ �ߴ�
            owner.AttackCoroutine = null; // ���۷��� �ʱ�ȭ
        }
        owner.WeaponAnimator.SetBool(owner.WeaponAnimId, false);
        owner.SpineAnimator.SetBool(owner.IdleAnimId, false);
    }
    public override void Update() 
    {
        Vector3 direction = (owner.PlayerTrans.position - owner.transform.position).normalized;
        direction.y = 0; // y���� �����Ͽ� ���Ʒ��� �����ϰ� �¿츸 ȸ���ϵ��� ����

        // �¿� ȸ�� ó�� (��ü)
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, lookRotation, Time.deltaTime * 50);

        // ���Ʒ� ȸ���� ���� ���� ���
        Vector3 headDirection = (owner.PlayerTrans.position - owner.HeadTreans.position).normalized;
        float pitch = -Mathf.Asin(headDirection.y) * Mathf.Rad2Deg;  // ���Ʒ� ���� ���

        // ���Ʒ� ȸ�� ó�� (�Ӹ�)
        Quaternion headRotation = Quaternion.Euler(pitch, owner.HeadTreans.localEulerAngles.y, owner.HeadTreans.localEulerAngles.z);
        owner.HeadTreans.localRotation = Quaternion.Slerp(owner.HeadTreans.localRotation, headRotation, Time.deltaTime * 50);
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
            owner.StopCoroutine(owner.AttackCoroutine); // ���� ���� �ڷ�ƾ �ߴ�
            owner.AttackCoroutine = null; // ���۷��� �ʱ�ȭ
        }
    }
    public override void Update() 
    {
        // ��ǥ ���� ���� ���� Ȯ��
        if (!owner.Agent.pathPending && // ������Ʈ�� ���ο� ��θ� ��� ������ �ƴ��ϰ�,
            owner.Agent.remainingDistance /* ��ġ���� �����Ÿ��� */<= owner.Agent.stoppingDistance/* �����ߴٰ� �Ǵ����ϸ� */)
        {
            Exit();
        }
    }
}
