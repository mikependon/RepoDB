#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Sap.Data.Hana;

namespace RepoDb.SapHana.BulkOperations
{
    /// <summary>
    /// A class that is being used to define a mapping for the bulk operations against SapHana.
    /// </summary>
    public class SapHanaBulkInsertMapItem : BulkInsertMapItem
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="SapHanaBulkInsertMapItem"/> object.
        /// </summary>
        /// <param name="sourceColumn">The name of the source column or property. This respects the mapping of the properties if the source type is an entity model.</param>
        /// <param name="destinationColumn">The name of the destination column in the database.</param>
        public SapHanaBulkInsertMapItem(string sourceColumn,
            string destinationColumn) :
            this(sourceColumn, destinationColumn, null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="SapHanaBulkInsertMapItem"/> object.
        /// </summary>
        /// <param name="sourceColumn">The name of the source column or property. This respects the mapping of the properties if the source type is an entity model.</param>
        /// <param name="destinationColumn">The name of the destination column in the database.</param>
        /// <param name="hanaDbType">
        /// The explicit <see cref="SapHana.HanaDbType"/> value to bind with for this column. When not
        /// provided, the type is inferred from the entity property's <c>[HanaDbType]</c>/<c>[SapHanaDbTypeEx]</c> attribute
        /// (if present) or, failing that, from the .NET CLR value itself.
        /// </param>
        public SapHanaBulkInsertMapItem(string sourceColumn,
            string destinationColumn,
            HanaDbType? hanaDbType) :
            base(sourceColumn, destinationColumn)
        {
            HanaDbType = hanaDbType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the explicit <see cref="SapHana.ManagedDataAccess.Client.HanaDbType"/> value to be used when writing.
        /// </summary>
        public HanaDbType? HanaDbType { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the string representation of the current object.
        /// </summary>
        /// <returns>The string representation of the current object.</returns>
        public override string ToString() =>
            $"{base.ToString()} ({HanaDbType})";

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
            hashCode = HashCode.Combine(hashCode, HanaDbType);

            return (this.hashCode = hashCode).Value;
        }

        #endregion
    }
}
