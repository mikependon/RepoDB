using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb.Requests
{
    /// <summary>
    /// A class that holds the value of the delete operation arguments.
    /// </summary>
    internal class DeleteRequest : BaseRequest, IEquatable<DeleteRequest>
    {
        private int? m_hashCode = null;

        /// <summary>
        /// Creates a new instance of <see cref="DeleteRequest"/> object.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="transaction">The transaction object.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public DeleteRequest(Type type,
            IDbConnection connection,
            IDbTransaction transaction,
            QueryGroup where = null,
            IStatementBuilder statementBuilder = null)
            : this(ClassMappedNameCache.Get(type),
                  connection,
                  transaction,
                  where,
                  statementBuilder)
        {
            Type = type;
        }

        /// <summary>
        /// Creates a new instance of <see cref="DeleteRequest"/> object.
        /// </summary>
        /// <param name="name">The name of the request.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="transaction">The transaction object.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public DeleteRequest(string name,
            IDbConnection connection,
            IDbTransaction transaction,
            QueryGroup where = null,
            IStatementBuilder statementBuilder = null)
            : base(name,
                  connection,
                  transaction,
                  statementBuilder)
        {
            Where = where;
        }

        /// <summary>
        /// Gets the query expression used.
        /// </summary>
        public QueryGroup Where { get; }

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="DeleteRequest"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            // Make sure to return if it is already provided
            if (m_hashCode != null)
            {
                return m_hashCode.Value;
            }

            // Get first the entity hash code
            var hashCode = string.Concat(Name, ".Delete").GetHashCode();

            // Get the properties hash codes
            if (Where != null)
            {
                hashCode += Where.GetHashCode();
            }

            // Set and return the hashcode
            return (m_hashCode = hashCode).Value;
        }

        /// <summary>
        /// Compares the <see cref="DeleteRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            return obj?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the <see cref="DeleteRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(DeleteRequest other)
        {
            return other?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <see cref="DeleteRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="DeleteRequest"/> object.</param>
        /// <param name="objB">The second <see cref="DeleteRequest"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(DeleteRequest objA, DeleteRequest objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objB?.GetHashCode() == objA.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="DeleteRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="DeleteRequest"/> object.</param>
        /// <param name="objB">The second <see cref="DeleteRequest"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(DeleteRequest objA, DeleteRequest objB)
        {
            return (objA == objB) == false;
        }

        #endregion
    }
}
