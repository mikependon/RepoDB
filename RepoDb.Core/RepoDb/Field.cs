using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using RepoDb.Extensions;

namespace RepoDb
{
    /// <summary>
    /// An object that signifies as data field in the query statement.
    /// </summary>
    public class Field
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
            if (obj.GetType().GetTypeInfo().IsGenericType == false)
            {
                throw new InvalidOperationException("Parameter 'obj' must be dynamic type.");
            }
            var properties = obj.GetType().GetTypeInfo().GetProperties();
            if (properties?.Any() == false)
            {
                throw new InvalidOperationException("Parameter 'obj' must have atleast one property.");
            }
            return properties.Select(property => new Field(property.GetMappedName()));
        }
    }
}
