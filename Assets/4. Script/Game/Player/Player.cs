using UnityEngine;
using InterfaceManager;
using StarterAssets;
using System.Collections;
public class Player : MonoBehaviour, IHitable
{
    #region 변수
    public  GameObject[]                        hitClips;
    public  Material                            playerMat;
    public  Material                            WeaponMat;
    private ThirdPersonController               thirdPersonController;
    private StarterAssetsInputs                 input;
    private StateMachine<Player>                sm;
    private Animator                            anim;
    private AudioManager                        amInstance;
    private Coroutine                           recoveryCo;
    private Coroutine                           resurrectyCo;
    private float                               hp;
    private float                               speed;
    private float                               sprintSpeed;
    private float                               weaponMatMinValue;
    private float                               weaponMatMaxValue;
    private float                               weaponMatGetValue;
    private float                               playerMatMinValue;
    private float                               playerMatMaxValue;
    private float                               playerMatGetValue;
    private float                               weaponMatSpeed;
    private float                               playerMatSpeed;
    private bool                                isHurt;
    #endregion

    #region 프로퍼티
    public bool IsHurt
    {
        get => isHurt;
        set => isHurt = value;
    }
    public float Speed
    {
        get => speed;
        set => speed = value;
    }
    public float SprintSpeed
    {
        get => sprintSpeed;
        set => sprintSpeed = value;
    }
    public float Hp
    {
        get => hp;
        set
        {
            hp = value;
            if(hp >= 4)
            {
                sm.SetState("Idle");
                if (recoveryCo != null)
                {
                    StopCoroutine(recoveryCo);
                    recoveryCo = null;
                }
            }
            else if(hp >= 3 && hp < 4)
                sm.SetState("Pain");
            else if (hp >= 2 && hp < 3)
                sm.SetState("Exhaustion");
            else if (hp >= 1 && hp < 2)
                sm.SetState("Dying");
            else
            {
                if(resurrectyCo == null)
                {
                    resurrectyCo = StartCoroutine(ResurrectyCo());
                }
            }
        }
    }
    public Animator Anim
    {
        get => anim;
        set => anim = value;
    }
    public StarterAssetsInputs _Input
    {
        get => input;
        set => input = value;
    }
    public ThirdPersonController _ThirdPersonController
    {
        get => thirdPersonController; 
        set => thirdPersonController = value;
    }

    

    #endregion
    private void Start()
    {
        hp =                        4;
        input =                     GetComponent<StarterAssetsInputs>();
        thirdPersonController =     GetComponent<ThirdPersonController>();
        anim =                      GetComponent<Animator>();
        amInstance =                AudioManager.Instance;
        sm =                        new StateMachine<Player>();
        sm.owner =                  this;
        weaponMatMinValue =         -0.15f;
        weaponMatMaxValue =         0.8f;
        playerMatMinValue =         0.1f;
        playerMatMaxValue =         2.1f;
        weaponMatGetValue =         weaponMatMaxValue;
        playerMatGetValue =         playerMatMaxValue;
        playerMatSpeed =            2f;
        weaponMatSpeed =            0.95f;

        sm.AddState("Idle", new PlayerIdleState());
        sm.AddState("Pain", new PlayerPainState());
        sm.AddState("Exhaustion", new PlayerExhaustionState());
        sm.AddState("Dying", new PlayerDyingState());
        sm.SetState("Idle");
    }
    private void Update()
    {
        sm.curState?.Update();
        if (Input.GetKeyDown(KeyCode.J)) Debug.Log(Hp);
    }
    public void Hit(IAttackable attackObj)
    {
        Hp -= attackObj.Power;
        int index = Random.Range(0, hitClips.Length);
        amInstance.PlaySound(hitClips[index], transform.position, 6);
        if (recoveryCo == null)
        {
            recoveryCo = StartCoroutine(RecoveryCo());
        }
    }
    IEnumerator RecoveryCo()
    {
        while (Hp < 4)
        {
            Hp += 0.05f;
            yield return new WaitForSeconds(0.15f);
        }
        Hp = 4;
        if(recoveryCo != null)
        {
            StopCoroutine(recoveryCo);
            recoveryCo = null;
        }
    }
    IEnumerator ResurrectyCo()
    {
        UIManager.Instance.sacrifice += 1;
        gameObject.layer = 10;
        while (weaponMatGetValue > weaponMatMinValue && playerMatGetValue > playerMatMinValue)
        {
            Hp = 4;
            weaponMatGetValue -= Time.deltaTime * weaponMatSpeed;
            playerMatGetValue -= Time.deltaTime * playerMatSpeed;
            playerMat.SetFloat("_Split_Value", playerMatGetValue);
            WeaponMat.SetFloat("_Split_Value", weaponMatGetValue);
            yield return null;
        }
        playerMat.SetFloat("_Split_Value", playerMatMinValue);
        WeaponMat.SetFloat("_Split_Value", weaponMatMinValue);
        while (weaponMatGetValue < weaponMatMaxValue && playerMatGetValue < playerMatMaxValue)
        {
            Hp = 4;
            weaponMatGetValue += Time.deltaTime * weaponMatSpeed;
            playerMatGetValue += Time.deltaTime * playerMatSpeed;
            playerMat.SetFloat("_Split_Value", weaponMatGetValue);
            WeaponMat.SetFloat("_Split_Value", playerMatGetValue);
            yield return null;
        }
        playerMat.SetFloat("_Split_Value", playerMatMaxValue);
        WeaponMat.SetFloat("_Split_Value", weaponMatMaxValue);
        yield return new WaitForSeconds(3);
        gameObject.layer = 7;
        resurrectyCo = null;
    }
}