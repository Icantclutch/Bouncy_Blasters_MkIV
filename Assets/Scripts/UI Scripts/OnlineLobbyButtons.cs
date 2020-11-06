using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlineLobbyButtons : MonoBehaviour
{
   
    [SerializeField]
    private Button _nextMapButton;
    [SerializeField]
    private Text _mapName;

    [SerializeField]
    private Button _nextGamemodeButton;
    [SerializeField]
    private Text _gamemodeName;
    private int _gamemodeInt = 0;

    [SerializeField]
    private Button _startMatchButton;

    private NetworkManager _networkManager;


    // Start is called before the first frame update
    void Start()
    {
        _startMatchButton.interactable = false;
        _startMatchButton.onClick.AddListener(StartMatch);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_networkManager)
        {
            _networkManager = GameObject.FindGameObjectWithTag("Management").GetComponent<NetworkManager>();
        }
        else
        {
            _startMatchButton.interactable = true;
        }

    }

    private void StartMatch()
    {
        _networkManager.GetComponent<LobbyManager>().SetMap(_mapName.text);
        _networkManager.GetComponent<LobbyManager>().SetGamemode(_gamemodeInt, 30, 0, 420);
    }

}
