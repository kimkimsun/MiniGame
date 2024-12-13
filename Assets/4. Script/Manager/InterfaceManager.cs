using UnityEngine;
using UnityEngine.UI;

namespace InterfaceManager
{
    public enum Hide_Type
    {
        Horizontal,
        Vertical,
    }
    public interface IHitable
    {
        public float Hp
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
    public interface IInteractive
    {
        public Transform CameraTrans
        { get; set; }
        public Transform FixTrans 
        {  get; set; }
        public Sprite KeyCodeEImg
            { get; set; }
        public string InteractiveText
        { get; set; }
        public Hide_Type HideType 
        { get; set; }
        public void Interactive();
    }
}

