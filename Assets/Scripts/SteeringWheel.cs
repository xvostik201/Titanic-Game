using System;
using UnityEngine;

public class SteeringWheel : MonoBehaviour
{

    [SerializeField] private float _maxSteerAngle = 1800f;
    [SerializeField] private float _activationDistance = 0.05f;
    [SerializeField] private float _inertiaDamping = 5f;
    [SerializeField] private float _maxDeltaPerFrame = 10f;

    public float MaxSteerAngle => _maxSteerAngle;
    public float CurrentAngle => _currentAngle;

    private bool _isDragging;
    private bool _hasStarted;
    private Vector3 _initPos;
    private Vector3 _lastDir;
    private float _currentAngle;
    private float _inertiaSpeed;

    public event Action<float> OnSteeringChangedNormalized;

    private void Update()
    {
        float steerAmount = 0f;

        if (_isDragging)
        {
            Vector3 world = GetMouseWorld();
            if (!_hasStarted)
            {
                if (Vector3.Distance(world, _initPos) > _activationDistance)
                {
                    _hasStarted = true;
                    _lastDir = (world - transform.position).normalized;
                }
                else return;
            }

            Vector3 dir = (world - transform.position).normalized;
            float raw = Vector3.SignedAngle(_lastDir, dir, transform.forward);
            raw = Mathf.Clamp(raw, -_maxDeltaPerFrame, _maxDeltaPerFrame);

            _currentAngle = Mathf.Clamp(_currentAngle + raw, -_maxSteerAngle, _maxSteerAngle);
            transform.localRotation = Quaternion.Euler(90f, 0f, _currentAngle);

            steerAmount = raw;
            _inertiaSpeed = raw;
            _lastDir = dir;
        }
        else if (Mathf.Abs(_inertiaSpeed) > 0.01f)
        {
            float raw = _inertiaSpeed;
            _currentAngle = Mathf.Clamp(_currentAngle + raw, -_maxSteerAngle, _maxSteerAngle);
            transform.localRotation = Quaternion.Euler(90f, 0f, _currentAngle);

            steerAmount = raw;
            _inertiaSpeed = Mathf.Lerp(_inertiaSpeed, 0f, Time.deltaTime * _inertiaDamping);
        }
        float steerFraction = _currentAngle / _maxSteerAngle;
        OnSteeringChangedNormalized?.Invoke(steerFraction);

        if (Input.GetMouseButtonUp(0))
            _isDragging = false;
    }

    private void OnMouseDown()
    {
        _isDragging = true;
        _hasStarted = false;
        _inertiaSpeed = 0f;
        _initPos = GetMouseWorld();
    }

    private Vector3 GetMouseWorld()
    {
        Vector3 m = Input.mousePosition;
        m.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(m);
    }
}