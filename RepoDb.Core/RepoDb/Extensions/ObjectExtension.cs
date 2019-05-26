using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="Object"/>.
    /// </summary>
    public static class ObjectExtension
    {
        /// <summary>
        /// Converts the current object to become an array.
        /// </summary>
        /// <param name="obj">The object to be converted.</param>
        /// <returns>A converted instance of <see cref="Array"/>.</returns>
        internal static Array AsArray(this object obj)
        {
            // Return immediately if it is an array
            if (obj is Array)
            {
                return (Array)obj;
            }

            // Get the type
            var array = (Array)null;
            var type = obj.GetType();

            // If this is a constructed
            if (type.IsConstructedGenericType == true)
            {
                // Get the first generic type
                type = type.GetTypeInfo().GetGenericArguments()[0];

                // Create a type from generic
                var listType = typeof(List<>).MakeGenericType(type);
                var toArrayMethod = listType.GetTypeInfo().GetMethod("get_Item", new[] { typeof(int) });
                var count = (int)listType.GetTypeInfo().GetProperty("Count").GetValue(obj);

                // Create a new instance of array
                array = Array.CreateInstance(type, count);

                // Iterate the values from the object
                for (var index = 0; index < count; index++)
                {
                    var value = toArrayMethod.GetValue(obj, new object[] { index });
                    array.SetValue(value, index);
                }
            }

            // Return the converted array
            return array;
        }

        /// <summary>
        /// Merges an object into an instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the object.</typeparam>
        /// <param name="obj">The object to be merged.</param>
        /// <param name="queryGroup">The <see cref="QueryGroup"/> object to be merged.</param>
        /// <returns>An instance of converted dynamic object.</returns>
        internal static object Merge<TEntity>(this TEntity obj, QueryGroup queryGroup)
            where TEntity : class
        {
            return Merge(obj, PropertyCache.Get<TEntity>().Select(p => p.PropertyInfo), queryGroup);
        }

        /// <summary>
        /// Merge the <see cref="QueryGroup"/> object into the current object.
        /// </summary>
        /// <param name="obj">The object where the <see cref="QueryGroup"/> object will be merged.</param>
        /// <param name="queryGroup">The <see cref="QueryGroup"/> object to merged.</param>
        /// <returns>A dynamic object with the merged fields from <see cref="QueryGroup"/>.</returns>
        internal static object Merge(this object obj, QueryGroup queryGroup)
        {
            return Merge(obj, obj?.GetType().GetTypeInfo().GetProperties(), queryGroup);
        }

        /// <summary>
        /// Merge the <see cref="QueryGroup"/> object into the current object.
        /// </summary>
        /// <param name="obj">The object where the <see cref="QueryGroup"/> object will be merged.</param>
        /// <param name="properties">The list of <see cref="PropertyInfo"/> objects.</param>
        /// <param name="queryGroup">The <see cref="QueryGroup"/> object to merged.</param>
        /// <returns>The object instance itself with the merged values.</returns>
        internal static object Merge(this object obj, IEnumerable<PropertyInfo> properties, QueryGroup queryGroup)
        {
            var expandObject = new ExpandoObject() as IDictionary<string, object>;
            foreach (var property in properties)
            {
                expandObject[PropertyMappedNameCache.Get(property, false)] = property.GetValue(obj);
            }
            if (queryGroup != null)
            {
                foreach (var queryField in queryGroup?.Fix().GetFields())
                {
                    expandObject[queryField.Parameter.Name] = queryField.Parameter.Value;
                }
            }
            return (ExpandoObject)expandObject;
        }

        /// <summary>
        /// Converts the data entity object into a dynamic object.
        /// </summary>
        /// <param name="obj">The object to be converted.</param>
        /// <returns>An instance of converted dynamic object.</returns>
        internal static object AsObject(this object obj)
        {
            return Merge(obj, null);
        }

        /// <summary>
        /// Converts an instance of an object into an enumerable list of query fields.
        /// </summary>
        /// <param name="obj">The instance of the object to be converted.</param>
        /// <returns>An enumerable list of query fields.</returns>
        internal static IEnumerable<QueryField> AsQueryFields(this object obj)
        {
            var expandoObject = obj as ExpandoObject;
            if (expandoObject != null)
            {
                var dictionary = (IDictionary<string, object>)expandoObject;
                var fields = dictionary.Select(item => new QueryField(item.Key, item.Value)).Cast<QueryField>();
                foreach (var field in fields)
                {
                    yield return field;
                }
            }
            else
            {
                var properties = obj.GetType().GetTypeInfo().GetProperties();
                foreach (var property in properties)
                {
                    yield return new QueryField(property.Name, property.GetValue(obj));
                }
            }
        }

        /// <summary>
        /// Converts an instance of an object into an enumerable list of field.
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="entity">The instance to be converted.</param>
        /// <returns>An enumerable list of fields.</returns>
        internal static IEnumerable<Field> AsFields<TEntity>(this TEntity entity)
            where TEntity : class
        {
            return FieldCache.Get<TEntity>();
        }

        /// <summary>
        /// Converts an instance of an object into an enumerable list of field.
        /// </summary>
        /// <param name="obj">The object to be converted.</param>
        /// <returns>An enumerable list of fields.</returns>
        internal static IEnumerable<Field> AsFields(this object obj)
        {
            return Field.Parse(obj);
        }

        /// <summary>
        /// Converts an instance of an object into an enumerable list of order fields.
        /// </summary>
        /// <param name="obj">The object to be converted.</param>
        /// <returns>An enumerable list of order fields.</returns>
        internal static IEnumerable<OrderField> AsOrderFields(this object obj)
        {
            return OrderField.Parse(obj);
        }

        /// <summary>
        /// Returns the first non-null occurence.
        /// </summary>
        /// <param name="obj">The current object.</param>
        /// <param name="parameters">The list of parameters.</param>
        /// <returns>The first non-null object.</returns>
        internal static object Coalesce(this object obj, params object[] parameters)
        {
            return parameters.First(param => param != null);
        }

        /// <summary>
        /// Returns the first non-defaulted occurence.
        /// </summary>
        /// <typeparam name="T">The target type of the object.</typeparam>
        /// <param name="obj">The current object.</param>
        /// <param name="parameters">The list of parameters.</param>
        /// <returns>The first non-defaulted object.</returns>
        internal static T Coalesce<T>(this object obj, params T[] parameters)
        {
            return parameters.First(param => Equals(param, default(T)) == false);
        }

        /// <summary>
        /// Converts an object to a <see cref="long"/>.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>A <see cref="long"/> value of the object.</returns>
        internal static long ToNumber(this object value)
        {
            return Convert.ToInt64(value);
        }
    }
}
