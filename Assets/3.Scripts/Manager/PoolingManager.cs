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
            Debug.Log("��ģ ���� ����");
            return bullet;
        }
        PoolingSetting(); // ť�� ��� Ȱ��ȭ�� ��� Pool �߰�
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
