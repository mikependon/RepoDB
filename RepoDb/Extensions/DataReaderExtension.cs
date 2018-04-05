using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace RepoDb.Extensions
{
    public static class DataReaderExtension
    {
        internal static T ToType<T>(this IDataReader reader)
        {
            var obj = Activator.CreateInstance<T>();
            var properties = typeof (T).GetProperties().ToList();
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var property =
                    properties.FirstOrDefault(
                        p => string.Equals(p.Name, reader.GetName(i), StringComparison.InvariantCultureIgnoreCase));
                var value = reader.GetValue(i);
                property?.SetValue(obj, value != DBNull.Value ? value : null);
            }
            return obj;
        }

        internal static T ToDataEntity<T>(this IDataReader reader)
            where T : DataEntity
        {
            return ToType<T>(reader);
        }

        internal static object ToObject(this IDataReader reader)
        {
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;
            for (var i = 0; i < reader.FieldCount; i++)
            {
                expandoObject.Add(reader.GetName(i), reader.GetValue(i));
            }
            return expandoObject;
        }
    }
}