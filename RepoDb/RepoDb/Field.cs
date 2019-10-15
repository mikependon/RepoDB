using System.Collections.Generic;
using System.Linq;
using System;
using RepoDb.Extensions;
using RepoDb.Interfaces;

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
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        public Field(string name,
            IDbSetting dbSetting) :
            this(name, null, dbSetting)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="Field"/> object.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="type">The type of the field.</param>
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        public Field(string name,
        Type type,
        IDbSetting dbSetting)
        {
            // Name is required
            if (string.IsNullOrEmpty(name))
            {
                throw new NullReferenceException(name);
            }

            // Set the name
            Name = name.AsQuoted(true, true, dbSetting);
            UnquotedName = name.AsUnquoted(true, dbSetting);

            // Set the type
            Type = type;

            // Set the DbSetting
            DbSetting = dbSetting;

            // Set the hashcode
            m_hashCode = Name.GetHashCode();
            if (type != null)
            {
                m_hashCode += type.GetHashCode();
            }
            //if (dbSetting != null)
            //{
            //    m_hashCode += dbSetting.GetHashCode();
            //}
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
        /// Gets the type of the field.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the database setting currently in used.
        /// </summary>
        public IDbSetting DbSetting { get; }

        /// <summary>
        /// Stringify the current field object.
        /// </summary>
        /// <returns>The string value equivalent to the name of the field.</returns>
        public override string ToString()
        {
            return string.Concat(Name, ", ", Type?.FullName, " (", m_hashCode, ")");
        }

        /// <summary>
        /// Creates an enumerable of <see cref="Field"/> objects that derived from the string value.
        /// </summary>
        /// <param name="name">The enumerable of string values that signifies the name of the fields (for each item).</param>
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        /// <returns>An enumerable of <see cref="Field"/> object.</returns>
        public static IEnumerable<Field> From(string name,
            IDbSetting dbSetting)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new NullReferenceException("The field name must be null or empty.");
            }
            return From(name.AsEnumerable(), dbSetting);
        }

        /// <summary>
        /// Creates an enumerable of <see cref="Field"/> objects that derived from the given array of string values.
        /// </summary>
        /// <param name="fields">The enumerable of string values that signifies the name of the fields (for each item).</param>
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        /// <returns>An enumerable of <see cref="Field"/> object.</returns>
        public static IEnumerable<Field> From(IEnumerable<string> fields,
            IDbSetting dbSetting)
        {
            if (fields == null)
            {
                throw new NullReferenceException("The list of fields must not be null.");
            }
            if (fields.Any(field => string.IsNullOrEmpty(field?.Trim())))
            {
                throw new NullReferenceException("The field name must be null or empty.");
            }
            foreach (var field in fields)
            {
                yield return new Field(field, dbSetting);
            }
        }

        /// <summary>
        /// Parses an object and creates an enumerable of <see cref="Field"/> objects.
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="entity">An object to be parsed.</param>
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        /// <returns>An enumerable of <see cref="Field"/> objects.</returns>
        internal static IEnumerable<Field> Parse<TEntity>(TEntity entity,
            IDbSetting dbSetting)
            where TEntity : class
        {
            foreach (var property in PropertyCache.Get<TEntity>(dbSetting))
            {
                yield return property.AsField();
            }
        }

        /// <summary>
        /// Parses an object and creates an enumerable of <see cref="Field"/> objects.
        /// </summary>
        /// <param name="obj">An object to be parsed.</param>
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        /// <returns>An enumerable of <see cref="Field"/> objects.</returns>
        internal static IEnumerable<Field> Parse(object obj,
            IDbSetting dbSetting)
        {
            foreach (var property in obj?.GetType().GetProperties())
            {
                yield return property.AsField(dbSetting);
            }
        }

        #region Equality and comparers

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
            //var hashCode = obj?.GetHashCode();
            //if (obj is string)
            //{
            //    if (DbSetting != null)
            //    {
            //        hashCode += DbSetting.GetHashCode();
            //    }
            //}
            //return hashCode == GetHashCode();
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

        #endregion
    }
}
