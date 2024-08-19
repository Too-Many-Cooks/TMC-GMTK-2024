using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
internal class HealthBar : MonoBehaviour
{
    Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    internal void UpdateHealthBar(float enemyHealth, float enemyMaxHealth)
    {
        slider.value = enemyHealth / enemyMaxHealth;
    }
}