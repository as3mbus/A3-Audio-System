using System;
using System.Collections.Generic;
using System.Linq;

namespace A3.Audio
{
    public class ChannelPlayer<TClip, TMixerGroup> : IAudioChannelPlayer<TClip, TMixerGroup>
    {
        public Func<IAudioSpecificationPlayer<TClip>> GetOrCreatePlayer = default;
        public IAudioChannel<TMixerGroup> Channel { get; }

        public ChannelPlayer(IAudioChannel<TMixerGroup> channel)
            => Channel = channel;

        public Func<IEnumerable<IAudioSpecificationPlayer<TClip>>> RunningPlayer { get; set; }

        public IAudioSpecificationPlayer<TClip> CorrespondingSpecificationPlayer(
            IAudioSpecification<TClip> specification)
            => RunningPlayer?.Invoke()?.FirstOrDefault(player => player.Specification == specification);

        public bool IsPlaying(IAudioSpecification<TClip> specification)
            => CorrespondingSpecificationPlayer(specification)?.IsPlaying ?? false;

        public void PlayAudio(IAudioSpecification<TClip> specification)
        {
            if (!Channel.MultiLayer && !IsPlaying(specification))
                Stop();
            if (!specification.CanOverlap)
                StopAudio(specification);
            GetOrCreatePlayer?.Invoke()?.Play(specification);
        }

        public void StopAudio(IAudioSpecification<TClip> specification)
            => RunningPlayer?.Invoke()?.FirstOrDefault(x => x.Specification == specification)?.Stop();

        public void PlayAudioOnce(IAudioSpecification<TClip> specification)
        {
            if (IsPlaying(specification)) return;
            PlayAudio(specification);
        }

        public void Stop()
        {
            if (RunningPlayer == null) return;
            IEnumerable<IAudioSpecificationPlayer<TClip>> runningPlayer =
                new List<IAudioSpecificationPlayer<TClip>>(RunningPlayer?.Invoke());
            foreach (IAudioSpecificationPlayer<TClip> player in runningPlayer)
                player?.Stop();
        }
    }
}