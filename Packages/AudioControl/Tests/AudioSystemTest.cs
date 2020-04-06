using NUnit.Framework;
using static A3.Audio.Test.AudioTestSuite;

namespace A3.Audio.Test
{
    [TestFixture]
    [Category("A3/Audio")]
    public class AudioSystemTest
    {
        private AudioSystem<string, string> testedObject;

        [SetUp]
        public void SetUp()
        {
            testedObject = new AudioSystem<string, string>()
            {
                GetOrCreateChannelPlayer = GetOrCreateChannelPlayer
            };
        }

        private ChannelPlayer<string, string> GetOrCreateChannelPlayer(IAudioChannel<string> channel)
        {
            return new ChannelPlayer<string, string>(channel);
        }

        [Test]
        public void RegisterChannel_Get_Or_Create_New_Channel_Player()
        {
            IAudioChannel<string> mockChannel = MockAudioChannel("TestChannel1");
            testedObject.RegisterChannel(mockChannel);
            Assert.That(testedObject.CorrespondingChannelPlayer(mockChannel), Is.Not.Null);
        }

        [Test]
        public void Registering_Registered_Channel_does_not_Create_Channel_Player(
            [Random(0, 3, 3)] int otherChannelCount,
            [Random(2, 4, 2)] int registerSimilarCount)
        {
            for (int i = 0; i < otherChannelCount; i++)
                testedObject.RegisterChannel(MockAudioChannel($"otherChannel1{i}"));

            IAudioChannel<string> testChannel = MockAudioChannel("TestChannel1");
            for (int i = 0; i < registerSimilarCount; i++)
                testedObject.RegisterChannel(testChannel);
            Assert.That(testedObject.ChannelPlayers.Count, Is.EqualTo(otherChannelCount + 1));
        }
    }
}