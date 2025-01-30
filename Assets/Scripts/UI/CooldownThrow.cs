using Creatures;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CooldownThrow : MonoBehaviour
    {
        [SerializeField] private Image cooldownImage;
        [SerializeField] private GameObject keybind;

        void Update()
        {
            var playerObject = AllegianceManager.TryGetPlayer();
            if (playerObject != null && playerObject.TryGetComponent<CreatureController>(out var player))
            {
                var cooldown = player.throwWeaponCooldown;
                var currentCooldown = player.currentThrowWeaponCooldown;
                if(currentCooldown < 0) currentCooldown = 0;
                cooldownImage.fillAmount = currentCooldown / cooldown;
                keybind.SetActive(currentCooldown <= 0);
            }
        }
    }
}
