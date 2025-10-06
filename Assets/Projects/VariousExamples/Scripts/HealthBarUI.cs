using System;
using UnityEngine;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField]
    private Transform _followTarget = null;
    [SerializeField]
    private float _heightOffset = 0f;
    [SerializeField]
    private CharacterHealth _characterHealth = null;

    [SerializeField]
    private RectTransform _fillTransform = null;
    [SerializeField]
    private RectTransform _fillSecondary = null;
    [SerializeField]
    private float _secondaryFillSharpness = 1.0f;

    private void Awake()
    {
        _characterHealth.HealthChangedNormalized += OnHealthChanged;
    }

    private void OnHealthChanged(float normalizedValue)
    {
        SetFill(normalizedValue);
    }

    private void SetFill(float normalizedValue)
    {
        _fillTransform.localScale = new Vector3(normalizedValue, 1f, 1f);
    }

    private void Update()
    {
        if (_followTarget != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(_followTarget.position + Vector3.up * _heightOffset);
        }

        if (_fillSecondary.localScale != _fillTransform.localScale)
        {
            _fillSecondary.localScale = Vector3.Lerp(
                _fillSecondary.localScale,
                _fillTransform.localScale,
                _secondaryFillSharpness * Time.deltaTime);
        }
    }
}
