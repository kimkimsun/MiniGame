using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // 감지만 움직이는 것은 저쪽에서
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
            // 움직이는 것은 state에서 한다.
            // 또한 옆에 있는 놈이 attackState라면 정보를 공유해서 한다
            // 짤짤이들만 옆에 있는 놈들이 attack인 것에 영향을 받는다.
            // 하지만 싸우게 되면 평생 싸워야 된다는 문제가 생긴다.
            // 싸움을 하게된다면 풀어지는 방법도 있어야 할 터
            // ChaseState가 된다면 sound를 쫓으면 되고
            // AttackState가 된다면 Player를 쫓으며 쏘면 된다.
            // 어떻게든 가능하다.라는 것.
            // enemy가 쏘는 총알들만 LayerMask를 줘서 해결해보자.
            // 방법 1. overlapSphare에서 protected니까 
        }
    }
}
