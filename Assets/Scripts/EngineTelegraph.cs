using System;
using UnityEngine;

public class EngineTelegraph : MonoBehaviour
{
    [Header("Throttle thresholds (pixels)")]
    [SerializeField] private float _slowThreshold = 50f;
    [SerializeField] private float _fullThreshold = 100f;

    [Header("Engine telegraph angles")]
    [SerializeField] private float _angleFullAhead = -68f;
    [SerializeField] private float _angleSlowAhead = -35f;
    [SerializeField] private float _angleStop = 0f;
    [SerializeField] private float _angleSlowAstern = 35f;
    [SerializeField] private float _angleFullAstern = 68f;

    [Header("Engine telegraph angle move speed")]
    [SerializeField] private float _moveSpeed = 40f;

    public event Action<float> OnThrottleChanged;

    private Vector3 _mouseDownPos;
    private bool _isDragging;
    private float _currentAngle;
    private float _targetAngle;
    private float _currentThrottle;

    private void Start()
    {
        transform.localRotation = Quaternion.Euler(_angleFullAhead, 0f, 0f);
    }
    private void OnMouseDown()
    {
        _isDragging = true;
        _mouseDownPos = Input.mousePosition;
    }

    private void OnMouseDrag()
    {
        if (!_isDragging) return;

        float deltaX = Input.mousePosition.x - _mouseDownPos.x;
        float newAngle, newThrottle;

        if (deltaX <= -_fullThreshold)
        {
            newAngle = _angleFullAhead;
            newThrottle = 1f;
        }
        else if (deltaX <= -_slowThreshold)
        {
            newAngle = _angleSlowAhead;
            newThrottle = 0.5f;
        }
        else if (deltaX >= _fullThreshold)
        {
            newAngle = _angleFullAstern;
            newThrottle = -1f;
        }
        else if (deltaX >= _slowThreshold)
        {
            newAngle = _angleSlowAstern;
            newThrottle = -0.5f;
        }
        else
        {
            newAngle = _angleStop;
            newThrottle = 0f;
        }

        _targetAngle = newAngle;
        _currentThrottle = newThrottle;
        OnThrottleChanged?.Invoke(_currentThrottle);
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
}
