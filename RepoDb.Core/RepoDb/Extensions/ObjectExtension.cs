using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <i>System.Object</i> object.
    /// </summary>
    internal static class ObjectExtension
    {
        /// <summary>
        /// Merge the <i>RepoDb.QueryGroup</i> object into the current object.
        /// </summary>
        /// <param name="obj">The object where the <i>RepoDb.QueryGroup</i> object will be merged.</param>
        /// <param name="queryGroup">The <i>RepoDb.QueryGroup</i> object to merged.</param>
        /// <returns>The object instance itself with the merged values.</returns>
        public static object Merge(this object obj, QueryGroup queryGroup)
        {
            var expandObject = new ExpandoObject() as IDictionary<string, object>;
            obj?.GetType()
                .GetTypeInfo()
                .GetProperties()
                .ToList()
                .ForEach(property =>
                {
                    expandObject[property.Name] = property.GetValue(obj);
                });
            var dictionary = queryGroup?.AsObject() as ExpandoObject as IDictionary<string, object>;
            if (dictionary != null)
            {
                foreach (var kvp in dictionary)
                {
                    expandObject[kvp.Key] = kvp.Value;
                }
            }
            return (ExpandoObject)expandObject;
        }

        /// <summary>
        /// Converts an instance of an object into an enumerable list of query fields.
        /// </summary>
        /// <param name="obj">The instance of the object to be converted.</param>
        /// <returns>An enumerable list of query fields.</returns>
        public static IEnumerable<QueryField> AsQueryFields(this object obj)
        {
            var list = new List<QueryField>();
            var expandoObject = obj as ExpandoObject;
            if (expandoObject != null)
            {
                var dictionary = (IDictionary<string, object>)expandoObject;
                list.AddRange(dictionary.Select(item => new QueryField(item.Key, item.Value)).Cast<QueryField>());
            }
            else
            {
                var properties = obj.GetType().GetTypeInfo().GetProperties().ToList();
                properties.ForEach(property =>
                {
                    list.Add(new QueryField(property.Name, property.GetValue(obj)));
                });
            }
            return list;
        }

        /// <summary>
        /// Converts an instance of an object into an enumerable list of field.
        /// </summary>
        /// <param name="obj">The object to be converted.</param>
        /// <returns>An enumerable list of fields.</returns>
        public static IEnumerable<Field> AsFields(this object obj)
        {
            return Field.Parse(obj);
        }

        /// <summary>
        /// Converts an instance of an object into an enumerable list of order fields.
        /// </summary>
        /// <param name="obj">The object to be converted.</param>
        /// <returns>An enumerable list of order fields.</returns>
        public static IEnumerable<OrderField> AsOrderFields(this object obj)
        {
            return OrderField.Parse(obj);
        }

        /// <summary>
        /// Returns the first non-null occurence.
        /// </summary>
        /// <param name="obj">The current object.</param>
        /// <param name="parameters">The list of parameters.</param>
        /// <returns>The first non-null object.</returns>
        public static object Coalesce(this object obj, params object[] parameters)
        {
            return parameters?.First(param => param != null);
        }
    }
}
