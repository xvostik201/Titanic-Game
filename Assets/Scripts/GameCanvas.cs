using System;
using TMPro;
using UnityEngine;

public class GameCanvas : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EngineTelegraph _telegraph;
    [SerializeField] private SteeringWheel   _steeringWheel;
    [SerializeField] private Engine   _engine;

    [Header("UI References")]
    [SerializeField] private TMP_Text _engineStatsText;
    [SerializeField] private TMP_Text _steeringWheelStatsText;

    private float _currentThrottle;
    private float _currentRPM;
    private float _currentSpeed;
    
    private float _currentSteerFraction;

    private void OnEnable()
    {
        _telegraph.OnThrottleChanged              += ThrottleChanged;
        _steeringWheel.OnSteeringChangedNormalized += SteeringChanged;
        _engine.OnRPMChanged += RPMChanged;
        _engine.OnSpeedChanged += SpeedChanged;
    }

    private void OnDisable()
    {
        _telegraph.OnThrottleChanged               -= ThrottleChanged;
        _steeringWheel.OnSteeringChangedNormalized -= SteeringChanged;
        _engine.OnRPMChanged -= RPMChanged;
        _engine.OnSpeedChanged -= SpeedChanged;
    }

    private void ThrottleChanged(float throttle)
    {
        _currentThrottle = throttle;
        UpdateEngineStatsText();
    }
    private void RPMChanged(float rpm)
    {
        _currentRPM = rpm;
        UpdateEngineStatsText();
    }
    private void SpeedChanged(float speed)
    {
        _currentSpeed = speed;
        UpdateEngineStatsText();
    }

    private void SteeringChanged(float steerFraction)
    {
        _currentSteerFraction = steerFraction;
        UpdateSteeringWheelStatsText();
    }

    private void UpdateEngineStatsText()
    {
        _engineStatsText.text =
            $"Engine stats:\n" +
            $"• Throttle: {_currentThrottle:F2}\n" +
            $"• RPM: {_currentRPM:F2}\n" +
            $"• Current speed: {_currentSpeed:F2}\n";

    }

    private void UpdateSteeringWheelStatsText()
    {
        _steeringWheelStatsText.text =
            $"Steering wheel:\n" +
            $"• Angle fraction: {_currentSteerFraction:F2}";
    }
}