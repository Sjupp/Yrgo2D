using System;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterHealth : MonoBehaviour
{
    [SerializeField]
    private int _maxHealth = 1;
    [SerializeField]
    private int _damageTaken = 0;
    [SerializeField]
    private TMP_Text _textA = null;
    [SerializeField]
    private TextMeshProUGUI _textB = null;

    public int CurrentHealth => _maxHealth - _damageTaken;
    public float HealthNormalized => (float)CurrentHealth / _maxHealth;

    public Action<float> HealthChangedNormalized;

    public void SetHealth(float normalizedValue)
    {
        _damageTaken = (int)(_maxHealth - (_maxHealth * normalizedValue));

        _textA.text = "Health: " + CurrentHealth;
        _textB.text = "Health: " + CurrentHealth;

        HealthChangedNormalized?.Invoke(HealthNormalized);
    }

    public void TakeDamage(int damage)
    {
        SceneManager.LoadScene(0);

        _damageTaken += damage;
        if (_damageTaken > _maxHealth)
        {
            _damageTaken = _maxHealth;
        }
        HealthChangedNormalized?.Invoke(HealthNormalized);

        _textA.text = "Health: " + CurrentHealth;
        _textB.text = "Health: " + CurrentHealth;
    }
}
