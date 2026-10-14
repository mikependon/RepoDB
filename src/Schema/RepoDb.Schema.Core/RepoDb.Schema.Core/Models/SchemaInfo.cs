#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// A class that holds the identity of a schema of a database: its name.
    /// </summary>
    public class SchemaInfo : IEquatable<SchemaInfo>
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="SchemaInfo"/> class.
        /// </summary>
        /// <param name="name">The name of the schema.</param>
        public SchemaInfo(string name)
        {
            Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the schema.
        /// </summary>
        public string Name { get; internal set; }

        #endregion

        #region Overrides

        /// <summary>
        /// Gets the string that represents the instance of this <see cref="SchemaInfo"/> object.
        /// </summary>
        /// <returns>The name of this <see cref="SchemaInfo"/> object.</returns>
        public override string ToString() =>
            Name;

        #endregion

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="SchemaInfo"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode() =>
            Name?.GetHashCode() ?? 0;

        /// <summary>
        /// Compares the <see cref="SchemaInfo"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public override bool Equals(object obj) =>
            Equals(obj as SchemaInfo);

        /// <summary>
        /// Compares the <see cref="SchemaInfo"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(SchemaInfo other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Equals(Name, other.Name);
        }

        /// <summary>
        /// Compares the equality of the two <see cref="SchemaInfo"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="SchemaInfo"/> object.</param>
        /// <param name="objB">The second <see cref="SchemaInfo"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(SchemaInfo objA,
            SchemaInfo objB)
        {
            if (objA is null)
            {
                return objB is null;
            }
            return objA.Equals(objB);
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="SchemaInfo"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="SchemaInfo"/> object.</param>
        /// <param name="objB">The second <see cref="SchemaInfo"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(SchemaInfo objA,
            SchemaInfo objB) =>
            !(objA == objB);

        #endregion
    }
}
