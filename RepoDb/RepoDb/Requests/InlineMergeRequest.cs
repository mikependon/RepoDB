using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RepoDb.Requests
{
    /// <summary>
    /// A class that holds the value of the inline-merge operation arguments.
    /// </summary>
    internal class InlineMergeRequest : BaseRequest, IEquatable<InlineMergeRequest>
    {
        private int? m_hashCode = null;

        /// <summary>
        /// Creates a new instance of <see cref="InlineMergeRequest"/> object.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="fields">The list of the target fields.</param>
        /// <param name="qualifiers">The list of the qualifier fields.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public InlineMergeRequest(Type entityType, IDbConnection connection, IEnumerable<Field> fields = null, IEnumerable<Field> qualifiers = null, IStatementBuilder statementBuilder = null)
            : base(entityType, connection, statementBuilder)
        {
            Fields = fields;
            Qualifiers = qualifiers;
        }

        /// <summary>
        /// Gets the list of the target fields.
        /// </summary>
        public IEnumerable<Field> Fields { get; set; }

        /// <summary>
        /// Gets the list of the qualifier fields.
        /// </summary>
        public IEnumerable<Field> Qualifiers { get; set; }

        // Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="InlineMergeRequest"/>.
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
            var hashCode = string.Concat(EntityType.FullName, ".InlineMerge").GetHashCode();

            // Gets the target fields
            if (Fields != null)
            {
                foreach (var field in Fields)
                {
                    hashCode ^= field.GetHashCode();
                }
            }

            // Gets the qualifier fields
            if (Qualifiers != null)
            {
                foreach (var field in Qualifiers)
                {
                    hashCode ^= field.GetHashCode();
                }
            }

            // Set back the hash code value
            m_hashCode = hashCode;

            // Return the actual value
            return hashCode;
        }

        /// <summary>
        /// Compares the <see cref="InlineMergeRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            return obj?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the <see cref="InlineMergeRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(InlineMergeRequest other)
        {
            return other?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <see cref="InlineMergeRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="InlineMergeRequest"/> object.</param>
        /// <param name="objB">The second <see cref="InlineMergeRequest"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(InlineMergeRequest objA, InlineMergeRequest objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objB?.GetHashCode() == objA.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="InlineMergeRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="InlineMergeRequest"/> object.</param>
        /// <param name="objB">The second <see cref="InlineMergeRequest"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(InlineMergeRequest objA, InlineMergeRequest objB)
        {
            return (objA == objB) == false;
        }
    }
}
