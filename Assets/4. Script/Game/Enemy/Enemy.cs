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
    #region ����
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

    #region ������Ƽ
    
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
        //����
        agent =                             GetComponent<NavMeshAgent>();
        heardTargetLayerMask =              1 << 6;
        lookTargetLayerMask =               1 << 7;
        sm =                                new StateMachine<Enemy>();
        sm.owner =                          this;
        maxDistance =                       15f;
        //����


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
            Debug.Log("��?");
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

// �����̴� ���� state���� �Ѵ�.
// ���� ���� �ִ� ���� attackState��� ������ �����ؼ� �Ѵ�
// ©©�̵鸸 ���� �ִ� ����� attack�� �Ϳ� ������ �޴´�.
// ������ �ο�� �Ǹ� ��� �ο��� �ȴٴ� ������ �����.
// �ο��� �ϰԵȴٸ� Ǯ������ ����� �־�� �� ��
// ChaseState�� �ȴٸ� sound�� ������ �ǰ�
// AttackState�� �ȴٸ� Player�� ������ ��� �ȴ�.
// ��Ե� �����ϴ�.��� ��.
// enemy�� ��� �Ѿ˵鸸 LayerMask�� �༭ �ذ��غ���.
// ���
// 1. overlapSphare���� ���Ͱ� �����ϴ� �Ѿ� Layer�� �����Ѵ�.
// 2. �Ѿ��� �������� �� sm.curstate != AttackState��� AttackState�� �ȴ�.