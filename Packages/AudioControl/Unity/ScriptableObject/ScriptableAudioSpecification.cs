using UnityEngine;
using UnityEngine.Audio;

namespace A3.Audio.Unity
{
    [CreateAssetMenu(fileName = "AudioSpec", menuName = "Audio/AudioSpecs", order = 0)]
    public class ScriptableAudioSpecification : ScriptableObject,  IAudioSpecification<AudioClip, AudioMixerGroup>
    {
        [SerializeField]
        private AudioClip _clip = default;

        public AudioClip Clip => _clip;

        [SerializeField]
        private bool _isLoop = false;
        
        public bool IsLoop => _isLoop;

        [SerializeField]
        private bool _canOverlap = false;

        public bool CanOverlap => _canOverlap;

        public IAudioChannel<AudioMixerGroup> Channel => _channel;

        [SerializeField]
        private ScriptableAudioChannel _channel = default;

    }
}