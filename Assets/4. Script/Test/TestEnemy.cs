using UnityEngine;
using UnityEngine.AI;
public enum TESTType
{
    WALK,
    RUN,
}

public class TestEnemy : MonoBehaviour
{
    public GameObject testObj;
    NavMeshAgent agent;
    Animator animator;
    [SerializeField] TESTType type;
    
    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(testObj.transform.position);
        if (type == TESTType.RUN)
        {
            agent.speed = 5;
            animator.SetBool("Run", true);
            
        }
    }
}
