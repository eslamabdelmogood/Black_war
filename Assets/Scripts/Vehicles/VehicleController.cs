using Unity.Netcode;
using UnityEngine;

namespace BlackWar.Unity.Vehicles
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class VehicleController : NetworkBehaviour
    {
        [SerializeField] private VehicleDefinition definition;
        [SerializeField] private Transform weaponOrigin;

        private Rigidbody body;
        private float armor;
        private float nitro;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            armor = definition != null ? definition.maxArmor : 100f;
            nitro = definition != null ? definition.nitroCapacity : 50f;
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;

            var throttle = Input.GetAxis("Vertical");
            var steering = Input.GetAxis("Horizontal");
            var maxSpeed = definition != null ? definition.maxSpeed : 80f;
            var acceleration = definition != null ? definition.acceleration : 18f;
            var turnSpeed = definition != null ? definition.turnSpeed : 60f;

            if (Input.GetKey(KeyCode.LeftShift) && nitro > 0f)
            {
                maxSpeed *= 1.35f;
                nitro = Mathf.Max(0f, nitro - Time.fixedDeltaTime * 15f);
            }

            body.AddForce(transform.forward * (throttle * acceleration), ForceMode.Acceleration);
            body.velocity = Vector3.ClampMagnitude(body.velocity, maxSpeed);
            transform.Rotate(Vector3.up, steering * turnSpeed * Time.fixedDeltaTime);
        }

        public void ApplyDamage(float amount)
        {
            if (!IsServer) return;
            armor = Mathf.Max(0f, armor - amount);
            if (armor <= 0f)
            {
                SpawnGroundPlayerClientRpc();
                NetworkObject.Despawn();
            }
        }

        [ClientRpc]
        private void SpawnGroundPlayerClientRpc()
        {
            Debug.Log("Vehicle destroyed. Switch player to ground combat loadout.");
        }
    }
}
