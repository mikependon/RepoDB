using System.Collections.Generic;
using System.Linq;
using System;
using RepoDb.Extensions;

namespace RepoDb
{
    /// <summary>
    /// An object that signifies as data field in the query statement.
    /// </summary>
    public class Field : IEquatable<Field>
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.Field</i> object.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        public Field(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Stringify the current field object.
        /// </summary>
        /// <returns>The string value equivalent to the name of the field.</returns>
        public override string ToString()
        {
            return $"[{Name}]";
        }

        /// <summary>
        /// Creates an enumerable of <i>RepoDb.Field</i> objects that derived from the given array of string values.
        /// </summary>
        /// <param name="fields">The array of string values that signifies the name of the fields (for each item).</param>
        /// <returns>An enumerable of <i>RepoDb.Field</i> object.</returns>
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
        /// Parse an object and creates an enumerable of <i>RepoDb.Field</i> objects. Each field is equivalent
        /// to each property of the given object. The parse operation uses a reflection operation.
        /// </summary>
        /// <param name="obj">An object to be parsed.</param>
        /// <returns>An enumerable of <i>RepoDb.Field</i> objects.</returns>
        public static IEnumerable<Field> Parse(object obj)
        {
            if (obj == null)
            {
                throw new NullReferenceException("Parameter 'obj' cannot be null.");
            }
            if (obj.GetType().IsGenericType == false)
            {
                throw new InvalidOperationException("Parameter 'obj' must be dynamic type.");
            }
            var properties = obj.GetType().GetProperties();
            if (properties?.Any() == false)
            {
                throw new InvalidOperationException("Parameter 'obj' must have atleast one property.");
            }
            return properties.Select(property => new Field(PropertyMappedNameCache.Get(property)));
        }

        // Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <i>Field</i>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        /// <summary>
        /// Compares the <i>Field</i> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            return GetHashCode() == obj?.GetHashCode();
        }

        /// <summary>
        /// Compares the <i>Field</i> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(Field other)
        {
            return GetHashCode() == other?.GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <i>Field</i> objects.
        /// </summary>
        /// <param name="objA">The first <i>Field</i> object.</param>
        /// <param name="objB">The second <i>Field</i> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(Field objA, Field objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objA?.GetHashCode() == objB?.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <i>Field</i> objects.
        /// </summary>
        /// <param name="objA">The first <i>Field</i> object.</param>
        /// <param name="objB">The second <i>Field</i> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(Field objA, Field objB)
        {
            return (objA == objB) == false;
        }
    }
}
