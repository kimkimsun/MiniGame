using UnityEngine;
using InterfaceManager;
public class TestHitable : MonoBehaviour, IHitable
{
    float hp = 100;

    public float Hp
    {
        get => hp;
        set
        {
            hp = value;
            if (hp < 0)
            {
                Debug.Log("����");
            }
        }
    }
}
