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

    private float _currentRPM;
    private float _currentThrottle;
    private float _targetThrottle;

    void Start()
    {
        _currentThrottle = _targetThrottle = 1f;
        _currentRPM = _maxRPM;
    }

    public void SetThrottle(float t)
    {
        _targetThrottle = Mathf.Clamp01(t);
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
        float newSpeed = Mathf.MoveTowards(curSpeed, speedTarget, delta);

        if (curSpeed > 0.01f)
            rb.velocity = v.normalized * newSpeed;
        else
            rb.velocity = transform.up * newSpeed;

    }
}
