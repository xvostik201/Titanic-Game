using UnityEngine;

public class IcebergShardActivator : MonoBehaviour
{
    [SerializeField] float _activationRadius = 3f;
    [SerializeField] float _explosionForce = 8f;
    [SerializeField] float _explosionUpwards = 1f;
    [SerializeField] private Rigidbody[] _shardBodies;
    private bool _activated;

    private void Awake()
    {
        foreach (var rb in _shardBodies) rb.isKinematic = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (_activated) return;
        if (!other.TryGetComponent<TitanicController>(out _)) return;
        _activated = true;
        Vector3 hitPoint = other.ClosestPoint(transform.position);
        Collider[] cols = Physics.OverlapSphere(hitPoint, _activationRadius);
        foreach (var col in cols)
        {
            var rb = col.attachedRigidbody;
            if (rb != null && rb.isKinematic)
            {
                rb.isKinematic = false;
                rb.AddExplosionForce(_explosionForce, hitPoint, _activationRadius, _explosionUpwards, ForceMode.Impulse);
            }
        }
    }
}
