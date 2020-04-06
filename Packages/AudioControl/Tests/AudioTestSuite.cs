using NSubstitute;

namespace A3.Audio.Test
{
    public static class AudioTestSuite
    {
        public static IAudioChannel<string> MockAudioChannel(string name)
        {
            IAudioChannel<string> mockObject = Substitute.For<IAudioChannel<string>>();
            mockObject.Name.Returns(name);
            return mockObject;
        }

        public static IAudioSpecification<string> MockAudioSpecification(string name)
        {
            IAudioSpecification<string> mockObject = Substitute.For<IAudioSpecification<string>>();
            mockObject.Clip.Returns(name);
            return mockObject;
        }
    }
}