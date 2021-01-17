using RepoDb.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb
{
    public partial class QueryGroup
    {
        /// <summary>
        /// Parses an object and convert back the result to an instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="obj">The instance of the object to be parsed.</param>
        /// <returns>An instance of the <see cref="QueryGroup"/> with parsed properties and values.</returns>
        public static QueryGroup Parse<T>(T obj) =>
            Parse<T>(obj, true);

        /// <summary>
        /// Parses an object and convert back the result to an instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="obj">The instance of the object to be parsed.</param>
        /// <param name="throwException">If true, an exception will be thrown if the type of 'obj' argument cannot be parsed.</param>
        /// <returns>An instance of the <see cref="QueryGroup"/> with parsed properties and values.</returns>
        public static QueryGroup Parse<T>(T obj,
            bool throwException = true)
        {
            // Check for value
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            // Type of the object
            var type = obj.GetType().GetUnderlyingType();

            // Filter the type
            if (type.IsClassType() == false)
            {
                if (throwException == true)
                {
                    throw new ArgumentException("Parameter 'obj' type cannot be parsed.");
                }
                else
                {
                    return null;
                }
            }

            // Declare variables
            var queryFields = new List<QueryField>();

            // Iterate every property
            foreach (var property in type.GetProperties())
            {
                queryFields.Add(new QueryField(property.AsField(), property.GetValue(obj)));
            }

            // Return
            return queryFields.Any() == true ? new QueryGroup(queryFields).Fix() : null;
        }
    }
}
