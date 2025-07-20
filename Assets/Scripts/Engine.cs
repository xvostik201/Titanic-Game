using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    [Header("Engine")]
    [SerializeField] private float _maxSpeed = 39f;
    [SerializeField] private float _maxRPM = 75f;
    [SerializeField] private float _rpmAcceleration = 0.41f;

    public float MaxSpeed => _maxSpeed;


    [Header("Throttle")]
    [SerializeField] private float _throttleResponseSpeed = 1f;

    private float _currentRPM;
    private float _currentThrottle;
    private float _targetThrottle;
    
    private float _currentSpeed;
    void Start()
    {
        _currentThrottle = 1f;
        _targetThrottle = 1f;
        _currentRPM = _maxRPM;
    }

    void Update()
    {
        float targetRPM = _currentThrottle * _maxRPM;
        _currentRPM = Mathf.MoveTowards(
            _currentRPM,
            targetRPM,
            _rpmAcceleration * Time.deltaTime
        );
    }
    public void SetThrottle(float throttle)
    {
        _targetThrottle = throttle;
    }
    public void ApplyEngineForce(Rigidbody rb)
    {
        _currentThrottle = Mathf.MoveTowards(
           _currentThrottle,
           _targetThrottle,
           _throttleResponseSpeed * Time.fixedDeltaTime
       );

        _currentSpeed = _maxSpeed * (_currentRPM / _maxRPM);

        if (rb.velocity.magnitude >= _currentSpeed)
            return;

        rb.AddForce(
            rb.gameObject.transform.up * _currentSpeed,
            ForceMode.Force
        );

    }
}
