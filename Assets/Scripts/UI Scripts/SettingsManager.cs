using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SettingsManager : MonoBehaviour, ISaveable
{

    public AudioMixer masterMixer;
    public Slider VolumeSlider;
    public Dropdown QualityDrop;
    public Toggle FullscreenToggle;

    public Slider SensitiviySlider;

    private int videoQuality;
    private float currVolume;
    private int windowType;
    private float sensitivity;

    public Slider opacitySlider;
    public RawImage minimap;
    

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

        LoadJsonData(this);

          currVolume = PlayerPrefs.GetFloat("masterVolume", currVolume);
          windowType = PlayerPrefs.GetInt("fullscreen", windowType);
          videoQuality = PlayerPrefs.GetInt("qualitylevel", videoQuality);

        if (sensitivity == -100)
        {
            sensitivity = SensitiviySlider.value;
        }
        else
        {
            SensitiviySlider.value = sensitivity;
        }
        
        

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

        SaveJsonData(this);

    }


    public void setOpacity(float mapSlider)
    {
        currSlider = mapSlider;
        //minimap.color = new Color(255.0f, 255.0f, 255.0f, currSlider);
        //opacitySlider.value = currSlider;
    }

    public void SetMasterVolume (float masterVolume)
    {
        VolumeSlider.value = currVolume;
        currVolume = masterVolume;
        masterMixer.SetFloat("MasterVolume", currVolume);

    }

    public void SetQuality(int quality)
    {
        videoQuality = quality;
        QualitySettings.SetQualityLevel(videoQuality);

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
        SaveJsonData(this);
        PlayerPrefs.SetFloat("masterVolume", currVolume);
        PlayerPrefs.SetInt("fullscreen", windowType);
        PlayerPrefs.SetInt("qualitylevel", videoQuality);
        SceneManager.LoadScene("Title Screen");
    }


    public static void SaveJsonData(SettingsManager a_settingManager)
    {
        SaveData sd = new SaveData();
        a_settingManager.PopulateSaveData(sd);

        if(FileManager.WriteToFile("SaveData.dat", sd.ToJson()))
        {
           // Debug.Log("Save Successful");
        }
    }
    
    public static void LoadJsonData(SettingsManager a_settingManager)
    {
        if(FileManager.LoadFromFile("SaveData.dat", out var json))
        {
            SaveData sd = new SaveData();
            sd.LoadFromJson(json);

            a_settingManager.LoadFromSaveData(sd);
            Debug.Log("Load Complete");
        }
    }

    public void PopulateSaveData(SaveData a_SaveData)
    {
        a_SaveData.videoQuality = videoQuality;
        a_SaveData.windowType = windowType;
        a_SaveData.currVolume = currVolume;
        a_SaveData.sensitivity = sensitivity;
    }

    public void LoadFromSaveData(SaveData a_SaveData)
    {
        videoQuality = a_SaveData.videoQuality;
        windowType = a_SaveData.windowType;
        currVolume = a_SaveData.currVolume;
        sensitivity = a_SaveData.sensitivity;
    }
}
