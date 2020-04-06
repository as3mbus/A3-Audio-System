namespace A3.Audio
{
    public interface IAudioSpecificationPlayer<TClip>
    {
        bool IsPlaying { get; }
        IAudioSpecification<TClip> Specification { get; }
        void Play(IAudioSpecification<TClip> audioSpec);
        void StartFade(bool fadeIn, float duration = 1);
        void Stop();
    }
}