using RepoDb.Enumerations;
using RepoDb.Interfaces;
using RepoDb.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <i>System.Data.IDataReader</i> object.
    /// </summary>
    public static class DataReaderExtension
    {
        internal static IEnumerable<T> AsEnumerable<T>(this IDataReader reader)
            where T : DataEntity
        {
            var properties = DataEntityExtension.GetPropertiesFor<T>(Command.None)
                .Where(property => property.CanWrite);
            var dictionary = new Dictionary<int, PropertyInfo>();
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var property = properties.FirstOrDefault(p => p.GetMappedName().ToLower() == reader.GetName(i).ToLower());
                if (property != null)
                {
                    dictionary.Add(i, property);
                }
            }
            var list = new List<T>();
            while (reader.Read())
            {
                var obj = Activator.CreateInstance<T>();
                foreach (var kvp in dictionary)
                {
                    var value = reader.IsDBNull(kvp.Key) ? null : reader.GetValue(kvp.Key);
                    kvp.Value.SetValue(obj, value);
                }
                list.Add(obj);
            }
            return list;
        }

        internal static IEnumerable<object> AsEnumerable(this IDataReader reader)
        {
            var list = new List<object>();
            while (reader.Read())
            {
                var obj = new ExpandoObject() as IDictionary<string, object>;
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var value = reader.IsDBNull(i) ? null : reader[i];
                    obj.Add(reader.GetName(i), value);
                }
                list.Add(obj);
            }
            return list;
        }
    }
}