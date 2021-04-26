﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
//using UnityEngine.Assertions;
//using NUnit.Framework;
using UnityEngine.SocialPlatforms;

[System.Serializable]
public class SettingsManager : MonoBehaviour, ISaveable
{

    public AudioMixerGroup musicMixer;
    public AudioMixerGroup SFXMixer;
    public AudioMixer MasterMixer;
    public Slider VolumeSlider;
    public Slider SFXSlider;
    public Dropdown QualityDrop;
    public Toggle FullscreenToggle;

    public Slider SensitiviySlider;

    private int videoQuality;
    private float currMusicVolume;
    private float currSFXVolume;
    private int windowType;
    private float sensitivity;

    public Slider opacitySlider;
    public RawImage minimap;
    

    Resolution[] resolutionsList;
    public Dropdown resolutionsDrop;

    //Auto-generated to fix compiler error
    private float currSlider;

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

          currMusicVolume = PlayerPrefs.GetFloat("MusicVolume", currMusicVolume);
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


        //Debug.Log(PlayerPrefs.GetFloat("MusicVolume", currMusicVolume));
        musicMixer.audioMixer.SetFloat("MusicVolume", currMusicVolume);
        VolumeSlider.value = currMusicVolume;
       // Debug.Log(PlayerPrefs.GetFloat("MusicVolume", currMusicVolume));

        SFXMixer.audioMixer.SetFloat("SFXVolume", currSFXVolume);
        SFXSlider.value = currSFXVolume;




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

    public void SetMusicVolume (float musicVolume)
    {
        VolumeSlider.value = currMusicVolume;
        currMusicVolume = musicVolume;
        musicMixer.audioMixer.SetFloat("MusicVolume", currMusicVolume);

    }

    
    public void SetSFXVolume(float sfxVolume)
    {
        SFXSlider.value = currSFXVolume;
        currSFXVolume = sfxVolume;
        SFXMixer.audioMixer.SetFloat("SFXVolume", currSFXVolume);

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

    //[Test]
    //public void LocationsClose(Vector3 pos1, Vector3 pos2)
    //{
    //    float dist = Vector3.Distance(pos1, pos2);
    //    bool close = false;
    //    if (dist < 5.0f)
    //    {
    //        close = true;
    //    }
    //    NUnit.Framework.Assert.IsTrue(close);
    //}
    ////Testing for settings manager
    //[Test]
    //public void DataCheck()
    //{
    //    //Get the current Settings Manager
    //    SettingsManager tester = this;
    //    SaveData sd = new SaveData();
    //    //Chance current data to 5
    //    currVolume = 5;
    //    float testVol = 10;
    //    tester.PopulateSaveData(sd);

    //    //Set the volume through the player prefab process
    //    PlayerPrefs.SetFloat("masterVolume", currVolume);
    //    LoadFromSaveData(sd);
    //    NUnit.Framework.Assert.AreEqual(sd.currVolume, testVol);
    //}


    //[UnityTest]
    //public IEnumerator LocationsClose(Vector3 pos1, Vector3 pos2)
    //{
    //    float dist = Vector3.Distance(pos1, pos2);
    //    bool close = false;

    //    if (dist < 5.0f)
    //    {
    //        close = true;
    //    }

    //    yield return new WaitForFixedUpdate();
    //    Assert.IsTrue(close);
    //}

    public void OnButtonPress()
    {
        SaveJsonData(this);
        PlayerPrefs.SetFloat("MusicVolume", currMusicVolume);
        PlayerPrefs.SetInt("fullscreen", windowType);
        PlayerPrefs.SetInt("qualitylevel", videoQuality);
        StaticClass.CrossSceneInformation = "Title Screen";
        SceneManager.LoadScene("Loading Screen");
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
        }
    }

    public void PopulateSaveData(SaveData a_SaveData)
    {
        a_SaveData.videoQuality = videoQuality;
        a_SaveData.windowType = windowType;
        a_SaveData.currMusicVolume = currMusicVolume;
        a_SaveData.sensitivity = sensitivity;
    }

    public void LoadFromSaveData(SaveData a_SaveData)
    {
        videoQuality = a_SaveData.videoQuality;
        windowType = a_SaveData.windowType;
        currMusicVolume = a_SaveData.currMusicVolume;
        sensitivity = a_SaveData.sensitivity;
    }
}
