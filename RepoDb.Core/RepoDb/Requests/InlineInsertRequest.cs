using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RepoDb.Requests
{
    /// <summary>
    /// A class that holds the value of the <i>InlineInsert</i> operation arguments.
    /// </summary>
    internal class InlineInsertRequest : BaseRequest, IEquatable<InlineInsertRequest>
    {
        private int? _hashCode = null;

        /// <summary>
        /// Creates a new instance of <i>InlineInsertRequest</i> object.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="fields">The list of the target fields.</param>
        /// <param name="overrideIgnore">The value whether to override the ignored fields.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public InlineInsertRequest(Type entityType, IDbConnection connection, IEnumerable<Field> fields = null, bool? overrideIgnore = null, IStatementBuilder statementBuilder = null)
            : base(entityType, connection, statementBuilder)
        {
            Fields = fields;
            OverrideIgnore = overrideIgnore;
        }
        /// <summary>
        /// Gets the target fields.
        /// </summary>
        public IEnumerable<Field> Fields { get; set; }

        /// <summary>
        /// Gets the value whether to override the ignored fields.
        /// </summary>
        public bool? OverrideIgnore { get; set; }

        // Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <i>InlineInsertRequest</i>.
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
            var hashCode = $"InlineInsert.{EntityType.FullName}".GetHashCode();

            // Get the qualifier fields
            if (Fields != null)
            {
                Fields.ToList().ForEach(field =>
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
        /// Compares the <i>InlineInsertRequest</i> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            return GetHashCode() == obj?.GetHashCode();
        }

        /// <summary>
        /// Compares the <i>InlineInsertRequest</i> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(InlineInsertRequest other)
        {
            return GetHashCode() == other?.GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <i>InlineInsertRequest</i> objects.
        /// </summary>
        /// <param name="objA">The first <i>InlineInsertRequest</i> object.</param>
        /// <param name="objB">The second <i>InlineInsertRequest</i> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(InlineInsertRequest objA, InlineInsertRequest objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objA?.GetHashCode() == objB?.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <i>InlineInsertRequest</i> objects.
        /// </summary>
        /// <param name="objA">The first <i>InlineInsertRequest</i> object.</param>
        /// <param name="objB">The second <i>InlineInsertRequest</i> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(InlineInsertRequest objA, InlineInsertRequest objB)
        {
            return (objA == objB) == false;
        }
    }
}
