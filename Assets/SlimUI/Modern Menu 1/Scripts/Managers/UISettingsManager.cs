using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using StarterAssets;

namespace SlimUI.ModernMenu
{
    public class UISettingsManager : MonoBehaviour
    {
        public enum Platform { Desktop, Mobile };
        public Platform platform;
        // sliders
        public GameObject BGMmusicSlider;
        public GameObject SFXmusicSlider;
        public GameObject mouseSensitivitySlider;
        public GameObject sensitivityVerticalSlider;
        public GameObject sensitivityHorizontalSlider;
        public GameObject sensitivityAimVerticalSlider;
        public GameObject sensitivityAimHorizontalSlider;


        private AudioSource audioSource;
        private AudioManager amInstance;
        private ThirdPersonController player;

        public void Start()
        {
            amInstance = AudioManager.Instance;
            player = amInstance.player;
            audioSource = Camera.main.GetComponent<AudioSource>();
            audioSource.volume = BGMmusicSlider.GetComponent<Slider>().value;
            amInstance.volume = SFXmusicSlider.GetComponent<Slider>().value;
            mouseSensitivitySlider.GetComponent<Slider>().value = 5f;
            sensitivityVerticalSlider.GetComponent<Slider>().value = 5f;
            sensitivityHorizontalSlider.GetComponent<Slider>().value = 5f;
            sensitivityAimVerticalSlider.GetComponent<Slider>().value = 5f;
            sensitivityAimHorizontalSlider.GetComponent<Slider>().value = 5f;
            // check full screen

            // check shadow distance/enabled
        }
        public void Update()
        {
            audioSource.volume = Mathf.Round(BGMmusicSlider.GetComponent<Slider>().value * 100f) / 1000f;
            amInstance.volume = Mathf.Round(SFXmusicSlider.GetComponent<Slider>().value * 100) / 1000f;
            player.mouseSensitivity = Mathf.Round(mouseSensitivitySlider.GetComponent<Slider>().value) / 5;
            player.mouseVerticalSensitivity = Mathf.Round(sensitivityVerticalSlider.GetComponent<Slider>().value) / 5;
            player.mouseHorizontalSensitivity = Mathf.Round(sensitivityHorizontalSlider.GetComponent<Slider>().value) / 5;
            player.mouseAimVerticalSensitivity = Mathf.Round(sensitivityAimVerticalSlider.GetComponent<Slider>().value) / 5;
            player.mouseAimHorizontalSensitivity = Mathf.Round(sensitivityAimHorizontalSlider.GetComponent<Slider>().value) / 5;
        }
    }
}