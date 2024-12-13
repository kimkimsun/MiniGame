using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class AudioManager : SingleTon<AudioManager>
{
    // 사운드 타입별로 AudioSource 풀을 저장하는 Dictionary
    private Dictionary<string, Queue<AudioSource>> audioSourcePools = new Dictionary<string, Queue<AudioSource>>();

    // 특정 사운드에 대한 풀 생성
    public void CreateSoundPool(GameObject audioSourcePrefab, int poolSize)
    {
        string soundKey = audioSourcePrefab.name;
        if(!audioSourcePools.ContainsKey(soundKey)) audioSourcePools[soundKey] = new Queue<AudioSource>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(audioSourcePrefab);
            AudioSource source = obj.GetComponent<AudioSource>();
            obj.SetActive(false);
            audioSourcePools[soundKey].Enqueue(source);
        }
    }

    // 사운드 재생
    public void PlaySound(GameObject audioSourcePrefab, Vector3 position)
    {
        string soundKey = audioSourcePrefab.name;
        if (audioSourcePools.ContainsKey(soundKey) && audioSourcePools[soundKey].Count > 0)
        {
            AudioSource source = audioSourcePools[soundKey].Dequeue();
            source.transform.position = position;
            source.gameObject.SetActive(true);
            source.gameObject.layer = 6;
            source.Play();
            StartCoroutine(ReturnToPool(soundKey, source, source.clip.length));
        }
        else
        {
            GameObject newSound = Instantiate(audioSourcePrefab);
            newSound.name = audioSourcePrefab.name;
            AudioSource source = newSound.GetComponent<AudioSource>();
            StartCoroutine(ReturnToPool(soundKey, source, source.clip.length));
        }
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
            source.gameObject.SetActive(false);
            audioSourcePools[soundKey].Enqueue(source);
        }
    }
}