using NUnit.Framework;
using rharel.M3PD.Common.Collections;
using rharel.M3PD.Communication.Packets;
using System;
using System.Linq;

namespace rharel.M3PD.Communication.Channels.Tests
{
    [TestFixture]
    public sealed class DoubleBufferDataChannelTest
    {
        private static readonly DataPacket<string> PACKET = (
            new DataPacket<string>("mock_sender_id", "mock_payload")
        );

        private DoubleBufferDataChannel<string> _channel;

        [SetUp]
        public void Setup()
        {
            _channel = new DoubleBufferDataChannel<string>();
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
            _channel.SwapBuffers();
            _channel.Post(PACKET.SenderID, PACKET.Payload);
            _channel.Clear();

            Assert.AreEqual(1, _channel.Packets.Count);
            Assert.AreEqual(PACKET, _channel.Packets.First());

            _channel.SwapBuffers();

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

            Assert.IsTrue(_channel.Packets.IsEmpty());

            _channel.SwapBuffers();

            Assert.AreEqual(1, _channel.Packets.Count);
            Assert.AreEqual(PACKET, _channel.Packets.First());
        }
    }
}
