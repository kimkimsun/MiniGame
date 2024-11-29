using UnityEngine;

public class GameManager : SingleTon<GameManager>
{
    [SerializeField] private Transform bulletPoint;
    [SerializeField]
    private GameObject bulletObj;
    private void Update()
    {
        Shooting();
    }
    public void Shooting()
    {
        Instantiate(bulletObj, bulletPoint.position, Quaternion.identity);
    }
}
