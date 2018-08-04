using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace RepoDb.Requests
{
    /// <summary>
    /// A class that holds the value of the Query operation arguments.
    /// </summary>
    internal class QueryRequest : IEquatable<QueryRequest>
    {
        private int? _hashCode = null;

        /// <summary>
        /// Creates a new instance of <i>QueryRequest</i> object.
        /// </summary>
        /// <param name="connection">The connection object.</param>
        /// <param name="entityType">The entity type.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="orderBy">The list of order fields.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        /// <param name="top">The filter for the rows.</param>
        public QueryRequest(Type entityType, IDbConnection connection, QueryGroup where, IEnumerable<OrderField> orderBy, int? top, IStatementBuilder statementBuilder = null)
        {
            EntityType = entityType;
            Connection = connection;
            Where = where;
            OrderBy = orderBy;
            Top = top;
            StatementBuilder = statementBuilder;
        }

        /// <summary>
        /// Gets the entity type.
        /// </summary>
        public Type EntityType { get; }

        /// <summary>
        /// Gets the connection object.
        /// </summary>
        public IDbConnection Connection { get; }

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

        /// <summary>
        /// Gets the statement builder.
        /// </summary>
        public IStatementBuilder StatementBuilder { get; }

        // Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <i>QueryRequest</i>.
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
            var hashCode = EntityType.FullName.GetHashCode();

            // Add the expression
            if (!ReferenceEquals(null, Where))
            {
                hashCode += Where.GetHashCode();
            }

            // Add the order fields
            if (!ReferenceEquals(null, OrderBy))
            {
                hashCode += OrderBy.GetHashCode();
            }

            // Add the filter
            if (!ReferenceEquals(null, Top))
            {
                hashCode += Top.GetHashCode();
            }

            // Set back the hash code value
            _hashCode = hashCode;

            // Return the actual value
            return hashCode;
        }

        /// <summary>
        /// Compares the <i>QueryRequest</i> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            return GetHashCode() == obj?.GetHashCode();
        }

        /// <summary>
        /// Compares the <i>QueryRequest</i> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(QueryRequest other)
        {
            return GetHashCode() == other?.GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <i>QueryRequest</i> objects.
        /// </summary>
        /// <param name="objA">The first <i>QueryRequest</i> object.</param>
        /// <param name="objB">The second <i>QueryRequest</i> object.</param>
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
        /// Compares the inequality of the two <i>QueryRequest</i> objects.
        /// </summary>
        /// <param name="objA">The first <i>QueryRequest</i> object.</param>
        /// <param name="objB">The second <i>QueryRequest</i> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(QueryRequest objA, QueryRequest objB)
        {
            return (objA == objB) == false;
        }
    }
}
