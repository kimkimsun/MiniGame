using UnityEngine;
using InterfaceManager;
public class TestHitable : MonoBehaviour, IHitable
{
    float hp = 10;

    public float Hp
    {
        get => hp;
        set
        {
            hp = value;
            if (hp <= 0)
            {
                Debug.Log("À¸¾Ç");
            }
        }
    }
}
