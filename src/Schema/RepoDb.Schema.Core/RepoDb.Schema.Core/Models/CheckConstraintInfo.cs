#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;


namespace RepoDb.Schema.Models
{
    /// <summary>
    /// A class that holds the definition of a check constraint of a table.
    /// </summary>
    public class CheckConstraintInfo : IEquatable<CheckConstraintInfo>
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="CheckConstraintInfo"/> class.
        /// </summary>
        /// <param name="name">The name of the constraint or index.</param>
        public CheckConstraintInfo(string name)
        {
            Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the check constraint.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets or sets the expression of the check constraint.
        /// </summary>
        public string Expression { get; internal set; }

        #endregion

        #region Overrides

        /// <summary>
        /// Gets the string that represents the instance of this <see cref="CheckConstraintInfo"/> object.
        /// </summary>
        /// <returns>The name of this <see cref="CheckConstraintInfo"/> object.</returns>
        public override string ToString() =>
            Name;

        #endregion

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="CheckConstraintInfo"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Name);
            hashCode.Add(Expression);
            return hashCode.ToHashCode();
        }

        /// <summary>
        /// Compares the <see cref="CheckConstraintInfo"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public override bool Equals(object obj) =>
            Equals(obj as CheckConstraintInfo);

        /// <summary>
        /// Compares the <see cref="CheckConstraintInfo"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(CheckConstraintInfo other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Equals(Name, other.Name)
                && Equals(Expression, other.Expression);
        }

        /// <summary>
        /// Compares the equality of the two <see cref="CheckConstraintInfo"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="CheckConstraintInfo"/> object.</param>
        /// <param name="objB">The second <see cref="CheckConstraintInfo"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(CheckConstraintInfo objA,
            CheckConstraintInfo objB)
        {
            if (objA is null)
            {
                return objB is null;
            }
            return objA.Equals(objB);
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="CheckConstraintInfo"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="CheckConstraintInfo"/> object.</param>
        /// <param name="objB">The second <see cref="CheckConstraintInfo"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(CheckConstraintInfo objA,
            CheckConstraintInfo objB) => !(objA == objB);

        #endregion
    }
}
