using BlackWar.Unity.Networking;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BlackWar.Unity.Core
{
    public sealed class BlackWarGameManager : MonoBehaviour
    {
        public static BlackWarGameManager Instance { get; private set; }

        [SerializeField] private string citadelScene = "Citadel";
        [SerializeField] private string arenaScene = "BlackWarArena";
        [SerializeField] private NetworkBootstrap networkBootstrap;

        public GameMode CurrentMode { get; private set; } = GameMode.Bootstrap;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private async void Start()
        {
            if (networkBootstrap != null)
            {
                await networkBootstrap.InitializeAsync();
            }

            LoadCitadel();
        }

        public void LoadCitadel()
        {
            CurrentMode = GameMode.Citadel;
            SceneManager.LoadScene(citadelScene);
        }

        public void LoadArena()
        {
            CurrentMode = GameMode.VehicleCombat;
            SceneManager.LoadScene(arenaScene);
        }

        public void EnterFinalShowdown()
        {
            CurrentMode = GameMode.FinalShowdown;
        }
    }
}
