using BlackWar.Unity.Core;
using UnityEngine;

namespace BlackWar.Unity.Citadel
{
    public sealed class CitadelHubController : MonoBehaviour
    {
        [SerializeField] private Transform weaponShopSpawn;
        [SerializeField] private Transform garageSpawn;
        [SerializeField] private Transform marketSpawn;
        [SerializeField] private Transform contractBoardSpawn;

        public void OpenWeaponShop() => Debug.Log($"Open Weapon Shop at {weaponShopSpawn?.name}");
        public void OpenGarage() => Debug.Log($"Open Vehicle Garage at {garageSpawn?.name}");
        public void OpenBlackMarket() => Debug.Log($"Open Black Market at {marketSpawn?.name}");
        public void OpenContractBoard() => Debug.Log($"Open Contract Board at {contractBoardSpawn?.name}");
        public void QueueTournament() => BlackWarGameManager.Instance.LoadArena();
    }
}
