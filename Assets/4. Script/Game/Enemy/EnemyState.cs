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

public class EnemyWanderState : EnemyState // ��ȸ State
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

    public class EnemyAttackState : EnemyState // ���� State
    {
        public override void Enter()
        {
            owner.Attack();
        }

        public override void Exit()
        {
            Debug.Log("��������");
            owner.StopCoroutine(owner.AttackCoroutine);
        }

        public override void Update()
        {
        }
    }
}