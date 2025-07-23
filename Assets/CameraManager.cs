using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera[] _allCameras;
    private AudioListener[] _audioListeners; 
    private int _currentCameraIndex = 0;

    private void Awake()
    {
        _audioListeners = new AudioListener[_allCameras.Length];
        for (int i = 0; i < _allCameras.Length; i++)
            _audioListeners[i] = _allCameras[i].gameObject.GetComponent<AudioListener>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchCamera(true);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchCamera(false);
        }
    }

    private void SwitchCamera(bool plus)
    {
        int dir = plus ? 1 : -1;
        _currentCameraIndex = (_currentCameraIndex + dir + _allCameras.Length) % _allCameras.Length;

        for (int i = 0; i < _allCameras.Length; i++)
        {
            _allCameras[i].enabled = i == _currentCameraIndex;
            _audioListeners[i].enabled = i == _currentCameraIndex;
        }
    }
}
