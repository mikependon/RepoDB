#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// A class that holds the whole schema of a table. It is the hand-off between an <see cref="ISchemaReader"/> and an <see cref="ISchemaComposer"/>.
    /// </summary>
    public class TableSchema : IEquatable<TableSchema>
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="TableSchema"/> class.
        /// </summary>
        /// <param name="name">The name of the table.</param>
        /// <param name="schema">The name of the schema that owns the table.</param>
        public TableSchema(string name,
            string schema)
        {
            Table = new TableInfo(name, schema);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the check constraints of the table.
        /// </summary>
        public IList<CheckConstraintInfo> CheckConstraints { get; internal set; } = new List<CheckConstraintInfo>();

        /// <summary>
        /// Gets or sets the columns of the table, in their ordinal order.
        /// </summary>
        public IList<ColumnInfo> Columns { get; internal set; } = new List<ColumnInfo>();

        /// <summary>
        /// Gets or sets the foreign keys of the table.
        /// </summary>
        public IList<ForeignKeyInfo> ForeignKeys { get; internal set; } = new List<ForeignKeyInfo>();

        /// <summary>
        /// Gets or sets the indexes of the table.
        /// </summary>
        public IList<IndexInfo> Indexes { get; internal set; } = new List<IndexInfo>();

        /// <summary>
        /// Gets or sets the primary key of the table, or <c>null</c> if the table has no primary key.
        /// </summary>
        public PrimaryKeyInfo PrimaryKey { get; internal set; }

        /// <summary>
        /// Gets or sets the identity (name and schema) of the table.
        /// </summary>
        public TableInfo Table { get; internal set; }

        /// <summary>
        /// Gets or sets the unique constraints of the table.
        /// </summary>
        public IList<UniqueConstraintInfo> UniqueConstraints { get; internal set; } = new List<UniqueConstraintInfo>();

        #endregion

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="TableSchema"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            foreach (var item in CheckConstraints) hashCode.Add(item);
            foreach (var item in Columns) hashCode.Add(item);
            foreach (var item in ForeignKeys) hashCode.Add(item);
            foreach (var item in Indexes) hashCode.Add(item);
            hashCode.Add(PrimaryKey);
            hashCode.Add(Table);
            foreach (var item in UniqueConstraints) hashCode.Add(item);
            return hashCode.ToHashCode();
        }

        /// <summary>
        /// Compares the <see cref="TableSchema"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public override bool Equals(object obj) =>
            Equals(obj as TableSchema);

        /// <summary>
        /// Compares the <see cref="TableSchema"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(TableSchema other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return CheckConstraints.SequenceEqual(other.CheckConstraints)
                && Columns.SequenceEqual(other.Columns)
                && ForeignKeys.SequenceEqual(other.ForeignKeys)
                && Indexes.SequenceEqual(other.Indexes)
                && Equals(PrimaryKey, other.PrimaryKey)
                && Equals(Table, other.Table)
                && UniqueConstraints.SequenceEqual(other.UniqueConstraints);
        }

        /// <summary>
        /// Compares the equality of the two <see cref="TableSchema"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="TableSchema"/> object.</param>
        /// <param name="objB">The second <see cref="TableSchema"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(TableSchema objA,
            TableSchema objB)
        {
            if (objA is null)
            {
                return objB is null;
            }
            return objA.Equals(objB);
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="TableSchema"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="TableSchema"/> object.</param>
        /// <param name="objB">The second <see cref="TableSchema"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(TableSchema objA,
            TableSchema objB) => !(objA == objB);

        #endregion
    }
}
