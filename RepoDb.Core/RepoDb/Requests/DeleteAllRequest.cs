using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb.Requests
{
    /// <summary>
    /// A class that holds the value of the delete-all operation arguments.
    /// </summary>
    internal class DeleteAllRequest : BaseRequest, IEquatable<DeleteAllRequest>
    {
        private int? m_hashCode = null;

        /// <summary>
        /// Creates a new instance of <see cref="DeleteAllRequest"/> object.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public DeleteAllRequest(Type entityType, IDbConnection connection, IStatementBuilder statementBuilder = null)
            : base(entityType, connection, statementBuilder)
        {
        }

        // Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="DeleteAllRequest"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            // Make sure to return if it is already provided
            if (!ReferenceEquals(null, m_hashCode))
            {
                return m_hashCode.Value;
            }

            // Get first the entity hash code
            var hashCode = $"DeleteAll.{EntityType.FullName}".GetHashCode();

            // Set back the hash code value
            m_hashCode = hashCode;

            // Return the actual value
            return hashCode;
        }

        /// <summary>
        /// Compares the <see cref="DeleteAllRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            return GetHashCode() == obj?.GetHashCode();
        }

        /// <summary>
        /// Compares the <see cref="DeleteAllRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(DeleteAllRequest other)
        {
            return GetHashCode() == other?.GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <see cref="DeleteAllRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="DeleteAllRequest"/> object.</param>
        /// <param name="objB">The second <see cref="DeleteAllRequest"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(DeleteAllRequest objA, DeleteAllRequest objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objA?.GetHashCode() == objB?.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="DeleteAllRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="DeleteAllRequest"/> object.</param>
        /// <param name="objB">The second <see cref="DeleteAllRequest"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(DeleteAllRequest objA, DeleteAllRequest objB)
        {
            return (objA == objB) == false;
        }
    }
}
