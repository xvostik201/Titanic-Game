using System;
using UnityEngine;

public class Engine : MonoBehaviour
{
    [Header("Engine")]
    [SerializeField] private float _maxSpeed = 39f;
    [SerializeField] private float _maxRPM = 75f;
    [SerializeField] private float _rpmAcceleration = 0.41f;

    [Header("Throttle")]
    [SerializeField] private float _throttleResponseSpeed = 1f;

    [Header("Thrust")]
    [SerializeField] private float _maxAcceleration = 10f;

    [Header("Reference")]
    [SerializeField] private EngineTelegraph _telegraph;
    
    public event Action<float> OnSpeedChanged;
    public event Action<float> OnRPMChanged;
    
    private float _currentRPM;
    private float _currentSpeed;
    private float _currentThrottle;
    private float _targetThrottle;

    void Start()
    {
        _currentThrottle = _targetThrottle = 1f;
        _currentRPM = _maxRPM;
    }

    public void SetThrottle(float t)
    {
        _targetThrottle = Mathf.Clamp(t, -1f, 1f);
    }
    public void ApplyEngineForce(Rigidbody rb)
    {
        _currentThrottle = Mathf.MoveTowards(
            _currentThrottle,
            _targetThrottle,
            _throttleResponseSpeed * Time.fixedDeltaTime
        );

        float desiredRPM = _currentThrottle * _maxRPM;
        _currentRPM = Mathf.MoveTowards(
            _currentRPM,
            desiredRPM,
            _rpmAcceleration * Time.fixedDeltaTime
        );

        float speedTarget = _maxSpeed * (_currentRPM / _maxRPM);

        Vector3 v = rb.velocity;
        float curSpeed = v.magnitude;
        float delta = _maxAcceleration * Time.fixedDeltaTime;
        _currentSpeed  = Mathf.MoveTowards(curSpeed, speedTarget, delta);

        if (curSpeed > 0.01f)
            rb.velocity = v.normalized * _currentSpeed;
        else
            rb.velocity = transform.up * _currentSpeed;
        
        OnSpeedChanged?.Invoke(_currentSpeed);
        OnRPMChanged?.Invoke(_currentRPM);
    }
    private void OnEnable()
    {
        _telegraph.OnThrottleChanged += SetThrottle;
    }

    private void OnDisable()
    {
        _telegraph.OnThrottleChanged -= SetThrottle;
    }
}
