using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RepoDb.Requests
{
    /// <summary>
    /// A class that holds the value of the <i>InlineMerge</i> operation arguments.
    /// </summary>
    internal class InlineMergeRequest : BaseRequest, IEquatable<InlineMergeRequest>
    {
        private int? _hashCode = null;

        /// <summary>
        /// Creates a new instance of <i>InlineMergeRequest</i> object.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="fields">The list of the target fields.</param>
        /// <param name="qualifiers">The list of the qualifier fields.</param>
        /// <param name="overrideIgnore">The value whether to override the ignored fields.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public InlineMergeRequest(Type entityType, IDbConnection connection, IEnumerable<Field> fields = null, IEnumerable<Field> qualifiers = null, bool? overrideIgnore = null, IStatementBuilder statementBuilder = null)
            : base(entityType, connection, statementBuilder)
        {
            Fields = fields;
            Qualifiers = qualifiers;
            OverrideIgnore = overrideIgnore;
        }

        /// <summary>
        /// Gets the list of the target fields.
        /// </summary>
        public IEnumerable<Field> Fields { get; set; }

        /// <summary>
        /// Gets the list of the qualifier fields.
        /// </summary>
        public IEnumerable<Field> Qualifiers { get; set; }

        /// <summary>
        /// Gets the value whether to override the ignored fields.
        /// </summary>
        public bool? OverrideIgnore { get; set; }

        // Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <i>InlineMergeRequest</i>.
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
            var hashCode = $"InlineMerge.{EntityType.FullName}".GetHashCode();

            // Gets the target fields
            if (Fields != null)
            {
                Fields.ToList().ForEach(field =>
                {
                    hashCode += field.GetHashCode();
                });
            }

            // Gets the qualifier fields
            if (Qualifiers != null)
            {
                Qualifiers.ToList().ForEach(field =>
                {
                    hashCode += field.GetHashCode();
                });
            }

            // Override ignore hashcode
            if (OverrideIgnore != null)
            {
                hashCode += OverrideIgnore.GetHashCode();
            }

            // Set back the hash code value
            _hashCode = hashCode;

            // Return the actual value
            return hashCode;
        }

        /// <summary>
        /// Compares the <i>InlineMergeRequest</i> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            return GetHashCode() == obj?.GetHashCode();
        }

        /// <summary>
        /// Compares the <i>InlineMergeRequest</i> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(InlineMergeRequest other)
        {
            return GetHashCode() == other?.GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <i>InlineMergeRequest</i> objects.
        /// </summary>
        /// <param name="objA">The first <i>InlineMergeRequest</i> object.</param>
        /// <param name="objB">The second <i>InlineMergeRequest</i> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(InlineMergeRequest objA, InlineMergeRequest objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objA?.GetHashCode() == objB?.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <i>InlineMergeRequest</i> objects.
        /// </summary>
        /// <param name="objA">The first <i>InlineMergeRequest</i> object.</param>
        /// <param name="objB">The second <i>InlineMergeRequest</i> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(InlineMergeRequest objA, InlineMergeRequest objB)
        {
            return (objA == objB) == false;
        }
    }
}
