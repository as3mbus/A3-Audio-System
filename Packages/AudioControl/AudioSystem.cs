using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace A3.Audio
{
    public class AudioSystem<TClip, TMixerGroup>
    {
        #region Fields and Properties

        public Func<IAudioChannel<TMixerGroup>, ChannelPlayer<TClip, TMixerGroup>> GetOrCreateChannelPlayer;

        private List<IAudioChannelPlayer<TClip, TMixerGroup>> channelPlayers;

        public ImmutableList<IAudioChannelPlayer<TClip, TMixerGroup>> ChannelPlayers
            => channelPlayers.ToImmutableList();

        #endregion

        public void RegisterChannel(IAudioChannel<TMixerGroup> channel)
        {
            channelPlayers = channelPlayers ?? new List<IAudioChannelPlayer<TClip, TMixerGroup>>();
            if (CorrespondingChannelPlayer(channel) != null) return;
            CreateChannelPlayer(channel);
        }

        private void CreateChannelPlayer(IAudioChannel<TMixerGroup> channel)
        {
            ChannelPlayer<TClip, TMixerGroup> channelPlayer = GetOrCreateChannelPlayer?.Invoke(channel);
            if (channelPlayer == null) throw new NullReferenceException("failed to get or create channel player");
            channelPlayers.Add(channelPlayer);
        }

        public void PlayAudio(IAudioSpecification<TClip, TMixerGroup> audioSpec)
            => CorrespondingChannelPlayer(audioSpec?.Channel)?.PlayAudio(audioSpec);

        public void PlayAudioOnce(IAudioSpecification<TClip, TMixerGroup> audioSpec)
            => CorrespondingChannelPlayer(audioSpec?.Channel)?.PlayAudioOnce(audioSpec);
        
        public IAudioChannelPlayer<TClip, TMixerGroup> CorrespondingChannelPlayer(
            IAudioChannel<TMixerGroup> channel)
            => channelPlayers.Find(channelPlayer => channelPlayer.Channel == channel);

        public IAudioSpecificationPlayer<TClip> CorrespondingSpecificationPlayer(
            IAudioSpecification<TClip, TMixerGroup> specification)
            => CorrespondingChannelPlayer(specification.Channel).CorrespondingSpecificationPlayer(specification);

        public void StopAudioChannel(IAudioChannel<TMixerGroup> channel)
            => CorrespondingChannelPlayer(channel)?.Stop();

        public void StopAudio(IAudioSpecification<TClip, TMixerGroup> audioSpec)
            => CorrespondingChannelPlayer(audioSpec.Channel)?.StopAudio(audioSpec);
    }
}