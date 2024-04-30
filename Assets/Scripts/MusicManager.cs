using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Slider bgmSlider;
    public Slider sfxSlider;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("BGM_Volume"))
        {
            bgmSlider.value = PlayerPrefs.GetFloat("BGM_Volume");
        }
        if (PlayerPrefs.HasKey("SFX_Volume"))
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SFX_Volume");
        }
        else
        {
            bgmSlider.value = 0.5f;
            sfxSlider.value = 0.5f;
        }
    }
    
    void Start()
    {
        bgmSlider.onValueChanged.AddListener(OnValueChanged);
        sfxSlider.onValueChanged.AddListener(OnValueChanged);
        audioMixer.SetFloat("BGM_Volume", Mathf.Log10(bgmSlider.value) * 20);
        audioMixer.SetFloat("SFX_Volume", Mathf.Log10(sfxSlider.value) * 20);
    }

    public void OnValueChanged(float music_value)
    {
        PlayerPrefs.SetFloat("BGM_Volume", bgmSlider.value);
        PlayerPrefs.SetFloat("SFX_Volume", sfxSlider.value);
    }

    public void SetBGM(float sliderValue)
    {
        audioMixer.SetFloat("BGM_Volume", Mathf.Log10(sliderValue) * 20);
    }

    public void SetSFX(float sliderValue)
    {
        audioMixer.SetFloat("SFX_Volume", Mathf.Log10(sliderValue) * 20);
    }
}
