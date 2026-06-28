using UnityEngine;

namespace BlackWar.Unity.Contracts
{
    public enum ContractKind { Assassination, Destruction, Protection, Control, Special, CounterAssassination }
    public enum ReputationGate { Unknown, Known, Trusted, Feared, Legendary }

    [CreateAssetMenu(menuName = "Black War/Contracts/Contract Definition")]
    public sealed class ContractDefinition : ScriptableObject
    {
        public ContractKind kind;
        public ReputationGate requiredReputation;
        public int activationFee = 50;
        public int rewardCoins = 250;
        public int rewardReputation = 80;
        [TextArea] public string objectiveText = "Eliminate the target.";
    }
}
