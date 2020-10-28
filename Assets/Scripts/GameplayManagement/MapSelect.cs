using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapSelect : MonoBehaviour
{
   [Tooltip("List of scenes that have been added to the build settings and can be cylced through as playable maps/scenes")]
    [SerializeField]
    private List<string> _mapNames = null;

    [SerializeField]
    private Button _nextMap = null;

    [SerializeField]
    private Text _mapNameText = null;

    private int _mapIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        _nextMap.onClick.AddListener(CycleMap);
        _mapNameText.text = _mapNames[_mapIndex];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CycleMap()
    {
        _mapIndex++;
        if (_mapIndex == _mapNames.Count)
        {
            _mapIndex = 0;
        }
        _mapNameText.text = _mapNames[_mapIndex];
        SelectMap(_mapNames[_mapIndex]);
    }
    public void SelectMap(string name)
    {
        GetComponent<NetworkManager>().onlineScene = name;
    }
}
