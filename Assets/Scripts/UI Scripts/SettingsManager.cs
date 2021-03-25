using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{

    public AudioMixer masterMixer;
    public Slider VolumeSlider;
    public Dropdown QualityDrop;
    public Toggle FullscreenToggle;
    public Slider opacitySlider;
    public RawImage minimap;

    private int videoQuality;
    private float currVolume;
    private int windowType;
    private float currSlider;
    

    Resolution[] resolutionsList;
    public Dropdown resolutionsDrop;
    private void Start()
    {
        resolutionsList = Screen.resolutions;
        resolutionsDrop.ClearOptions();

        int currentResolution = 0;

        List<string> options = new List<string>();
        for(int i = 0; i < resolutionsList.Length; i++)
        {
            string option = resolutionsList[i].width + "x" + resolutionsList[i].height;
            options.Add(option);

            if (resolutionsList[i].width == Screen.currentResolution.width &&
                resolutionsList[i].height == Screen.currentResolution.height)
            {
                currentResolution = i;
            }
        }


        resolutionsDrop.AddOptions(options);
        resolutionsDrop.value = currentResolution;
        resolutionsDrop.RefreshShownValue();

        videoQuality = PlayerPrefs.GetInt("quality");
        currVolume = PlayerPrefs.GetFloat("volume");
        windowType = PlayerPrefs.GetInt("fullscreen");

        masterMixer.SetFloat("MasterVolume", currVolume);
        VolumeSlider.value = currVolume;

       

        QualitySettings.SetQualityLevel(videoQuality);
        QualityDrop.value = videoQuality;


        if(windowType == 1)
        {
            Screen.fullScreen = true;
            FullscreenToggle.isOn = true;
        }
        else
        {
            Screen.fullScreen = false;
            FullscreenToggle.isOn = false;
        }

    }

    private void FixedUpdate()
    {
        PlayerPrefs.SetInt("quality", videoQuality);
        PlayerPrefs.SetInt("fullscreen", windowType);
        PlayerPrefs.SetFloat("volume", currVolume);


        if (windowType == 1)
        {
            Screen.fullScreen = true;
        }
        else
        {
            Screen.fullScreen = false;
        }
        masterMixer.SetFloat("MasterVolume", currVolume);
        QualitySettings.SetQualityLevel(videoQuality);
    }


    public void setOpacity(float mapSlider)
    {
        currSlider = mapSlider;
        minimap.color = new Color(255.0f, 255.0f, 255.0f, currSlider);
        //opacitySlider.value = currSlider;
    }

    public void SetMasterVolume (float masterVolume)
    { 
        currVolume = masterVolume;
    }

    public void SetQuality(int quality)
    {
        videoQuality = quality;
    }

    public void WindowMode(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        if (isFullscreen)
        {
            windowType = 1;
        }
        else
        {
            windowType = 0;
        }
    }

    public void OnButtonPress()
    {
        SceneManager.LoadScene("Title Screen");
    }

}
