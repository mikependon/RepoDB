#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// A class that holds the definition of a foreign key of a table.
    /// </summary>
    public class ForeignKeyInfo : IEquatable<ForeignKeyInfo>
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="ForeignKeyInfo"/> class.
        /// </summary>
        /// <param name="name">The name of the constraint or index.</param>
        public ForeignKeyInfo(string name)
        {
            Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the foreign key constraint.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets or sets the ordered names of the columns of the foreign key.
        /// </summary>
        public IList<string> Columns { get; internal set; } = new List<string>();

        /// <summary>
        /// Gets or sets the identity (name and schema) of the table that is referenced by the foreign key.
        /// </summary>
        public TableInfo ReferencedTable { get; internal set; }

        /// <summary>
        /// Gets or sets the ordered names of the referenced columns.
        /// </summary>
        public IList<string> ReferencedColumns { get; internal set; } = new List<string>();

        /// <summary>
        /// Gets or sets the rule that is applied when the referenced row is updated.
        /// </summary>
        public CopySchemaForeignKeyRule UpdateRule { get; internal set; } = CopySchemaForeignKeyRule.NoAction;

        /// <summary>
        /// Gets or sets the rule that is applied when the referenced row is deleted.
        /// </summary>
        public CopySchemaForeignKeyRule DeleteRule { get; internal set; } = CopySchemaForeignKeyRule.NoAction;

        #endregion

        #region Overrides

        /// <summary>
        /// Gets the string that represents the instance of this <see cref="ForeignKeyInfo"/> object.
        /// </summary>
        /// <returns>The name of this <see cref="ForeignKeyInfo"/> object.</returns>
        public override string ToString() =>
            Name;

        #endregion

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="ForeignKeyInfo"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Name);
            foreach (var item in Columns) hashCode.Add(item);
            hashCode.Add(ReferencedTable);
            foreach (var item in ReferencedColumns) hashCode.Add(item);
            hashCode.Add(UpdateRule);
            hashCode.Add(DeleteRule);
            return hashCode.ToHashCode();
        }

        /// <summary>
        /// Compares the <see cref="ForeignKeyInfo"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public override bool Equals(object obj) =>
            Equals(obj as ForeignKeyInfo);

        /// <summary>
        /// Compares the <see cref="ForeignKeyInfo"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(ForeignKeyInfo other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Equals(Name, other.Name)
                && Columns.SequenceEqual(other.Columns)
                && Equals(ReferencedTable, other.ReferencedTable)
                && ReferencedColumns.SequenceEqual(other.ReferencedColumns)
                && Equals(UpdateRule, other.UpdateRule)
                && Equals(DeleteRule, other.DeleteRule);
        }

        /// <summary>
        /// Compares the equality of the two <see cref="ForeignKeyInfo"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="ForeignKeyInfo"/> object.</param>
        /// <param name="objB">The second <see cref="ForeignKeyInfo"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(ForeignKeyInfo objA,
            ForeignKeyInfo objB)
        {
            if (objA is null)
            {
                return objB is null;
            }
            return objA.Equals(objB);
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="ForeignKeyInfo"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="ForeignKeyInfo"/> object.</param>
        /// <param name="objB">The second <see cref="ForeignKeyInfo"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(ForeignKeyInfo objA,
            ForeignKeyInfo objB) => !(objA == objB);

        #endregion
    }
}
