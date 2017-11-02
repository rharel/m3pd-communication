using Moq;
using NUnit.Framework;
using rharel.M3PD.Common.Collections;
using System;
using System.Linq;
using static rharel.M3PD.Communication.Management.CommunicationManager;

namespace rharel.M3PD.Communication.Management.Tests
{
    [TestFixture]
    public sealed class CommunicationManagerTest
    {
        private CommunicationManager _manager;

        [SetUp]
        public void Setup()
        {
            _manager = new CommunicationManager.Builder()
                .Support<int>()
                .Build();
        }

        [Test]
        public void Test_InitialState()
        {
            Assert.AreEqual(1, _manager.DataTypes.Count);
            Assert.IsTrue(_manager.DataTypes.Contains(typeof(int)));

            Assert.IsTrue(_manager.Agents.IsEmpty());
        }

        [Test]
        public void Test_Engage_WithInvalidArgs()
        {
            Assert.Throws<ArgumentNullException>(
                () => _manager.Engage(null)
            );
        }
        [Test]
        public void Test_Engage()
        {
            var agent = new Mock<Agent>().Object;

            Assert.IsTrue(_manager.Engage(agent));
            Assert.AreEqual(1, _manager.Agents.Count);
            Assert.IsTrue(_manager.Agents.Contains(agent));
            Assert.IsFalse(_manager.Engage(agent));
        }

        [Test]
        public void Test_Disengage_WithInvalidArgs()
        {
            Assert.Throws<ArgumentNullException>(
                () => _manager.Disengage(null)
            );
        }
        [Test]
        public void Test_Disengage()
        {
            var agent = new Mock<Agent>().Object;

            Assert.IsTrue(_manager.Engage(agent));
            Assert.IsTrue(_manager.Disengage(agent));
            Assert.AreEqual(0, _manager.Agents.Count);
            Assert.IsFalse(_manager.Disengage(agent));
        }

        [Test]
        public void Test_Update()
        {
            var agent = new Mock<Agent>();
            string id = "mock_id";
            int data = 1;
            DataSubmission expired_submission = null;

            Assert.IsTrue(_manager.Engage(agent.Object));

            agent.SetupGet(x => x.ID).Returns(id);
            agent
                .Setup(x => x.Perceive(It.IsAny<CommunicationManager>(),
                                       It.IsAny<ImmutableChannelBatch>()))
                .Callback<CommunicationManager, ImmutableChannelBatch>(
                     (manager, channels) =>
                     {
                         Assert.AreSame(_manager, manager);
                         Assert.IsTrue(channels.GetPackets<int>().IsEmpty());
                     }
                );
            agent
                .Setup(x => x.Act(It.IsAny<DataSubmission>()))
                .Callback<DataSubmission>(submission =>
                {
                    Assert.AreSame(_manager, submission.Manager);
                    Assert.AreEqual(id, submission.SenderID);
                    Assert.IsTrue(submission.IsActive);

                    submission.Add(data);

                    expired_submission = submission;
                });

            _manager.Update();

            agent.Verify(
                x => x.Perceive(It.IsAny<CommunicationManager>(),
                                It.IsAny<ImmutableChannelBatch>()),
                Times.Once()
            );
            agent.Verify(x => x.Act(It.IsAny<DataSubmission>()),
                         Times.Once());


            Assert.IsNotNull(expired_submission);
            Assert.IsFalse(expired_submission.IsActive);

            agent
                .Setup(x => x.Perceive(It.IsAny<CommunicationManager>(),
                                       It.IsAny<ImmutableChannelBatch>()))
                .Callback<CommunicationManager, ImmutableChannelBatch>(
                     (manager, channels) =>
                     {
                         Assert.AreSame(_manager, manager);
                         Assert.AreEqual(1, channels.GetPackets<int>().Count);

                         var packet = channels.GetPackets<int>().First();

                         Assert.AreEqual(id, packet.SenderID);
                         Assert.AreEqual(data, packet.Payload);
                     }
                );

            _manager.Update();

            agent.Verify(
                x => x.Perceive(It.IsAny<CommunicationManager>(),
                                It.IsAny<ImmutableChannelBatch>()),
                Times.Exactly(2)
            );
            agent.Verify(x => x.Act(It.IsAny<DataSubmission>()),
                         Times.Exactly(2));
        }
    }
}
