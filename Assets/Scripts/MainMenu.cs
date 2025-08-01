using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button[] _allButtons;
    [SerializeField] private GameObject _tutorialGO;
    void Start()
    {
        _allButtons[0].onClick.AddListener(()=>SceneLoader.LoadScene(1));
        
        _allButtons[1].transform.DOScale(Vector3.one * 1.05f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        _allButtons[1].onClick.AddListener(() => _tutorialGO.SetActive(true));
        
        _allButtons[2].onClick.AddListener(Application.Quit);
    }

    
}
