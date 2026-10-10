#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// A class that holds the relationships of a table with the other tables, as defined by the foreign keys.
    /// The relationships of all the tables form a graph: the same <see cref="RelationshipInfo"/> instance is shared by the
    /// <see cref="Parents"/> and the <see cref="Children"/> of the other tables, and the graph can be circular
    /// (i.e.: 2 tables that reference each other), so do not serialize it as it is.
    /// </summary>
    public class RelationshipInfo : IEquatable<RelationshipInfo>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the schema of the table.
        /// </summary>
        public TableSchema Schema { get; internal set; }

        /// <summary>
        /// Gets or sets the tables that this table references through its foreign keys (the tables that this table depends on,
        /// so they must exist first). A table that references itself is not included.
        /// </summary>
        public IList<RelationshipInfo> Parents { get; internal set; } = new List<RelationshipInfo>();

        /// <summary>
        /// Gets or sets the tables that reference this table through their foreign keys (the tables that depend on this table).
        /// A table that references itself is not included.
        /// </summary>
        public IList<RelationshipInfo> Children { get; internal set; } = new List<RelationshipInfo>();

        #endregion

        #region Overrides

        /// <summary>
        /// Gets the string that represents the instance of this <see cref="RelationshipInfo"/> object.
        /// </summary>
        /// <returns>The name of the table of the schema of this <see cref="RelationshipInfo"/> object (<c>null</c> if it has no schema).</returns>
        public override string ToString() =>
            Schema?.ToString();

        #endregion

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="RelationshipInfo"/>.
        /// </summary>
        /// <remarks>
        /// Only the <see cref="Schema"/> is taken into account. The <see cref="Parents"/> and the <see cref="Children"/> form a graph
        /// that can be circular, so comparing them by value would never end.
        /// </remarks>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Schema);
            return hashCode.ToHashCode();
        }

        /// <summary>
        /// Compares the <see cref="RelationshipInfo"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public override bool Equals(object obj) =>
            Equals(obj as RelationshipInfo);

        /// <summary>
        /// Compares the <see cref="RelationshipInfo"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(RelationshipInfo other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Equals(Schema, other.Schema);
        }

        /// <summary>
        /// Compares the equality of the two <see cref="RelationshipInfo"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="RelationshipInfo"/> object.</param>
        /// <param name="objB">The second <see cref="RelationshipInfo"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(RelationshipInfo objA,
            RelationshipInfo objB)
        {
            if (objA is null)
            {
                return objB is null;
            }
            return objA.Equals(objB);
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="RelationshipInfo"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="RelationshipInfo"/> object.</param>
        /// <param name="objB">The second <see cref="RelationshipInfo"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(RelationshipInfo objA,
            RelationshipInfo objB) => !(objA == objB);

        #endregion
    }
}
