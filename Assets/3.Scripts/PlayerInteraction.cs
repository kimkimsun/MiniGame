using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("GunHole")]
    [SerializeField] private Transform muzzle;

    private Vector3 direction;
    private Ray     muzzleRay;

    void Start()
    {
        direction = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
        muzzleRay = new Ray(muzzle.position, direction);
    }
    void Update()
    {
        // 쏘는 거
        // transform.position += muzzleRay.direction * 50 * Time.deltaTime;
        // ↑ 총알이 이동하는 거니까 잘 생각
    }
}
