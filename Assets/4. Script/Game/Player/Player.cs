using TMPro;
using UnityEngine;
using InterfaceManager;
using StarterAssets;
using System.Collections;

public class Player : MonoBehaviour, IHitable
{
    #region º¯¼ö

    private IEnumerator                         wornOutCoroutine; 
    private StarterAssetsInputs                 input;
    private StateMachine<Player>                sm;
    private Animator                            anim;
    private float                               hp;
    private bool                                isRunning;
    #endregion

    #region ÇÁ·ÎÆÛÆ¼
    public float Hp
    {
        get => hp;
        set
        {
            hp = value;
            if (hp < 4 && isRunning)
            {
                StartCoroutine(wornOutCoroutine);
                isRunning = false;
            }
            else if (hp == 4)
            {
                StopCoroutine(wornOutCoroutine);
                isRunning = true;
            }
            switch (hp)
            {
                case 4:
                    sm.SetState("Idle");
                    break;
                case 3:
                    sm.SetState("Pain");
                    break;
                case 2:
                    sm.SetState("Exhaustion");
                    break;
                case 1:
                    sm.SetState("Dying");
                    break;
                case 0:
                    Debug.Log("¾ê Á×À½");
                    break;
            }
        }
    }
    public Animator Anim
    {
        get => anim;
        set => anim = value;
    }
    public StarterAssetsInputs Input
    {
        get => input;
        set => input = value;
    }
    #endregion
    private void Start()
    {
        hp =                4;
        wornOutCoroutine =  WornOutCo();
        input =             GetComponent<StarterAssetsInputs>();
        anim =              GetComponent<Animator>();
        sm =                new StateMachine<Player>();
        sm.owner =          this;
        isRunning =         true;


        sm.AddState("Idle", new PlayerIdleState());
        sm.AddState("Pain", new PlayerPainState());
        sm.AddState("Exhaustion", new PlayerExhaustionState());
        sm.AddState("Dying", new PlayerDyingState());
        sm.SetState("Idle");
    }
    private void Update()
    {
        sm.curState?.Update();
    }
    IEnumerator WornOutCo()
    {
        //float time = 0;
        //while (Hp <= 3.9)
        //{
        //    time += Time.deltaTime;
        //    Hp += time / 3f; // Ensure division is a float
        //    Debug.Log($"Time: {time}, Hp: {Hp}"); // Debugging
        yield return null;
        //}
        //Hp = 4;
        //Debug.Log("Hp is greater than 4. Coroutine finished.");
    }
}