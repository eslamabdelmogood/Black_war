using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BlackWar.Unity.Networking
{
    public sealed class NetworkBootstrap : MonoBehaviour
    {
        [SerializeField] private bool autoStartHostInEditor = true;

        public Task InitializeAsync()
        {
            if (NetworkManager.Singleton == null)
            {
                Debug.LogWarning("NetworkManager is missing from the Bootstrap scene.");
                return Task.CompletedTask;
            }

#if UNITY_EDITOR
            if (autoStartHostInEditor && !NetworkManager.Singleton.IsListening)
            {
                NetworkManager.Singleton.StartHost();
            }
#endif
            return Task.CompletedTask;
        }
    }
}
