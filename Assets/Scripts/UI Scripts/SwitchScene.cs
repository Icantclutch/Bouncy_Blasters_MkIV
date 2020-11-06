﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SwitchScene : MonoBehaviour
{
    [SerializeField]
    private string _sceneName;

    [SerializeField]
    private Button _menuButton;

    // Start is called before the first frame update
    void Start()
    {
        _menuButton.onClick.AddListener(SwapScene);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SwapScene()
    {
        SceneManager.LoadScene(_sceneName);
    }
}
