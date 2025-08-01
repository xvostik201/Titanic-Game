using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject[] _allTutorialsGO;
    [SerializeField] private Button _nextTutorialButton;
    [SerializeField] private Button _exitTutorialButton;
    private int _currentButtonIndex = 0;
    void Start()
    {
        _nextTutorialButton.onClick.AddListener(SwitchTutorialGO);
        _exitTutorialButton.onClick.AddListener(EndTutorial);
    }

    private void EndTutorial()
    {
        SwitchTutorialGO();
        gameObject.SetActive(false);
    }

    private void SwitchTutorialGO()
    {
        _currentButtonIndex =  (_currentButtonIndex + 1) % _allTutorialsGO.Length;
        for (int i = 0; i < _allTutorialsGO.Length; i++)
        {
            _allTutorialsGO[i].SetActive(false);
            _exitTutorialButton.gameObject.SetActive(false);
        }
        if(_currentButtonIndex == _allTutorialsGO.Length - 1)
            _exitTutorialButton.gameObject.SetActive(true);
        _allTutorialsGO[_currentButtonIndex].SetActive(true);
    }
}
