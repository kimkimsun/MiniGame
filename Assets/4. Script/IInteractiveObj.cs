using UnityEngine;
using InterfaceManager;
public class IInteractiveObj : MonoBehaviour, IInteractive
{
    private UIManager umInstance;
    public void Interactive()
    {
        umInstance.Interactive();
    }

    void Start()
    {
        umInstance = UIManager.Instance;    
    }
}
