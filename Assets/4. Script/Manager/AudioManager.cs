using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class AudioManager : SingleTon<AudioManager>
{
    // ���� Ÿ�Ժ��� AudioSource Ǯ�� �����ϴ� Dictionary
    private Dictionary<string, Queue<AudioSource>> audioSourcePools = new Dictionary<string, Queue<AudioSource>>();

    // Ư�� ���忡 ���� Ǯ ����
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

    // ���� ���
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

    // Ǯ�� �ٽ� �߰�
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