using NUnit.Framework;
using System;

namespace rharel.M3PD.Communication.Packets.Test
{
    [TestFixture]
    public sealed class DataPacketTest
    {
        private class Foo { }
        private sealed class FooDerived: Foo { }

        private static readonly string SENDER_ID = "mock_sender_id";
        private static readonly string PAYLOAD = "Hello world!";
        
        [Test]
        public void Test_Constructor_WithInvalidArgs()
        {
            Assert.Throws<ArgumentException>(
                () => new DataPacket<string>(null, PAYLOAD)
            );
            Assert.Throws<ArgumentException>(
                () => new DataPacket<string>(" ", PAYLOAD)
            );
            Assert.Throws<ArgumentNullException>(
                () => new DataPacket<string>(SENDER_ID, null)
            );
        }
        [Test]
        public void Test_Constructor()
        {
            var packet = new DataPacket<string>(SENDER_ID, PAYLOAD);

            Assert.AreEqual(SENDER_ID, packet.SenderID);
            Assert.AreEqual(PAYLOAD, packet.Payload);
            Assert.AreSame(packet.Payload, ((DataPacket)packet).Payload);
        }

        [Test]
        public void Test_Equality()
        {
            var original = new DataPacket<string>(SENDER_ID, PAYLOAD);
            var good_copy = new DataPacket<string>(
                original.SenderID, 
                original.Payload
            );
            var flawed_author_copy = new DataPacket<string>(
                $"another_{SENDER_ID}", 
                original.Payload
            );
            var flawed_payload_copy = new DataPacket<string>(
                original.SenderID,
                $"wrong {original.Payload}"
            );

            Assert.AreNotEqual(original, null);
            Assert.AreNotEqual(original, "incompatible type");
            Assert.AreNotEqual(original, flawed_author_copy);
            Assert.AreNotEqual(original, flawed_payload_copy);

            Assert.AreEqual(original, original);
            Assert.AreEqual(original, good_copy);
        }
        [Test]
        public void Test_Equality_WhenCast()
        {
            var payload = new FooDerived();

            Assert.AreEqual(
                new DataPacket<Foo>(SENDER_ID, payload),
                new DataPacket<FooDerived>(SENDER_ID, payload)
            );
        }
    }
}
