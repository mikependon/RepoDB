using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace RepoDb.Requests
{
    /// <summary>
    /// A class that holds the value of the inline-insert operation arguments.
    /// </summary>
    internal class InlineInsertRequest : BaseRequest, IEquatable<InlineInsertRequest>
    {
        private int? m_hashCode = null;

        /// <summary>
        /// Creates a new instance of <see cref="InlineInsertRequest"/> object.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="fields">The list of the target fields.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public InlineInsertRequest(Type entityType, IDbConnection connection, IEnumerable<Field> fields = null, IStatementBuilder statementBuilder = null)
            : this(entityType.FullName, connection, fields, statementBuilder)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="InlineInsertRequest"/> object.
        /// </summary>
        /// <param name="name">The name of the request.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="fields">The list of the target fields.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public InlineInsertRequest(string name, IDbConnection connection, IEnumerable<Field> fields = null, IStatementBuilder statementBuilder = null)
            : base(name, connection, statementBuilder)
        {
            Fields = fields;
        }

        /// <summary>
        /// Gets the target fields.
        /// </summary>
        public IEnumerable<Field> Fields { get; set; }

        // Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="InlineInsertRequest"/>.
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
            var hashCode = string.Concat(Name, ".InlineInsert").GetHashCode();

            // Get the qualifier fields
            if (Fields != null)
            {
                foreach (var field in Fields)
                {
                    hashCode += field.GetHashCode();
                }
            }

            // Set back the hash code value
            m_hashCode = hashCode;

            // Return the actual value
            return hashCode;
        }

        /// <summary>
        /// Compares the <see cref="InlineInsertRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            return obj?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the <see cref="InlineInsertRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(InlineInsertRequest other)
        {
            return other?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <see cref="InlineInsertRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="InlineInsertRequest"/> object.</param>
        /// <param name="objB">The second <see cref="InlineInsertRequest"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(InlineInsertRequest objA, InlineInsertRequest objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objB?.GetHashCode() == objA.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="InlineInsertRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="InlineInsertRequest"/> object.</param>
        /// <param name="objB">The second <see cref="InlineInsertRequest"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(InlineInsertRequest objA, InlineInsertRequest objB)
        {
            return (objA == objB) == false;
        }
    }
}
