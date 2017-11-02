using rharel.Debug;
using rharel.M3PD.Common.Hashing;
using System;
using System.Linq;

namespace rharel.M3PD.Communication.Management
{
    public sealed partial class CommunicationManager
    {
        /// <summary>
        /// Represents a handle through which agents submit data to the 
        /// communication manager.
        /// </summary>
        public sealed class DataSubmission
        {
            /// <summary>
            /// Creates a new submission with the specified manager and sender.
            /// Any data submitted through this instance will be in the 
            /// sender's name.
            /// </summary>
            /// <param name="manager">The submission's manager.</param>
            /// <param name="sender_id">
            /// The identifier of the agent in whose 
            /// name to submit.
            /// </param>
            /// <exception cref="ArgumentNullException">
            /// When either <paramref name="manager"/> or
            /// <paramref name="sender_id"/> is null.
            /// </exception>
            /// <exception cref="ArgumentException">
            /// When <paramref name="sender_id"/> is not engaged in 
            /// <paramref name="manager"/>
            /// </exception>
            internal DataSubmission(CommunicationManager manager, 
                                    string sender_id)
            {
                Require.IsNotNull(manager);
                Require.IsNotNull(sender_id);
                Require.IsTrue(
                    manager.Agents.Any(agent => agent.ID == sender_id)
                );

                Manager = manager;
                SenderID = sender_id;
            }

            /// <summary>
            /// Gets this submission's manager.
            /// </summary>
            public CommunicationManager Manager { get; }
            /// <summary>
            /// Gets this submission's sender identifier.
            /// </summary>
            public string SenderID { get; }

            /// <summary>
            /// Indicates whether this submission is active. That is, whether
            /// it is permitted to submit data at this time.
            /// </summary>
            public bool IsActive => (
                Manager.ActiveDataSubmission.Contains(this)
            );

            /// <summary>
            /// Adds the specified data to the submission.
            /// </summary>
            /// <typeparam name="T">The type of data to submit.</typeparam>
            /// <param name="data">The data to submit.</param>
            /// <exception cref="ArgumentException">
            /// When submitting data of an unsupported type.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// When the submission is not active.
            /// </exception>
            public void Add<T>(T data)
            {
                if (!IsActive)
                {
                    throw new InvalidOperationException(
                        "Cannot add through an inactive submission."
                    );
                }
                if (!Manager.DataTypes.Contains(typeof(T)))
                {
                    throw new ArgumentException(nameof(T));
                }
                Manager._channels.Get<T>().Post(SenderID, data);
            }

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
                var other = obj as DataSubmission;

                if (ReferenceEquals(other, null)) { return false; }
                if (ReferenceEquals(other, this)) { return true; }

                return other.Manager.Equals(Manager) &&
                       other.SenderID.Equals(SenderID);
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
                hash = HashCombiner.Hash(hash, Manager);
                hash = HashCombiner.Hash(hash, SenderID);

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
                return $"{nameof(DataSubmission)}{{ " +
                       $"{nameof(Manager)} = {Manager}, " +
                       $"{nameof(SenderID)} = {SenderID} }}";
            }
        }
    }
}
