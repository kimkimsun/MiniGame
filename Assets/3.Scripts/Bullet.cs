using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float       moveSpeed;
    private float       time;
    private Vector3     direction;
    Ray                 cameraRay;

    void Start()
    {
        moveSpeed = 50f;
        direction = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
        cameraRay = new Ray(transform.position, direction);
    }
    private void Update()
    {
        time += Time.deltaTime;
        if (time > 3)
        {
            time = 0;
            PoolingManager.Instance.EnqueueBullet(this);
        }
        transform.position += cameraRay.direction * moveSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        time = 0;
        PoolingManager.Instance.EnqueueBullet(this);
    }
}
