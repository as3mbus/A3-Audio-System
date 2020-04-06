using System;
using System.Collections.Generic;

namespace A3.Audio
{
    public interface IAudioChannelPlayer<TClip, out TMixerGroup>
    {
        Func<IEnumerable<IAudioSpecificationPlayer<TClip>>> RunningPlayer { get; }
        IAudioChannel<TMixerGroup> Channel { get; }
        IAudioSpecificationPlayer<TClip> CorrespondingSpecificationPlayer(IAudioSpecification<TClip> specification);
        void PlayAudio(IAudioSpecification<TClip> specification);
        void PlayAudioOnce(IAudioSpecification<TClip> specification);
        void StopAudio(IAudioSpecification<TClip> specification);
        void Stop();
    }
}