using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingleTon<GameManager>
{
    public Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    private void Start()
    {
    }
    public void CreatePool(GameObject prefab, int count)
    {
        string key = prefab.name;

        if (!poolDictionary.ContainsKey(key))
            poolDictionary[key] = new Queue<GameObject>();

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.name = prefab.name; // Key�� ã�� ���� �̸� ����
            obj.SetActive(false);
            poolDictionary[key].Enqueue(obj);
        }
    }

    public GameObject GetFromPool(GameObject prefab)
    {
        string key = prefab.name;

        if (poolDictionary.ContainsKey(key) && poolDictionary[key].Count > 0)
        {
            GameObject obj = poolDictionary[key].Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject newObj = Instantiate(prefab);
            newObj.name = prefab.name;
            return newObj; // ������ ��� ���� ����
        }
    }
    public void ReturnToPool(GameObject obj)
    {
        string key = obj.name;

        obj.SetActive(false);
        if (poolDictionary.ContainsKey(key))
        {
            poolDictionary[key].Enqueue(obj);
        }
        else
        {
            Destroy(obj); // Ǯ�� ���� ���ϰ� ������� ���� ����� ����ó��
        }
    }
}