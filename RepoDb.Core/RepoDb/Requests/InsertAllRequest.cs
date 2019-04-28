using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace RepoDb.Requests
{
    /// <summary>
    /// A class that holds the value of the insert operation arguments.
    /// </summary>
    internal class InsertAllRequest : BaseRequest, IEquatable<InsertAllRequest>
    {
        private int? m_hashCode = null;

        /// <summary>
        /// Creates a new instance of <see cref="InsertAllRequest"/> object.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="fields">The list of the target fields.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public InsertAllRequest(Type type,
            IDbConnection connection,
            IEnumerable<Field> fields = null,
            IStatementBuilder statementBuilder = null)
            : this(ClassMappedNameCache.Get(type),
                  connection,
                  fields,
                  statementBuilder)
        {
            Type = type;
        }

        /// <summary>
        /// Creates a new instance of <see cref="InsertAllRequest"/> object.
        /// </summary>
        /// <param name="name">The name of the request.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="fields">The list of the target fields.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public InsertAllRequest(string name,
            IDbConnection connection,
            IEnumerable<Field> fields = null,
            IStatementBuilder statementBuilder = null)
            : base(name,
                  connection,
                  statementBuilder)
        {
            Fields = fields;
        }

        /// <summary>
        /// Gets the target fields.
        /// </summary>
        public IEnumerable<Field> Fields { get; set; }

        // Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="InsertAllRequest"/>.
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
            var hashCode = string.Concat(Name, ".InsertAll").GetHashCode();

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
        /// Compares the <see cref="InsertAllRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            return obj?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the <see cref="InsertAllRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(InsertAllRequest other)
        {
            return other?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <see cref="InsertAllRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="InsertAllRequest"/> object.</param>
        /// <param name="objB">The second <see cref="InsertAllRequest"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(InsertAllRequest objA, InsertAllRequest objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objB?.GetHashCode() == objA.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="InsertAllRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="InsertAllRequest"/> object.</param>
        /// <param name="objB">The second <see cref="InsertAllRequest"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(InsertAllRequest objA, InsertAllRequest objB)
        {
            return (objA == objB) == false;
        }
    }
}
