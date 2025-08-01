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
            $"Статистика двигателя:\n" +
            $"• Газ: {ThrottleToString(_currentThrottle)}\n" +
            $"• Обороты винтов: {_currentRPM:F1} в минуту\n" +
            $"• Текущая скорость: {_currentSpeed:F1} км/ч\n";

    }

    private string ThrottleToString(float throttle)
    {
        switch (throttle)
        {
            case 1f:
                return "Полный вперед";
            case 0.5f:
                return "Вперед";
            case 0f:
                return "Стоп";
            case -0.5f:
                return "Назад";
            case -1f:
                return "Полный назад";
            default:
                return "None throttle";
        }
    }

    private void UpdateSteeringWheelStatsText()
    {
        _steeringWheelStatsText.text =
            $"Статистика руля:\n" +
            $"• Поворот руля: {_currentSteerFraction:F2}";
    }
}