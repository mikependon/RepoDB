using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="IDataReader"/> object.
    /// </summary>
    public static class DataReaderExtension
    {
        /// <summary>
        /// Converts the <see cref="IDataReader"/> object into an enumerable list of data entity objects.
        /// </summary>
        /// <typeparam name="TEntity">The target type of the data entity.</typeparam>
        /// <param name="reader">The data reader object to be converted.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        [Obsolete("This extended method will be removed soon.")]
        public static IEnumerable<TEntity> AsEnumerable<TEntity>(this IDataReader reader)
            where TEntity : class
        {
            var properties = PropertyCache.Get<TEntity>()
                .Where(property => property.PropertyInfo.CanWrite);
            var dictionary = new Dictionary<int, ClassProperty>();
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var property = properties.FirstOrDefault(p => string.Equals(p.GetMappedName(), reader.GetName(i), StringComparison.OrdinalIgnoreCase));
                if (property != null)
                {
                    dictionary.Add(i, property);
                }
            }
            while (reader.Read())
            {
                var obj = Activator.CreateInstance<TEntity>();
                foreach (var kvp in dictionary)
                {
                    var value = reader.IsDBNull(kvp.Key) ? null : reader.GetValue(kvp.Key);
                    kvp.Value.PropertyInfo.SetValue(obj, value);
                }
                yield return obj;
            }
        }

        /// <summary>
        /// Converts the <see cref="IDataReader"/> object into an enumerable list of dynamic objects containing the schema of the reader.
        /// </summary>
        /// <param name="reader">The data reader object to be converted.</param>
        /// <returns>An enumerable list of dynamic objects containing the schema of the reader.</returns>
        [Obsolete("This extended method will be removed soon.")]
        public static IEnumerable<object> AsEnumerable(this IDataReader reader)
        {
            while (reader.Read())
            {
                var obj = new ExpandoObject() as IDictionary<string, object>;
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var value = reader.IsDBNull(i) ? null : reader[i];
                    obj.Add(reader.GetName(i), value);
                }
                yield return obj;
            }
        }
    }
}