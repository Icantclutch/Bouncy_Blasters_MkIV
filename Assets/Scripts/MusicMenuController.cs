using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicMenuController : MonoBehaviour
{
    private static MusicMenuController musicMenu = null;
    public AudioSource music;
    public AudioListener listener;
    // Start is called before the first frame update
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (musicMenu != null && musicMenu != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            musicMenu = this;
        }
        DontDestroyOnLoad(this.gameObject);

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "The Main Event" ||
            SceneManager.GetActiveScene().name == "Building Brawl" ||
            SceneManager.GetActiveScene().name == "Angle Alley" ||
            SceneManager.GetActiveScene().name == "Maze Mayhem" ||
            SceneManager.GetActiveScene().name == "Super Nova" ||
            SceneManager.GetActiveScene().name == "Tutorial2")
        {
            music.mute = true;
            listener.enabled = false;
        }
        else
        {
            music.mute = false;
            listener.enabled = true;
        }
    }
}
