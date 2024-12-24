using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public TextMeshPro progressText;
    private void Start()
    {
        PlayCampaign();
    }

    public void PlayCampaign()
    {
        StartCoroutine(LoadSceneCoroutine(GameManager.MainGameIndex));
    }
    private IEnumerator LoadSceneCoroutine(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;


        while (operation.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            yield return null;
        }

        progressText.text = "계속 진행하려면 아무 키나 누르세요.";
        yield return new WaitUntil(() => Input.anyKeyDown);
        // 씬 활성화
        operation.allowSceneActivation = true;
    }
}
