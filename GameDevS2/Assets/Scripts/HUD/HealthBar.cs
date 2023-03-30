using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private SlicedFilledImage _barSlider;
    [SerializeField] private Image _shieldImage;
    [SerializeField] private TextMeshProUGUI _shieldText;
    private int _maxHealth = 0;
    private int _currentHealth = 0;

    private int _currentShields = 0;
    [SerializeField] private int _criticalShieldsNum = 2;
    [SerializeField] private Color _criticalShieldColor = Color.red;
    private Color _standardShieldColor = Color.black;

    private void Awake()
    {
        if(!_shieldText)
            _shieldText = _shieldImage.GetComponentInChildren<TextMeshProUGUI>();

        _standardShieldColor = _shieldImage.color;
    }

    public void SetupBar(int max)
    {
        _maxHealth = max;
        _currentHealth = max;
        _currentShields = 0;

        _standardShieldColor = _shieldImage.color;

        UpdateHealthUI();
        UpdateShieldUI();
    }

    private int DamageShield(int dmg)
    {
        if (_currentShields <= 0)
            return dmg;

        _currentShields -= dmg;

        if (_currentShields < 0)
        {
            int remainder = Mathf.Abs(_currentShields);
            _currentShields = 0;
            return remainder;
        }

        return 0;
    }

    public void TakeDamage(int dmg)
    {
        if (_currentHealth <= 0)
            return;

        int remainder = DamageShield(dmg);

        _currentHealth -= remainder;

        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);

        UpdateHealthUI();
        UpdateShieldUI();
    }

    public void HealHealth(int amount)
    {
        if (_currentHealth <= 0)
            return;

        _currentHealth += amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);

        UpdateHealthUI();
    }

    public void IncreaseShield(int amount)
    {
        _currentShields += amount;
        UpdateShieldUI();
    }

    private void UpdateHealthUI()
    {
        _barSlider.fillAmount = (float)_currentHealth / _maxHealth;
    }

    private void UpdateShieldUI()
    {
        _shieldText.text = _currentShields.ToString();

        if (_currentShields <= 0)
            _shieldImage.gameObject.SetActive(false);
        else
            _shieldImage.gameObject.SetActive(true);

        if (_currentShields <= _criticalShieldsNum)
            _shieldImage.color = _criticalShieldColor;
        else
            _shieldImage.color = _standardShieldColor;
    }
}
