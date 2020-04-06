using A3.Utilities.Unity;
using UnityEngine;
using UnityEngine.Audio;

namespace A3.Audio.Unity
{
    public class AudioChannelPlayerBehavior : HumbleBehavior<ChannelPlayer<AudioClip,AudioMixerGroup>>
    {
        public void Init(IAudioChannel<AudioMixerGroup> channel)
        {
            Object = new ChannelPlayer<AudioClip, AudioMixerGroup>(channel)
            {
                RunningPlayer = GetComponentsInChildren<IAudioSpecificationPlayer<AudioClip>>,
                GetOrCreatePlayer = GetPlayer
            };
        }

        private IAudioSpecificationPlayer<AudioClip> GetPlayer()
        {
            AudioSpecificationPlayer newPlayer = new GameObject().AddComponent<AudioSpecificationPlayer>();
            newPlayer.GetComponent<AudioSource>().outputAudioMixerGroup = Object.Channel.MixerGroup;
            newPlayer.transform.SetParent(transform);
            return newPlayer;
        }
    }
}