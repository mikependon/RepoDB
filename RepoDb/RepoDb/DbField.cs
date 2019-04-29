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
            bool isNullable)
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

            // Set the hashcode
            m_hashCode = name.GetHashCode() + isPrimary.GetHashCode() + isIdentity.GetHashCode() + isNullable.GetHashCode();
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
