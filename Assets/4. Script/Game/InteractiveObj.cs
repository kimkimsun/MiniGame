using UnityEngine;
using InterfaceManager;
using UnityEngine.UI;
public class InteractiveObj : MonoBehaviour, IHideable
{
    [SerializeField] private Transform cameraTrans;
    [SerializeField] private Transform fixTrans;
    [SerializeField] private Sprite    keyCodeEImg;
    [SerializeField] private string   interactiveText;
    [SerializeField] private Hide_Type hideType;
    public Sprite KeyCodeEImg
    {
        get => keyCodeEImg;
        set => keyCodeEImg = value;
    }
    public string InteractiveText 
    {
        get => interactiveText;
        set => interactiveText = value;
    }
    public Transform FixTrans 
    { 
        get => fixTrans; 
        set => fixTrans = value; 
    }
    public Transform CameraTrans
    {
        get => cameraTrans;
        set => cameraTrans = value;
    }
    public Hide_Type HideType 
    {
        get => hideType;
        set => hideType = value;
    }
}
