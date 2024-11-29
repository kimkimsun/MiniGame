using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : SingleTon<UIManager>
{
    public Button GameStartBtn;

    [DllImport("__Internal")]
    private static extern void WebSocketSetting();
    
    public void GameStart()
    {
        WebSocketSetting();
        SceneManager.LoadScene("GameScene");
    }
}
