using System.Collections.Generic;
using A3.Utilities.Unity;
using UnityEngine;
using UnityEngine.Audio;

namespace A3.Audio.Unity
{
    public class AudioSystemBehavior : HumbleBehavior<AudioSystem<AudioClip, AudioMixerGroup>>
    {
        #region Singleton Definition

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(gameObject);
            DontDestroyOnLoad(this);
        }

        private static AudioSystemBehavior _instance;

        public static AudioSystemBehavior Instance => _instance;

        #endregion

        #region Fields and Properties

        [SerializeField]
        private List<ScriptableAudioChannel> registeredChannel = default;

        #endregion

        public void Init()
        {
            Object = new AudioSystem<AudioClip, AudioMixerGroup> {GetOrCreateChannelPlayer = CreateChannelPlayer};
            foreach (ScriptableAudioChannel audioChannel in registeredChannel)
                Object.RegisterChannel(audioChannel);
        }

        private ChannelPlayer<AudioClip, AudioMixerGroup> CreateChannelPlayer(
            IAudioChannel<AudioMixerGroup> channel)
        {
            AudioChannelPlayerBehavior channelPlayer =
                new GameObject(channel.Name).AddComponent<AudioChannelPlayerBehavior>();
            channelPlayer.transform.SetParent(transform);
            channelPlayer.Init(channel);
            return channelPlayer;
        }
    }
}