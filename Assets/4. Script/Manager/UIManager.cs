using TMPro;
using UnityEngine;
using UnityEngine.UI;
using InterfaceManager;
using System.Collections;

public class UIManager : SingleTon<UIManager>
{
    public TextMeshProUGUI currentBullet;
    public TextMeshProUGUI maxBullet;
    public TextMeshProUGUI bulletCount;
    public TextMeshProUGUI interactiveText;
    public Image thirdPersonAimImage;
    public Image interactiveImage;
    public Image firstPersonAimImage;
    public Image leftArrowImg;
    public Image rightArrowImg;
    public Image upArrowImg;
    public Image downArrowImg;
    public Image[] infoImage;
    public PlayerAim player;
    public GameObject triggerEventObj;

    private Vector3 rotation;
    private Vector3 position;
    private Vector3 originPosition;
    private Vector3 originRotation;
    private Vector3 smallUiImageSet;
    private Vector3 bigUiImageSet;
    private Coroutine uiCoroutine;
    private bool plus = true;
    private bool minus = false;
    private int index;
    private int left;
    private int right;
    private int up;
    private int down;

    private void Start()
    {
        smallUiImageSet = Vector3.zero;
        bigUiImageSet = new Vector3(5.775f, 2.15f, 1);
    }
    public void BulletCountUpdate(int current)
    {
        currentBullet.text = current.ToString();
    }
    public void ArrowUISet(bool show, string hideType)
    {
        if (hideType == "Vertical")
        {
            leftArrowImg.gameObject.SetActive(show);
            rightArrowImg.gameObject.SetActive(show);
        }
        else
        {
            upArrowImg.gameObject.SetActive(show);
            downArrowImg.gameObject.SetActive(show);
        }

    }
    public void NeckRotate(string direction)
    {
        switch (direction)
        {
            case "LEFT":
                ChangeTransRotate("x", minus, 0.4f, "z", minus, 15);
                left--; right++;
                CheckVerticalNum();
                break;
            case "RIGHT":
                ChangeTransRotate("x", plus, 0.4f, "z", plus, 15);
                right--; left++;
                CheckVerticalNum();
                break;
            case "UP":
                ChangeTransRotate("y", plus, 1, "", plus, 0);
                up--; down++;
                CheckHorizontalNum();
                break;
            case "DOWN":
                ChangeTransRotate("y", minus, 1, "", plus, 0);
                up++; down--;
                CheckHorizontalNum();
                break;
        }
    }
    public void ChangeTransRotate(string transAxis, bool transSign, float transValue, string rotateAxis, bool rotateSign, float rotateValue)
    {
        originRotation = player.transform.rotation.eulerAngles;
        originPosition = player.transform.position;
        rotation = originRotation;
        position = originPosition;
        switch (transAxis)
        {
            case "x":
                position.x += transSign ? transValue : -transValue;
                break;
            case "y":
                position.y += transSign ? transValue : -transValue;
                break;
            case "z":
                position.z += transSign ? transValue : -transValue;
                break;
            default:
                break;
        }
        switch (rotateAxis)
        {
            case "x":
                rotation.x += rotateSign ? rotateValue : -rotateValue;
                break;
            case "y":
                rotation.y += rotateSign ? rotateValue : -rotateValue;
                break;
            case "z":
                rotation.z += rotateSign ? rotateValue : -rotateValue;
                break;
            default:
                break;
        }
        player.transform.rotation = Quaternion.Euler(rotation);
        player.transform.position = position;
    }
    public void CheckVerticalNum()
    {
        leftArrowImg.gameObject.SetActive(left >= 0);
        rightArrowImg.gameObject.SetActive(right >= 0);
    }
    public void CheckHorizontalNum()
    {
        upArrowImg.gameObject.SetActive(up >= 0);
        downArrowImg.gameObject.SetActive(down >= 1);
    }
    public void ResetAll()
    {
        player.transform.rotation = Quaternion.Euler(originRotation);
        player.transform.position = originPosition;
        left = 0; right = 0; up = 0; down = 0;
    }
    //인터페이스 구현
    public void Interactive()
    {
        uiCoroutine = StartCoroutine(UICo(index - 1, index));
        index++;
        if (index > infoImage.Length)
        {
            CutSceneManager.Instance.CutSceneStart(); 
            return;
        }
    }
    public void ExitTrigger()
    {
        index = 0;
        if (uiCoroutine != null)
        {
            StopCoroutine(uiCoroutine);
            uiCoroutine = null;
        }
        for (int i = 0; i < infoImage.Length; i++)
        {
            infoImage[i].gameObject.SetActive(false);
            infoImage[i].rectTransform.localScale = Vector3.zero;
        }
    }
    IEnumerator UICo(int preIndex, int currentIndex)
    {
        if (preIndex >= 0)
        {
            LeanTween.scale(infoImage[preIndex].rectTransform, smallUiImageSet, 0.5f);
            yield return new WaitForSeconds(0.5f);
            infoImage[preIndex].gameObject.SetActive(false);
        }
        infoImage[currentIndex].gameObject.SetActive(true);
        LeanTween.scale(infoImage[currentIndex].rectTransform, bigUiImageSet, 0.5f);
    }
}
