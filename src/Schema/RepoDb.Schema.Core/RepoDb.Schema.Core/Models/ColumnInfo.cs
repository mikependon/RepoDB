#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// A class that holds the full definition of a column of a table.
    /// </summary>
    public class ColumnInfo : IEquatable<ColumnInfo>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the character set reported by the provider, or null when unknown.
        /// </summary>
        public string CharacterSet { get; set; }

        /// <summary>
        /// Gets or sets the collation of the column, or <c>null</c> if it is not applicable.
        /// </summary>
        public string Collation { get; set; }

        /// <summary>
        /// Gets or sets the comment of the column, or null if there is none.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the expression of a computed or generated column, or <c>null</c> if the column is not computed.
        /// </summary>
        public string ComputedExpression { get; set; }

        /// <summary>
        /// Gets or sets the default expression of the column, or <c>null</c> if there is none.
        /// </summary>
        public string DefaultExpression { get; set; }

        /// <summary>
        /// Gets or sets the kind of the default value of the column (see <see cref="ColumnDefaultValue"/>), which the composer of another database uses to create the equivalent
        /// default value. It is <see cref="ColumnDefaultValue.Unclassified"/> if the schema reader does not set it.
        /// </summary>
        public ColumnDefaultValue DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the base field information (name, type, size, precision, scale, nullability, primary and identity flags).
        /// </summary>
        public DbField Field { get; set; }

        /// <summary>
        /// Gets or sets the increment of the identity column, or <c>null</c> if the column is not an identity.
        /// </summary>
        public long? IdentityIncrement { get; set; }

        /// <summary>
        /// Gets or sets the seed of the identity column, or <c>null</c> if the column is not an identity.
        /// </summary>
        public long? IdentitySeed { get; set; }

        /// <summary>
        /// Gets or sets whether the text encoding supports Unicode, or null when unknown.
        /// </summary>
        public bool? IsUnicode { get; set; }

        /// <summary>
        /// Gets or sets the unit of Field.Size (bytes, characters, or utf16CodeUnits), or null when unknown.
        /// </summary>
        public string LengthUnit { get; set; }

        /// <summary>
        /// Gets or sets the full provider type declaration when available.
        /// </summary>
        public string NativeType { get; set; }

        /// <summary>
        /// Gets or sets the numeric precision without DbField's byte limit, or null when unknown.
        /// </summary>
        public int? NumericPrecision { get; set; }

        /// <summary>
        /// Gets or sets the numeric scale, including negative scales, or null when unknown.
        /// </summary>
        public int? NumericScale { get; set; }

        /// <summary>
        /// Gets or sets the position of the column within the table (starting at 1).
        /// </summary>
        public int Ordinal { get; set; }

        /// <summary>
        /// Gets or sets the fractional-second precision, or null when unknown.
        /// </summary>
        public int? TemporalPrecision { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the string that represents the instance of this <see cref="ColumnInfo"/> object.
        /// </summary>
        /// <returns>The name of the field of this <see cref="ColumnInfo"/> object (<c>null</c> if it has no field).</returns>
        public override string ToString() =>
            Field?.Name;

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
            hashCode.Add(DefaultValue);
            hashCode.Add(IdentitySeed);
            hashCode.Add(IdentityIncrement);
            hashCode.Add(ComputedExpression);
            hashCode.Add(Collation);
            hashCode.Add(NativeType);
            hashCode.Add(NumericPrecision);
            hashCode.Add(NumericScale);
            hashCode.Add(Comment);
            hashCode.Add(CharacterSet);
            hashCode.Add(IsUnicode);
            hashCode.Add(LengthUnit);
            hashCode.Add(TemporalPrecision);
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
            if (other is null)
                return false;
            if (ReferenceEquals(this,
                other))
                return true;
            return Equals(Field,
                other.Field) && Equals(Ordinal,
                    other.Ordinal) && Equals(DefaultExpression,
                    other.DefaultExpression) && Equals(DefaultValue,
                    other.DefaultValue) && Equals(IdentitySeed,
                    other.IdentitySeed) && Equals(IdentityIncrement,
                    other.IdentityIncrement) && Equals(ComputedExpression,
                    other.ComputedExpression) && Equals(Collation,
                    other.Collation) && Equals(NativeType,
                    other.NativeType) && Equals(NumericPrecision,
                    other.NumericPrecision) && Equals(NumericScale,
                    other.NumericScale) && Equals(Comment,
                    other.Comment) && Equals(CharacterSet,
                    other.CharacterSet) && Equals(IsUnicode,
                    other.IsUnicode) && Equals(LengthUnit,
                    other.LengthUnit) && Equals(TemporalPrecision,
                    other.TemporalPrecision);
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
            ColumnInfo objB) =>
            !(objA == objB);

        #endregion
    }
}
