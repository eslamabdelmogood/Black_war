using UnityEngine;

namespace BlackWar.Unity.Audio
{
    public sealed class AudioDirector : MonoBehaviour
    {
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioClip citadelMusic;
        [SerializeField] private AudioClip vehicleCombatMusic;
        [SerializeField] private AudioClip finalShowdownMusic;

        public void PlayCitadel() => Play(citadelMusic);
        public void PlayVehicleCombat() => Play(vehicleCombatMusic);
        public void PlayFinalShowdown() => Play(finalShowdownMusic);

        private void Play(AudioClip clip)
        {
            if (musicSource == null || clip == null) return;
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }
}
