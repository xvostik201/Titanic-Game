using UnityEngine;
using UnityEngine.Events;

public class SoundEmitter : MonoBehaviour
{
    public enum TriggerEvent { OnStart, OnEnable, OnDisable, OnDestroy, OnTriggerEnter, OnCollisionEnter, Custom }

    [SerializeField] private string _soundName;
    [SerializeField] private TriggerEvent _trigger = TriggerEvent.OnStart;
    [SerializeField] private UnityEvent _onCustom;

    private void Start()
    {
        if (_trigger == TriggerEvent.OnStart)
            AudioManager.Instance.Play(_soundName, transform.position,true, transform);
    }

    private void OnEnable()
    {
        if (_trigger == TriggerEvent.OnEnable)
            AudioManager.Instance.Play(_soundName, transform.position,true, transform);
    }

    private void OnDisable()
    {
        if (_trigger == TriggerEvent.OnDisable)
            AudioManager.Instance.Play(_soundName, transform.position,true, transform);
    }

    private void OnDestroy()
    {
        if (_trigger == TriggerEvent.OnDestroy)
            AudioManager.Instance.Play(_soundName, transform.position,true, transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_trigger == TriggerEvent.OnTriggerEnter)
            AudioManager.Instance.Play(_soundName, transform.position,true, transform);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_trigger == TriggerEvent.OnCollisionEnter)
            AudioManager.Instance.Play(_soundName, transform.position,true, transform);
    }

    public void PlayCustom()
    {
        if (_trigger == TriggerEvent.Custom)
        {
            AudioManager.Instance.Play(_soundName, transform.position,true, transform);
            _onCustom.Invoke();
        }
    }
}