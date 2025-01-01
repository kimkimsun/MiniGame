using StarterAssets;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SecondCutSceneManager : SingleTon<SecondCutSceneManager>
{
    public Material         weaponSpawnMat;
    public Material         playerSpawnMat;
    public TimelineAsset    cutSceneTL;


    private PlayerAim           player;
    private PlayableDirector    playableDirector;
    private Vector3             originWeaponSize;
    private float               weaponMatGetValue;
    private float               weaponMatMinValue;
    private float               weaponMatMaxValue;
    private float               playerMatGetValue;
    private float               playerMatMinValue;
    private float               playerMatMaxValue;
    private float               loadPlayerSpawnSpeed;
    private float               loadWeaponSpawnSpeed;

    private void Start()
    {
        weaponMatMinValue = -0.15f;
        weaponMatMaxValue = 0.8f;
        weaponMatGetValue = weaponMatMinValue;
        playerMatMinValue = 0.1f;
        playerMatMaxValue = 2.1f;
        playerMatGetValue = playerMatMinValue;
        loadPlayerSpawnSpeed = 1f;
        loadWeaponSpawnSpeed = 0.475f;
        originWeaponSize = new Vector3(1, 1, 1);
        playableDirector = GetComponent<PlayableDirector>();
        player =         GameManager.Instance.player.GetComponent<PlayerAim>();
        weaponSpawnMat.SetFloat("_Split_Value", weaponMatGetValue);
        playerSpawnMat.SetFloat("_Split_Value", playerMatGetValue);
         
        FindTrack();
        playableDirector.Play();
    }
    public void FindTrack()
    {
        // 타임라인의 트랙 가져오기
        var trackList = playableDirector.playableAsset as TimelineAsset;
        if (trackList == null) return;

        foreach (var track in trackList.GetOutputTracks())
        {
            // Cinemachine Track 처리
            if (track.name == "Cinemachine Track")
            {
                var cinemachineTrack = track as CinemachineTrack; // CinemachineTrack으로 캐스팅
                if (cinemachineTrack != null)
                {
                    var cinemachineBrain = Camera.main?.GetComponent<CinemachineBrain>();
                    if (cinemachineBrain != null)
                    {
                        playableDirector.SetGenericBinding(cinemachineTrack, cinemachineBrain);
                    }
                }
            }
            // Animation Track 처리
            else if (track.name == "Animation Track")
            {
                var animationTrack = track as AnimationTrack; // AnimationTrack으로 캐스팅
                if (animationTrack != null)
                {
                    var animationtrack = player.GetComponent<Animator>();
                    playableDirector.SetGenericBinding(animationTrack, animationtrack);
                    break;
                }
            }
        }
    }
    public void OriginEveryThing()
    {
        player.Gun.gameObject.SetActive(true);
        player.Gun.transform.parent = player.HandGunSlot;
        player.Gun.transform.localScale = originWeaponSize;
        player.Gun.transform.position = player.HandGunSlot.transform.position;
        player.Gun.transform.rotation = player.HandGunSlot.transform.rotation;
    }

    public void CutSceneStart()
    {
        UIManager.Instance.player = player;
        UIManager.Instance.playerController = player.GetComponent<ThirdPersonController>();
        player._Input.aim = false;
        player._Input.cursorInputForLook = false;
        UIManager.Instance.isCutScene = true;
    }

    public void CutSceneEnd()
    {
        player._Input.aim = true;
        player._Input.cursorInputForLook = true;
        UIManager.Instance.isCutScene = false;
    }
    public void LoadWithSpawn()
    {
        StartCoroutine(LoadWithSpawnCo());
    }
    IEnumerator LoadWithSpawnCo()
    {
        playerMatGetValue = playerMatMinValue;
        weaponMatGetValue = weaponMatMinValue;
        playerSpawnMat.SetFloat("_Split_Value", playerMatGetValue);
        weaponSpawnMat.SetFloat("_Split_Value", weaponMatGetValue);
        while (weaponMatGetValue < weaponMatMaxValue && playerMatGetValue < playerMatMaxValue)
        {
            weaponMatGetValue += Time.deltaTime * loadWeaponSpawnSpeed;
            playerMatGetValue += Time.deltaTime * loadPlayerSpawnSpeed;
            playerSpawnMat.SetFloat("_Split_Value", playerMatGetValue);
            weaponSpawnMat.SetFloat("_Split_Value", weaponMatGetValue);
            yield return null;
        }
    }
}
