using rharel.M3PD.Common.Collections;
using rharel.M3PD.Communication.Packets;
using System;

namespace rharel.M3PD.Communication.Channels
{
    /// <summary>
    /// Represents a communication channel carrying data packets.
    /// </summary>
    /// <remarks>
    /// <see cref="DataChannel{T}"/> for the generic version.
    /// </remarks>
    internal interface DataChannel
    {
        /// <summary>
        /// Gets the type of data this channel carries.
        /// </summary>
        Type DataType { get; }
        /// <summary>
        /// Gets the packets currently live on the channel.
        /// </summary>
        ImmutableCollection<DataPacket> Packets { get; }

        /// <summary>
        /// Clears the channel.
        /// </summary>
        void Clear();

        /// <summary>
        /// Posts the specified packet onto the channel.
        /// </summary>
        /// <param name="sender_id">The packet's sender identifier.</param>
        /// <param name="payload">The packet's enclosed data.</param>
        void Post(string sender_id, object payload);
    }
    /// <summary>
    /// Represents a communication channel carrying data packets.
    /// </summary>
    /// <typeparam name="T">The type of data the channel carries.</typeparam>
    /// <remarks>
    /// <see cref="DataChannel"/> for the non-generic version.
    /// </remarks>
    internal interface DataChannel<T>: DataChannel
    {
        /// <summary>
        /// Gets the packets currently live on the channel.
        /// </summary>
        new ImmutableCollection<DataPacket<T>> Packets { get; }

        /// <summary>
        /// Posts the specified packet onto the channel.
        /// </summary>
        /// <param name="sender_id">The packet's sender identifier.</param>
        /// <param name="payload">The packet's enclosed data.</param>
        void Post(string sender_id, T payload);
    }
}
