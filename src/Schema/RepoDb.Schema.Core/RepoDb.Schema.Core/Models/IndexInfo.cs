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
    /// A class that holds the definition of an index of a table.
    /// </summary>
    public class IndexInfo : IEquatable<IndexInfo>
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="IndexInfo"/> class.
        /// </summary>
        /// <param name="name">The name of the constraint or index.</param>
        public IndexInfo(string name)
        {
            Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the index.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the index is unique.
        /// </summary>
        public bool IsUnique { get; internal set; }

        /// <summary>
        /// Gets or sets the ordered names of the key columns of the index.
        /// </summary>
        public IList<string> Columns { get; internal set; } = new List<string>();

        /// <summary>
        /// Gets or sets the names of the non-key (included) columns of the index.
        /// </summary>
        public IList<string> IncludedColumns { get; internal set; } = new List<string>();

        /// <summary>
        /// Gets or sets the names of the key columns that are sorted in descending order. The other key columns are sorted in ascending order.
        /// </summary>
        public IList<string> DescendingColumns { get; internal set; } = new List<string>();

        /// <summary>
        /// Gets or sets a value that indicates whether the index is clustered.
        /// </summary>
        public bool IsClustered { get; internal set; }

        /// <summary>
        /// Gets or sets the filter expression of the index, or <c>null</c> if the index is not filtered.
        /// </summary>
        public string Filter { get; internal set; }

        #endregion

        #region Overrides

        /// <summary>
        /// Gets the string that represents the instance of this <see cref="IndexInfo"/> object.
        /// </summary>
        /// <returns>The name of this <see cref="IndexInfo"/> object.</returns>
        public override string ToString() =>
            Name;

        #endregion

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="IndexInfo"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Name);
            hashCode.Add(IsUnique);
            foreach (var item in Columns) hashCode.Add(item);
            foreach (var item in IncludedColumns) hashCode.Add(item);
            foreach (var item in DescendingColumns) hashCode.Add(item);
            hashCode.Add(IsClustered);
            hashCode.Add(Filter);
            return hashCode.ToHashCode();
        }

        /// <summary>
        /// Compares the <see cref="IndexInfo"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public override bool Equals(object obj) =>
            Equals(obj as IndexInfo);

        /// <summary>
        /// Compares the <see cref="IndexInfo"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(IndexInfo other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Equals(Name, other.Name)
                && Equals(IsUnique, other.IsUnique)
                && Columns.SequenceEqual(other.Columns)
                && IncludedColumns.SequenceEqual(other.IncludedColumns)
                && DescendingColumns.SequenceEqual(other.DescendingColumns)
                && Equals(IsClustered, other.IsClustered)
                && Equals(Filter, other.Filter);
        }

        /// <summary>
        /// Compares the equality of the two <see cref="IndexInfo"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="IndexInfo"/> object.</param>
        /// <param name="objB">The second <see cref="IndexInfo"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(IndexInfo objA,
            IndexInfo objB)
        {
            if (objA is null)
            {
                return objB is null;
            }
            return objA.Equals(objB);
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="IndexInfo"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="IndexInfo"/> object.</param>
        /// <param name="objB">The second <see cref="IndexInfo"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(IndexInfo objA,
            IndexInfo objB) => !(objA == objB);

        #endregion
    }
}
