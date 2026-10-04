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
    /// A class that holds the definition of the primary key of a table.
    /// </summary>
    public class PrimaryKeyInfo : IEquatable<PrimaryKeyInfo>
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="PrimaryKeyInfo"/> class.
        /// </summary>
        /// <param name="name">The name of the constraint or index.</param>
        public PrimaryKeyInfo(string name)
        {
            Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the primary key constraint.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets or sets the ordered names of the columns of the primary key.
        /// </summary>
        public IList<string> Columns { get; internal set; } = new List<string>();

        /// <summary>
        /// Gets or sets a value that indicates whether the primary key is clustered. The default is <c>true</c>, which is the default of the databases that cluster their primary keys.
        /// </summary>
        public bool IsClustered { get; internal set; } = true;

        #endregion

        #region Overrides

        /// <summary>
        /// Gets the string that represents the instance of this <see cref="PrimaryKeyInfo"/> object.
        /// </summary>
        /// <returns>The name of this <see cref="PrimaryKeyInfo"/> object.</returns>
        public override string ToString() =>
            Name;

        #endregion

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="PrimaryKeyInfo"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Name);
            foreach (var item in Columns) hashCode.Add(item);
            hashCode.Add(IsClustered);
            return hashCode.ToHashCode();
        }

        /// <summary>
        /// Compares the <see cref="PrimaryKeyInfo"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public override bool Equals(object obj) =>
            Equals(obj as PrimaryKeyInfo);

        /// <summary>
        /// Compares the <see cref="PrimaryKeyInfo"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(PrimaryKeyInfo other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Equals(Name, other.Name)
                && Columns.SequenceEqual(other.Columns)
                && Equals(IsClustered, other.IsClustered);
        }

        /// <summary>
        /// Compares the equality of the two <see cref="PrimaryKeyInfo"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="PrimaryKeyInfo"/> object.</param>
        /// <param name="objB">The second <see cref="PrimaryKeyInfo"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(PrimaryKeyInfo objA,
            PrimaryKeyInfo objB)
        {
            if (objA is null)
            {
                return objB is null;
            }
            return objA.Equals(objB);
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="PrimaryKeyInfo"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="PrimaryKeyInfo"/> object.</param>
        /// <param name="objB">The second <see cref="PrimaryKeyInfo"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(PrimaryKeyInfo objA,
            PrimaryKeyInfo objB) => !(objA == objB);

        #endregion
    }
}
