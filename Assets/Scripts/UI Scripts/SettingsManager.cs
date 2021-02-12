using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{

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
    }



    public AudioMixer masterMixer;
    public void SetMasterVolume (float masterVolume)
    {
        masterMixer.SetFloat("MasterVolume", masterVolume);
    }

    public void SetQuality(int quality)
    {
        QualitySettings.SetQualityLevel(quality);
    }

    public void WindowMode(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void OnButtonPress()
    {
        SceneManager.LoadScene("Title Screen");
    }

}
