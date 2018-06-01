using System.Collections.Generic;
using System.Linq;
using RepoDb.Interfaces;
using System;

namespace RepoDb
{
    /// <summary>
    /// An object that signifies as data field in the query statement.
    /// </summary>
    public class Field : IField
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
        /// Creates an enumerable of <i>RepoDb.Interfaces.IField</i> objects that derived from the given array of string values.
        /// </summary>
        /// <param name="fields">The array of string values that signifies the name of the fields (for each item).</param>
        /// <returns>An enumerable of <i>RepoDb.Interfaces.IField</i> object.</returns>
        public static IEnumerable<IField> From(params string[] fields)
        {
            return fields.ToList().Select(field => new Field(field));
        }

        /// <summary>
        /// Parse an object and creates an enumerable of <i>RepoDb.Interfaces.IField</i> objects. Each field is equivalent
        /// to each property of the given object. The parse operation uses a reflection operation.
        /// </summary>
        /// <param name="obj">An object to be parsed.</param>
        /// <returns>An enumerable of <i>RepoDb.Interfaces.IField</i> objects.</returns>
        public static IEnumerable<IField> Parse(object obj)
        {
            if (obj == null)
            {
                throw new InvalidOperationException("Parameter 'obj' cannot be null.");
            }
            var list = new List<IField>();
            obj.GetType()
                .GetProperties()
                .ToList()
                .ForEach(property =>
                {
                    list.Add(new Field(property.Name));
                });
            return list;
        }
    }
}