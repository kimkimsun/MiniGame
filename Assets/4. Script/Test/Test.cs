using TMPro;
using UnityEngine;

public class Test : MonoBehaviour
{
    int nameIndex;
    int rankIndex;
    public TextMeshProUGUI[] rankNameArray = new TextMeshProUGUI[2];
    public TextMeshProUGUI[] rankScoreArray = new TextMeshProUGUI[2];
    public int hp = 100;

    // ���� ��ȭ�� ������ ȣ���ϴ� ������Ƽ
    public int Hp
    {
        get => hp;
        set
        {
            if (hp != value)
            {
                Debug.Log("�޶�� !!");
                Debug.Log("111" + hp);
                hp = value;
                Debug.Log(hp);
            }
        }
    }
    public void RankSetting(string combinedData)
    {
        // '|'�� ���յ� ���ڿ��� �и�
        string[] data = combinedData.Split('|');
        string[] nameArray = data[0].Split(',');
        string[] scoreArray = data[1].Split(',');

        // �̸��� ���� ó��
        for (int i = 0; i < nameArray.Length; i++)
        {
            rankNameArray[i].text = nameArray[i];
            rankScoreArray[i].text = scoreArray[i];
        }
    }
}

