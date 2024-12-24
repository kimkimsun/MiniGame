using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class UIManager : SingleTon<UIManager>
{
    public TextMeshProUGUI currentBullet;
    public TextMeshProUGUI maxBullet;
    public TextMeshProUGUI bulletCount;
    public TextMeshProUGUI interactiveText;
    public TextMeshProUGUI killText;
    public TextMeshProUGUI remainText;
    public TextMeshProUGUI sacrificeText;
    public TextMeshProUGUI playTimeText;
    
    [Header("Finish")]
    public TextMeshProUGUI   finishDeath;
    public TextMeshProUGUI   finishDeathText;
    public TextMeshProUGUI   finishDeathBun;
    public TextMeshProUGUI   finishPlayTime;
    public TextMeshProUGUI   finishPlayTimeText;
    public TextMeshProUGUI   finishTotal;
    public TextMeshProUGUI   finishTotalText;
    public TextMeshProUGUI[] InputNameText;
    public Image             finishImg;
    public Image             InputNameImage;
    public Button            OKButton;


    public Image thirdPersonAimImage;
    public Image interactiveImage;
    public Image firstPersonAimImage;
    public Image leftArrowImg;
    public Image rightArrowImg;
    public Image upArrowImg;
    public Image downArrowImg;
    public Image settingImg;
    public Image tabImg;
    public Canvas optionCanvas;
    public Image[] infoImage;
    public PlayerAim player;
    public ThirdPersonController playerController;
    public GameObject triggerEventObj;
    public Stack<GameObject> uiStack;
    public bool isCutScene;
    public int kills;
    public int sacrifice;

    private Vector3 rotation;
    private Vector3 position;
    private Vector3 originPosition;
    private Vector3 originRotation;
    private Vector3 smallUiImageSet;
    private Vector3 bigUiImageSet;
    private Coroutine uiCoroutine;
    private Coroutine finishCoroutine;
    private bool plus = true;
    private bool minus = false;
    private bool isTab;
    private bool isFinish;
    private float playTime;
    private string name;
    private int index;
    private int left;
    private int right;
    private int up;
    private int down;
    private int lastPlayTime;
    private int total;
    private int _totalScore;

    [DllImport("__Internal")]
    private static extern void SendScore(string userName, int score);
    private void Start()
    {
        total = 27;
        smallUiImageSet = Vector3.zero;
        bigUiImageSet = new Vector3(5.775f, 2.15f, 1);
        uiStack = new Stack<GameObject>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
            CutSceneManager.Instance.CutSceneStart(0); 
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
    private void Update()
    {
        if(player == null && SecondCutSceneManager.Instance != null)
        {
            player = SecondCutSceneManager.Instance.player;
            playerController = player.GetComponent<ThirdPersonController>();
            isTab = true;
        }
        if (isTab)
        {
            playTime += Time.deltaTime;
            int roundedPlayTime = Mathf.FloorToInt(playTime);  // 소수점 아래 버리기
            if (roundedPlayTime > lastPlayTime)  // 1초 단위로 값이 증가할 때마다
            {
                lastPlayTime = roundedPlayTime;
                TextSetting();
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab) && isTab)
        {
            tabImg.gameObject.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab) && isTab)
        {
            tabImg.gameObject.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Escape) && !isCutScene)
        {
            if (uiStack.Count > 0)
            {
                GetBack();
            }
            else
            {
                Time.timeScale = 0;
                settingImg.gameObject.SetActive(true);
                uiStack.Push(settingImg.gameObject);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                player._Input.cursorInputForLook = false;
                playerController.LockCameraPosition = true;
            }
        }
        if (isFinish)
        {
            // 어떤 키든 입력되면 실행
            if (Input.anyKeyDown)
            {
                foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(key))
                    {
                        // Escape 키를 누르면 모든 텍스트를 초기화
                        if (key == KeyCode.Escape)
                        {
                            foreach (var text in InputNameText)
                            {
                                text.text = "ㅡ";
                            }
                            return; // 더 이상 처리하지 않음
                        }

                        // ㅡ인 항목에 입력된 키를 할당
                        for (int i = 0; i < InputNameText.Length; i++)
                        {
                            if (InputNameText[i].text == "ㅡ")
                            {
                                InputNameText[i].text = key.ToString();
                                return; // 첫 번째 빈칸만 업데이트하고 종료
                            }
                        }
                    }
                }
            }
            // 모든 텍스트가 ㅡ가 아닌 다른 문자로 채워졌는지 확인
            bool allFilled = true;
            foreach (var text in InputNameText)
            {
                if (text.text == "ㅡ")
                {
                    allFilled = false;
                    break;
                }
            }
            // 상태를 디버그 로그로 출력하거나 추가 동작
            if (allFilled)
            {
                OKButton.gameObject.SetActive(allFilled);
                name = null;
                for (int i = 0; i < InputNameText.Length; i++)
                {
                    name += InputNameText[i];
                }
            }
            else
            {
                OKButton.gameObject.SetActive(allFilled);
            }
        }
    }
    private void TextSetting()
    {
        killText.text = kills.ToString();
        remainText.text = (total - kills).ToString();
        sacrificeText.text = sacrifice.ToString();
        playTimeText.text = lastPlayTime.ToString();
        if(total - kills == 0 && finishCoroutine == null)
        {
            finishCoroutine = StartCoroutine(FinishUiCo());
        }
    }
    public void GetBack()
    {
        Time.timeScale = 1;
        GameObject stackPopResult = uiStack.Pop();
        player.GetComponent<StarterAssetsInputs>().cursorInputForLook = true;
        stackPopResult.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerController.LockCameraPosition = false;
    }
    public void ClickOption()
    {
        GameObject stackPopResult = uiStack.Pop();
        stackPopResult.SetActive(false);
        optionCanvas.gameObject.SetActive(true);
        uiStack.Push(optionCanvas.gameObject);
    }
    public void ClickOK()
    {
        //SendScore(name, _totalScore);
        SceneManager.LoadScene(0);
    }
    IEnumerator FinishUiCo()
    {
        finishDeathText.text = sacrifice.ToString();
        finishPlayTimeText.text = playTime.ToString();
        finishImg.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        AudioManager.Instance.PlaySound(player.ShootSound, position);
        finishDeath.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        AudioManager.Instance.PlaySound(player.ShootSound, position);
        finishDeathText.gameObject.SetActive(true);
        finishDeathBun.gameObject.SetActive(true);
        yield return StartCoroutine(ChangeNum(sacrifice, 0.05f, finishDeathText));
        yield return new WaitForSeconds(0.5f);
        AudioManager.Instance.PlaySound(player.ShootSound, position);
        finishPlayTime.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        AudioManager.Instance.PlaySound(player.ShootSound, position);
        finishPlayTimeText.text = Mathf.FloorToInt(lastPlayTime).ToString();
        finishPlayTimeText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(ChangeNum(Mathf.FloorToInt(lastPlayTime), 0.03f, finishPlayTimeText));
        AudioManager.Instance.PlaySound(player.ShootSound, position);
        finishTotal.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        AudioManager.Instance.PlaySound(player.ShootSound, position);
        finishTotalText.text = _totalScore.ToString();
        finishTotalText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.75f);
        AudioManager.Instance.PlaySound(player.ShootSound, position);
        isFinish = true;
        finishImg.gameObject.SetActive(true);
        InputNameImage.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        player._Input.cursorInputForLook = false;
        playerController.LockCameraPosition = true;
    }

    IEnumerator ChangeNum(int score, float standardTime, TextMeshProUGUI text)
    {
        float time = 0;
        _totalScore += 10000;

        while (score > 0)
        {
            time += Time.deltaTime;

            if (time > standardTime)
            {
                score--;
                text.text = score.ToString();
                time = 0;
            }
            _totalScore -= 100;
            yield return null;
        }
    }
    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }
}
