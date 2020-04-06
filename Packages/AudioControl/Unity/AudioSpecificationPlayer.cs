using System.Collections;
using UnityEngine;

namespace A3.Audio.Unity
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSpecificationPlayer : MonoBehaviour, IAudioSpecificationPlayer<AudioClip>
    {
        private AudioSource _audioSource;
        private Coroutine _fadeCoroutine;

        public bool IsPlaying => _audioSource.isPlaying;
        public IAudioSpecification<AudioClip> Specification { get; private set; }

        public void Play(IAudioSpecification<AudioClip> audioSpec)
        {
            Specification = audioSpec;
            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = audioSpec.Clip;
            _audioSource.loop = audioSpec.IsLoop;
            _audioSource.Play();
            StartCoroutine(AudioFinishCheck());
        }

        public void StartFade(bool fadeIn, float duration = 1)
        {
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(FadeProcess(fadeIn, duration));
        }

        private IEnumerator AudioFinishCheck()
        {
            yield return new WaitUntil(() => !_audioSource.isPlaying);
            Stop();
        }

        private IEnumerator FadeProcess(bool fadeIn, float duration = 1)
        {
            while (fadeIn ? (_audioSource.volume < 1) : (_audioSource.volume > 0))
            {
                _audioSource.volume += fadeIn ? 1 : -1 * Time.deltaTime / duration;
                yield return null;
            }
        }

        public void Stop()
        {
            Destroy(gameObject);
        }
    }
}