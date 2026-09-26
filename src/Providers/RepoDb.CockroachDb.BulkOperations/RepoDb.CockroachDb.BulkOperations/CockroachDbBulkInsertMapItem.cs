#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using RepoDb.Connector.CockroachDb;

namespace RepoDb.CockroachDb.BulkOperations
{
    /// <summary>
    /// A class that is being used to define a mapping for the bulk operations against CockroachDB.
    /// </summary>
    public class CockroachDbBulkInsertMapItem : BulkInsertMapItem
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="CockroachDbBulkInsertMapItem"/> object.
        /// </summary>
        /// <param name="sourceColumn">The name of the source column or property. This respects the mapping of the properties if the source type is an entity model.</param>
        /// <param name="destinationColumn">The name of the destination column in the database.</param>
        public CockroachDbBulkInsertMapItem(string sourceColumn,
            string destinationColumn) :
            this(sourceColumn, destinationColumn, null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="CockroachDbBulkInsertMapItem"/> object.
        /// </summary>
        /// <param name="sourceColumn">The name of the source column or property. This respects the mapping of the properties if the source type is an entity model.</param>
        /// <param name="destinationColumn">The name of the destination column in the database.</param>
        /// <param name="cockroachDbType">
        /// The explicit <see cref="RepoDb.Connector.CockroachDb.CockroachDbType"/> value to bind with for this column. When not
        /// provided, the type is inferred from the entity property's <c>[CockroachDbType]</c> attribute
        /// (if present) or, failing that, from the .NET CLR value itself.
        /// </param>
        public CockroachDbBulkInsertMapItem(string sourceColumn,
            string destinationColumn,
            CockroachDbType? cockroachDbType) :
            base(sourceColumn, destinationColumn)
        {
            CockroachDbType = cockroachDbType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the explicit <see cref="RepoDb.Connector.CockroachDb.CockroachDbType"/> value to be used when writing.
        /// </summary>
        public CockroachDbType? CockroachDbType { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the string representation of the current object.
        /// </summary>
        /// <returns>The string representation of the current object.</returns>
        public override string ToString() =>
            $"{base.ToString()} ({CockroachDbType})";

        #endregion

        #region Equality and comparers

        private int? hashCode = null;

        /// <summary>
        /// Returns the hashcode of the current instance.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            if (hashCode != null)
            {
                return hashCode.Value;
            }

            hashCode = HashCode.Combine(hashCode, base.GetHashCode(), CockroachDbType);
            return hashCode.Value;
        }

        #endregion
    }
}
