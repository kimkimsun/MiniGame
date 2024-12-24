using UnityEngine;
using TMPro;
using System.Runtime.InteropServices;

public class WebSocketHandler : MonoBehaviour
{
    public TextMeshProUGUI test;
    public int score;
    public string userName;
    [DllImport("__Internal")]
    private static extern void SendScore(string userName, int score);
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //SendScore(score);
        }
    }

}