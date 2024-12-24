using InterfaceManager;
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
    public abstract void Hit();
    public abstract void Die();
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

    public override void Hit()
    {
    }

    public override void Die()
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

    public override void Hit()
    {
    }

    public override void Die()
    {
    }
}
public class MiniEnemyStrategy : EnemyStrategy
{
    GameManager gmInstance = GameManager.Instance;
    GameObject bullet;
    float fireRate = 1f;
    float speed = 40f;
    float time = 0;
    float matValue;
    float dieSpeed = 1.7f;
    public MiniEnemyStrategy(Enemy owner)
    {
        this.owner = owner;
        Init();
    }
    public override void Init()
    {
        bullet = owner.Bullet;
        gmInstance.CreatePool(owner.Bullet, 10);
        owner.SoundDetectionRange = 15f;
        owner.LookDetectionRange = 20f;
        owner.MaxValue = 1.9f;
        owner.MinValue = -1.6f;
        matValue = owner.MaxValue;
        owner.CopyMat.SetFloat("_Split_Value", matValue);
    }
    public override void Attack()
    {
        owner.AttackCoroutine = owner.StartCoroutine(FireCo());
    }

    public override void Hit()
    {
        if (owner.HitCoroutine == null)
        {
            owner.HitCoroutine = owner.StartCoroutine(HitCo());
        }
        else
        {
            time = 0;
        }
    }
    public override void Die()
    {
        if(owner.DieCoroutine == null)
        {
            owner.DieCoroutine = owner.StartCoroutine(DieCo());
        }
    }
    IEnumerator DieCo()
    {
        owner.SpineAnimator.SetBool(owner.SpineDieAnimId, true);
        owner.Agent.enabled = false;
        while(matValue > owner.MinValue)
        {
            matValue -= Time.deltaTime * dieSpeed;
            owner.CopyMat.SetFloat("_Split_Value", matValue);
            yield return null;
        }
        UIManager.Instance.kills += 1;
        owner.DieCoroutine = null;
        owner.gameObject.SetActive(false);
    }
    IEnumerator FireCo()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireRate);
            Vector3 bulletDirection = (owner.PlayerTrans.position + owner.CenterTarget - owner.BulletHole.transform.position).normalized;
            bullet = gmInstance.GetFromPool(owner.Bullet);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.angularVelocity = Vector3.zero; // 초기화(필요하다면)
            bullet.transform.position = owner.BulletHole.transform.position;
            rb.AddForce(bulletDirection * speed, ForceMode.Impulse);
        }
    }
    IEnumerator HitCo()
    {
        owner.SoundDetectionRange = 20f;
        owner.LookDetectionRange = 30f;
        time = 0;
        owner.HeadLight.color = Color.red;
        owner.Agent.speed = 5f;
        while (time < 4)
        {
            time += Time.deltaTime;
            owner.HeadLight.transform.rotation *= Quaternion.Euler(0, 180 * Time.deltaTime, 0);
            yield return null;
        }
        owner.HeadLight.color = Color.white;
        owner.HeadLight.transform.rotation = Quaternion.Euler(0, 90, 0);
        owner.Agent.speed = 3.5f;
        owner.HitCoroutine = null;
        owner.SoundDetectionRange = 15f;
        owner.LookDetectionRange = 20f;
    }

}

public enum ENEMY_TYPE
{
    BOSS,
    HORROR,
    MINI,
}


public class Enemy : MonoBehaviour, IHitable
{
    #region 변수
    [Header("Serializefield")]
    [SerializeField] private ENEMY_TYPE     type;
    [SerializeField] private Transform[]    nextPos;
    [SerializeField] private Transform      headTrans;
    [SerializeField] private Light          headLight;
    [SerializeField] private GameObject     bullet;
    [SerializeField] private GameObject     bulletHole;
    [SerializeField] private Animator       spineAnimator;
    [SerializeField] private Animator       weaponAnimator;
    [SerializeField] private Material       enemyMat;
    [SerializeField] private Renderer[]     enemyRenderer;

    private EnemyStrategy       enStrategy;
    private StateMachine<Enemy> sm;
    private NavMeshAgent        agent;
    private Collider[]          soundCol;
    private Collider[]          lookCol;
    private Coroutine           hitCoroutine;
    private Coroutine           attackCoroutine;
    private Coroutine           dieCoroutine;
    private Transform           soundTrans;
    private Transform           playerTrans;
    private GameObject          obj;
    private Material            copyMat;
    private LayerMask           heardTargetLayerMask;
    private LayerMask           lookTargetLayerMask;
    private Vector3             direction;
    private Vector3             centerTarget;
    private IEnumerator         attackKeepCoroutine;
    private float               soundDetectionRange;
    private float               lookDetectionRange;
    private float               maxDistance;
    private float               hp;
    private float               time;
    private float               minValue;
    private float               maxValue;
    private int                 spineRunAnimId;
    private int                 spineDieAnimId;
    private int                 weaponAnimId;
    private int                 idleAnimId;
    private bool                isKeep;

    #endregion

    #region 프로퍼티

    public Vector3 CenterTarget
    {
        get => centerTarget; 
        set => centerTarget = value;
    }
    public float MaxValue
    {
        get => maxValue;
        set => maxValue = value;
    }
    public float MinValue
    {
        get => minValue; 
        set => minValue = value;
    }
    public Material CopyMat
    {
        get => copyMat;
        set => copyMat = value;
    }
    public Material EnemyMat
    {
        get => enemyMat;
        set => enemyMat = value;
    }
    public Coroutine DieCoroutine
    {
        get => dieCoroutine; 
        set => dieCoroutine = value;
    }
    public Light HeadLight
    {
        get => headLight; 
        set => headLight = value;
    }
    public float _Time
    {
        get => time;
        set => time = value;
    }
    public Transform HeadTreans
    {
        get => headTrans;
        set => headTrans = value;
    }
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
    public int IdleAnimId
    {
        get => idleAnimId;
        set => idleAnimId = value;
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
    public Coroutine HitCoroutine
    {
        get => hitCoroutine;
        set => hitCoroutine = value;
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
    public float Hp 
    {
        get => hp;
        set
        {
            if (hp != value)
            {
                Hit();
                hp = value;
            }
            if (hp <= 0)
            {
                Die();
            }
        }
    }
    #endregion
    protected virtual void Start()
    {
        //정의
        centerTarget = new Vector3(0, 0.4f, 0);
        heardTargetLayerMask =      1 << 6;
        lookTargetLayerMask =       1 << 7;
        agent =                     GetComponent<NavMeshAgent>();
        sm =                        new StateMachine<Enemy>();
        IsKeep =                    true;
        sm.owner =                  this;
        playerTrans =               null;
        soundTrans =                null;
        maxDistance =               15f;
        hp =                        80;
        copyMat = Instantiate(enemyMat);
        //정의
        spineRunAnimId =            Animator.StringToHash("Run");
        spineDieAnimId =            Animator.StringToHash("Die");
        weaponAnimId =              Animator.StringToHash("EnemyFire");
        idleAnimId =                Animator.StringToHash("Idle");
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
        for (int i = 0; i < enemyRenderer.Length; i++)
        {
            enemyRenderer[i].material = copyMat;
        }
    }
    void Update()
    {
        sm.curState?.Update();
        if (Input.GetKeyDown(KeyCode.X)) Hp -= 10;
        soundCol = Physics.OverlapSphere(transform.position, soundDetectionRange, heardTargetLayerMask);
        lookCol = Physics.OverlapSphere(transform.position, lookDetectionRange, lookTargetLayerMask);
        HandleStateChange();
    }
    void HandleStateChange()
    {
        Vector3 leftBoundary = Quaternion.Euler(0, -95, 0) * transform.forward * maxDistance;
        Vector3 rightBoundary = Quaternion.Euler(0, 95, 0) * transform.forward * maxDistance;

        Debug.DrawLine(transform.position, transform.position + leftBoundary, Color.green);
        Debug.DrawLine(transform.position, transform.position + rightBoundary, Color.green);
        if (lookCol.Length > 0) // 내 시야 범위에는 들어왔다.
        {
            RaycastHit hit;

            // 방향 벡터 계산
            direction = (lookCol[0].transform.position - transform.position).normalized;
                
            // 정면 벡터와의 각도를 계산
            float angle = Vector3.Dot(transform.forward, direction); // -1 ~ 1 사이 값
            if (angle > Mathf.Cos(95f * Mathf.Deg2Rad)) // 90도 시야 -> 각도 절반인 45도 사용
            {
                Debug.DrawLine(headTrans.position, headTrans.position + (direction * maxDistance), Color.red);
                if (Physics.Raycast(headTrans.position, direction, out hit, maxDistance)) // 그 방향대로 들어온놈이랑 선을 이어본다
                {
                    if (CheckInLayerMask(hit.collider.gameObject.layer)) // 이었더니 그 놈이 내가 찾는 놈이고 장애물이 없다
                    {
                        playerTrans = hit.collider.gameObject.transform;
                        sm.SetState("Attack");
                    }
                    else if(!CheckInLayerMask(hit.collider.gameObject.layer) && sm.curState is EnemyAttackState)
                    {
                        sm.SetState("KeepAttack");
                    }
                }
            }
        }
        else if (sm.curState is EnemyAttackState)
        {
            sm.SetState("KeepAttack");
        }
        if (soundCol.Length > 0 && isKeep && sm.curState is not EnemyAttackState) // soundCol이 비어있지 않으면 추적 상태로
        {
            soundTrans = soundCol[0].transform;
            sm.SetState("Chase");
        }
        else if (playerTrans == null && soundTrans == null && isKeep) // lookCol과 soundCol이 모두 비어 있으면 방황 상태로
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
    public void Hit()
    {
        enStrategy.Hit();
    }
    public void Die()
    {
        enStrategy.Die();
    }
    public void Hit(IAttackable attackObj)
    {
        Hp -= attackObj.Power;
    }
}