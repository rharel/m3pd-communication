using NUnit.Framework;
using System;

namespace rharel.M3PD.Communication.Management.Tests
{
    [TestFixture]
    public sealed class ChannelBatchTest
    {
        private ChannelBatch _channels;

        [SetUp]
        public void Setup()
        {
            _channels = new ChannelBatch.Builder()
                .WithChannel<int>()
                .Build();
        }

        [Test]
        public void Test_InitialState()
        {
            Assert.AreEqual(1, _channels.DataTypes.Count);
            Assert.IsTrue(_channels.DataTypes.Contains(typeof(int)));
        }

        [Test]
        public void Test_DataTypes()
        {
            Assert.IsFalse(_channels.DataTypes.Contains(typeof(string)));
            Assert.IsTrue(_channels.DataTypes.Contains(typeof(int)));
        }

        [Test]
        public void Test_Get_WithInvalidArgs()
        {
            Assert.Throws<ArgumentException>(() => _channels.Get<string>());
        }
        [Test]
        public void Test_Get()
        { 
            Assert.IsNotNull(_channels.Get<int>());
        }
    }
}
