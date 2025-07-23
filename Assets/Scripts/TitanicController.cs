using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TitanicController : MonoBehaviour
{
    [Header("Steering")]
    [SerializeField] private float _maxSteerTorque = 1000f;
    [SerializeField] private float _velocityAlignSpeed = 30f;

    [Header("Debug rays")]
    [SerializeField] private float _rayDistance = 1000f;

    [Header("References")]
    [SerializeField] private Engine _engine;
    [SerializeField] private SteeringWheel _wheel;

    private float _currentTorque;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _engine.ApplyEngineForce();
        AlignVelocityToHeading();

        Debug.DrawRay(transform.position, transform.up * _rayDistance, Color.green);
        Debug.DrawRay(transform.position, _rb.velocity.normalized * _rayDistance, Color.red);
    }

    public void ApplySteering(float steerFraction)
    {
        _currentTorque = steerFraction * _maxSteerTorque;
        _rb.AddTorque(transform.up * -_currentTorque, ForceMode.Force);
    }
    private void AlignVelocityToHeading()
    {
        if (_rb.velocity.sqrMagnitude < 0.01f) return;

        Vector3 currentDir = _rb.velocity.normalized;
        Vector3 targetDir = transform.up;
        float maxRadians = _velocityAlignSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime;
        Vector3 newDir = Vector3.RotateTowards(currentDir, targetDir, maxRadians, 0f);
        _rb.velocity = newDir * _rb.velocity.magnitude;
    }
    private void OnEnable()
    {
        _wheel.OnSteeringChangedNormalized += ApplySteering;
    }

    private void OnDisable()
    {
        _wheel.OnSteeringChangedNormalized -= ApplySteering;
    }
}