using System.Collections.Generic;
using InterfaceManager;

public abstract class State
{
    public IStateMachine sm = null;
    public virtual void Init(IStateMachine sm)
    {
        this.sm = sm;
    }
    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
public class StateMachine<T> : IStateMachine where T : class
{
    
    public T owner = null;
    public State curState = null;

    Dictionary<string, State> stateDic = null;
    public StateMachine()
    {
        stateDic = new Dictionary<string, State>();
    }
    public void AddState(string name, State state)
    {
        if (stateDic.ContainsKey(name))
            return;

        state.Init(this);
        stateDic.Add(name, state);
    }
    public object GetOwner()
    {
        return owner;
    }
    public void SetState(string name)
    {
        if (stateDic.ContainsKey(name))
        {
            if (curState == stateDic[name]) return;
            if (curState != null)
                curState.Exit();

            curState = stateDic[name];
            curState.Enter();
        }
    }
    public void Update()
    {
        curState.Update();
    }
}