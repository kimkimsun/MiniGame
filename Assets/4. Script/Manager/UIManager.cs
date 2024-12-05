using TMPro;
using UnityEngine;

public class UIManager : SingleTon<UIManager>
{
    public TextMeshProUGUI currentBullet;
    public TextMeshProUGUI maxBullet;

    public void BulletCountUpdate(int current)
    {
        currentBullet.text = current.ToString();
    }
}
