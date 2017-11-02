using rharel.Debug;
using rharel.M3PD.Common.Collections;
using rharel.M3PD.Communication.Packets;
using System;
using System.Collections.Generic;

namespace rharel.M3PD.Communication.Channels
{
    /// <summary>
    /// Represents a communication channel carrying data packets.
    /// </summary>
    /// <typeparam name="T">The type of data the channel carries.</typeparam>
    /// <remarks>
    /// <see cref="DataChannel"/> for the non-generic version.
    /// </remarks>
    internal sealed class SingleBufferDataChannel<T> : DataChannel<T>
    {
        /// <summary>
        /// Creates a new channel.
        /// </summary>
        public SingleBufferDataChannel()
        {
            Packets = new CollectionView<DataPacket<T>>(_packets);
        }

        /// <summary>
        /// Gets the type of data this channel carries.
        /// </summary>
        public Type DataType { get; } = typeof(T);

        /// <summary>
        /// Gets the packets currently live on the channel.
        /// </summary>
        /// <remarks>
        /// <see cref="DataChannel.Packets"/> for the non-generic version.
        /// </remarks>
        public ImmutableCollection<DataPacket<T>> Packets { get; }
        /// <summary>
        /// Gets the packets currently live on the channel.
        /// </summary>
        /// <remarks>
        /// <see cref="Packets"/> for the generic version.
        /// </remarks>
        ImmutableCollection<DataPacket> DataChannel.Packets => (
            (ImmutableCollection<DataPacket>)Packets
        );

        /// <summary>
        /// Clears the channel.
        /// </summary>
        public void Clear()
        {
            _packets.Clear();
        }

        /// <summary>
        /// Posts the specified packet onto the channel.
        /// </summary>
        /// <param name="sender_id">The packet's sender identifier.</param>
        /// <param name="payload">The packet's enclosed data.</param>
        /// <exception cref="ArgumentException">
        /// When <paramref name="sender_id"/> is blank.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// When <paramref name="payload"/> is null.  
        /// </exception>
        public void Post(string sender_id, T payload)
        {
            Require.IsNotBlank(sender_id);
            Require.IsNotNull(payload);

            _packets.Add(new DataPacket<T>(sender_id, payload));
        }
        /// <summary>
        /// Posts the specified packet onto the channel.
        /// </summary>
        /// <param name="sender_id">The packet's sender identifier.</param>
        /// <param name="payload">The packet's enclosed data.</param>
        /// <exception cref="ArgumentException">
        /// When <paramref name="sender_id"/> is blank or if 
        /// <paramref name="payload"/> is not of the required type.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// When <paramref name="payload"/> is null.  
        /// </exception>
        public void Post(string sender_id, object payload)
        {
            Require.IsNotBlank(sender_id);
            Require.IsNotNull(payload);
            Require.IsTrue(payload is T);

            Post(sender_id, (T)payload);
        }

        /// <summary>
        /// Returns a string that represents this instance.
        /// </summary>
        /// <returns>
        /// A human-readable string.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(SingleBufferDataChannel<T>)}{{ " +
                   $"{nameof(DataType)} = {DataType}, " +
                   $"{nameof(Packets)} = {Packets} }}";
        }

        private readonly ICollection<DataPacket<T>> _packets = (
            new List<DataPacket<T>>()
        );
    }
}
