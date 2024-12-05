using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // ������ �����̴� ���� ���ʿ���
    protected StateMachine<Enemy>   sm;
    protected Collider[]            soundCol;
    protected NavMeshAgent          agent;
    protected LayerMask             heardTargetLayerMask; 
    protected float                 soundDetectionRange;
    protected virtual void Start()
    {
        sm.AddState("Wander", new EnemyWanderState());
        sm.AddState("Chase", new EnemyChaseState());
        sm.AddState("Attack", new EnemyAttackState());
        sm.SetState("Wander");
    }
    void Update()
    {
        sm.curState.Update();
        soundCol = Physics.OverlapSphere(transform.position, soundDetectionRange, heardTargetLayerMask);
        if(soundCol.Length > 0 )
        {
            sm.SetState("Chase");
            // �����̴� ���� state���� �Ѵ�.
            // ���� ���� �ִ� ���� attackState��� ������ �����ؼ� �Ѵ�
            // ©©�̵鸸 ���� �ִ� ����� attack�� �Ϳ� ������ �޴´�.
            // ������ �ο�� �Ǹ� ��� �ο��� �ȴٴ� ������ �����.
            // �ο��� �ϰԵȴٸ� Ǯ������ ����� �־�� �� ��
            // ChaseState�� �ȴٸ� sound�� ������ �ǰ�
            // AttackState�� �ȴٸ� Player�� ������ ��� �ȴ�.
            // ��Ե� �����ϴ�.��� ��.
            // enemy�� ��� �Ѿ˵鸸 LayerMask�� �༭ �ذ��غ���.
            // ��� 1. overlapSphare���� protected�ϱ� 
        }
    }
}
