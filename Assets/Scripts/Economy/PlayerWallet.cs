using Unity.Netcode;

namespace BlackWar.Unity.Economy
{
    public sealed class PlayerWallet : NetworkBehaviour
    {
        public NetworkVariable<int> GoldCoins { get; } = new(800);
        public NetworkVariable<int> ReputationPoints { get; } = new(0);

        public bool TrySpend(int amount)
        {
            if (!IsServer || GoldCoins.Value < amount) return false;
            GoldCoins.Value -= amount;
            return true;
        }

        public void Reward(int coins, int reputation)
        {
            if (!IsServer) return;
            GoldCoins.Value += coins;
            ReputationPoints.Value = System.Math.Max(0, ReputationPoints.Value + reputation);
        }
    }
}
