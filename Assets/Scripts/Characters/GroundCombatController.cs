using Unity.Netcode;
using UnityEngine;

namespace BlackWar.Unity.Characters
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class GroundCombatController : NetworkBehaviour
    {
        [SerializeField] private float walkSpeed = 4.5f;
        [SerializeField] private float sprintSpeed = 7.5f;
        [SerializeField] private float coverCheckDistance = 1.2f;
        [SerializeField] private LayerMask coverMask;

        private CharacterController controller;
        private bool inCover;

        private void Awake() => controller = GetComponent<CharacterController>();

        private void Update()
        {
            if (!IsOwner) return;

            var input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            var speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
            controller.SimpleMove(transform.TransformDirection(input.normalized) * speed);

            if (Input.GetKeyDown(KeyCode.C))
            {
                inCover = Physics.Raycast(transform.position + Vector3.up, transform.forward, coverCheckDistance, coverMask);
            }
        }
    }
}
