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
                () => new Packet<string>(null, PAYLOAD)
            );
            Assert.Throws<ArgumentException>(
                () => new Packet<string>(" ", PAYLOAD)
            );
            Assert.Throws<ArgumentNullException>(
                () => new Packet<string>(SENDER_ID, null)
            );
        }
        [Test]
        public void Test_Constructor()
        {
            var packet = new Packet<string>(SENDER_ID, PAYLOAD);

            Assert.AreEqual(SENDER_ID, packet.SenderID);
            Assert.AreEqual(PAYLOAD, packet.Payload);
            Assert.AreSame(packet.Payload, ((DataPacket)packet).Payload);
        }

        [Test]
        public void Test_Equality()
        {
            var original = new Packet<string>(SENDER_ID, PAYLOAD);
            var good_copy = new Packet<string>(
                original.SenderID, 
                original.Payload
            );
            var flawed_author_copy = new Packet<string>(
                $"another_{SENDER_ID}", 
                original.Payload
            );
            var flawed_payload_copy = new Packet<string>(
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
                new Packet<Foo>(SENDER_ID, payload),
                new Packet<FooDerived>(SENDER_ID, payload)
            );
        }
    }
}
