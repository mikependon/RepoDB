using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RepoDb.Requests
{
    /// <summary>
    /// A class that holds the value of the <i>Merge</i> operation arguments.
    /// </summary>
    internal class MergeRequest : BaseRequest, IEquatable<MergeRequest>
    {
        private int? _hashCode = null;

        /// <summary>
        /// Creates a new instance of <i>MergeRequest</i> object.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <param name="qualifiers">The list of qualifier fields.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public MergeRequest(Type entityType, IDbConnection connection, IEnumerable<Field> qualifiers = null, IStatementBuilder statementBuilder = null)
            : base(entityType, connection, statementBuilder)
        {
            Qualifiers = qualifiers;
        }

        /// <summary>
        /// Gets the qualifier fields.
        /// </summary>
        public IEnumerable<Field> Qualifiers { get; set; }

        // Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <i>MergeRequest</i>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            // Make sure to return if it is already provided
            if (!ReferenceEquals(null, _hashCode))
            {
                return _hashCode.Value;
            }

            // Get first the entity hash code
            var hashCode = $"Merge.{EntityType.FullName}".GetHashCode();

            // Get the qualifier fields
            if (Qualifiers != null) // Much faster than Qualifers?.<Methods|Properties>
            {
                Qualifiers.ToList().ForEach(field =>
                {
                    hashCode += field.GetHashCode();
                });
            }

            // Set back the hash code value
            _hashCode = hashCode;

            // Return the actual value
            return hashCode;
        }

        /// <summary>
        /// Compares the <i>MergeRequest</i> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            return GetHashCode() == obj?.GetHashCode();
        }

        /// <summary>
        /// Compares the <i>MergeRequest</i> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(MergeRequest other)
        {
            return GetHashCode() == other?.GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <i>MergeRequest</i> objects.
        /// </summary>
        /// <param name="objA">The first <i>MergeRequest</i> object.</param>
        /// <param name="objB">The second <i>MergeRequest</i> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(MergeRequest objA, MergeRequest objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objA?.GetHashCode() == objB?.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <i>MergeRequest</i> objects.
        /// </summary>
        /// <param name="objA">The first <i>MergeRequest</i> object.</param>
        /// <param name="objB">The second <i>MergeRequest</i> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(MergeRequest objA, MergeRequest objB)
        {
            return (objA == objB) == false;
        }
    }
}
