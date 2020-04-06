using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

namespace A3.Audio.Unity
{
    // OLD VERSION
    public class AudioManager : MonoBehaviour
    {
        #region Singleton Definition

        private void Awake()
        {
            if (AudioManager._instance == null)
                AudioManager._instance = this;
            else
                Destroy(gameObject);
            DontDestroyOnLoad(this);
        }

        private static AudioManager _instance;

        public static AudioManager Instance => _instance;

        #endregion

        #region Fields and Properties

        [SerializeField]
        private AudioMixer _mainMixer = default;

        public AudioMixer MainMixer => _mainMixer;

        [SerializeField]
        private List<ScriptableAudioSpecification> AudioLibrary = default;

        [SerializeField]
        private List<ScriptableAudioChannel> AudioChannels = default;

        public List<Transform> channelContainer = new List<Transform>();

        #endregion

        private bool IsPlayingClip(ScriptableAudioSpecification audioSpec) => FindPlayingClip(audioSpec);

        // find / create parent transform that represent channel that house all playing clip inside a channel
        private Transform RelatedContainer(IAudioChannel<AudioMixerGroup> channel)
        {
            Transform foundContainer = channelContainer.Find(chCont => chCont.gameObject.name == channel.Name);
            if (foundContainer) return foundContainer;
            foundContainer = new GameObject(channel.Name).transform;
            foundContainer.SetParent(transform);
            channelContainer.Add(foundContainer);
            return foundContainer;
        }

        private AudioSource FindPlayingClip(ScriptableAudioSpecification audioSpec)
        {
            Transform curContainer = RelatedContainer(audioSpec.Channel);
            foreach (Transform child in curContainer)
            {
                AudioSource audioSource = child.GetComponent<AudioSource>();
                if (audioSource.clip == audioSpec.Clip) return audioSource;
            }

            return null;
        }

        public void PlayAudio(ScriptableAudioSpecification audioSpec)
        {
            if (!audioSpec) return; // IF NULL
            
            if (!audioSpec.Channel.MultiLayer && !IsPlayingClip(audioSpec)) // IF CHANNEL SINGLE LAYER
                StopAudioChannel(audioSpec.Channel);
            
            if (audioSpec.IsLoop) // IF LOOPING AUDIO
            {
                if (IsPlayingClip(audioSpec)) return; // IF STILL PLAYING
                PlayAndLoop(audioSpec);
            }
            else
            {
                if (audioSpec.CanOverlap) // IF OVERLAPPING AUDIO
                    StartCoroutine(PlayAndDestroy(audioSpec));
                else
                {
                    if (IsPlayingClip(audioSpec)) return; // IF CLIP NOT ALREADY
                    StartCoroutine(PlayAndDestroy(audioSpec));
                }
            }
        }

        // stopping channel by destroying channel transform by itself
        public void StopAudioChannel(IAudioChannel<AudioMixerGroup> channel)
        {
            foreach (Transform source in RelatedContainer(channel))
                Destroy(source.gameObject);
        }

        public void StopAudioChannel(string channelName)
        {
            StopAudioChannel(ChannelByName(channelName));
        }

        public void StopAudio(ScriptableAudioSpecification audioSpec)
        {
            AudioSource src = FindPlayingClip(audioSpec);
            if (src != null) Destroy(src.gameObject);
        }

        public void PlayAudio(string audioSpecString)
        {
            PlayAudio(SpecByName(audioSpecString));
        }

        private void PlayAndLoop(ScriptableAudioSpecification audioSpec)
        {
            AudioSource aSrc = CreateNewSource(audioSpec);
            aSrc.loop = true;
            aSrc.Play();
        }

        private IEnumerator PlayAndDestroy(ScriptableAudioSpecification audioSpec)
        {
            AudioSource aSrc = CreateNewSource(audioSpec);
            aSrc.Play();
            yield return new WaitUntil(() => !aSrc.isPlaying);
            if (aSrc) Destroy(aSrc.gameObject);
        }

        private AudioSource CreateNewSource(ScriptableAudioSpecification audioSpec)
        {
            AudioSource aSrc = new GameObject().AddComponent<AudioSource>();
            aSrc.transform.SetParent(RelatedContainer(audioSpec.Channel));
            aSrc.outputAudioMixerGroup = audioSpec.Channel.MixerGroup;
            aSrc.clip = audioSpec.Clip;
            return aSrc;
        }

        public ScriptableAudioSpecification SpecByName(string audioSpecName)
            => AudioLibrary.Find(auSpec => auSpec.name == audioSpecName);


        public ScriptableAudioChannel ChannelByName(string channelName)
            => AudioChannels.Find(auChnl => auChnl.name == channelName);
    }
}