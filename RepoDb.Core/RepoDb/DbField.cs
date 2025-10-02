﻿using System;

namespace RepoDb
{
    /// <summary>
    /// A class the holds the column definition of the table.
    /// </summary>
    public class DbField : IEquatable<DbField>
    {
        private int? hashCode = null;

        /// <summary>
        /// Creates a new instance of <see cref="DbField"/> object.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="isPrimary">The value that indicates whether the field is primary.</param>
        /// <param name="isIdentity">The value that indicates whether the field is identity.</param>
        /// <param name="isNullable">The value that indicates whether the field is nullable.</param>
        /// <param name="type">The equivalent .NET CLR type of the field.</param>
        /// <param name="size">The size of the field.</param>
        /// <param name="precision">The precision of the field.</param>
        /// <param name="scale">The scale of the field.</param>
        /// <param name="databaseType">The database type of the field.</param>
        /// <param name="hasDefaultValue">The value that defines whether the column has a default value..</param>
        /// <param name="provider">The database provider who created this instance.</param>
        public DbField(string name,
            bool isPrimary,
            bool isIdentity,
            bool isNullable,
            Type type,
            int? size,
            byte? precision,
            byte? scale,
            string databaseType,
            bool hasDefaultValue = false,
            string provider = null)
        {
            // Name is required
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new NullReferenceException("Name");
            }

            // Set the properties
            Name = name;
            IsPrimary = isPrimary;
            IsIdentity = isIdentity;
            IsNullable = isNullable;
            Type = type;
            Size = size;
            if (type == StaticType.Double && precision > 38)
            {
                Precision = 38;
            }
            else
            {
                Precision = precision;
            }
            Scale = scale;
            DatabaseType = databaseType;
            HasDefaultValue = hasDefaultValue;
            Provider = provider;
        }

        #region Properties

        /// <summary>
        /// Gets the quoted name of the database field.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the value that indicates whether the column is a primary column.
        /// </summary>
        public bool IsPrimary { get; }

        /// <summary>
        /// Gets the value that indicates whether the column is an identify column.
        /// </summary>
        public bool IsIdentity { get; }

        /// <summary>
        /// Gets the value that indicates whether the column is nullable.
        /// </summary>
        public bool IsNullable { get; }

        /// <summary>
        /// Gets the .NET type of the column.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the size of the column.
        /// </summary>
        public int? Size { get; }

        /// <summary>
        /// Gets the precision of the column.
        /// </summary>
        public byte? Precision { get; }

        /// <summary>
        /// Gets the scale of the column.
        /// </summary>
        public byte? Scale { get; }

        /// <summary>
        /// Gets the database type of the column.
        /// </summary>
        public string DatabaseType { get; }

        /// <summary>
        /// Gets the value that defines whether the column has a default value.
        /// </summary>
        public bool HasDefaultValue { get; }

        /// <summary>
        /// Gets the database provider who created this instance.
        /// </summary>
        public string Provider { get; }


        /// <summary>
        /// Gets the type to map to, including nullable
        /// </summary>
        /// <returns></returns>
        public Type TypeNullable() => IsNullable && Type.IsValueType ? typeof(System.Nullable<>).MakeGenericType(Type) : Type;

        #endregion

        #region Methods

        /// <summary>
        /// Gets the string that represents the instance of this <see cref="DbField"/> object.
        /// </summary>
        /// <returns>The string that represents the instance of this <see cref="DbField"/> object.</returns>
        public override string ToString() =>
            string.Concat(Name, ", ", IsPrimary.ToString(), " (", hashCode.ToString(), ")");

        #endregion

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="DbField"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            if (this.hashCode != null)
            {
                return this.hashCode.Value;
            }

            // Set the hashcode
            var hashCode = HashCode.Combine(Name, IsPrimary, IsIdentity, IsNullable);

            if (Type != null)
            {
                hashCode = HashCode.Combine(hashCode, Type);
            }
            if (Size != null)
            {
                hashCode = HashCode.Combine(hashCode, Size);
            }
            if (Precision != null)
            {
                hashCode = HashCode.Combine(hashCode, Precision);
            }
            if (Scale != null)
            {
                hashCode = HashCode.Combine(hashCode, Scale);
            }
            if (DatabaseType != null)
            {
                hashCode = HashCode.Combine(hashCode, DatabaseType);
            }
            if (Provider != null)
            {
                hashCode = HashCode.Combine(hashCode, Provider);
            }

            // Set and return the hashcode
            return (this.hashCode = hashCode).Value;
        }

        /// <summary>
        /// Compares the <see cref="DbField"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;

            return obj.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the <see cref="DbField"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(DbField other)
        {
            if (other is null) return false;

            return other.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <see cref="DbField"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="DbField"/> object.</param>
        /// <param name="objB">The second <see cref="DbField"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(DbField objA,
            DbField objB)
        {
            if (objA is null)
            {
                return objB is null;
            }
            return objA.Equals(objB);
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="DbField"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="DbField"/> object.</param>
        /// <param name="objB">The second <see cref="DbField"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(DbField objA,
            DbField objB) =>
            (objA == objB) == false;

        #endregion
    }
}
