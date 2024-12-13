using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

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
    GameManager gmInstance = GameManager.Instance;
    GameObject bullet;
    float fireRate = 1f;
    float speed = 25f;
    bool isShooting;
    public MiniEnemyStrategy(Enemy owner)
    {
        this.owner = owner;
        isShooting = true;
        Init();
    }
    public override void Init()
    {
        bullet = owner.Bullet;
        gmInstance.CreatePool(owner.Bullet, 10);
        owner.SoundDetectionRange = 20f;
        owner.LookDetectionRange = 15f;
    }
    public override void Attack()
    {
        owner.AttackCoroutine = owner.StartCoroutine(FireCo());
    }
    IEnumerator FireCo()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireRate);
            bullet = gmInstance.GetFromPool(owner.Bullet);
            bullet.transform.position = owner.BulletHole.transform.position;
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.angularVelocity = Vector3.zero; // 초기화(필요하다면)
            rb.AddForce((owner.PlayerTrans.position - owner.BulletHole.transform.position).normalized * speed, ForceMode.Impulse);
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
    [SerializeField] private ENEMY_TYPE     type;
    [SerializeField] private Transform[]    nextPos;
    [SerializeField] private Transform      headTrans;
    [SerializeField] private GameObject     bullet;
    [SerializeField] private GameObject     bulletHole;
    [SerializeField] private Animator       spineAnimator;
    [SerializeField] private Animator       weaponAnimator;

    private EnemyStrategy       enStrategy;
    private StateMachine<Enemy> sm;
    private Collider[]          soundCol;
    private Collider[]          lookCol;
    private Transform           soundTrans;
    private Transform           playerTrans;
    private NavMeshAgent        agent;
    private GameObject          obj;
    private LayerMask           heardTargetLayerMask;
    private LayerMask           lookTargetLayerMask;
    private Vector3             direction;
    private Coroutine           attackCoroutine;
    private IEnumerator         attackKeepCoroutine;
    private float               soundDetectionRange;
    private float               lookDetectionRange;
    private float               maxDistance;
    private int                 spineRunAnimId;
    private int                 spineDieAnimId;
    private int                 weaponAnimId;
    private bool                isHearable;
    private bool                isPlayerCheck;
    private bool                isAttack;
    private bool                isKeep;

    #endregion

    #region 프로퍼티

    public Transform PlayerTrans
    {
        get => playerTrans;
        set => playerTrans = value;
    }
    public GameObject BulletHole
    {
        get => bulletHole;
        set => bulletHole = value;
    }
    public Animator SpineAnimator
    {
        get => spineAnimator;
        set => spineAnimator = value;
    }
    public Animator WeaponAnimator
    {
        get => weaponAnimator;
        set => weaponAnimator = value;
    }
    public int SpineRunAnimId
    {
        get => spineRunAnimId;
        set => spineRunAnimId = value;
    }
    public int SpineDieAnimId
    {
        get => spineDieAnimId;
        set => spineDieAnimId = value;
    }
    public int WeaponAnimId
    {
        get => weaponAnimId;
        set => weaponAnimId = value;
    }
    public bool IsKeep
    {
        get => isKeep;
        set => isKeep = value;
    }
    public IEnumerator AttackKeepCoroutine
    {
        get => attackKeepCoroutine; 
        set => attackKeepCoroutine = value;
    }
    public Vector3 Direction
    {
        get => direction;
        set => direction = value;
    }
    public Coroutine AttackCoroutine
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
        agent = GetComponent<NavMeshAgent>();
        heardTargetLayerMask = 1 << 6;
        lookTargetLayerMask = 1 << 7;
        sm = new StateMachine<Enemy>();
        sm.owner = this;
        maxDistance = 15f;
        //정의
        spineRunAnimId = Animator.StringToHash("Run");
        spineDieAnimId = Animator.StringToHash("Die");
        weaponAnimId = Animator.StringToHash("EnemyFire");

        sm.AddState("Wander", new EnemyWanderState());
        sm.AddState("Attack", new EnemyAttackState());
        sm.AddState("Chase", new EnemyChaseState());
        sm.AddState("KeepAttack", new EnemyKeepAttackState());
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
        HandleStateChange();
        if(Input.GetKeyDown(KeyCode.Z)) Debug.Log(sm.curState);
    }
    void HandleStateChange()
    {
        if (lookCol.Length > 0) // 내 시야 범위에는 들어왔다.
        {
            RaycastHit hit;
            direction = ((lookCol[0].transform.position) - transform.position).normalized; // 그래서 내 방향을 설정한다
            Debug.DrawLine(headTrans.position, headTrans.position + (direction * maxDistance), Color.red);
            if (Physics.Raycast(headTrans.position, direction * maxDistance, out hit, maxDistance)) // 그 방향대로 들어온놈이랑 선을 이어본다
            {
                if (CheckInLayerMask(hit.collider.gameObject.layer)) // 이었더니 그 놈이 내가 찾는 놈이고 장애물이 없다
                {
                    PlayerTrans = hit.collider.gameObject.transform;
                    sm.SetState("Attack");
                }
                else
                {
                    sm.SetState("KeepAttack");
                }
            }
        }
        else if (sm.curState is EnemyAttackState)
        {
            sm.SetState("KeepAttack");
        }
        else if (soundCol.Length > 0 && IsKeep) // soundCol이 비어있지 않으면 추적 상태로
        {
            soundTrans = soundCol[0].transform;
            sm.SetState("Chase");
        }
        else if (lookCol.Length == 0 && soundCol.Length == 0 && IsKeep) // lookCol과 soundCol이 모두 비어 있으면 방황 상태로
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