using SlimUI.ModernMenu;
using StarterAssets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : SingleTon<GameManager>
{
    public Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();
    public Player player;
    public ThirdPersonController playerController;
    public Slider mouseSensitivitySlider;
    public Slider sensitivityVerticalSlider;
    public Slider sensitivityHorizontalSlider;
    public Slider sensitivityAimVerticalSlider;
    public Slider sensitivityAimHorizontalSlider;

    public static int SelectGameIndex = 1;
    public static int LoadingSceneIndex = 2;
    public static int MainGameIndex = 3;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U)) SceneManager.LoadScene(3);
    }
    public void CreatePool(GameObject prefab, int count)
    {
        string key = prefab.name;

        if (!poolDictionary.ContainsKey(key))
            poolDictionary[key] = new Queue<GameObject>();

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.name = prefab.name; // Key로 찾기 위해 이름 통일
            obj.SetActive(false);
            poolDictionary[key].Enqueue(obj);
        }
    }

    public GameObject GetFromPool(GameObject prefab)
    {
        string key = prefab.name;

        // 키가 존재하고 큐에 요소가 있을 경우
        if (poolDictionary.ContainsKey(key) && poolDictionary[key].Count > 0)
        {
            GameObject obj = poolDictionary[key].Dequeue(); // 큐에서 객체를 가져옴

            // Dequeue된 객체가 null일 경우를 체크
            if (obj == null)
            {
                Debug.LogWarning($"Object in pool '{key}' was null. Creating a new instance.");
                return Instantiate(prefab); // 새로 생성하여 반환
            }

            obj.SetActive(true);
            return obj;
        }
        else
        {
            Debug.LogWarning($"Pool for '{key}' is empty or does not exist. Creating a new instance.");
            GameObject newObj = Instantiate(prefab);
            newObj.name = prefab.name; // 키 통일을 위해 이름 설정
            return newObj;
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
            poolDictionary[key] = new Queue<GameObject>();
            obj.gameObject.SetActive(false);
            poolDictionary[key].Enqueue(obj);
        }
    }
}