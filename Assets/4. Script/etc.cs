using UnityEngine;

public class etc : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
            Debug.Log("asd");
    }
}
