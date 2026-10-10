#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// A class that holds the full definition of a column of a table.
    /// </summary>
    public class ColumnInfo : IEquatable<ColumnInfo>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the base field information (name, type, size, precision, scale, nullability, primary and identity flags).
        /// </summary>
        public DbField Field { get; internal set; }

        /// <summary>
        /// Gets or sets the position of the column within the table (starting at 1).
        /// </summary>
        public int Ordinal { get; internal set; }

        /// <summary>
        /// Gets or sets the default expression of the column, or <c>null</c> if there is none.
        /// </summary>
        public string DefaultExpression { get; internal set; }

        /// <summary>
        /// Gets or sets the seed of the identity column, or <c>null</c> if the column is not an identity.
        /// </summary>
        public long? IdentitySeed { get; internal set; }

        /// <summary>
        /// Gets or sets the increment of the identity column, or <c>null</c> if the column is not an identity.
        /// </summary>
        public long? IdentityIncrement { get; internal set; }

        /// <summary>
        /// Gets or sets the expression of a computed or generated column, or <c>null</c> if the column is not computed.
        /// </summary>
        public string ComputedExpression { get; internal set; }

        /// <summary>
        /// Gets or sets the collation of the column, or <c>null</c> if it is not applicable.
        /// </summary>
        public string Collation { get; internal set; }

        /// <summary>
        /// Gets or sets the comment of the column, or <c>null</c> if there is none.
        /// </summary>
        public string Comment { get; internal set; }

        #endregion

        #region Overrides

        /// <summary>
        /// Gets the string that represents the instance of this <see cref="ColumnInfo"/> object.
        /// </summary>
        /// <returns>The name of the field of this <see cref="ColumnInfo"/> object (<c>null</c> if it has no field).</returns>
        public override string ToString() =>
            Field?.Name;

        #endregion

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="ColumnInfo"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Field);
            hashCode.Add(Ordinal);
            hashCode.Add(DefaultExpression);
            hashCode.Add(IdentitySeed);
            hashCode.Add(IdentityIncrement);
            hashCode.Add(ComputedExpression);
            hashCode.Add(Collation);
            hashCode.Add(Comment);
            return hashCode.ToHashCode();
        }

        /// <summary>
        /// Compares the <see cref="ColumnInfo"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public override bool Equals(object obj) =>
            Equals(obj as ColumnInfo);

        /// <summary>
        /// Compares the <see cref="ColumnInfo"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(ColumnInfo other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Equals(Field, other.Field)
                && Equals(Ordinal, other.Ordinal)
                && Equals(DefaultExpression, other.DefaultExpression)
                && Equals(IdentitySeed, other.IdentitySeed)
                && Equals(IdentityIncrement, other.IdentityIncrement)
                && Equals(ComputedExpression, other.ComputedExpression)
                && Equals(Collation, other.Collation)
                && Equals(Comment, other.Comment);
        }

        /// <summary>
        /// Compares the equality of the two <see cref="ColumnInfo"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="ColumnInfo"/> object.</param>
        /// <param name="objB">The second <see cref="ColumnInfo"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(ColumnInfo objA,
            ColumnInfo objB)
        {
            if (objA is null)
            {
                return objB is null;
            }
            return objA.Equals(objB);
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="ColumnInfo"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="ColumnInfo"/> object.</param>
        /// <param name="objB">The second <see cref="ColumnInfo"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(ColumnInfo objA,
            ColumnInfo objB) => !(objA == objB);

        #endregion
    }
}
