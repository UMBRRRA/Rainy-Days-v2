using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenuFunctions : MonoBehaviour
{
    public GameObject child;
    public bool fromPause = false;
    public bool fromMain = false;
    private Player player;
    public Slider brightnessSlider, masterVolumeSlider, fxVolumeSlider, musicVolumeSlider;
    public AudioMixer mixer;
    public Text brightnessText, masterVolumeText, fxVolumeText, musicVolumeText;

    public void ActivateOptionsMenu()
    {
        if (fromPause)
            FindObjectOfType<HudFunctions>().DeactivateHud();
        child.SetActive(true);
    }

    public void DeactivateOptionsMenu()
    {
        if (fromPause)
        {
            FindObjectOfType<HudFunctions>().ActivateHud();
            PlayerGoNeutral();
        }
        else if (fromMain)
        {
            FindObjectOfType<MainMenuFunctions>().ActivateButtons();
        }
        child.SetActive(false);
        fromPause = false;
        fromMain = false;
    }

    public void PlayerGoNeutral()
    {
        StartCoroutine(PlayerNeutral());
    }

    public IEnumerator PlayerNeutral()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        player.State = PlayerState.Neutral;
    }

    private void Start()
    {
        brightnessSlider.onValueChanged.AddListener(delegate { BrightnessValueChange(); });
        masterVolumeSlider.onValueChanged.AddListener(delegate { MasterVolumeValueChange(); });
        fxVolumeSlider.onValueChanged.AddListener(delegate { FXVolumeValueChange(); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { MusicVolumeValueChange(); });
    }

    public void MusicVolumeValueChange()
    {
        mixer.SetFloat("MusicVolume", LogarhythmicVolume(musicVolumeSlider.value));
        musicVolumeText.text = $"{Mathf.Round(musicVolumeSlider.value * 100)}%";
    }

    public void FXVolumeValueChange()
    {
        mixer.SetFloat("FXVolume", LogarhythmicVolume(fxVolumeSlider.value));
        fxVolumeText.text = $"{Mathf.Round(fxVolumeSlider.value * 100)}%";
    }
    public void MasterVolumeValueChange()
    {
        mixer.SetFloat("MasterVolume", LogarhythmicVolume(masterVolumeSlider.value));
        masterVolumeText.text = $"{Mathf.Round(masterVolumeSlider.value * 100)}%";
    }

    public void BrightnessValueChange()
    {
        FindObjectOfType<LightBrightnessAdjust>().LightAdjust();
        string plus = "";
        if (brightnessSlider.value >= 0)
            plus = "+";
        brightnessText.text = $"{plus}{Mathf.Round(brightnessSlider.value * 100)}%";
    }


    private float LogarhythmicVolume(float input)
    {
        int constans = 20;
        return Mathf.Log10(input) * constans;
    }
}
