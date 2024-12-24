using TMPro;
using UnityEngine;

public class Test : MonoBehaviour
{
    public TextMeshProUGUI[] rankNameArray = new TextMeshProUGUI[4];
    public TextMeshProUGUI[] rankScoreArray = new TextMeshProUGUI[4];

    public void RankSetting(string combinedData)
    {
        // '|'로 결합된 문자열을 분리
        string[] data = combinedData.Split('|');
        string[] nameArray = data[0].Split(',');
        string[] scoreArray = data[1].Split(',');

        // 이름과 점수 처리
        for (int i = 0; i < nameArray.Length; i++)
        {
            if (nameArray[i] != null)
            {
                rankNameArray[i].text = nameArray[i];
                rankScoreArray[i].text = scoreArray[i];
            }
        }
    }
}

