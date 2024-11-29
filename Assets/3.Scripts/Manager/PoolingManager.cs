using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : SingleTon<PoolingManager>
{
    public Queue<Bullet> bullets = new Queue<Bullet>();
    [SerializeField] private Bullet bullet;
    private int poolingCount;
    private void Start()
    {
        poolingCount = 10;
        PoolingSetting();
    }

    private void PoolingSetting()
    {
        for (int i = 0; i < poolingCount; i++)
        {
            EnqueueBullet(Instantiate(bullet));
        }
    }

    public void EnqueueBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
        bullets.Enqueue(bullet);
        bullet.gameObject.transform.parent = this.transform;
    }
    public Bullet DequeueBullet()
    {
        if(bullets.Count > 0)
        {
            Bullet bullet = bullets.Dequeue();
            bullet.gameObject.SetActive(true);
            Debug.Log("미친 듯이 들어옴");
            return bullet;
        }
        PoolingSetting(); // 큐가 모두 활성화된 경우 Pool 추가
        return DequeueBullet();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log(bullets.Count);
        }
    }
}
