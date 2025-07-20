using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TitanicController : MonoBehaviour
{
    [Header("Steering")]
    [SerializeField] private float _maxRudderAngle = 30f;

    [Header("References")]
    [SerializeField] private Engine _engine;
    [SerializeField] private SteeringWheel _wheel;

    private float _currentRudderAngle;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _currentRudderAngle = 0f;
    }

    private void FixedUpdate()
    {
        _engine.ApplyEngineForce(_rb);
        ApplySteering();
    }

    public void ApplySteering()
    {
        _currentRudderAngle = _maxRudderAngle * (_wheel.CurrentAngle / _wheel.MaxSteerAngle);

        _rb.AddTorque(transform.up * _currentRudderAngle, ForceMode.Force);
    }

    private void OnTriggerEnter(Collider other)
    {
        var iceberg = other.GetComponentInParent<Iceberg>();
        if (iceberg != null)
        {
            Time.timeScale = 0f;
            Debug.LogWarning("GAME OVER, YOU HIT AN ICEBERG");
        }
    }
}