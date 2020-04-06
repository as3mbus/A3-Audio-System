using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using static A3.Audio.Test.AudioTestSuite;

namespace A3.Audio.Test
{
    [TestFixture]
    [Category("A3/Audio")]
    public class ChannelPlayerTest
    {
        private List<IAudioSpecificationPlayer<string>> _runningPlayer;
        private ChannelPlayer<string, string> testObject;

        private IAudioSpecificationPlayer<string> GetOrCreateTestAudioPlayer()
        {
            IAudioSpecificationPlayer<string> specificationPlayer = Substitute.For<IAudioSpecificationPlayer<string>>();
            specificationPlayer
                .When(x => x.Play(Arg.Any<IAudioSpecification<string>>()))
                .Do(x =>
                {
                    specificationPlayer.Specification.Returns(x.Arg<IAudioSpecification<string>>());
                    specificationPlayer.IsPlaying.Returns(true);
                });
            specificationPlayer
                .When(x => x.Stop())
                .Do(x =>
                {
                    specificationPlayer.IsPlaying.Returns(false);
                    _runningPlayer.Remove(specificationPlayer);
                });
            _runningPlayer.Add(specificationPlayer);
            return specificationPlayer;
        }
        
        

        private ChannelPlayer<string, string> ConstructTestObject(string channelName)
        {
            return new ChannelPlayer<string, string>(MockAudioChannel(channelName))
            {
                RunningPlayer = () => _runningPlayer,
                GetOrCreatePlayer = GetOrCreateTestAudioPlayer
            };
        }

        [SetUp]
        public void SetUp()
        {
            _runningPlayer = new List<IAudioSpecificationPlayer<string>>();
            testObject = ConstructTestObject("testChannel");
        }

        [Test]
        public void ConstructChannelPlayer_Assign_Channel()
        {
            Assert.That(testObject.Channel.Name, Is.EqualTo("testChannel"));
        }

        [Test]
        public void PlayAudio_Create_SpecificationPlayer()
        {
            testObject.PlayAudio(MockAudioSpecification("testAudio"));
            Assert.That(_runningPlayer.Count > 0);
        }

        [Test]
        public void PlayAudio_Execute_Play_SpecificationPlayer()
        {
            IAudioSpecification<string> mockSpec = MockAudioSpecification("testAudio");
            testObject.PlayAudio(mockSpec);
            Assert.That(testObject.IsPlaying(mockSpec));
        }

        [Test]
        public void MultiLayerChannel_Play_Multiple_Spec()
        {
            testObject.Channel.MultiLayer.Returns(true);
            testObject.PlayAudio(MockAudioSpecification("Audio1"));
            testObject.PlayAudio(MockAudioSpecification("Audio2"));
            Assert.That(_runningPlayer.Count > 1);
        }

        [Test]
        public void SingleLayerChannel_Play_Single_Spec()
        {
            testObject.Channel.MultiLayer.Returns(false);
            testObject.PlayAudio(MockAudioSpecification("Audio1"));
            testObject.PlayAudio(MockAudioSpecification("Audio2"));
            Assert.That(_runningPlayer.Count <= 1);
        }

        [Test]
        public void NonOverlappingSpecification_Only_Run_Once([Random(1, 10, 3)] int number)
        {
            testObject.Channel.MultiLayer.Returns(true);
            IAudioSpecification<string> mockSpecification = MockAudioSpecification("Audio1");
            mockSpecification.CanOverlap.Returns(false);
            for (int i = 0; i < number; i++)
                testObject.PlayAudio(mockSpecification);
            Assert.That(_runningPlayer.Count <= 1);
        }

        [Test]
        public void OverlappingSpecification_Can_Play_Multiple_Times([Random(2, 10, 3)] int number)
        {
            testObject.Channel.MultiLayer.Returns(true);
            IAudioSpecification<string> mockSpecification = MockAudioSpecification("Audio1");
            mockSpecification.CanOverlap.Returns(true);
            for (int i = 0; i < number; i++)
                testObject.PlayAudio(mockSpecification);
            Assert.That(_runningPlayer.Count, Is.GreaterThan(1));
        }

        [Test]
        public void StopAudio_Stop_Single_Playing_Audio(
            [Random(2, 10, 2)] int numberAudio,
            [Random(2, 10, 2)] int numberOtherAudio)
        {
            testObject.Channel.MultiLayer.Returns(true);
            IAudioSpecification<string> mockSpecification = MockAudioSpecification("Audio1");
            mockSpecification.CanOverlap.Returns(true);
            IAudioSpecification<string> mockSpecification2 = MockAudioSpecification("Audio2");
            mockSpecification2.CanOverlap.Returns(true);
            for (int i = 0; i < numberOtherAudio; i++)
                testObject.PlayAudio(mockSpecification2);
            for (int i = 0; i < numberAudio; i++)
                testObject.PlayAudio(mockSpecification);
            testObject.StopAudio(mockSpecification);
            int expectedPlayerCount = numberAudio + numberOtherAudio - 1;
            Assert.That(_runningPlayer.Count, Is.EqualTo(expectedPlayerCount));
        }

        [Test]
        public void Stop_Stops_All_Playing_Audio([Random(1, 10, 5)] int runningAudio)
        {
            testObject.Channel.MultiLayer.Returns(true);
            IAudioSpecification<string> mockSpecification = MockAudioSpecification("Audio1");
            mockSpecification.CanOverlap.Returns(true);
            for (int i = 0; i < runningAudio; i++)
                testObject.PlayAudio(mockSpecification);
            testObject.Stop();
            Assert.That(_runningPlayer.Count, Is.Zero);
        }
    }
}