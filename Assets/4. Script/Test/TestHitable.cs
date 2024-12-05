using UnityEngine;
using InterfaceManager;
public class TestHitable : MonoBehaviour, IHitable
{
    int hp = 100;

    public int Hp
    {
        get => hp; 
        set
        {
            hp = value;
            if(hp < 0)
            {
                Debug.Log("À¸¾Ç");
            }
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log(hp);
        }
    }
}
