using System.Collections;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class CutSceneManager : SingleTon<CutSceneManager>
{
    public Image            productionImage;
    public TextMeshProUGUI  productionText;
    public TimelineAsset[]  cutSceneTL;
    public Material         weaponSpawnMat;
    public Material         playerSpawnMat;

    private StringBuilder       stringBuilder;
    private PlayableDirector    playableDirector;
    private PlayerAim           player;
    private Vector3             newImageSize;
    private Vector3             weaponSize;
    private char[]              preSettingChars;
    private int                 randomIndex;
    private float               weaponMatGetValue;
    private float               weaponMatMinValue;
    private float               weaponMatMaxValue;
    private float               playerMatGetValue;
    private float               playerMatMinValue;
    private float               playerMatMaxValue;
    private float               weaponSpawnSpeed;
    private float               playerSpawnSpeed;

    private void Start()
    {
        player =                       GameManager.Instance.player.GetComponent<PlayerAim>();
        playableDirector =             GetComponent<PlayableDirector>();
        preSettingChars =              new char[] { '��', '��', ' ',':', ' ', '��', '��', '��', ' ', '��', ' ', '��', '��', 
                                                  '\n','\n', '��' ,'��','��',' ',':',' ', '��','��',
                                                  '\n','\n', '��','��',' ','��','��',' ','ó','ġ','��','��','\n','��','��','��',' ','��','��','��','��','��' };
        stringBuilder =                new StringBuilder(preSettingChars.Length); // �ʱ�ȭ
        newImageSize =                 new Vector3(0.45f, 0.6f, 0);
        weaponSize =                   new Vector3(0.9f, 0.9f, 0.9f);
        weaponMatMinValue =            -0.15f;
        weaponMatMaxValue =            0.8f;
        weaponSpawnSpeed =             0.3f;
        weaponMatGetValue =            weaponMatMinValue;
        playerSpawnSpeed =             0.275f;
        playerMatMinValue =            0.1f;
        playerMatMaxValue =            2.1f;
        playerMatGetValue =            playerMatMaxValue;
        
        
        
        weaponSpawnMat.SetFloat("_Split_Value", weaponMatMaxValue);
        playerSpawnMat.SetFloat("_Split_Value", playerMatGetValue);
    }
    public void StartSetting()
    {
        weaponSpawnMat.SetFloat("_Split_Value", weaponMatGetValue);
        player.Gun.transform.parent = player.BackGunSlot;
        player.Gun.transform.localScale = weaponSize;
        player.Gun.transform.position = player.BackGunSlot.transform.position;
        player.Gun.transform.rotation = player.BackGunSlot.transform.rotation;
        player._Input.aim = false;
        player._Input.cursorInputForLook = false;

    }
    public void MissionStart()
    {
        StartCoroutine(MissionStartCo());
    }
    public void WeaponSpawnStart()
    {
        StartCoroutine(SpawnWeaponStartCo());
    }
    public void PlayerSpawnStart()
    {
        StartCoroutine(SpawnPlayerStartCo());
    }
    public void CutSceneStart()
    {
        playableDirector.Play(cutSceneTL[0]);
    }
    IEnumerator SpawnWeaponStartCo()
    {
        while(true)
        {
            weaponMatGetValue += weaponSpawnSpeed * Time.deltaTime;
            weaponSpawnMat.SetFloat("_Split_Value", weaponMatGetValue);
            yield return null;
            if (weaponMatGetValue > weaponMatMaxValue)
            {
                yield break;
            }
        }
    }
    IEnumerator MissionStartCo()
    {
        for (int i = 0; i < preSettingChars.Length; i++)
        {
            stringBuilder.Append(preSettingChars[i]);
            for (int j = 0; j < 10; j++)
            {
                // ���� ���� ����
                randomIndex = Random.Range(0x0370, 0x03FF);
                // StringBuilder�� ���ڿ� ����
                stringBuilder[i] = (char)randomIndex;
                productionText.text = stringBuilder.ToString(); // TextMeshProUGUI�� ����
                yield return new WaitForSeconds(0.015f);
            }
            // ������ ���ڸ� Ȯ���Ͽ� �״�� ����
            stringBuilder[i] = preSettingChars[i];
            productionText.text = stringBuilder.ToString(); // ���������� ����� �ؽ�Ʈ ����
        }
        LeanTween.scale(productionImage.rectTransform, newImageSize, 1);
    }
    IEnumerator SpawnPlayerStartCo()
    {
        player.Gun.gameObject.SetActive(false);
        while (true)
        {
            playerMatGetValue -= playerSpawnSpeed * Time.deltaTime;
            playerSpawnMat.SetFloat("_Split_Value", playerMatGetValue);
            yield return null;
            if (playerMatGetValue < playerMatMinValue)
            {
                yield break;
            }
        }
    }
}