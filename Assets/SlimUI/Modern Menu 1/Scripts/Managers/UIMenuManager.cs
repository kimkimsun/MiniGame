using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace SlimUI.ModernMenu
{
    public class UIMenuManager : MonoBehaviour
    {
        private Animator CameraObject;
        private AudioManager amInstance;
        private int SelectGameIndex;

        public AudioSource audioSource;
        public GameObject loadingScreen;
        public Slider progressBar;
        public TextMeshProUGUI progressText;
        // campaign button sub menu
        [Header("MENUS")]
        [Tooltip("The Menu for when the MAIN menu buttons")]
        public GameObject mainMenu;
        [Tooltip("THe first list of buttons")]
        public GameObject firstMenu;
        [Tooltip("The Menu for when the PLAY button is clicked")]
        public GameObject playMenu;
        [Tooltip("The Menu for when the EXIT button is clicked")]
        public GameObject exitMenu;
        [Tooltip("Optional 4th Menu")]
        public GameObject extrasMenu;
        public GameObject rankingMenu;

        public enum Theme { custom1, custom2, custom3 };
        [Header("THEME SETTINGS")]
        public Theme theme;
        private int themeIndex;
        public ThemedUIData themeController;

        [Header("PANELS")]
        [Tooltip("The UI Panel parenting all sub menus")]
        public GameObject mainCanvas;
        [Tooltip("The UI Panel that holds the CONTROLS window tab")]
        public GameObject PanelControls;
        [Tooltip("The UI Panel that holds the VIDEO window tab")]
        public GameObject PanelVideo;


        // highlights in settings screen
        [Header("SETTINGS SCREEN")]
        [Tooltip("Highlight Image for when GAME Tab is selected in Settings")]
        public GameObject lineVideo;
        [Tooltip("Highlight Image for when CONTROLS Tab is selected in Settings")]
        public GameObject lineControls;

        [Header("LOADING SCREEN")]
        [Tooltip("If this is true, the loaded scene won't load until receiving user input")]
        public bool waitForInput = true;
        public GameObject loadingMenu;
        [Tooltip("The loading bar Slider UI element in the Loading Screen")]
        public Slider loadingBar;
        public TMP_Text loadPromptText;
        public KeyCode userPromptKey;

        [Header("SFX")]
        [Tooltip("The GameObject holding the Audio Source component for the HOVER SOUND")]
        public AudioSource hoverSound;
        [Tooltip("The GameObject holding the Audio Source component for the SWOOSH SOUND when switching to the Settings Screen")]
        public AudioSource swooshSound;

        void Start()
        {
            SelectGameIndex = 1;
            audioSource = GetComponent<AudioSource>();
            amInstance = AudioManager.Instance;
            if (SceneManager.GetActiveScene().name == "0. MainMenu")
            {
                CameraObject = transform.GetComponent<Animator>();
                playMenu.SetActive(false);
                exitMenu.SetActive(false);
                if (extrasMenu) extrasMenu.SetActive(false);
                if (rankingMenu) rankingMenu.SetActive(false);
                firstMenu.SetActive(true);
                mainMenu.SetActive(true);
                SetThemeColors();
            }
        }

        void SetThemeColors()
        {
            switch (theme)
            {
                case Theme.custom1:
                    themeController.currentColor = themeController.custom1.graphic1;
                    themeController.textColor = themeController.custom1.text1;
                    themeIndex = 0;
                    break;
                case Theme.custom2:
                    themeController.currentColor = themeController.custom2.graphic2;
                    themeController.textColor = themeController.custom2.text2;
                    themeIndex = 1;
                    break;
                case Theme.custom3:
                    themeController.currentColor = themeController.custom3.graphic3;
                    themeController.textColor = themeController.custom3.text3;
                    themeIndex = 2;
                    break;
                default:
                    Debug.Log("Invalid theme selected.");
                    break;
            }
        }

        public void PlayCampaign()
        {
            StartCoroutine(LoadSceneCoroutine(GameManager.SelectGameIndex));
        }
        private IEnumerator LoadSceneCoroutine(int sceneIndex)
        {
            mainCanvas.SetActive(false);
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
            operation.allowSceneActivation = false;

            loadingScreen.SetActive(true);

            while (operation.progress < 0.9f)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                progressBar.value = progress;
                progressText.text = $"로딩 중... {Mathf.RoundToInt(progress * 100)}%";
                yield return null;
            }

            // 로딩 완료 연출
            progressBar.value = 1f;
            progressText.text = "계속 진행하려면 아무 키나 누르세요.";
            yield return new WaitUntil(() => Input.anyKeyDown);

            // 씬 활성화
            operation.allowSceneActivation = true;
            loadingScreen.SetActive(false);
        }


        public void PlayCampaignMobile()
        {
            exitMenu.SetActive(false);
            if (extrasMenu) extrasMenu.SetActive(false);
            if (rankingMenu) rankingMenu.SetActive(false);

            playMenu.SetActive(true);
            mainMenu.SetActive(false);
        }

        public void ReturnMenu()
        {
            playMenu.SetActive(false);
            if (extrasMenu) extrasMenu.SetActive(false);
            if (rankingMenu) rankingMenu.SetActive(false);

            exitMenu.SetActive(false);
            mainMenu.SetActive(true);
        }
        public void DisablePlayCampaign()
        {
            playMenu.SetActive(false);
        }

        public void Position2()
        {
            DisablePlayCampaign();
            CameraObject.SetFloat("Animate", 1);
        }

        public void Position1()
        {
            CameraObject.SetFloat("Animate", 0);
        }

        void DisablePanels()
        {
            PanelControls.SetActive(false);
            PanelVideo.SetActive(false);
            lineControls.SetActive(false);
            lineVideo.SetActive(false);
        }

        public void GamePanel()
        {
            DisablePanels();
        }

        public void VideoPanel()
        {
            DisablePanels();
            PanelVideo.SetActive(true);
            lineVideo.SetActive(true);
        }

        public void ControlsPanel()
        {
            DisablePanels();
            PanelControls.SetActive(true);
            lineControls.SetActive(true);
        }

        public void KeyBindingsPanel()
        {
            DisablePanels();
            MovementPanel();
        }

        public void MovementPanel()
        {
            DisablePanels();
        }

        public void CombatPanel()
        {
            DisablePanels();
        }

        public void GeneralPanel()
        {
            DisablePanels();
        }

        public void PlayHover()
        {
            amInstance.PlaySound(hoverSound.gameObject, transform.position);
        }

        public void PlaySwoosh()
        {
            amInstance.PlaySound(swooshSound.gameObject, transform.position);
        }

        // Are You Sure - Quit Panel Pop Up
        public void AreYouSure()
        {
            exitMenu.SetActive(true);
            if (extrasMenu) extrasMenu.SetActive(false);
            if (rankingMenu) rankingMenu.SetActive(false);

            DisablePlayCampaign();
        }

        public void AreYouSureMobile()
        {
            exitMenu.SetActive(true);
            if (extrasMenu) extrasMenu.SetActive(false);
            if (rankingMenu) rankingMenu.SetActive(false);

            mainMenu.SetActive(false);
            DisablePlayCampaign();
        }

        public void ExtrasMenu()
        {
            playMenu.SetActive(false);
            if (extrasMenu) extrasMenu.SetActive(true);
            if (rankingMenu) rankingMenu.SetActive(false);

            exitMenu.SetActive(false);
        }

        public void RankingMenu()
        {
            playMenu.SetActive(false);
            if (extrasMenu) extrasMenu.SetActive(false);
            if (rankingMenu) rankingMenu.SetActive(true);
            exitMenu.SetActive(false);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
				Application.Quit();
#endif
        }
    }
}