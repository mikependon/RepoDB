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
    /// A class that holds the value of the 'Update' operation arguments.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of <see cref="UpdateRequest"/> object.
    /// </remarks>
    /// <param name="name">The name of the request.</param>
    /// <param name="connection">The connection object.</param>
    /// <param name="transaction">The transaction object.</param>
    /// <param name="where">The query expression.</param>
    /// <param name="fields">The list of the target fields.</param>
    /// <param name="hints">The hints for the table.</param>
    /// <param name="statementBuilder">The statement builder.</param>
    internal class UpdateRequest(string name,
        IDbConnection connection,
        IDbTransaction transaction,
        QueryGroup where = null,
        IEnumerable<Field> fields = null,
        string hints = null,
        IStatementBuilder statementBuilder = null) : BaseRequest(name,
              connection,
              transaction,
              statementBuilder)
    {
        private int? hashCode = null;

        /// <summary>
        /// Creates a new instance of <see cref="UpdateRequest"/> object.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="transaction">The transaction object.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="fields">The list of the target fields.</param>
        /// <param name="hints">The hints for the table.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public UpdateRequest(Type type,
            IDbConnection connection,
            IDbTransaction transaction,
            QueryGroup where = null,
            IEnumerable<Field> fields = null,
            string hints = null,
            IStatementBuilder statementBuilder = null)
            : this(ClassMappedNameCache.Get(type),
                connection,
                transaction,
                where,
                fields,
                hints,
                statementBuilder)
        {
            Type = type;
        }

        /// <summary>
        /// Gets the query expression used.
        /// </summary>
        public QueryGroup Where { get; } = where;

        /// <summary>
        /// Gets the target fields.
        /// </summary>
        public IEnumerable<Field> Fields { get; set; } = fields?.AsList();

        /// <summary>
        /// Gets the hints for the table.
        /// </summary>
        public string Hints { get; } = hints;

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="UpdateRequest"/>.
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
            var computedHashCode = HashCode.Combine(base.GetHashCode(), Name, ".Update");

            // Get the properties hash codes
            if (Where != null)
            {
                computedHashCode = HashCode.Combine(computedHashCode, Where);
            }

            // Get the qualifier <see cref="Field"/> objects
            if (Fields != null)
            {
                foreach (var field in Fields)
                {
                    computedHashCode = HashCode.Combine(computedHashCode, field);
                }
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
