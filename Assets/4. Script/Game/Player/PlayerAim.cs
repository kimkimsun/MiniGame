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
    [SerializeField] private Image                    aimImage;
    [SerializeField] private GameObject               aimObj;
    [SerializeField] private Transform                bulletPoint;
    [SerializeField] private GameObject               bulletObj;
    [SerializeField] private TextMeshProUGUI          bulletCountText;

    [Header("Sound")]
    [SerializeField] private GameObject               shootSound;
    [SerializeField] private GameObject               ReloadSound;


    private StarterAssetsInputs   input;
    private GameObject            bulletParticle;
    private ThirdPersonController controller;
    private Animator              anim;
    private GameManager           gmInstance;
    private UIManager             umInstance;    
    private AudioManager          amInstance;
    private Color                 findHitableColor;
    private Color                 originColor;
    private IEnumerator           shootingCo;
    private IHitable              enemy;
    private float                 aimObjDistance;
    private float                 rotationSpeed;
    private float                 fireRate;
    private int                   currentBullet;
    private int                   power;
    private bool                  isShooting;

    public int Power 
    {
        get => power;
        set
        {
            power = value;
        }
    }

    private void Start()
    {
        gmInstance = GameManager.Instance;
        umInstance = UIManager.Instance;
        amInstance = AudioManager.Instance;

        shootingCo = ShootingCo();
        fireRate = 0.2f;
        aimObjDistance = 20f;
        rotationSpeed = 120f;
        currentBullet = 30;
        power = 10;
        input = GetComponent<StarterAssetsInputs>();
        controller = GetComponent<ThirdPersonController>();
        anim = GetComponent<Animator>();
        gmInstance.CreatePool(bulletObj, 5);
        amInstance.CreateSoundPool(shootSound,5);

        originColor = aimImage.color;
        findHitableColor = Color.red;
    }
    void Update()
    {
        AimCheck();
    }

    private void AimCheck()
    {
        if (input.reload)
        {
            StopCoroutine(shootingCo);
            amInstance.PlaySound(ReloadSound, this.transform.position);
            input.reload = false;
            if (controller.isReload) return;
            AimControll(false);
            anim.SetLayerWeight(1, 1);
            anim.SetTrigger("Reload");
            controller.isReload = true;
            currentBullet = 30;
            BulletCountUpdate(currentBullet);
        }

        if (controller.isReload)
        {
            return;
        }

        if (input.aim)
        {
            AimControll(true);
            anim.SetLayerWeight(1, 1);
            Transform camTransform = Camera.main.transform;
            RaycastHit hit;

            Vector3 targetPosition = Vector3.zero;
            if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, Mathf.Infinity))
            {
                aimObj.transform.position = hit.point;
                targetPosition = hit.point;
                if (hit.collider.gameObject.TryGetComponent<IHitable>(out IHitable obj))
                {
                    aimImage.color = findHitableColor;
                    enemy = obj;
                }
                else
                {
                    enemy = null;
                    aimImage.color = originColor;
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
                if (currentBullet == 0)
                {
                    input.reload = true;
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
            // 여기서 사운드 출력 하면 될 듯?
            currentBullet--;
            BulletCountUpdate(currentBullet);
            bulletParticle = gmInstance.GetFromPool(bulletObj);
            bulletParticle.transform.position = bulletPoint.position;
            yield return new WaitForSeconds(fireRate);
            gmInstance.ReturnToPool(bulletParticle);
        }
    }
    private void AimControll(bool isCheck)
    {
        aimCam.gameObject.SetActive(isCheck);
        aimImage.gameObject.SetActive(isCheck);
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
}
