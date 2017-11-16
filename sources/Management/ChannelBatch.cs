using rharel.Debug;
using rharel.M3PD.Common.Collections;
using rharel.M3PD.Common.DesignPatterns;
using rharel.M3PD.Communication.Channels;
using rharel.M3PD.Communication.Packets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rharel.M3PD.Communication.Management
{
    /// <summary>
    /// Represents a batch of double-buffered data channels, organized by the 
    /// type of data they carry.
    /// </summary>
    internal sealed class ChannelBatch: ImmutableChannelBatch
    {
        /// <summary>
        /// Builds instances of <see cref="ChannelBatch"/>. 
        /// </summary>
        public sealed class Builder: ObjectBuilder<ChannelBatch>
        {
            /// <summary>
            /// Specifies that the batch should include a channel for the 
            /// specified type.
            /// </summary>
            /// <typeparam name="T">
            /// The type of data the channel carries.
            /// </typeparam>
            /// <returns>This instance (for method call-chaining).</returns>
            /// <exception cref="InvalidOperationException">
            /// When called on an already built object.
            /// </exception>
            public Builder WithChannel<T>()
            {
                Require.IsFalse<InvalidOperationException>(IsBuilt);

                Type type = typeof(T);

                if (_channel_by_data_type.ContainsKey(type)) { return this; }

                _channel_by_data_type.Add(
                    type,
                    new DoubleBufferChannel<T>()
                );

                return this;
            }

            /// <summary>
            /// Creates the object.
            /// </summary>
            /// <returns>
            /// The built object.
            /// </returns>
            protected override ChannelBatch CreateObject()
            {
                return new ChannelBatch(_channel_by_data_type.Values);
            }

            private readonly Dictionary<Type, DoubleBufferDataChannel>
                _channel_by_data_type = (
                    new Dictionary<Type, DoubleBufferDataChannel>()
                );
        }

        /// <summary>
        /// Creates a new batch from the specified channels.
        /// </summary>
        /// <param name="channels">The channels to hold.</param>
        /// <exception cref="ArgumentNullException">
        /// When <paramref name="channels"/> is null.
        /// </exception>
        private ChannelBatch(IEnumerable<DoubleBufferDataChannel> channels)
        {
            Require.IsNotNull(channels);

            foreach (var channel in channels)
            {
                _channel_by_data_type.Add(channel.DataType, channel);
            }
            DataTypes = new CollectionView<Type>(_channel_by_data_type.Keys);
        }

        /// <summary>
        /// Gets the supported data types.
        /// </summary>
        public ImmutableCollection<Type> DataTypes { get; }

        /// <summary>
        /// Gets the channel corresponding to the specified data type.
        /// </summary>
        /// <typeparam name="T">
        /// The type of data the channel carries.
        /// </typeparam>
        /// <returns>
        /// The corresponding channel of the specified data type.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// When attempting to retrieve a channel of an unsupported data type.
        /// </exception>
        public Channel<T> Get<T>()
        {
            Require.IsTrue(DataTypes.Contains(typeof(T)));

            return (Channel<T>)_channel_by_data_type[typeof(T)];
        }

        /// <summary>
        /// Gets the packets of the channel corresponding to the specified data 
        /// type.
        /// </summary>
        /// <typeparam name="T">
        /// The type of data the channel carries.
        /// </typeparam>
        /// <returns>The packets live on the specified channel.</returns>
        /// <exception cref="ArgumentException">
        /// When attempting to retrieve packets of an unsupported data type.
        /// </exception>
        /// <remarks>
        /// <see cref="GetPackets(Type)"/> for the non-generic version.
        /// </remarks>
        public ImmutableCollection<Packet<T>> GetPackets<T>()
        {
            if (!DataTypes.Contains(typeof(T)))
            {
                throw new ArgumentException(nameof(T));
            }
            return ((Channel<T>)_channel_by_data_type[typeof(T)]).Packets;
        }
        /// <summary>
        /// Gets the packets of the channel corresponding to the specified data 
        /// type.
        /// </summary>
        /// <param name="type">The type of data the channel carries.</param>
        /// <returns>The packets live on the specified channel.</returns>
        /// <exception cref="ArgumentException">
        /// When attempting to retrieve packets of an unsupported data type.
        /// </exception>
        /// <remarks>
        /// <see cref="GetPackets{T}"/> for the generic version.
        /// </remarks>
        public ImmutableCollection<DataPacket> GetPackets(Type type)
        {
            if (!DataTypes.Contains(type))
            {
                throw new ArgumentException(nameof(type));
            }
            return _channel_by_data_type[type].Packets;
        }

        /// <summary>
        /// Clears all channel back buffers.
        /// </summary>
        public void Clear()
        {
            foreach (var channel in Channels)
            {
                channel.Clear();
            }
        }

        /// <summary>
        /// Swaps all channel buffers.
        /// </summary>
        public void SwapBuffers()
        {
            foreach (var channel in Channels)
            {
                channel.SwapBuffers();
            }
        }

        /// <summary>
        /// Returns a string that represents this instance.
        /// </summary>
        /// <returns>
        /// A human-readable string.
        /// </returns>
        public override string ToString()
        {
            string types = string.Join(
                ", ",
                DataTypes.Select(item => item.ToString())
                         .ToArray()
            );
            return $"{nameof(ChannelBatch)}{{ " +
                   $"{nameof(DataTypes)} = [{types}] }}";
        }

        /// <summary>
        /// Gets an enumeration of all channels.
        /// </summary>
        private IEnumerable<DoubleBufferDataChannel> Channels => (
            _channel_by_data_type.Values                
        );

        private readonly Dictionary<Type, DoubleBufferDataChannel>
            _channel_by_data_type = (
                new Dictionary<Type, DoubleBufferDataChannel>()
            );
    }
}
