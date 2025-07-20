using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TitanicController : MonoBehaviour
{
    [Header("Steering")]
    [SerializeField] private float _maxSteerTorque = 1000f;

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
        _engine.ApplyEngineForce(_rb);
        ApplySteering();
    }

    public void ApplySteering()
    {
        float steerFraction = _wheel.CurrentAngle / _wheel.MaxSteerAngle;
        _currentTorque = steerFraction * _maxSteerTorque;
        _rb.AddTorque(transform.up * _currentTorque, ForceMode.Force);
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