using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb.Requests
{
    /// <summary>
    /// A class that holds the value of the truncate operation arguments.
    /// </summary>
    internal class TruncateRequest : BaseRequest, IEquatable<TruncateRequest>
    {
        private int? m_hashCode = null;

        /// <summary>
        /// Creates a new instance of <see cref="TruncateRequest"/> object.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public TruncateRequest(Type type,
            IDbConnection connection,
            IStatementBuilder statementBuilder = null)
            : this(ClassMappedNameCache.Get(type),
                  connection,
                  statementBuilder)
        {
            Type = type;
        }

        /// <summary>
        /// Creates a new instance of <see cref="TruncateRequest"/> object.
        /// </summary>
        /// <param name="name">The name of the request.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public TruncateRequest(string name,
            IDbConnection connection,
            IStatementBuilder statementBuilder = null)
            : base(name,
                  connection,
                  statementBuilder)
        { }

        // Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="TruncateRequest"/>.
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
            var hashCode = string.Concat(Name, ".Truncate").GetHashCode();

            // Set back the hash code value
            m_hashCode = hashCode;

            // Return the actual value
            return hashCode;
        }

        /// <summary>
        /// Compares the <see cref="TruncateRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            return obj?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the <see cref="TruncateRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(TruncateRequest other)
        {
            return other?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <see cref="TruncateRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="TruncateRequest"/> object.</param>
        /// <param name="objB">The second <see cref="TruncateRequest"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(TruncateRequest objA, TruncateRequest objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objB?.GetHashCode() == objA.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="TruncateRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="TruncateRequest"/> object.</param>
        /// <param name="objB">The second <see cref="TruncateRequest"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(TruncateRequest objA, TruncateRequest objB)
        {
            return (objA == objB) == false;
        }
    }
}
