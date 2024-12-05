using UnityEngine;

namespace InterfaceManager
{
    public interface IHitable
    {
        public int Hp
        {  get; set; }
    }
    public interface IAttackable
    {
        public int Power
        { get; set; }
    }
    public interface INode
    {
        public enum STATE
        {
            RUN,
            SUCCESS,
            FAIL
        }
        public INode.STATE Evaluate();
    }
    public interface IStateMachine
    {
        public void SetState(string name);
        public object GetOwner();
    }
}

