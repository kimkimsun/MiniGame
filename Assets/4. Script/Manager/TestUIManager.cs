using Unity.Cinemachine;
using UnityEngine;

[System.Serializable]
public class RowArray
{
    public GameObject[] row; //행에 들어갈 배열들
}

public class TestUIManager : MonoBehaviour
{

    public CinemachineCamera[] cinemachines = new CinemachineCamera[4];
    public GameObject[] ChangeUISet;
    public GameObject[] OriginUISet;
    public RowArray[] upArray;
    public RowArray[] downArray;
    public RowArray[] leftArray;
    public RowArray[] rightArray;

    private int index;
    //private int up;
    //private int down;
    //private int left;
    //private int right;

    private void Start()
    {
        //up = 0;
        //down = 1;
        //left = 2;
        //right = 3;
    }
    void Update()
    {
        MoveUI();
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log(leftArray[0].row[0].name); // left 1-1
            Debug.Log(leftArray[0].row[1].name); // left 1-2
            Debug.Log(leftArray[1].row[0].name); // left 2-1
        }
    }
    private void MoveUI()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            UISetting(true);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            UISetting(false);
        }
    }
    private void UISetting(bool right)
    {
        cinemachines[index].gameObject.SetActive(false);
        int setFalseIndex = index;
        index = right ? index - 1 : index + 1;
        if (index < 0) index = cinemachines.Length - 1;
        if (index > cinemachines.Length - 1) index = 0;
        cinemachines[index].gameObject.SetActive(true);
        int setTrueIndex = index;
        UIAnimation(setFalseIndex, setTrueIndex);
    }

    private void UIAnimation(int setFalseIndex,int setTrueIndex)
    {
        LeanTween.move(upArray[setFalseIndex].row[0], OriginUISet[0].transform.position, 1.2f).setEase(LeanTweenType.easeInBounce);
        LeanTween.move(leftArray[setFalseIndex].row[0], OriginUISet[1].transform.position, 1.2f).setEase(LeanTweenType.easeInBounce);
        LeanTween.move(leftArray[setFalseIndex].row[1], OriginUISet[2].transform.position, 1.2f).setEase(LeanTweenType.easeInBounce);
        LeanTween.move(rightArray[setFalseIndex].row[0], OriginUISet[3].transform.position, 1.2f).setEase(LeanTweenType.easeInBounce);
        LeanTween.move(rightArray[setFalseIndex].row[1], OriginUISet[4].transform.position, 1.2f).setEase(LeanTweenType.easeInBounce);
        LeanTween.move(downArray[setFalseIndex].row[0], OriginUISet[5].transform.position, 1.2f).setEase(LeanTweenType.easeInBounce);
        //upArray[setFalseIndex].row[0].gameObject.SetActive(false);
        //leftArray[setFalseIndex].row[0].gameObject.SetActive(false);
        //leftArray[setFalseIndex].row[1].gameObject.SetActive(false);
        //rightArray[setFalseIndex].row[0].gameObject.SetActive(false);
        //rightArray[setFalseIndex].row[1].gameObject.SetActive(false);
        //downArray[setFalseIndex].row[0].gameObject.SetActive(false);

        upArray[setTrueIndex].row[0].gameObject.SetActive(true);
        leftArray[setTrueIndex].row[0].gameObject.SetActive(true);
        leftArray[setTrueIndex].row[1].gameObject.SetActive(true);
        rightArray[setTrueIndex].row[0].gameObject.SetActive(true);
        rightArray[setTrueIndex].row[1].gameObject.SetActive(true);
        downArray[setTrueIndex].row[0].gameObject.SetActive(true);
        LeanTween.move(upArray[setTrueIndex].row[0], ChangeUISet[0].transform.position, 1.2f).setEase(LeanTweenType.easeOutBounce);
        LeanTween.move(leftArray[setTrueIndex].row[0], ChangeUISet[1].transform.position, 1.2f).setEase(LeanTweenType.easeOutBounce); 
        LeanTween.move(leftArray[setTrueIndex].row[1], ChangeUISet[2].transform.position, 1.2f).setEase(LeanTweenType.easeOutBounce); 
        LeanTween.move(rightArray[setTrueIndex].row[0], ChangeUISet[3].transform.position, 1.2f).setEase(LeanTweenType.easeOutBounce);
        LeanTween.move(rightArray[setTrueIndex].row[1], ChangeUISet[4].transform.position, 1.2f).setEase(LeanTweenType.easeOutBounce);
        LeanTween.move(downArray[setTrueIndex].row[0], ChangeUISet[5].transform.position, 1.2f).setEase(LeanTweenType.easeOutBounce);
    }
}
