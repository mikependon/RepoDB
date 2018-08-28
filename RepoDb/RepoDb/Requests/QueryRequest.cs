using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RepoDb.Requests
{
    /// <summary>
    /// A class that holds the value of the query operation arguments.
    /// </summary>
    internal class QueryRequest : BaseRequest, IEquatable<QueryRequest>
    {
        private int? m_hashCode = null;

        /// <summary>
        /// Creates a new instance of <see cref="QueryRequest"/> object.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="orderBy">The list of order fields.</param>
        /// <param name="top">The filter for the rows.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public QueryRequest(Type entityType, IDbConnection connection, QueryGroup where = null, IEnumerable<OrderField> orderBy = null, int? top = null, IStatementBuilder statementBuilder = null)
            : base(entityType, connection, statementBuilder)
        {
            Where = where;
            OrderBy = orderBy;
            Top = top;
        }

        /// <summary>
        /// Gets the query expression used.
        /// </summary>
        public QueryGroup Where { get; }

        /// <summary>
        /// Gets the list of the order fields.
        /// </summary>
        public IEnumerable<OrderField> OrderBy { get; }

        /// <summary>
        /// Gets the filter for the rows.
        /// </summary>
        public int? Top { get; }

        // Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="QueryRequest"/>.
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
            var hashCode = $"Query.{EntityType.FullName}".GetHashCode();

            // Add the expression
            if (!ReferenceEquals(null, Where))
            {
                hashCode += Where.GetHashCode();
            }

            // Add the order fields
            if (!ReferenceEquals(null, OrderBy))
            {
                OrderBy.ToList().ForEach(order =>
                {
                    hashCode += order.GetHashCode();
                });
            }

            // Add the filter
            if (!ReferenceEquals(null, Top))
            {
                hashCode += Top.GetHashCode();
            }

            // Set back the hash code value
            m_hashCode = hashCode;

            // Return the actual value
            return hashCode;
        }

        /// <summary>
        /// Compares the <see cref="QueryRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            return GetHashCode() == obj?.GetHashCode();
        }

        /// <summary>
        /// Compares the <see cref="QueryRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(QueryRequest other)
        {
            return GetHashCode() == other?.GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <see cref="QueryRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="QueryRequest"/> object.</param>
        /// <param name="objB">The second <see cref="QueryRequest"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(QueryRequest objA, QueryRequest objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objA?.GetHashCode() == objB?.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="QueryRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="QueryRequest"/> object.</param>
        /// <param name="objB">The second <see cref="QueryRequest"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(QueryRequest objA, QueryRequest objB)
        {
            return (objA == objB) == false;
        }
    }
}
