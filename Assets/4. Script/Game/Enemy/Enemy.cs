using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static EnemyChaseState;

public abstract class EnemyStrategy
{
    protected Enemy owner;
    public abstract void Init();
    public abstract void Attack();
}
public class BossEnemyStrategy : EnemyStrategy
{
    public BossEnemyStrategy(Enemy owner)
    {
        this.owner = owner;
        Init();
    }
    public override void Init()
    {
    }
    public override void Attack()
    {

    }
}
public class HorrorEnemyStrategy : EnemyStrategy
{
    public HorrorEnemyStrategy(Enemy owner)
    {
        this.owner = owner;
        Init();
    }
    public override void Init()
    {
    }
    public override void Attack()
    {

    }
}
public class MiniEnemyStrategy : EnemyStrategy
{
    GameObject bullet;
    GameManager gmInstance = GameManager.Instance;
    float fireRate = 1f;
    bool isShooting;
    public MiniEnemyStrategy(Enemy owner)
    {
        this.owner = owner;
        isShooting = true;
        Init();
    }
    public override void Init()
    {
        gmInstance.CreatePool(owner.Bullet, 10);
        owner.SoundDetectionRange = 20f;
        owner.LookDetectionRange = 15f;
        owner.AttackCoroutine = FireCo();
    }
    public override void Attack()
    {
        if (isShooting)
        {
            owner.StartCoroutine(owner.AttackCoroutine);
            isShooting = false;
        }
    }
    IEnumerator FireCo()
    {
        while(true)
        {
            yield return new WaitForSeconds(fireRate);
            bullet = GameManager.Instance.GetFromPool(owner.Bullet);
            bullet.transform.position = owner.transform.position;
        }
    }
}

public enum ENEMY_TYPE
{
    BOSS,
    HORROR,
    MINI,
}


public class Enemy : MonoBehaviour
{
    #region 변수
    [Header("Serializefield")]
    [SerializeField] private ENEMY_TYPE type;
    [SerializeField] private Transform[] nextPos;
    [SerializeField] private GameObject bullet;

    private EnemyStrategy enStrategy;
    private StateMachine<Enemy> sm;
    private Collider[] soundCol;
    private Collider[] lookCol;
    private Transform soundTrans;
    private NavMeshAgent agent;
    private GameObject obj;
    private LayerMask heardTargetLayerMask;
    private LayerMask lookTargetLayerMask;
    private IEnumerator attackCoroutine;
    private float soundDetectionRange;
    private float lookDetectionRange;
    private float maxDistance;
    private bool isHearable;
    private bool isPlayerCheck;

    #endregion

    #region 프로퍼티
    
    public IEnumerator AttackCoroutine
    {
        get => attackCoroutine;
        set => attackCoroutine = value;
    }

    public GameObject Bullet
    {
        get => bullet;
        set => bullet = value;
    }
    public Transform SoundTrans
    {
        get => soundTrans;
        set => soundTrans = value;
    }
    public Transform[] NextPos
    {
        get => nextPos;
        set => nextPos = value;
    }
    public GameObject Obj
    {
        get => obj;
        set => obj = value;
    }
    public Collider[] SoundCol
    {
        get => soundCol;
        set => soundCol = value;
    }
    public NavMeshAgent Agent
    {
        get => agent;
        set => agent = value;
    }
    public float SoundDetectionRange
    {
        get => soundDetectionRange;
        set => soundDetectionRange = value;
    }
    public float LookDetectionRange
    {
        get => lookDetectionRange;
        set => lookDetectionRange = value;
    }
    #endregion
    protected virtual void Start()
    {
        //정의
        agent =                             GetComponent<NavMeshAgent>();
        heardTargetLayerMask =              1 << 6;
        lookTargetLayerMask =               1 << 7;
        sm =                                new StateMachine<Enemy>();
        sm.owner =                          this;
        maxDistance =                       15f;
        //정의


        sm.AddState("Wander", new EnemyWanderState());
        sm.AddState("Chase", new EnemyChaseState());
        sm.AddState("Attack", new EnemyAttackState());
        sm.SetState("Wander");
        Init();
    }

    private void Init()
    {
        switch (type)
        {
            case ENEMY_TYPE.BOSS:
                enStrategy = new BossEnemyStrategy(this);
                break;
            case ENEMY_TYPE.HORROR:
                enStrategy = new HorrorEnemyStrategy(this);
                break;
            case ENEMY_TYPE.MINI:
                enStrategy = new MiniEnemyStrategy(this);
                break;
        }
    }
    void Update()
    {
        sm.curState?.Update();
        soundCol = Physics.OverlapSphere(transform.position, soundDetectionRange, heardTargetLayerMask);
        lookCol = Physics.OverlapSphere(transform.position, lookDetectionRange, lookTargetLayerMask);
        Debug.Log(sm.curState);
        HandleStateChange();
    }
    void HandleStateChange()
    {
        if (lookCol.Length > 0)
        {
            RaycastHit hit;
            Vector3 direction = ((lookCol[0].transform.position) - transform.position).normalized;
            Debug.DrawLine(transform.position, transform.position + (direction * maxDistance), Color.red);
            if (Physics.Raycast(transform.position, direction * maxDistance, out hit, maxDistance) && CheckInLayerMask(hit.collider.gameObject.layer))
                sm.SetState("Attack");
        }
        else if (soundCol.Length > 0)
        {
            Debug.Log("예?");
            soundTrans = soundCol[0].transform;
            sm.SetState("Chase");
        }
        else if (soundCol.Length == 0)
        {
            sm.SetState("Wander");
        }
    }
    private bool CheckInLayerMask(int layerIndex)
    {
        return (lookTargetLayerMask & (1 << layerIndex)) != 0;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, soundDetectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookDetectionRange);
    }
    public void Attack()
    {
        enStrategy.Attack();
    }
}

// 움직이는 것은 state에서 한다.
// 또한 옆에 있는 놈이 attackState라면 정보를 공유해서 한다
// 짤짤이들만 옆에 있는 놈들이 attack인 것에 영향을 받는다.
// 하지만 싸우게 되면 평생 싸워야 된다는 문제가 생긴다.
// 싸움을 하게된다면 풀어지는 방법도 있어야 할 터
// ChaseState가 된다면 sound를 쫓으면 되고
// AttackState가 된다면 Player를 쫓으며 쏘면 된다.
// 어떻게든 가능하다.라는 것.
// enemy가 쏘는 총알들만 LayerMask를 줘서 해결해보자.
// 방법
// 1. overlapSphare에서 몬스터가 생성하는 총알 Layer를 감지한다.
// 2. 총알을 감지했을 때 sm.curstate != AttackState라면 AttackState가 된다.