using static rharel.M3PD.Communication.Management.CommunicationManager;

namespace rharel.M3PD.Communication.Management
{
    /// <summary>
    /// Represents an agent in the scene.
    /// </summary>
    public interface Agent
    {
        /// <summary>
        /// This agent's identifier.
        /// </summary>
        string ID { get; }

        /// <summary>
        /// Invoked by <see cref="CommunicationManager"/> during the perception
        /// stage of an update iteration.
        /// </summary>
        /// <param name="manager">The manager invoking this method.</param>
        /// <param name="channels">
        /// The data available for perception, organized through channels.
        /// </param>
        void Perceive(CommunicationManager manager, 
                      ImmutableChannelBatch channels);
        /// <summary>
        /// Invoked by <see cref="CommunicationManager"/> during the action
        /// stage of an update iteration.
        /// </summary>
        /// <param name="submission">
        /// A handle through which agents submit data to the communication
        /// manager.
        /// </param>
        void Act(DataSubmission submission);
    }
}
