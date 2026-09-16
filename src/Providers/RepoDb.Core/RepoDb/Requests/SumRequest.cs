#region Copyright Attributions

// Copyright (c) 2018 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb.Requests
{
    /// <summary>
    /// A class that holds the value of the 'Sum' operation arguments.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of <see cref="SumRequest"/> object.
    /// </remarks>
    /// <param name="name">The name of the request.</param>
    /// <param name="connection">The connection object.</param>
    /// <param name="transaction">The transaction object.</param>
    /// <param name="field">The field object.</param>
    /// <param name="where">The query expression.</param>
    /// <param name="hints">The hints for the table.</param>
    /// <param name="statementBuilder">The statement builder.</param>
    internal class SumRequest(string name,
        IDbConnection connection,
        IDbTransaction transaction,
        Field field = null,
        QueryGroup where = null,
        string hints = null,
        IStatementBuilder statementBuilder = null) : BaseRequest(name,
              connection,
              transaction,
              statementBuilder)
    {
        private int? hashCode = null;

        /// <summary>
        /// Creates a new instance of <see cref="SumRequest"/> object.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="transaction">The transaction object.</param>
        /// <param name="field">The field object.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The hints for the table.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public SumRequest(Type type,
            IDbConnection connection,
            IDbTransaction transaction,
            Field field = null,
            QueryGroup where = null,
            string hints = null,
            IStatementBuilder statementBuilder = null)
            : this(ClassMappedNameCache.Get(type),
                  connection,
                  transaction,
                  field,
                  where,
                  hints,
                  statementBuilder)
        {
            Type = type;
        }

        /// <summary>
        /// Gets the field to be summarized.
        /// </summary>
        public Field Field { get; } = field;

        /// <summary>
        /// Gets the query expression used.
        /// </summary>
        public QueryGroup Where { get; } = where;

        /// <summary>
        /// Gets the hints for the table.
        /// </summary>
        public string Hints { get; } = hints;

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="SumRequest"/>.
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
            var computedHashCode = HashCode.Combine(base.GetHashCode(), Name, ".Sum");

            // Add the field
            if (Field != null)
            {
                computedHashCode = HashCode.Combine(computedHashCode, Field);
            }

            // Add the where
            if (Where != null)
            {
                computedHashCode = HashCode.Combine(computedHashCode, Where);
            }

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
