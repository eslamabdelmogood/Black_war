using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackWar.Unity.UI
{
    public sealed class HudController : MonoBehaviour
    {
        [SerializeField] private Slider healthBar;
        [SerializeField] private Slider vehicleArmorBar;
        [SerializeField] private Slider nitroBar;
        [SerializeField] private TMP_Text remainingPlayersText;
        [SerializeField] private TMP_Text activeContractText;

        public void SetDrivingHud(float armor01, float nitro01, int remainingPlayers, string contract)
        {
            if (vehicleArmorBar != null) vehicleArmorBar.value = armor01;
            if (nitroBar != null) nitroBar.value = nitro01;
            if (remainingPlayersText != null) remainingPlayersText.text = remainingPlayers.ToString();
            if (activeContractText != null) activeContractText.text = contract;
        }

        public void SetGroundHud(float health01)
        {
            if (healthBar != null) healthBar.value = health01;
        }
    }
}
