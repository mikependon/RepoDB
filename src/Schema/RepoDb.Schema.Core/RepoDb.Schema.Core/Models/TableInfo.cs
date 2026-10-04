#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// A class that holds the identity of a table: its name and the name of the schema that owns it.
    /// </summary>
    public class TableInfo : IEquatable<TableInfo>
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="TableInfo"/> class.
        /// </summary>
        /// <param name="name">The name of the table.</param>
        /// <param name="schema">The name of the schema that owns the table.</param>
        public TableInfo(string name,
            string schema)
        {
            Name = name;
            Schema = schema;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets or sets the name of the schema that owns the table.
        /// </summary>
        public string Schema { get; internal set; }

        #endregion

        #region Overrides

        /// <summary>
        /// Gets the string that represents the instance of this <see cref="TableInfo"/> object.
        /// </summary>
        /// <returns>The name of this <see cref="TableInfo"/> object.</returns>
        public override string ToString() =>
            Name;

        #endregion

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="TableInfo"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Name);
            hashCode.Add(Schema);
            return hashCode.ToHashCode();
        }

        /// <summary>
        /// Compares the <see cref="TableInfo"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public override bool Equals(object obj) =>
            Equals(obj as TableInfo);

        /// <summary>
        /// Compares the <see cref="TableInfo"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(TableInfo other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Equals(Name, other.Name)
                && Equals(Schema, other.Schema);
        }

        /// <summary>
        /// Compares the equality of the two <see cref="TableInfo"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="TableInfo"/> object.</param>
        /// <param name="objB">The second <see cref="TableInfo"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(TableInfo objA,
            TableInfo objB)
        {
            if (objA is null)
            {
                return objB is null;
            }
            return objA.Equals(objB);
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="TableInfo"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="TableInfo"/> object.</param>
        /// <param name="objB">The second <see cref="TableInfo"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(TableInfo objA,
            TableInfo objB) => !(objA == objB);

        #endregion
    }
}
