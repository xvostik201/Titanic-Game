using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sound Data Assets")]
    [SerializeField] private SoundData[] _soundDatas;

    [Header("Sound prefab")]
    [SerializeField] private GameObject _audioSourcePrefab;

    [Header("Range settings")]
    [SerializeField] private float _nearMin = 1f, _nearMax = 5f;
    [SerializeField] private float _normalMin = 5f, _normalMax = 15f;
    [SerializeField] private float _farMin = 15f, _farMax = 30f;

    private Dictionary<string, SoundData> _soundDict;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _soundDict = new Dictionary<string, SoundData>(_soundDatas.Length);
            foreach (var sd in _soundDatas)
            {
                if (!_soundDict.ContainsKey(sd.soundName))
                    _soundDict.Add(sd.soundName, sd);
                else
                    Debug.LogWarning($"[AudioManager] Duplicate SoundData name: {sd.soundName}");
            }
        }
        else
            Destroy(gameObject);
    }

    public void Play(string soundName, Vector3 position, Transform parent = null, bool hasParent = true)
    {
        if (!_soundDict.TryGetValue(soundName, out var sd))
        {
            Debug.LogWarning($"[AudioManager] Sound «{soundName}» not found!");
            return;
        }
        
        var go = Instantiate(_audioSourcePrefab, position, Quaternion.identity);
        var src = go.GetComponent<AudioSource>();
        
        if(hasParent) go.transform.parent = parent.transform;

        src.clip = sd.clip;
        src.volume = sd.volume;
        src.loop = sd.loop;
        src.spatialBlend = 1f;
        src.rolloffMode = AudioRolloffMode.Linear;

        switch (sd.distanceType)
        {
            case SoundDistanceType.Near:
                src.minDistance = _nearMin;
                src.maxDistance = _nearMax;
                break;
            case SoundDistanceType.Normal:
                src.minDistance = _normalMin;
                src.maxDistance = _normalMax;
                break;
            case SoundDistanceType.Far:
                src.minDistance = _farMin;
                src.maxDistance = _farMax;
                break;
        }

        src.Play();

        if (!sd.loop)
            Destroy(go, sd.clip.length / src.pitch);
    }
}
