using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    GameManager gmInstance = GameManager.Instance;
    Coroutine setActiveCo;
    private void OnEnable()
    {
        setActiveCo = StartCoroutine(SetActiveCo());
    }

    private void OnCollisionEnter(Collision collision)
    {
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
