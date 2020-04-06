namespace A3.Audio
{
    public interface IAudioChannel<out TMixerGroup>
    {
        string Name { get; }
        bool MultiLayer { get; }
        TMixerGroup MixerGroup { get; }
    }
}