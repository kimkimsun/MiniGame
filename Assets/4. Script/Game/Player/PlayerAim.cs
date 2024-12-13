using InterfaceManager;
using StarterAssets;
using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAim : MonoBehaviour, IAttackable
{
    [Header("Aim")]
    [SerializeField] private CinemachineVirtualCamera aimCam;
    [SerializeField] private Image                    thirdPersonaimImage;
    [SerializeField] private GameObject               aimObj;
    [SerializeField] private GameObject               bulletObj;
    [SerializeField] private TextMeshProUGUI          bulletCountText;
    [SerializeField] private Transform                bulletPoint;
    [SerializeField] private Transform                playerSpine;

    [Header("Hide")]
    [SerializeField] private Image                    interactiveImage;
    [SerializeField] private Image                    firstPersonaimImage;
    [SerializeField] private TextMeshProUGUI          interactiveText;
    [SerializeField] private CinemachineVirtualCamera firstPersonCamera;
    [SerializeField] private Transform                firstControllerCameraTarget;


    [Header("Sound")]
    [SerializeField] private GameObject               shootSound;
    [SerializeField] private GameObject               ReloadSound;


    private StarterAssetsInputs   input;
    private GameObject            bulletParticle;
    private ThirdPersonController controller;
    private CharacterController   characterController;
    private Animator              anim;
    private GameManager           gmInstance;
    private UIManager             umInstance;    
    private AudioManager          amInstance;
    private Color                 findHitableColor;
    private Color                 originColor;
    private Vector3               screenCenter;
    private LayerMask             playerLayer;
    private Ray                   ray;
    private RaycastHit            hit;
    private IEnumerator           shootingCo;
    private IHitable              enemy;
    private Coroutine             shootingCoroutine;
    private float                 aimObjDistance;
    private float                 maxDistance;
    private float                 rotationSpeed;
    private float                 fireRate;
    private int                   currentBullet;
    private int                   power;
    private int                   layerMask;
    private bool                  isShooting;
    private bool                  isHide;
    private string                hideType;


    public int Power 
    {
        get => power;
        set
        {
            power = value;
        }
    }
    public Transform FirstControllerCameraTarget
    {
        get => firstControllerCameraTarget;
        set => firstControllerCameraTarget = value;
    }

    private void Start()
    {
        gmInstance =            GameManager.Instance;
        umInstance =            UIManager.Instance;
        amInstance =            AudioManager.Instance;
        shootingCo =            ShootingCo();
        screenCenter =          new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);
        fireRate =              0.1f;
        aimObjDistance =        20f;
        rotationSpeed =         120f;
        currentBullet =         30;
        power =                 10;
        maxDistance =           6;
        playerLayer =           1 << 7;
        layerMask =             ~playerLayer;
        characterController =   GetComponent<CharacterController>();
        input =                 GetComponent<StarterAssetsInputs>();
        controller =            GetComponent<ThirdPersonController>();
        anim =                  GetComponent<Animator>();

        gmInstance.CreatePool(bulletObj, 5);
        amInstance.CreateSoundPool(shootSound,5);

        originColor = thirdPersonaimImage.color;
        findHitableColor = Color.red;
    }
    void Update()
    {
        AimCheck();
        HideAimCheck();
    }

    private void AimCheck()
    {
        if (input.reload && !controller.isReload)
        {
            StopCoroutine(shootingCo);
            amInstance.PlaySound(ReloadSound, this.transform.position);
            input.reload = false;
            AimControll(false);
            anim.SetLayerWeight(1,1);
            anim.SetTrigger("Reload");
            controller.isReload = true;
            currentBullet = 30;
            BulletCountUpdate(currentBullet);
        }

        if (controller.isReload)
        {
            input.reload = false;
            return;
        }

        if (input.aim)
        {
            AimControll(true);
            anim.SetLayerWeight(1,1);
            Transform camTransform = Camera.main.transform;
            Vector3 targetPosition = Vector3.zero;
            if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, Mathf.Infinity, layerMask))
            {
                aimObj.transform.position = hit.point;
                targetPosition = hit.point;

                if (hit.collider.gameObject.TryGetComponent<IHitable>(out IHitable obj))
                {
                    thirdPersonaimImage.color = findHitableColor;
                    enemy = obj;
                }
                else
                {
                    enemy = null;
                    thirdPersonaimImage.color = originColor;
                }
            }
            else
            {
                targetPosition = camTransform.position + camTransform.forward * aimObjDistance;
                aimObj.transform.position = camTransform.position + camTransform.forward * aimObjDistance;
            }
            Vector3 targetAim = targetPosition;
            targetAim.y = transform.position.y;
            Vector3 aimDir = (targetAim - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * rotationSpeed);
            if (input.shoot)
            {
                anim.SetBool("Shoot", true);
                if (isShooting)
                {
                    StartCoroutine(shootingCo);
                    isShooting = false;
                }
            }
            else
            {
                anim.SetBool("Shoot", false);
                StopCoroutine(shootingCo);
                isShooting = true;
            }
        }
        else
        {
            StopCoroutine(shootingCo);
            AimControll(false);
            anim.SetLayerWeight(1, 0);
        }
    }
    IEnumerator ShootingCo()
    {
        while(true)
        {
            if (enemy != null) enemy.Hp -= power;
            amInstance.PlaySound(shootSound, this.transform.position);
            currentBullet--;
            BulletCountUpdate(currentBullet);
            bulletParticle = gmInstance.GetFromPool(bulletObj);
            bulletParticle.transform.position = bulletPoint.position;
            yield return new WaitForSeconds(fireRate);
            gmInstance.ReturnToPool(bulletParticle);
            if (currentBullet == 0)
            {
                input.reload = true;
            }
            if (input.cursorInputForLook)
            {
                controller._cinemachineTargetPitch -= input.crouch ? 0.75f : 1.5f;
            }
        }
    }
    private void AimControll(bool isCheck)
    {
        aimCam.gameObject.SetActive(isCheck);
        thirdPersonaimImage.gameObject.SetActive(isCheck);
        controller.isAimMove = isCheck;
    }
    public void Reload()
    {
        controller.isReload = false;
        anim.SetLayerWeight(1, 0);
    }
    private void BulletCountUpdate(int current)
    {
        umInstance.BulletCountUpdate(current);
    }

    private void HideAimCheck()
    {
        if (isHide)
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, firstPersonaimImage.transform.position);
            firstPersonaimImage.transform.position = mousePos; // 에임 이미지 위치를 마우스 위치로 설정

            ray = Camera.main.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out hit)) // Ray가 물리적 충돌체와 교차하면
            {
                if(hit.collider.TryGetComponent<IHitable>(out IHitable obj))    enemy = obj;
                else enemy = null;
            }

            if (input.hide)
            {
                umInstance.ResetAll();
                input.hide = false;
                Hide(false);
                input.hideRegulate = false;
            }
            if (input.shoot)
            {
                if (input.reload) return;
                if (isShooting)
                {
                    shootingCoroutine = StartCoroutine(ShootingCo()); // 코루틴 시작
                    isShooting = false;
                }
            }
            else
            {
                if (shootingCoroutine != null)
                {
                    StopCoroutine(shootingCoroutine); // 실행 중인 코루틴 중단
                    shootingCoroutine = null; // 레퍼런스 초기화
                }
                isShooting = true;
            }
        }
        else
        {
            ray = Camera.main.ScreenPointToRay(screenCenter);
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * maxDistance, Color.red);
            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                if (hit.transform.TryGetComponent<IInteractive>(out IInteractive hitResult) &&
                    controller.Grounded && input.IsDefaultState() && !isHide)
                {
                    InteractiveUISet(true);
                    interactiveImage.sprite = hitResult.KeyCodeEImg;
                    interactiveText.text = hitResult.InteractiveText;
                    if (input.hide)
                    {
                        input.hide = false;
                        input.hideRegulate = false;
                        hideType = hitResult.HideType.ToString();
                        transform.position = hitResult.FixTrans.position;
                        transform.forward = -hitResult.FixTrans.forward;
                        firstControllerCameraTarget.transform.position = hitResult.CameraTrans.transform.position;
                        Hide(true);
                    }
                }
                else
                {
                    interactiveText.text = "";
                    InteractiveUISet(false);
                }
            }
            else
            {
                interactiveText.text = "";
                InteractiveUISet(false);
            }
        }
    }
    private void InteractiveUISet(bool show)
    {
        interactiveImage.gameObject.SetActive(show);
        interactiveText.gameObject.SetActive(show);
    }

    public void Hide(bool show)
    {
        input.cursorInputForLook = !show;
        characterController.enabled = !show;
        firstPersonCamera.gameObject.SetActive(show);
        firstPersonaimImage.gameObject.SetActive(show);
        thirdPersonaimImage.gameObject.SetActive(!show);
        Cursor.visible = !show;
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
        anim.SetBool(hideType, show);
        isHide = show;
        StartCoroutine(HideCo(show));
    }
    IEnumerator HideCo(bool show)
    {
        if (show)
        {
            InteractiveUISet(!show);
            yield return new WaitForSeconds(2f);
            Camera.main.cullingMask &= ~(playerLayer);
            umInstance.ArrowUISet(show,hideType);
            input.hideRegulate = true;
        }
        else
        {
            Camera.main.cullingMask |= (playerLayer);
            umInstance.ArrowUISet(show, hideType);
            yield return new WaitForSeconds(2f);
            InteractiveUISet(!show);
            input.hideRegulate = true;
        }
    }
}