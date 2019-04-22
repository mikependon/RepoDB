using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="Object"/>.
    /// </summary>
    internal static class ObjectExtension
    {
        /// <summary>
        /// Merge the <see cref="QueryGroup"/> object into the current object.
        /// </summary>
        /// <param name="obj">The object where the <see cref="QueryGroup"/> object will be merged.</param>
        /// <param name="queryGroup">The <see cref="QueryGroup"/> object to merged.</param>
        /// <returns>The object instance itself with the merged values.</returns>
        public static object Merge(this object obj, QueryGroup queryGroup)
        {
            var expandObject = new ExpandoObject() as IDictionary<string, object>;
            foreach (var property in obj?.GetType().GetProperties())
            {
                expandObject[property.Name] = property.GetValue(obj);
            }
            foreach (var queryField in queryGroup?.Fix().GetFields())
            {
                expandObject[queryField.Parameter.Name] = queryField.Parameter.Value;
            }
            return (ExpandoObject)expandObject;
        }

        /// <summary>
        /// Merges an object into an instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="obj">The object to be merged.</param>
        /// <param name="queryGroup">The <see cref="QueryGroup"/> object to be merged.</param>
        /// <returns>An instance of converted dynamic object.</returns>
        internal static object AsMergedObject(this object obj, QueryGroup queryGroup)
        {
            var expandObject = new ExpandoObject() as IDictionary<string, object>;
            var properties = DataEntityExtension.GetProperties(obj.GetType());
            foreach (var property in properties)
            {
                expandObject[property.GetUnquotedMappedName()] = property.PropertyInfo.GetValue(obj);
            }
            if (queryGroup != null)
            {
                foreach (var queryField in queryGroup.Fix().GetFields())
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
            return AsMergedObject(obj, null);
        }

        /// <summary>
        /// Converts an instance of an object into an enumerable list of query fields.
        /// </summary>
        /// <param name="obj">The instance of the object to be converted.</param>
        /// <returns>An enumerable list of query fields.</returns>
        public static IEnumerable<QueryField> AsQueryFields(this object obj)
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
                var properties = obj.GetType().GetProperties();
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
        public static IEnumerable<Field> AsFields<TEntity>(this TEntity entity)
            where TEntity : class
        {
            return Field.Parse<TEntity>(entity);
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

        /// <summary>
        /// Identify whether an object is a decimal.
        /// </summary>
        /// <param name="value">The value to be identified.</param>
        /// <returns>True if the value is a decimal.</returns>
        public static bool IsDecimal(this object value)
        {
            return value is float ||
                value is double ||
                value is decimal;
        }

        /// <summary>
        /// Identify whether an object is a number.
        /// </summary>
        /// <param name="value">The value to be identified.</param>
        /// <returns>True if the value is a number.</returns>
        public static bool IsNumber(this object value)
        {
            return value is sbyte ||
                value is byte ||
                value is short ||
                value is ushort ||
                value is int ||
                value is uint ||
                value is long ||
                value is ulong ||
                value is float ||
                value is double ||
                value is decimal;
        }

        /// <summary>
        /// Converts an object to a <see cref="decimal"/>.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>A <see cref="decimal"/> value of the object.</returns>
        public static double ToDecimal(this object value)
        {
            return Convert.ToDouble(value);
        }

        /// <summary>
        /// Converts an object to a <see cref="long"/>.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>A <see cref="long"/> value of the object.</returns>
        public static long ToNumber(this object value)
        {
            return Convert.ToInt64(value);
        }
    }
}
