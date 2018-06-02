using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using RepoDb.Interfaces;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <i>System.Object</i> object.
    /// </summary>
    public static class ObjectExtension
    {
        // AsObject
        internal static object Merge(this object obj, IQueryGroup queryGroup)
        {
            var expandObject = new ExpandoObject() as IDictionary<string, object>;
            obj?.GetType()
                .GetProperties()
                .ToList()
                .ForEach(property =>
                {
                    expandObject[property.Name] = property.GetValue(obj);
                });
            var queryGroupExpandObject = queryGroup?.AsObject() as ExpandoObject as IDictionary<string, object>;
            if (queryGroupExpandObject != null)
            {
                foreach(var kvp in queryGroupExpandObject)
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
        public static IEnumerable<IQueryField> AsQueryFields(this object obj)
        {
            var list = new List<IQueryField>();
            var expandoObject = obj as ExpandoObject;
            if (expandoObject != null)
            {
                var dictionary = (IDictionary<string, object>)expandoObject;
                list.AddRange(dictionary.Select(item => new QueryField(item.Key, item.Value)).Cast<IQueryField>());
            }
            else
            {
                var properties = obj.GetType().GetProperties().ToList();
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
        public static IEnumerable<IField> AsFields(this object obj)
        {
            return Field.Parse(obj);
        }

        /// <summary>
        /// Converts an instance of an object into an enumerable list of order fields.
        /// </summary>
        /// <param name="obj">The object to be converted.</param>
        /// <returns>An enumerable list of order fields.</returns>
        public static IEnumerable<IOrderField> AsOrderFields(this object obj)
        {
            return OrderField.Parse(obj);
        }
    }
}
