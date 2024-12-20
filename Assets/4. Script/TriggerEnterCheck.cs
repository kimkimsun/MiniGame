using UnityEngine;

public class TriggerEnterCheck : MonoBehaviour
{
    private UIManager umInstance;
    private bool isFirstCheck;
    private void Start()
    {
        umInstance = UIManager.Instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isFirstCheck) return;
        else if (other.TryGetComponent<Player>(out Player player))
        {
            isFirstCheck = true;
            umInstance.Interactive();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other != null)
        {
            isFirstCheck = false;
            umInstance.ExitTrigger();
        }
    }
}
