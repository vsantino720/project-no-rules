using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthController : MonoBehaviour
{
    [Header("Health Parameters")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    [SerializeField] private float smoothDecreaseDuration = 0.5f;

    [Header("UI Parameters")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image heartIcon;

    [Header("Damage Color Parameters")]
    [SerializeField] private Color originalHealthColor;
    [SerializeField] private Color damageHealthColor;

    [Header("Animator Parameters")]
    [SerializeField] private Animator heartAnim;
    [SerializeField] private string heartShrinkAnim;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthText();
    }

    public void TakeDamage(float damage)
    {
        heartAnim.Play(heartShrinkAnim, 0, 0.0f);
        StartCoroutine(SmoothDecreaseHealth(damage));
    }

    private IEnumerator SmoothDecreaseHealth(float damage)
    {
        healthText.color = damageHealthColor;
        heartIcon.color = damageHealthColor;

        float damagePerTick = damage / smoothDecreaseDuration;
        float elapsedTime = 0f;

        while (elapsedTime < smoothDecreaseDuration)
        {
            float currentDamage = damagePerTick * Time.deltaTime;
            currentHealth -= currentDamage;
            elapsedTime += Time.deltaTime;

            UpdateHealthText();

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                //Player Death
                break;
            }
            yield return null;
        }

    healthText.color = originalHealthColor;
    heartIcon.color = originalHealthColor;
}
    void UpdateHealthText() 
    {
        healthText.text = currentHealth.ToString("0");
    }
}
