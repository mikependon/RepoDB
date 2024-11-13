using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace RepoDb.Requests
{
    /// <summary>
    /// A class that holds the value of the 'BatchQuery' operation arguments.
    /// </summary>
    internal class SkipQueryRequest : BaseRequest
    {
        private int? hashCode = null;

        /// <summary>
        /// Creates a new instance of <see cref="BatchQueryRequest"/> object.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="transaction">The transaction object.</param>
        /// <param name="fields">The list of the target fields.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="take">The number of rows per batch.</param>
        /// <param name="orderBy">The list of order fields.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The hints for the table.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public SkipQueryRequest(Type type,
            IDbConnection connection,
            IDbTransaction transaction,
            IEnumerable<Field> fields,
            int skip,
            int take,
            IEnumerable<OrderField> orderBy,
            QueryGroup? where = null,
            string? hints = null,
            IStatementBuilder? statementBuilder = null)
            : this(ClassMappedNameCache.Get(type),
                  connection,
                  transaction,
                  fields,
                  skip,
                  take,
                  orderBy,
                  where,
                  hints,
                  statementBuilder)
        {
            Type = type;
        }

        /// <summary>
        /// Creates a new instance of <see cref="BatchQueryRequest"/> object.
        /// </summary>
        /// <param name="name">The name of the request.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="transaction">The transaction object.</param>
        /// <param name="fields">The list of the target fields.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="take">The number of rows per batch.</param>
        /// <param name="orderBy">The list of order fields.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The hints for the table.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public SkipQueryRequest(string name,
            IDbConnection connection,
            IDbTransaction transaction,
            IEnumerable<Field> fields,
            int skip,
            int take,
            IEnumerable<OrderField> orderBy,
            QueryGroup? where = null,
            string? hints = null,
            IStatementBuilder? statementBuilder = null)
            : base(name,
                  connection,
                  transaction,
                  statementBuilder)
        {
            Fields = fields?.AsList();
            Where = where;
            Skip = skip;
            RowsPerBatch = take;
            OrderBy = orderBy?.AsList();
            Hints = hints;
        }

        /// <summary>
        /// Gets the target fields.
        /// </summary>
        public IEnumerable<Field> Fields { get; set; }

        /// <summary>
        /// Gets the query expression used.
        /// </summary>
        public QueryGroup Where { get; }

        /// <summary>
        /// Gets the number of rows to skip.
        /// </summary>
        public int Skip { get; }

        /// <summary>
        /// Gets the number of rows per batch.
        /// </summary>
        public int RowsPerBatch { get; }

        /// <summary>
        /// Gets the list of the order fields.
        /// </summary>
        public IEnumerable<OrderField> OrderBy { get; }

        /// <summary>
        /// Gets the hints for the table.
        /// </summary>
        public string Hints { get; }

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="BatchQueryRequest"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            // Make sure to return if it is already provided
            if (this.hashCode != null)
            {
                return this.hashCode.Value;
            }

            // Get first the entity hash code
            var hashCode = HashCode.Combine(base.GetHashCode(), Name, ".BatchQuery");

            // Add the fields
            if (Fields != null)
            {
                foreach (var field in Fields)
                {
                    hashCode = HashCode.Combine(hashCode, field);
                }
            }

            // Add the expression
            if (Where != null)
            {
                hashCode = HashCode.Combine(hashCode, Where);
            }

            // Add the order fields
            if (OrderBy != null)
            {
                foreach (var orderField in OrderBy)
                {
                    hashCode = HashCode.Combine(hashCode, orderField);
                }
            }

            // Add the page
            hashCode = HashCode.Combine(hashCode, Skip);

            // Add the rows per batch
            hashCode = HashCode.Combine(hashCode, RowsPerBatch);

            // Add the hints
            if (!string.IsNullOrWhiteSpace(Hints))
            {
                hashCode = HashCode.Combine(hashCode, Hints);
            }

            // Set and return the hashcode
            return (this.hashCode = hashCode).Value;
        }

        #endregion
    }
}
