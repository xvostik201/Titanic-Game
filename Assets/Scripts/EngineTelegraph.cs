using System;
using UnityEngine;

public class EngineTelegraph : MonoBehaviour
{
    [Header("Throttle thresholds (pixels)")]
    [SerializeField] private float _slowThreshold = 50f;
    [SerializeField] private float _fullThreshold = 100f;

    [Header("Engine telegraph angles")]
    [SerializeField] private float _angleFullAhead   = -68f;
    [SerializeField] private float _angleSlowAhead   = -35f;
    [SerializeField] private float _angleStop        =   0f;
    [SerializeField] private float _angleSlowAstern  =  35f;
    [SerializeField] private float _angleFullAstern  =  68f;

    [Header("Tween speed")]
    [SerializeField] private float _moveSpeed = 40f;

    [Header("Switch sound")]
    [SerializeField] private float _minPlayInterval = 0.1f;
    public event Action<float> OnThrottleChanged;

    private float _lastPlayTime = -999f;
    private Vector3 _mouseDownPos;
    private bool    _isDragging;
    private float   _currentAngle;
    private float   _targetAngle;
    private float   _lastNotifiedThrottle = float.NaN;

    private float _startAngle;
    private float _startThrottle;

    private void Start()
    {
        transform.localRotation   = Quaternion.Euler(_angleFullAhead, 0f, 0f);
        _currentAngle             = _targetAngle = _angleFullAhead;
        _lastNotifiedThrottle     = 1f;
        OnThrottleChanged?.Invoke(1f);
    }

    private void OnMouseDown()
    {
        _isDragging     = true;
        _mouseDownPos   = Input.mousePosition;

        _startAngle     = _targetAngle;
        _startThrottle  = _lastNotifiedThrottle;
    }

    private void OnMouseDrag()
    {
        if (!_isDragging) return;

        float dx = Input.mousePosition.x - _mouseDownPos.x;

        float newAngle    = _startAngle;
        float newThrottle = _startThrottle;

        if (dx <= -_fullThreshold)       { newAngle = _angleFullAhead;   newThrottle =  1f;  }
        else if (dx <= -_slowThreshold)  { newAngle = _angleSlowAhead;   newThrottle =  0.5f;}
        else if (dx >=  _fullThreshold)  { newAngle = _angleFullAstern;  newThrottle = -1f;  }
        else if (dx >=  _slowThreshold)  { newAngle = _angleSlowAstern;  newThrottle = -0.5f;}

        ApplyState(newAngle, newThrottle);
    }

    private void OnMouseUp()
    {
        _isDragging = false;
    }

    private void Update()
    {
        if (!Mathf.Approximately(_currentAngle, _targetAngle))
        {
            _currentAngle = Mathf.MoveTowards(_currentAngle, _targetAngle, _moveSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Euler(_currentAngle, 0f, 0f);
        }
    }

    private void ApplyState(float angle, float throttle)
    {
        _targetAngle = angle;

        if (!Mathf.Approximately(throttle, _lastNotifiedThrottle))
        {
            _lastNotifiedThrottle = throttle;
            OnThrottleChanged?.Invoke(throttle);
            TryPlaySwitchSound();
        }
    }

    private void TryPlaySwitchSound()
    {
        if (Time.time - _lastPlayTime < _minPlayInterval) return;
        _lastPlayTime = Time.time;
        AudioManager.Instance.Play("EngineTelegraphSwitcher", transform.position, true, transform);
    }
}
