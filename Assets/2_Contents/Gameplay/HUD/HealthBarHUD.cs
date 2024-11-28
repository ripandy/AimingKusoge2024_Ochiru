using System;
using UnityEngine;
using UnityEngine.UI;

namespace Contents.Gameplay.HUD
{
    public class HealthBarHUD : MonoBehaviour
    {
        [SerializeField] private HealthPercentageVariable healthPercentageVariable;
        [SerializeField] private Image[] healthBarIcons;

        private IDisposable subscription;

        private void Awake()
        {
            healthBarIcons = GetComponentsInChildren<Image>(includeInactive: true);
        }

        private void Start()
        {
            subscription = healthPercentageVariable.Subscribe(UpdateHealthBar);
        }

        private void UpdateHealthBar(float healthPercentage)
        {
            var ratioPerIcon = 1f / healthBarIcons.Length;
            for (var i = 0; i < healthBarIcons.Length; i++)
            {
                var icon = healthBarIcons[i];
                var iconFillAmount = Mathf.Clamp01(healthPercentage - ratioPerIcon * i) / ratioPerIcon;
                icon.fillAmount = iconFillAmount;
            }
        }
        
        private void OnDestroy()
        {
            subscription?.Dispose();
        }
    }
}