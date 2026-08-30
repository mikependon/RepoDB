#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using FirebirdSql.Data.FirebirdClient;

namespace RepoDb.Firebird.BulkOperations
{
    /// <summary>
    /// A class that is being used to define a mapping for the bulk operations against Firebird.
    /// </summary>
    public class FirebirdCommandBatcherMapItem : BulkInsertMapItem
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="FirebirdCommandBatcherMapItem"/> object.
        /// </summary>
        /// <param name="sourceColumn">The name of the source column or property. This respects the mapping of the properties if the source type is an entity model.</param>
        /// <param name="destinationColumn">The name of the destination column in the database.</param>
        public FirebirdCommandBatcherMapItem(string sourceColumn,
            string destinationColumn) :
            this(sourceColumn, destinationColumn, null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="FirebirdCommandBatcherMapItem"/> object.
        /// </summary>
        /// <param name="sourceColumn">The name of the source column or property. This respects the mapping of the properties if the source type is an entity model.</param>
        /// <param name="destinationColumn">The name of the destination column in the database.</param>
        /// <param name="fbDbType">
        /// The explicit <see cref="FirebirdSql.Data.FirebirdClient.FbDbType"/> value to bind with for this column. Rarely
        /// needed - Firebird's DSQL layer determines a bind parameter's wire format from the destination
        /// column's actual server-side type, not the client-declared type, so this is only an escape hatch
        /// for genuinely ambiguous cases (e.g. forcing a specific <c>BLOB</c> sub-type).
        /// </param>
        public FirebirdCommandBatcherMapItem(string sourceColumn,
            string destinationColumn,
            FbDbType? fbDbType) :
            base(sourceColumn, destinationColumn)
        {
            FbDbType = fbDbType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the explicit <see cref="FirebirdSql.Data.FirebirdClient.FbDbType"/> value to be used when writing.
        /// </summary>
        public FbDbType? FbDbType { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the string representation of the current object.
        /// </summary>
        /// <returns>The string representation of the current object.</returns>
        public override string ToString() =>
            $"{base.ToString()} ({FbDbType})";

        #endregion

        #region Equality and comparers

        private int? hashCode = null;

        /// <summary>
        /// Returns the hashcode of the current instance.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            if (this.hashCode != null)
            {
                return this.hashCode.Value;
            }

            var hashCode = base.GetHashCode();
            hashCode = HashCode.Combine(hashCode, FbDbType);

            return (this.hashCode = hashCode).Value;
        }

        #endregion
    }
}
