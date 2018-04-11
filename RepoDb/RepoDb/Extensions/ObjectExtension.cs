using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using RepoDb.Interfaces;

namespace RepoDb.Extensions
{
    public static class ObjectExtension
    {
        // ToQueryFields
        internal static IEnumerable<IQueryField> ToQueryFields(this object obj)
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

        // AsOrderFields
        internal static IEnumerable<IOrderField> AsOrderFields(this object obj)
        {
            return OrderField.Parse(obj);
        }
    }
}
