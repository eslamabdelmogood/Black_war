using UnityEngine;

namespace BlackWar.Unity.Vehicles
{
    public enum VehicleClass { Scout, Assault, Tank, Interceptor, Support }
    public enum VehicleHardpoint { Front, Left, Right, Rear, Utility }

    [CreateAssetMenu(menuName = "Black War/Vehicles/Vehicle Definition")]
    public sealed class VehicleDefinition : ScriptableObject
    {
        public string displayName = "Ravager";
        public VehicleClass vehicleClass = VehicleClass.Assault;
        public float maxSpeed = 95f;
        public float acceleration = 22f;
        public float turnSpeed = 75f;
        public float maxArmor = 220f;
        public float nitroCapacity = 70f;
        public VehicleWeaponDefinition[] weapons = new VehicleWeaponDefinition[0];
    }

    [System.Serializable]
    public sealed class VehicleWeaponDefinition
    {
        public string weaponName = "Machine Gun";
        public VehicleHardpoint hardpoint = VehicleHardpoint.Front;
        public float damage = 35f;
        public int ammo = 500;
        public float cooldown = 0.12f;
    }
}
