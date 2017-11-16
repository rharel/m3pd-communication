using rharel.Debug;
using rharel.M3PD.Common.Collections;
using rharel.M3PD.Communication.Packets;
using System;

namespace rharel.M3PD.Communication.Channels
{
    /// <summary>
    /// Represents a double-buffered communication channel of data packets.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Double-buffering here means that all accessor operations act on a front 
    /// buffer, while all mutator operations (such as packet- posting and 
    /// clearing) affect a back buffer. When ready to make the changes made
    /// to the back buffer visible, the two buffers may be swapped.
    /// </para>
    /// <para>
    /// <see cref="DoubleBufferChannel{T}"/> for the generic version.
    /// </para>
    /// </remarks>
    internal interface DoubleBufferDataChannel: DataChannel
    {
        /// <summary>
        /// Swaps the front and back buffers.
        /// </summary>
        void SwapBuffers();
    }
    /// <summary>
    /// Represents a double-buffered communication channel of data packets.
    /// </summary>
    /// <typeparam name="T">The type of data the channel carries.</typeparam>
    /// <remarks>
    /// <see cref="DoubleBufferDataChannel"/> for the non-generic version.
    /// </remarks>
    internal sealed class DoubleBufferChannel<T> : DoubleBufferDataChannel,
                                                   Channel<T>
    {
        /// <summary>
        /// Gets the type of data the channel carries.
        /// </summary>
        public Type DataType { get; } = typeof(T);

        /// <summary>
        /// Gets the packets currently live on the channel.
        /// </summary>
        /// <remarks>
        /// <see cref="DataChannel.Packets"/> for the non-generic version.
        /// </remarks>
        public ImmutableCollection<Packet<T>> Packets => (
            _front_buffer.Packets
        );
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
        /// Clears the back buffer.
        /// </summary>
        public void Clear()
        {
            _back_buffer.Clear();
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

            _back_buffer.Post(sender_id, payload);
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
        /// Swaps the front and back buffers.
        /// </summary>
        public void SwapBuffers()
        {
            var formerly_front = _front_buffer;
            _front_buffer = _back_buffer;
            _back_buffer = formerly_front;
        }

        /// <summary>
        /// Returns a string that represents this instance.
        /// </summary>
        /// <returns>
        /// A human-readable string.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(DoubleBufferChannel<T>)}{{ " +
                   $"{nameof(DataType)} = {DataType}, " +
                   $"{nameof(Packets)} = {Packets} }}";
        }

        private Channel<T> _front_buffer = (
            new SingleBufferChannel<T>()
        );
        private Channel<T> _back_buffer = (
            new SingleBufferChannel<T>()
        );
    }
}
