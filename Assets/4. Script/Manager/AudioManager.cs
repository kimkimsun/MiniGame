using SlimUI.ModernMenu;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class AudioManager : SingleTon<AudioManager>
{
    // 사운드 타입별로 AudioSource 풀을 저장하는 Dictionary
    public Dictionary<string, Queue<AudioSource>> audioSourcePools = new Dictionary<string, Queue<AudioSource>>();
    public ThirdPersonController player;
    public Slider BGMmusicSlider;
    public Slider SFXmusicSlider;
    public float volume = 10;

    private AudioSource audioSource;
    private bool isSceneChange;
    // 특정 사운드에 대한 풀 생성
    private void Start()
    {
        isSceneChange = false;
        SceneManager.sceneLoaded += OnSceneLoaded;
        audioSource = Camera.main.GetComponent<AudioSource>();
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "1. LOBBY")
        {
            LobbySetting();
            ChangeMusicSet();
        }
    }
    private void LobbySetting()
    {
        foreach (Slider slider in Resources.FindObjectsOfTypeAll<Slider>())
        {
            if (slider.gameObject.name == "BGMsound")
            {
                BGMmusicSlider = slider;
            }
            if (slider.gameObject.name == "SFXSound")
            {
                SFXmusicSlider = slider;
            }
        }
    }
    private void ChangeMusicSet()
    {
        BGMmusicSlider.value = UISettingsManager.bgmVolume;
        SFXmusicSlider.value = UISettingsManager.sfxVolume;
        audioSource = Camera.main.GetComponent<AudioSource>();
        isSceneChange = true;
    }

    private void Update()
    {
        if (isSceneChange)
        {
            SoundManage();
        } 
    }
    private void SoundManage()
    {
        audioSource.volume = Mathf.Round(BGMmusicSlider.value * 100f) / 1000f;
        volume = Mathf.Round(SFXmusicSlider.value * 100) / 1000f;
    }
    public void CreateSoundPool(GameObject audioSourcePrefab, int poolSize)
    {
        string soundKey = audioSourcePrefab.name;
        if (!audioSourcePools.ContainsKey(soundKey))
            audioSourcePools[soundKey] = new Queue<AudioSource>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(audioSourcePrefab);
            AudioSource source = obj.GetComponent<AudioSource>();
            obj.SetActive(false);
            audioSourcePools[soundKey].Enqueue(source);
        }
    }

    // 사운드 재생
    public void PlaySound(GameObject audioSourcePrefab, Vector3 position, int layer = -1)
    {
        string soundKey = audioSourcePrefab.name;

        // 큐가 비어있는지 먼저 확인
        if (audioSourcePools.ContainsKey(soundKey) && audioSourcePools[soundKey].Count > 0)
        {
            AudioSource source = audioSourcePools[soundKey].Dequeue();

            // Dequeue된 객체가 null인 경우
            if (source == null)
            {
                audioSourcePools[soundKey].Clear();
                CreateAndPlayNewAudioSource(audioSourcePrefab, position, layer);
            }
            else
            {
                source.transform.position = position;
                source.gameObject.SetActive(true);
                if (layer != -1) source.gameObject.layer = layer;
                source.volume = this.volume;
                source.Play();
                StartCoroutine(ReturnToPool(soundKey, source, source.clip.length));
            }
        }
        else
        {
            CreateAndPlayNewAudioSource(audioSourcePrefab, position, layer);
        }
    }

    // 새로운 AudioSource 생성 및 재생
    private void CreateAndPlayNewAudioSource(GameObject audioSourcePrefab, Vector3 position, int layer = -1)
    {
        GameObject newSound = Instantiate(audioSourcePrefab);
        newSound.name = audioSourcePrefab.name;
        AudioSource source = newSound.GetComponent<AudioSource>();
        source.transform.position = position;
        source.gameObject.SetActive(true);
        if (layer != -1) source.gameObject.layer = layer;
        source.volume = this.volume;
        source.Play();
        StartCoroutine(ReturnToPool(newSound.name, source, source.clip.length));
    }

    // 풀에 다시 추가
    IEnumerator ReturnToPool(string soundKey, AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        source.gameObject.SetActive(false);
        if (audioSourcePools.ContainsKey(soundKey))
        {
            audioSourcePools[soundKey].Enqueue(source);
        }
        else
        {
            audioSourcePools[soundKey] = new Queue<AudioSource>();
            audioSourcePools[soundKey].Enqueue(source);
        }
    }
}