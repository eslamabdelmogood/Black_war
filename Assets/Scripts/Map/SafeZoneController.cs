using Unity.Netcode;
using UnityEngine;

namespace BlackWar.Unity.Map
{
    public sealed class SafeZoneController : NetworkBehaviour
    {
        [SerializeField] private float startRadius = 4000f;
        [SerializeField] private float minRadius = 400f;
        [SerializeField] private float shrinkEverySeconds = 180f;
        [SerializeField] private float shrinkAmount = 450f;

        public NetworkVariable<float> CurrentRadius { get; } = new();
        private float timer;

        public override void OnNetworkSpawn()
        {
            if (IsServer) CurrentRadius.Value = startRadius;
        }

        private void Update()
        {
            if (!IsServer) return;
            timer += Time.deltaTime;
            if (timer < shrinkEverySeconds) return;
            timer = 0f;
            CurrentRadius.Value = Mathf.Max(minRadius, CurrentRadius.Value - shrinkAmount);
        }
    }
}
