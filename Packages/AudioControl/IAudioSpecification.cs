namespace A3.Audio
{
    public interface IAudioSpecification<TClip> 
    {
        bool IsLoop { get; }
        bool CanOverlap { get; }
        TClip Clip { get; }
    }

    public interface IAudioSpecification<TClip, out TMixerGroup> : IAudioSpecification<TClip>
    {
        IAudioChannel<TMixerGroup> Channel { get; }

    }
}