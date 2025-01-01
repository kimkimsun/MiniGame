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

        public static float bgmVolume;
        public static float sfxVolume;

        public Slider BGMmusicSlider;
        public Slider SFXmusicSlider;

        private AudioSource audioSource;
        private AudioManager amInstance;

        public void Start()
        {
            FirstSetting();
        }
        public void Update()
        {
            OptionSetting();
            VolumeSave();
        }
        private void FirstSetting()
        {
            amInstance = AudioManager.Instance;
            audioSource = Camera.main.GetComponent<AudioSource>();
            audioSource.volume = BGMmusicSlider.value;
            amInstance.volume = SFXmusicSlider.value;
        }
        private void OptionSetting()
        {
            audioSource.volume = Mathf.Round(BGMmusicSlider.value * 100f) / 1000f;
            amInstance.volume = Mathf.Round(SFXmusicSlider.value * 100) / 1000f;
        }
        private void VolumeSave()
        {
            bgmVolume = audioSource.volume;
            sfxVolume = amInstance.volume;
        }
    }
}