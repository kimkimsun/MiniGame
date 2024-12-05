using TMPro;
using UnityEngine;

public class Test : MonoBehaviour
{
    int nameIndex;
    int rankIndex;
    public TextMeshProUGUI[] rankNameArray = new TextMeshProUGUI[2];
    public TextMeshProUGUI[] rankScoreArray = new TextMeshProUGUI[2];
    public int hp = 100;

    // 값이 변화할 때마다 호출하는 프로퍼티
    public int Hp
    {
        get => hp;
        set
        {
            if (hp != value)
            {
                Debug.Log("달라요 !!");
                Debug.Log("111" + hp);
                hp = value;
                Debug.Log(hp);
            }
        }
    }
    public void RankSetting(string combinedData)
    {
        // '|'로 결합된 문자열을 분리
        string[] data = combinedData.Split('|');
        string[] nameArray = data[0].Split(',');
        string[] scoreArray = data[1].Split(',');

        // 이름과 점수 처리
        for (int i = 0; i < nameArray.Length; i++)
        {
            rankNameArray[i].text = nameArray[i];
            rankScoreArray[i].text = scoreArray[i];
        }
    }
}

