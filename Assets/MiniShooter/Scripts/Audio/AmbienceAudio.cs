using MasterServerToolkit.MasterServer;
using UnityEngine;

namespace MiniShooter
{
    public class AmbienceAudio : MonoBehaviour
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        private AudioSource musicSource;

        #endregion

        private void Awake()
        {
            OnlinePlayer.OnLocalPlayerCreatedEvent += OnlinePlayer_OnLocalPlayerCreatedEvent;
            OnlinePlayer.OnLocalPlayerDestroyedEvent += OnlinePlayer_OnLocalPlayerDestroyedEvent;

            Mst.Events.AddListener(GameEventKeys.playAmbienceMusic, OnPlayAmbienceMusic);
        }

        private void OnDestroy()
        {
            OnlinePlayer.OnLocalPlayerCreatedEvent -= OnlinePlayer_OnLocalPlayerCreatedEvent;
            OnlinePlayer.OnLocalPlayerDestroyedEvent -= OnlinePlayer_OnLocalPlayerDestroyedEvent;
        }

        private void OnPlayAmbienceMusic(EventMessage message)
        {
            if (musicSource && OnlinePlayer.Local)
            {
                bool isOn = message.AsBool();

                if (isOn)
                {
                    musicSource.Play();
                }
                else
                {
                    musicSource.Pause();
                }
            }
        }

        private void OnlinePlayer_OnLocalPlayerCreatedEvent(OnlinePlayer player)
        {
            if (musicSource)
                musicSource.Play();
        }

        private void OnlinePlayer_OnLocalPlayerDestroyedEvent()
        {
            if (musicSource)
                musicSource.Stop();
        }
    }
}
