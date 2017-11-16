using rharel.Debug;
using rharel.M3PD.Common.Hashing;

namespace rharel.M3PD.Communication.Packets
{
    /// <summary>
    /// Envelopes a piece of data in transit.
    /// </summary>
    /// <remarks>
    /// <see cref="Packet{T}"/> for the generic version.
    /// </remarks>
    public interface DataPacket
    {
        /// <summary>
        /// Gets this packet's sender identifier.
        /// </summary>
        string SenderID { get; }
        /// <summary>
        /// Gets the enclosed data.
        /// </summary>
        object Payload { get; }
    }
    /// <summary>
    /// Envelopes a piece of data in transit. 
    /// </summary>
    /// <typeparam name="T">The type of the payload data.</typeparam>
    /// <remarks>
    /// <see cref="DataPacket"/> for the non-generic version.
    /// </remarks>
    public struct Packet<T>: DataPacket
    {
        /// <summary>
        /// Creates a new packet.
        /// </summary>
        /// <param name="sender_id">The sender identifier.</param>
        /// <param name="payload">The payload to enclose.</param>
        /// <exception cref="ArgumentException">
        /// When <paramref name="sender_id"/> is blank.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// When <paramref name="payload"/> is null.  
        /// </exception>
        internal Packet(string sender_id, T payload)
        {
            Require.IsNotBlank(sender_id);
            Require.IsNotNull(payload);

            SenderID = sender_id;
            Payload = payload;
        }

        /// <summary>
        /// Gets this packet's sender identifier.
        /// </summary>
        public string SenderID { get; }

        /// <summary>
        /// Gets the enclosed data.
        /// </summary>
        /// <remarks>
        /// <see cref="DataPacket.Payload"/> for the non-generic version.
        /// </remarks>
        public T Payload { get; }
        /// <summary>
        /// Gets the enclosed data.
        /// </summary>
        /// <remarks>
        /// <see cref="Payload"/> for the generic version.
        /// </remarks>
        object DataPacket.Payload => Payload;

        /// <summary>
        /// Determines whether the specified object is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>
        /// True if the specified object is equal to this instance; 
        /// otherwise false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is DataPacket other)
            {
                if (other.Payload is T other_payload)
                {
                    return other.SenderID.Equals(SenderID) &&
                           other_payload.Equals(Payload);
                }
            }
            return false;
        }
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing 
        /// algorithms and data structures like a hash table. 
        /// </returns>  
        public override int GetHashCode()
        {
            var hash = HashCombiner.Initialize();
            hash = HashCombiner.Hash(hash, SenderID);
            hash = HashCombiner.Hash(hash, Payload);

            return hash;
        }

        /// <summary>
        /// Returns a string that represents this instance.
        /// </summary>
        /// <returns>
        /// A human-readable string.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Packet<T>)}{{ " +
                   $"{nameof(SenderID)} = {SenderID}, " +
                   $"{nameof(Payload)} = {Payload} }}";
        }
    }
}
