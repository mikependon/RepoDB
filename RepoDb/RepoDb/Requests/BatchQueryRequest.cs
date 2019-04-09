using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace RepoDb.Requests
{
    /// <summary>
    /// A class that holds the value of the batch query operation arguments.
    /// </summary>
    internal class BatchQueryRequest : BaseRequest, IEquatable<BatchQueryRequest>
    {
        private int? m_hashCode = null;

        /// <summary>
        /// Creates a new instance of <see cref="BatchQueryRequest"/> object.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="page">The page of the batch.</param>
        /// <param name="rowsPerBatch">The number of rows per batch.</param>
        /// <param name="orderBy">The list of order fields.</param>
        /// <param name="hints">The hints for the table.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public BatchQueryRequest(Type entityType, IDbConnection connection, QueryGroup where = null, int? page = null, int? rowsPerBatch = null,
            IEnumerable<OrderField> orderBy = null, string hints = null, IStatementBuilder statementBuilder = null)
            : base(entityType, connection, statementBuilder)
        {
            Where = where;
            Page = page;
            RowsPerBatch = rowsPerBatch;
            OrderBy = orderBy;
            Hints = hints;
        }

        /// <summary>
        /// Gets the query expression used.
        /// </summary>
        public QueryGroup Where { get; }

        /// <summary>
        /// Gets the filter for the rows.
        /// </summary>
        public int? Page { get; }

        /// <summary>
        /// Gets the number of rows per batch.
        /// </summary>
        public int? RowsPerBatch { get; }

        /// <summary>
        /// Gets the list of the order fields.
        /// </summary>
        public IEnumerable<OrderField> OrderBy { get; }

        /// <summary>
        /// Gets the hints for the table.
        /// </summary>
        public string Hints { get; }

        // Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="BatchQueryRequest"/>.
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
            var hashCode = string.Concat(EntityType.FullName, ".BatchQuery").GetHashCode();

            // Add the expression
            if (!ReferenceEquals(null, Where))
            {
                hashCode ^= Where.GetHashCode();
            }

            // Add the filter
            if (!ReferenceEquals(null, Page))
            {
                hashCode ^= Page.GetHashCode();
            }

            // Add the filter
            if (!ReferenceEquals(null, RowsPerBatch))
            {
                hashCode ^= RowsPerBatch.GetHashCode();
            }

            // Add the order fields
            if (!ReferenceEquals(null, OrderBy))
            {
                foreach (var orderField in OrderBy)
                {
                    hashCode ^= orderField.GetHashCode();
                }
            }

            // Add the hints
            if (!ReferenceEquals(null, Hints))
            {
                hashCode ^= Hints.GetHashCode();
            }

            // Set back the hash code value
            m_hashCode = hashCode;

            // Return the actual value
            return hashCode;
        }

        /// <summary>
        /// Compares the <see cref="BatchQueryRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            return obj?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the <see cref="BatchQueryRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(BatchQueryRequest other)
        {
            return other?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <see cref="BatchQueryRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="BatchQueryRequest"/> object.</param>
        /// <param name="objB">The second <see cref="BatchQueryRequest"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(BatchQueryRequest objA, BatchQueryRequest objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objB?.GetHashCode() == objA.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="BatchQueryRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="BatchQueryRequest"/> object.</param>
        /// <param name="objB">The second <see cref="BatchQueryRequest"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(BatchQueryRequest objA, BatchQueryRequest objB)
        {
            return (objA == objB) == false;
        }
    }
}
