using System.Threading.Tasks;
using UnityEngine;

namespace BlackWar.Unity.Firebase
{
    public sealed class FirebaseService : MonoBehaviour
    {
        public bool IsReady { get; private set; }

        public Task InitializeAsync()
        {
            // Add Firebase Unity SDK packages, then initialize Auth, Firestore, Storage, and Analytics here.
            IsReady = true;
            Debug.Log("Firebase placeholder initialized. Replace with FirebaseApp.CheckAndFixDependenciesAsync().");
            return Task.CompletedTask;
        }
    }
}
