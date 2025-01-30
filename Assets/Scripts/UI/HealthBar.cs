using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image _healthBar;

        public void SetHealth(float curentHealth, float maxHealth)
        {
            _healthBar.fillAmount = curentHealth / maxHealth;
        }

        public void UpdatePosition( Vector3 creatureWorldPosition)
        {
            if (Camera.main is { } mainCamera)
            {
                var canvasPosition = mainCamera.WorldToScreenPoint(creatureWorldPosition);
                canvasPosition.y -= 100;
                transform.position = canvasPosition;
            }            
        }
    }
}
