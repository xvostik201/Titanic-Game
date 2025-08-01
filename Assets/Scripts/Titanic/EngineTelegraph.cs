using System;
using UnityEngine;

public class EngineTelegraph : MonoBehaviour
{
    [SerializeField] private float _pixelsPerStep = 45f;
    [SerializeField] private float _hysteresis = 8f;

    [SerializeField] private float[] _angles = { -68f, -35f, 0f, 35f, 68f };
    [SerializeField] private float _moveSpeed = 40f;
    [SerializeField] private float _minPlayInterval = 0.1f;

    public event Action<float> OnThrottleChanged;

    private float _lastPlayTime = -999f;
    private Vector3 _mouseDownPos;
    private bool _isDragging;

    private int _currentIndex;
    private int _targetIndex;
    private int _startIndex;

    private float _currentAngle;
    private float _targetAngle;

    private readonly float[] _throttles = { 1f, 0.5f, 0f, -0.5f, -1f };
    private float _lastNotifiedThrottle = float.NaN;

    private void Start()
    {
        _currentIndex = _targetIndex = 0;
        _currentAngle = _targetAngle = _angles[_currentIndex];
        transform.localRotation = Quaternion.Euler(_currentAngle, 0f, 0f);
        _lastNotifiedThrottle = _throttles[_currentIndex];
        OnThrottleChanged?.Invoke(_lastNotifiedThrottle);
    }

    private void OnMouseDown()
    {
        _isDragging = true;
        _mouseDownPos = Input.mousePosition;
        _startIndex = _targetIndex;
    }

    private void OnMouseDrag()
    {
        if (!_isDragging) return;

        float dx = Input.mousePosition.x - _mouseDownPos.x;
        int steps = Mathf.Abs(dx) > _hysteresis ? (int)Mathf.Floor(Mathf.Abs(dx) / _pixelsPerStep) * Math.Sign(dx) : 0;
        int newIndex = Mathf.Clamp(_startIndex + steps, 0, _angles.Length - 1);

        if (newIndex != _targetIndex)
            ApplyIndex(newIndex);
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

    private void ApplyIndex(int idx)
    {
        _targetIndex = idx;
        _currentIndex = idx;
        _targetAngle = _angles[idx];

        float throttle = _throttles[idx];
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
