using rharel.M3PD.Common.Collections;
using rharel.M3PD.Communication.Packets;
using System;

namespace rharel.M3PD.Communication.Management
{
    /// <summary>
    /// Represents a batch of data channels, organized by the type of data they
    /// carry.
    /// </summary>
    public interface ImmutableChannelBatch
    {
        /// <summary>
        /// Gets the supported data types.
        /// </summary>
        ImmutableCollection<Type> DataTypes { get; }

        /// <summary>
        /// Gets the packets of the channel corresponding to the specified data 
        /// type.
        /// </summary>
        /// <typeparam name="T">
        /// The type of data the channel carries.
        /// </typeparam>
        /// <returns>The packets live on the specified channel.</returns>
        /// <remarks>
        /// <see cref="GetPackets(Type)"/> for the non-generic version.
        /// </remarks>
        ImmutableCollection<DataPacket<T>> GetPackets<T>();
        /// <summary>
        /// Gets the packets of the channel corresponding to the specified data 
        /// type.
        /// </summary>
        /// <param name="type">The type of data the channel carries.</param>
        /// <returns>The packets live on the specified channel.</returns>
        /// <remarks>
        /// <see cref="GetPackets{T}"/> for the generic version.
        /// </remarks>
        ImmutableCollection<DataPacket> GetPackets(Type type);
    }
}
