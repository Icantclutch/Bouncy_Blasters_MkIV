using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamemodeSpecificSettings : MonoBehaviour
{
    [SerializeField]
    private Dropdown _currentGamemode;

    [SerializeField]
    private Dropdown _maxScore;

    [SerializeField]
    private Dropdown _overchargeTime;

    [SerializeField]
    private GameObject _overchargeLabel;

    private List<string> _tdmScores = new List<string> { "15", "30", "45" };

    private List<string> _overchargeScores = new List<string> { "150", "200", "250" };

    // Start is called before the first frame update
    void Start()
    {
        _currentGamemode.onValueChanged.AddListener(delegate
        {
            ChangeGameSettings();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ChangeGameSettings()
    {
        if (_currentGamemode.value == 0)
        {
            _maxScore.ClearOptions();
            _maxScore.AddOptions(_tdmScores);
            _overchargeLabel.SetActive(false);
        }
        else if (_currentGamemode.value == 1)
        {
            _overchargeLabel.SetActive(true);
            _maxScore.ClearOptions();
            _maxScore.AddOptions(_overchargeScores);
            
        }
    }
}
