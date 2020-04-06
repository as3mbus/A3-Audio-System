using UnityEngine;
using UnityEngine.Audio;

namespace A3.Audio.Unity
{
    [CreateAssetMenu(fileName = "AudioChannel", menuName = "Audio/AudioChannel", order = 0)]
    public class ScriptableAudioChannel : ScriptableObject, IAudioChannel<AudioMixerGroup>
    {
        [SerializeField]
        private AudioMixerGroup _mixerGroup = default;

        [SerializeField]
        private bool _multiLayer = default;

        public AudioMixerGroup MixerGroup => _mixerGroup;
        public bool MultiLayer => _multiLayer;
        public string Name => this.name;
    }
}