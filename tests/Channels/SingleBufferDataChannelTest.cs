using NUnit.Framework;
using rharel.M3PD.Common.Collections;
using rharel.M3PD.Communication.Packets;
using System;
using System.Linq;

namespace rharel.M3PD.Communication.Channels.Tests
{
    [TestFixture]
    public sealed class SingleBufferDataChannelTest
    {
        private static readonly Packet<string> PACKET = (
            new Packet<string>("mock_sender_id", "mock_payload")
        );

        private SingleBufferChannel<string> _channel;

        [SetUp]
        public void Setup()
        {
            _channel = new SingleBufferChannel<string>();
        }

        [Test]
        public void Test_InitialState()
        {
            Assert.AreEqual(typeof(string), _channel.DataType);
            Assert.IsTrue(_channel.Packets.IsEmpty());
        }

        [Test]
        public void Test_Clear()
        {
            _channel.Post(PACKET.SenderID, PACKET.Payload);
            _channel.Clear();

            Assert.IsTrue(_channel.Packets.IsEmpty());
        }

        [Test]
        public void Test_Post_WithInvalidArgs()
        {
            Assert.Throws<ArgumentException>(
                () => _channel.Post(" ", PACKET.Payload)
            );
            Assert.Throws<ArgumentNullException>(
                () => _channel.Post(PACKET.SenderID, null)
            );
        }
        [Test]
        public void Test_Post()
        {
            _channel.Post(PACKET.SenderID, PACKET.Payload);

            Assert.AreEqual(1, _channel.Packets.Count);
            Assert.AreEqual(PACKET, _channel.Packets.First());
        }
    }
}
