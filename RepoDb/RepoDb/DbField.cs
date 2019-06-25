using RepoDb.Extensions;
using System;

namespace RepoDb
{
    /// <summary>
    /// A class the holds the column definition of the database table columns.
    /// </summary>
    public class DbField : IEquatable<DbField>
    {
        private int m_hashCode = 0;

        /// <summary>
        /// Creates a new instance of <see cref="DbField"/> object.
        /// </summary>
        public DbField(string name,
            bool isPrimary,
            bool isIdentity,
            bool isNullable,
            Type type,
            int? size,
            byte? precision,
            byte? scale)
            : this(name,
                  isPrimary,
                  isIdentity,
                  isNullable,
                  type,
                  size,
                  precision,
                  scale,
                  null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="DbField"/> object.
        /// </summary>
        internal DbField(string name,
            bool isPrimary,
            bool isIdentity,
            bool isNullable,
            Type type,
            int? size,
            byte? precision,
            byte? scale,
            string databaseType)
        {
            // Name is required
            if (string.IsNullOrEmpty(name))
            {
                throw new NullReferenceException("Name");
            }

            // Set the properties
            Name = name.AsQuoted(true);
            UnquotedName = name.AsUnquoted(true);
            IsPrimary = isPrimary;
            IsIdentity = isIdentity;
            IsNullable = isNullable;
            Type = type;
            Size = size;
            Precision = precision;
            Scale = scale;
            DatabaseType = databaseType;

            // Set the hashcode
            m_hashCode = name.GetHashCode() ^ isPrimary.GetHashCode() ^ isIdentity.GetHashCode() ^ isNullable.GetHashCode();
            if (type != null)
            {
                m_hashCode ^= type.GetHashCode();
            }
            if (size != null)
            {
                m_hashCode ^= size.GetHashCode();
            }
            if (precision != null)
            {
                m_hashCode ^= precision.GetHashCode();
            }
            if (scale != null)
            {
                m_hashCode ^= scale.GetHashCode();
            }
            if (databaseType != null)
            {
                m_hashCode ^= databaseType.GetHashCode();
            }
        }

        /// <summary>
        /// Gets the quoted name of the database field.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the unquoted name of the database field.
        /// </summary>
        public string UnquotedName { get; }

        /// <summary>
        /// Gets the value whether the column is a primary column.
        /// </summary>
        public bool IsPrimary { get; }

        /// <summary>
        /// Gets the value whether the column is an identify column.
        /// </summary>
        public bool IsIdentity { get; }

        /// <summary>
        /// Gets the value whether the column is nullable.
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

        // Methods

        /// <summary>
        /// Gets the string that represents the instance of this <see cref="DbField"/> object.
        /// </summary>
        /// <returns>The string that represents the instance of this <see cref="DbField"/> object.</returns>
        public override string ToString()
        {
            return string.Concat(Name, " (", m_hashCode, ")");
        }

        // Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="DbField"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            return m_hashCode;
        }

        /// <summary>
        /// Compares the <see cref="DbField"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            return obj?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the <see cref="DbField"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(DbField other)
        {
            return other?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <see cref="DbField"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="DbField"/> object.</param>
        /// <param name="objB">The second <see cref="DbField"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(DbField objA, DbField objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objB?.GetHashCode() == objA.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="DbField"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="DbField"/> object.</param>
        /// <param name="objB">The second <see cref="DbField"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(DbField objA, DbField objB)
        {
            return (objA == objB) == false;
        }
    }
}
