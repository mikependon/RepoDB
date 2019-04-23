using System.Collections.Generic;
using System.Linq;
using System;
using RepoDb.Extensions;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// An object that signifies as data field in the query statement.
    /// </summary>
    public class Field : IEquatable<Field>
    {
        private int m_hashCode = 0;

        /// <summary>
        /// Creates a new instance of <see cref="Field"/> object.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        public Field(string name)
        {
            // Name is required
            if (string.IsNullOrEmpty(name))
            {
                throw new NullReferenceException(name);
            }

            // Set the name
            Name = name.AsQuoted(true);
            UnquotedName = name.AsUnquoted(true);

            // Set the hashcode here
            m_hashCode = name.GetHashCode();
        }

        /// <summary>
        /// Gets the quoted name of the field.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the unquoted name of the field.
        /// </summary>
        public string UnquotedName { get; }

        /// <summary>
        /// Stringify the current field object.
        /// </summary>
        /// <returns>The string value equivalent to the name of the field.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Creates an enumerable of <see cref="Field"/> objects that derived from the given array of string values.
        /// </summary>
        /// <param name="fields">The array of string values that signifies the name of the fields (for each item).</param>
        /// <returns>An enumerable of <see cref="Field"/> object.</returns>
        public static IEnumerable<Field> From(params string[] fields)
        {
            if (fields == null)
            {
                throw new NullReferenceException($"List of fields must not be null.");
            }
            if (fields.Any(field => string.IsNullOrEmpty(field?.Trim())))
            {
                throw new NullReferenceException($"Field name must not be null.");
            }
            return fields.Select(field => new Field(field));
        }

        /// <summary>
        /// Parses an object and creates an enumerable of <see cref="Field"/> objects.
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="entity">An object to be parsed.</param>
        /// <returns>An enumerable of <see cref="Field"/> objects.</returns>
        internal static IEnumerable<Field> Parse<TEntity>(TEntity entity)
            where TEntity : class
        {
            foreach (var property in PropertyCache.Get<TEntity>())
            {
                yield return new Field(PropertyMappedNameCache.Get(property.PropertyInfo, false));
            }
        }

        /// <summary>
        /// Parses an object and creates an enumerable of <see cref="Field"/> objects.
        /// </summary>
        /// <param name="obj">An object to be parsed.</param>
        /// <returns>An enumerable of <see cref="Field"/> objects.</returns>
        internal static IEnumerable<Field> Parse(object obj)
        {
            foreach (var property in obj.GetType().GetTypeInfo().GetProperties())
            {
                yield return new Field(PropertyMappedNameCache.Get(property, false));
            }
        }

        // Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="Field"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            return m_hashCode;
        }

        /// <summary>
        /// Compares the <see cref="Field"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            return obj?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the <see cref="Field"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(Field other)
        {
            return other?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <see cref="Field"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="Field"/> object.</param>
        /// <param name="objB">The second <see cref="Field"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(Field objA, Field objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objB?.GetHashCode() == objA.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="Field"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="Field"/> object.</param>
        /// <param name="objB">The second <see cref="Field"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(Field objA, Field objB)
        {
            return (objA == objB) == false;
        }
    }
}
