using InterfaceManager;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour, IAttackable
{
    GameManager gmInstance = GameManager.Instance;
    Coroutine setActiveCo;
    private int power;

    public int Power 
    { 
        get => power;
        set => power = value; 
    }

    private void OnEnable()
    {
        setActiveCo = StartCoroutine(SetActiveCo());
        power = 1;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent<IHitable>(out IHitable obj))
        {
            obj.Hp -= power;
        }
        gmInstance.ReturnToPool(gameObject);
        if (setActiveCo != null)
        {
            StopCoroutine(setActiveCo);
            setActiveCo = null;
        }
    }
    IEnumerator SetActiveCo()
    {
        yield return new WaitForSeconds(3);
        gmInstance.ReturnToPool(gameObject);
    }
}
