using UnityEngine;
using TMPro;
using System.Runtime.InteropServices;

public class WebSocketHandler : MonoBehaviour
{
    public TextMeshProUGUI test;
    public int score;
    [DllImport("__Internal")]
    private static extern void SendScore(int score);
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            score++;
            test.text += score.ToString();
            SendScore(score);
        }
    }

}