using Creatures;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CooldownDash : MonoBehaviour
    {
    
        [SerializeField] private Image cooldownImage;
        [SerializeField] private GameObject keybind;
        void Update()
        {
            var playerObject = AllegianceManager.TryGetPlayer();
            if (playerObject != null && playerObject.TryGetComponent<CreatureController>(out var player))
            {
                var cooldown = player.dashCooldown;
                var currentCooldown = player.currentDashCooldown;
                if(currentCooldown < 0) currentCooldown = 0;
                cooldownImage.fillAmount = currentCooldown / cooldown;
                keybind.SetActive(currentCooldown <= 0);
            }
        }
    }
}
