#region Copyright Attributions

// Copyright (c) 2018 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

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
    /// <remarks>
    /// Creates a new instance of <see cref="BatchQueryRequest"/> object.
    /// </remarks>
    /// <param name="name">The name of the request.</param>
    /// <param name="connection">The connection object.</param>
    /// <param name="transaction">The transaction object.</param>
    /// <param name="fields">The list of the target fields.</param>
    /// <param name="page">The page of the batch.</param>
    /// <param name="rowsPerBatch">The number of rows per batch.</param>
    /// <param name="orderBy">The list of order fields.</param>
    /// <param name="where">The query expression.</param>
    /// <param name="hints">The hints for the table.</param>
    /// <param name="statementBuilder">The statement builder.</param>
    internal class BatchQueryRequest(string name,
        IDbConnection connection,
        IDbTransaction transaction,
        IEnumerable<Field> fields,
        int page,
        int rowsPerBatch,
        IEnumerable<OrderField> orderBy,
        QueryGroup where = null,
        string hints = null,
        IStatementBuilder statementBuilder = null) : BaseRequest(name,
              connection,
              transaction,
              statementBuilder)
    {
        private int? hashCode = null;

        /// <summary>
        /// Creates a new instance of <see cref="BatchQueryRequest"/> object.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="transaction">The transaction object.</param>
        /// <param name="fields">The list of the target fields.</param>
        /// <param name="page">The page of the batch.</param>
        /// <param name="rowsPerBatch">The number of rows per batch.</param>
        /// <param name="orderBy">The list of order fields.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The hints for the table.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public BatchQueryRequest(Type type,
            IDbConnection connection,
            IDbTransaction transaction,
            IEnumerable<Field> fields,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup where = null,
            string hints = null,
            IStatementBuilder statementBuilder = null)
            : this(ClassMappedNameCache.Get(type),
                  connection,
                  transaction,
                  fields,
                  page,
                  rowsPerBatch,
                  orderBy,
                  where,
                  hints,
                  statementBuilder)
        {
            Type = type;
        }

        /// <summary>
        /// Gets the target fields.
        /// </summary>
        public IEnumerable<Field> Fields { get; set; } = fields?.AsList();

        /// <summary>
        /// Gets the query expression used.
        /// </summary>
        public QueryGroup Where { get; } = where;

        /// <summary>
        /// Gets the filter for the rows.
        /// </summary>
        public int Page { get; } = page;

        /// <summary>
        /// Gets the number of rows per batch.
        /// </summary>
        public int RowsPerBatch { get; } = rowsPerBatch;

        /// <summary>
        /// Gets the list of the order fields.
        /// </summary>
        public IEnumerable<OrderField> OrderBy { get; } = orderBy?.AsList();

        /// <summary>
        /// Gets the hints for the table.
        /// </summary>
        public string Hints { get; } = hints;

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
            var computedHashCode = HashCode.Combine(base.GetHashCode(), Name, ".BatchQuery");

            // Add the fields
            if (Fields != null)
            {
                foreach (var field in Fields)
                {
                    computedHashCode = HashCode.Combine(computedHashCode, field);
                }
            }

            // Add the expression
            if (Where != null)
            {
                computedHashCode = HashCode.Combine(computedHashCode, Where);
            }

            // Add the order fields
            if (OrderBy != null)
            {
                foreach (var orderField in OrderBy)
                {
                    computedHashCode = HashCode.Combine(computedHashCode, orderField);
                }
            }

            // Add the page
            computedHashCode = HashCode.Combine(computedHashCode, Page);

            // Add the rows per batch
            computedHashCode = HashCode.Combine(computedHashCode, RowsPerBatch);

            // Add the hints
            if (!string.IsNullOrWhiteSpace(Hints))
            {
                computedHashCode = HashCode.Combine(computedHashCode, Hints);
            }

            // Set and return the hashcode
            return (this.hashCode = computedHashCode).Value;
        }

        #endregion
    }
}
