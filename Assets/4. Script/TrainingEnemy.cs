using InterfaceManager;
using System.Collections;
using UnityEngine;

public class TrainingEnemy : MonoBehaviour,IHitable
{
    private float       hp;
    private int         animatorDieId;
    private Animator    animator;
    private BoxCollider myCollider;
    
    public float Hp 
    { 
        get => hp; 
        set
        {
            hp = value;
            if (hp == 0)
            {
                animator.SetTrigger(animatorDieId);
                StartCoroutine(resurrectionCo());
                Hp = 30;
            }
        }
    }
    private void Start()
    {
        hp = 30;
        animatorDieId = Animator.StringToHash("Die");
        animator = GetComponent<Animator>();
        myCollider = GetComponent<BoxCollider>();
    }
    IEnumerator resurrectionCo()
    {
        myCollider.enabled = false;
        yield return new WaitForSeconds(2);
        transform.GetChild(0).gameObject.SetActive(false);
        yield return new WaitForSeconds(2);
        animator.SetTrigger(animatorDieId);
        transform.GetChild(0).gameObject.SetActive(true);
        myCollider.enabled = true;
    }
}
