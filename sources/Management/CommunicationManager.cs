using rharel.Debug;
using rharel.Functional;
using rharel.M3PD.Common.Collections;
using rharel.M3PD.Common.DesignPatterns;
using System;
using System.Collections.Generic;

namespace rharel.M3PD.Communication.Management
{
    /// <summary>
    /// Manages emitted/perceived data by agents in the scene.
    /// </summary>
    public sealed partial class CommunicationManager
    {
        /// <summary>
        /// Builds instances of <see cref="CommunicationManager"/>.
        /// </summary>
        public sealed class Builder: ObjectBuilder<CommunicationManager>
        {
            /// <summary>
            /// Specifies that the manager should support the specified data 
            /// type.
            /// </summary>
            /// <typeparam name="T">The type of data to support.</typeparam>
            /// <returns>This instance (for method call-chaining).</returns>
            /// <exception cref="InvalidOperationException">
            /// When called on an already built object.
            /// </exception>
            public Builder Support<T>()
            {
                if (IsBuilt)
                {
                    throw new InvalidOperationException(
                        "Object is already built."
                    );
                }

                _channel_batch_builder.WithChannel<T>();

                return this;
            }

            /// <summary>
            /// Creates the object.
            /// </summary>
            /// <returns>
            /// The built object.
            /// </returns>
            protected override CommunicationManager CreateObject()
            {
                var batch = _channel_batch_builder.Build();
                return new CommunicationManager(batch);
            }

            private readonly ChannelBatch.Builder _channel_batch_builder = (
                new ChannelBatch.Builder()
            );
        }

        /// <summary>
        /// Creates a new manager with the specified channels.
        /// </summary>
        /// <param name="channels">The channels to support.</param>
        /// <exception cref="ArgumentNullException">
        /// When <paramref name="channels"/> is null.
        /// </exception>
        private CommunicationManager(ChannelBatch channels)
        {
            Require.IsNotNull(channels);

            _channels = channels;

            Channels = _channels;
            Agents = new CollectionView<Agent>(_agents);
        }

        /// <summary>
        /// Gets supported data types.
        /// </summary>
        public ImmutableChannelBatch Channels { get; }

        /// <summary>
        /// Gets engaged agents.
        /// </summary>
        public ImmutableCollection<Agent> Agents { get; }

        /// <summary>
        /// Engages the specified agent in communications.
        /// </summary>
        /// <param name="agent">The agent to engage.</param>
        /// <returns>
        /// True iff <paramref name="agent"/> was not already engaged.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// When <paramref name="agent"/> is null.
        /// </exception>
        public bool Engage(Agent agent)
        {
            if (agent == null)
            {
                throw new ArgumentNullException(nameof(agent));
            }
            if (Agents.Contains(agent)) { return false; }
            else
            {
                _agents.Add(agent);
                return true;
            }
        }
        /// <summary>
        /// Disengages the specified agent from communications.
        /// </summary>
        /// <param name="agent">The agent to disengage.</param>
        /// <returns>
        /// True iff <paramref name="agent"/> was not already disengaged.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// When <paramref name="agent"/> is null.
        /// </exception>
        public bool Disengage(Agent agent)
        {
            if (agent == null)
            {
                throw new ArgumentNullException(nameof(agent));
            }
            return _agents.Remove(agent);
        }

        /// <summary>
        /// Performs an iteration of the global update routine.
        /// </summary>
        public void Update()
        {
            foreach (var agent in Agents)
            {
                agent.Perceive(this, _channels);
                
                var submission = new DataSubmission(this, agent.ID);
                ActiveDataSubmission = new Some<DataSubmission>(submission);
                agent.Act(submission);
            }

            ActiveDataSubmission = new None<DataSubmission>();

            _channels.SwapBuffers();
            _channels.Clear();
        }

        /// <summary>
        /// Returns a string that represents this instance.
        /// </summary>
        /// <returns>
        /// A human-readable string.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(CommunicationManager)}{{ " +
                   $"{nameof(Channels)} = {Channels} }}";
        }

        /// <summary>
        /// Gets or sets the active data submission.
        /// </summary>
        private Optional<DataSubmission> ActiveDataSubmission { get; set; } = (
            new None<DataSubmission>()
        );

        private readonly ChannelBatch _channels;
        private readonly ICollection<Agent> _agents = new HashSet<Agent>();
    }
}
