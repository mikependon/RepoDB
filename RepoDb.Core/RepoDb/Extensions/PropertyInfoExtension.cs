using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using System;
using RepoDb.Interfaces;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="PropertyInfo"/> object.
    /// </summary>
    public static class PropertyInfoExtension
    {
        /// <summary>
        /// Gets a value of the property.
        /// </summary>
        /// <param name="property">The property where to get the value of the object.</param>
        /// <param name="obj">The object that contains the defined property.</param>
        /// <returns>The value of the property.</returns>
        public static object GetValue(this PropertyInfo property,
            object obj)
        {
            return property.GetValue(obj, null);
        }

        /// <summary>
        /// Sets a value of the property.
        /// </summary>
        /// <param name="property">The property where to set the value of the object.</param>
        /// <param name="obj">The object that contains the defined property.</param>
        /// <param name="value">The value to be set for the property.</param>
        public static void SetValue(this PropertyInfo property,
            object obj,
            object value)
        {
            property.SetValue(obj, value, null);
        }

        /// <summary>
        /// Gets a custom attribute defined on the property.
        /// </summary>
        /// <typeparam name="T">The custom attribute that is defined into the property.</typeparam>
        /// <param name="property">The type of where the custom attribute is defined.</param>
        /// <returns>The custom attribute.</returns>
        public static T GetCustomAttribute<T>(this PropertyInfo property)
            where T : Attribute
        {
            return (T)GetCustomAttribute(property, typeof(T));
        }

        /// <summary>
        /// Gets a custom attribute defined on the property.
        /// </summary>
        /// <param name="property">The type of where the custom attribute is defined.</param>
        /// <param name="type">The custom attribute that is defined into the property.</param>
        /// <returns>The custom attribute.</returns>
        public static Attribute GetCustomAttribute(this PropertyInfo property,
            Type type)
        {
            var attributes = property.GetCustomAttributes(type, false)?.OfType<Attribute>();
            return attributes?.FirstOrDefault(a => a.GetType() == type);
        }

        /// <summary>
        /// Converts an instance of <see cref="PropertyInfo"/> into an enumerable list of <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="property">The <see cref="PropertyInfo"/> instance to be converted.</param>
        /// <returns>An enumerable list of <see cref="PropertyInfo"/>.</returns>
        public static IEnumerable<PropertyInfo> AsEnumerable(this PropertyInfo property)
        {
            yield return property;
        }

        /// <summary>
        /// Gets the mapped name of the propery.
        /// </summary>
        /// <param name="property">The property where the mapped name will be retrieved.</param>
        /// <returns>A string containing the mapped name.</returns>
        public static string GetMappedName(this PropertyInfo property)
        {
            var attribute = (MapAttribute)GetCustomAttribute(property, typeof(MapAttribute));
            return attribute?.Name ??
                PropertyMapper.Get(property) ??
                property.Name;
        }

        /// <summary>
        /// Checks whether the <see cref="PropertyInfo"/> is a primary property.
        /// </summary>
        /// <param name="property">The instance of <see cref="PropertyInfo"/> to be checked.</param>
        /// <returns>A boolean value that holds a value whether the <see cref="PropertyInfo"/> is a primary property.</returns>
        public static bool IsPrimary(this PropertyInfo property)
        {
            return (property.GetCustomAttribute<PrimaryAttribute>() != null);
        }

        /// <summary>
        /// Converts a <see cref="PropertyInfo"/> into a query field object.
        /// </summary>
        /// <param name="property">The instance of <see cref="PropertyInfo"/> to be converted.</param>
        /// <param name="entity">The entity object where the value of the property will be retrieved.</param>
        /// <returns>An instance of query field object that holds the converted name and values of the property.</returns>
        public static QueryField AsQueryField(this PropertyInfo property,
            object entity)
        {
            return AsQueryField(property, entity, false);
        }

        /// <summary>
        /// Converts a <see cref="PropertyInfo"/> into a query field object.
        /// </summary>
        /// <param name="property">The instance of <see cref="PropertyInfo"/> to be converted.</param>
        /// <param name="entity">The entity object where the value of the property will be retrieved.</param>
        /// <returns>An instance of query field object that holds the converted name and values of the property.</returns>
        /// <param name="appendUnderscore">The value to identify whether the underscope prefix will be appended to the parameter name.</param>
        internal static QueryField AsQueryField(this PropertyInfo property,
            object entity,
            bool appendUnderscore)
        {
            var field = new Field(PropertyMappedNameCache.Get(property), property.PropertyType.GetUnderlyingType());
            return new QueryField(field, Operation.Equal, property.GetValue(entity), appendUnderscore);
        }

        /// <summary>
        /// Converts a <see cref="PropertyInfo"/> into a mapped name.
        /// </summary>
        /// <param name="property">The instance of the property to be converted.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>A instance of string containing the value of a mapped name.</returns>
        internal static string AsFieldAsString(this PropertyInfo property,
            IDbSetting dbSetting)
        {
            return PropertyMappedNameCache.Get(property).AsQuoted(true, dbSetting);
        }

        /// <summary>
        /// Converts a <see cref="PropertyInfo"/> into a parameterized name.
        /// </summary>
        /// <param name="property">The instance of the property to be converted.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>A instance of string containing the value of a parameterized name.</returns>
        internal static string AsParameterAsString(this PropertyInfo property,
            IDbSetting dbSetting)
        {
            return string.Concat(dbSetting.ParameterPrefix, PropertyMappedNameCache.Get(property));
        }

        /// <summary>
        /// Converts a <see cref="PropertyInfo"/> into a parameterized (as field) name.
        /// </summary>
        /// <param name="property">The instance of the property to be converted.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>A instance of string containing the value of a parameterized (as field) name.</returns>
        internal static string AsParameterAsFieldAsString(this PropertyInfo property,
            IDbSetting dbSetting)
        {
            return string.Concat(AsParameterAsString(property, dbSetting), " AS ", AsFieldAsString(property, dbSetting));
        }

        /// <summary>
        /// Converts a <see cref="PropertyInfo"/> into a field and parameter name.
        /// </summary>
        /// <param name="property">The instance of the property to be converted.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>A instance of string containing the value of a field and parameter name.</returns>
        internal static string AsFieldAndParameterAsString(this PropertyInfo property,
            IDbSetting dbSetting)
        {
            return string.Concat(AsFieldAsString(property, dbSetting), " = ", AsParameterAsString(property, dbSetting));
        }

        /// <summary>
        /// Converts a <see cref="PropertyInfo"/> into a field (and its alias) name.
        /// </summary>
        /// <param name="property">The instance of the property to be converted.</param>
        /// <param name="alias">The alias to be used.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>A instance of string containing the value of a field (and its alias) name.</returns>
        internal static string AsFieldAndAliasField(this PropertyInfo property,
            string alias,
            IDbSetting dbSetting)
        {
            return string.Concat(AsFieldAsString(property, dbSetting), " = ", alias, dbSetting.SchemaSeparator, AsFieldAsString(property, dbSetting));
        }

        /* IEnumerable<PropertyInfo> */

        /// <summary>
        /// Converts an enumerable array of <see cref="PropertyInfo"/> objects into an enumerable array of string (as field).
        /// </summary>
        /// <param name="properties">The enumerable array of properties to be converted.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>An enumerable array of strings containing the converted values of the given properties (as field).</returns>
        internal static IEnumerable<string> AsFieldsAsStrings(this IEnumerable<PropertyInfo> properties,
            IDbSetting dbSetting)
        {
            foreach (var property in properties)
            {
                yield return property.AsFieldAsString(dbSetting);
            }
        }

        /// <summary>
        /// Converts an enumerable array of <see cref="PropertyInfo"/> objects into an enumerable array of string (as parameters).
        /// </summary>
        /// <param name="properties">The enumerable array of properties to be converted.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>An enumerable array of strings containing the converted values of the given properties (as parameters).</returns>
        internal static IEnumerable<string> AsParameters(this IEnumerable<PropertyInfo> properties,
            IDbSetting dbSetting)
        {
            return properties?.Select(property => property.AsParameterAsString(dbSetting));
        }

        /// <summary>
        /// Converts an enumerable array of <see cref="PropertyInfo"/> objects into an enumerable array of string (as parameters as fields).
        /// </summary>
        /// <param name="properties">The enumerable array of properties to be converted.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>An enumerable array of strings containing the converted values of the given properties (as parameters as fields).</returns>
        internal static IEnumerable<string> AsParametersAsFields(this IEnumerable<PropertyInfo> properties,
            IDbSetting dbSetting)
        {
            return properties?.Select(property => property.AsParameterAsFieldAsString(dbSetting));
        }

        /// <summary>
        /// Converts an enumerable array of <see cref="PropertyInfo"/> objects into an enumerable array of string (as field and parameters).
        /// </summary>
        /// <param name="properties">The enumerable array of properties to be converted.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>An enumerable array of strings containing the converted values of the given properties (as field and parameters).</returns>
        internal static IEnumerable<string> AsFieldsAndParameters(this IEnumerable<PropertyInfo> properties,
            IDbSetting dbSetting)
        {
            return properties?.Select(property => property.AsFieldAndParameterAsString(dbSetting));
        }

        /// <summary>
        /// Converts an enumerable array of <see cref="PropertyInfo"/> objects into an enumerable array of string (as field and its alias).
        /// </summary>
        /// <param name="properties">The enumerable array of properties to be converted.</param>
        /// <param name="alias">The alias to be used.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>An enumerable array of strings containing the converted values of the given properties (as field and its alias).</returns>
        internal static IEnumerable<string> AsFieldsAndAliasFields(this IEnumerable<PropertyInfo> properties,
            string alias,
            IDbSetting dbSetting)
        {
            return properties?.Select(property => property.AsFieldAndAliasField(alias, dbSetting));
        }

        /// <summary>
        /// Generates a hashcode of the <see cref="PropertyInfo"/> object based on the parent class name and its own name.
        /// </summary>
        /// <param name="property">The instance of the <see cref="PropertyInfo"/> object.</param>
        /// <returns>The generated hashcode.</returns>
        internal static int GenerateCustomizedHashCode(this PropertyInfo property)
        {
            return (property.DeclaringType.FullName.GetHashCode() ^
                property.Name.GetHashCode() ^
                property.PropertyType.FullName.GetHashCode());
        }

        /// <summary>
        /// Converts an instance of <see cref="PropertyInfo"/> object into <see cref="Field"/> object.
        /// </summary>
        /// <param name="property">The instance of <see cref="PropertyInfo"/> object to be converted.</param>
        /// <returns>The converted instance of <see cref="Field"/> object.</returns>
        public static Field AsField(this PropertyInfo property)
        {
            return new Field(PropertyMappedNameCache.Get(property), property.PropertyType.GetUnderlyingType());
        }

        /// <summary>
        /// Converts an enumerable of <see cref="PropertyInfo"/> objects into an enumerable array of <see cref="Field"/>.
        /// </summary>
        /// <param name="properties">The enumerable array of properties to be converted.</param>
        /// <returns>An enumerable array of <see cref="Field"/>.</returns>
        public static IEnumerable<Field> AsFields(this IEnumerable<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                yield return property.AsField();
            }
        }

        /// <summary>
        /// Converts an array of <see cref="PropertyInfo"/> objects into an enumerable array of <see cref="Field"/>.
        /// </summary>
        /// <param name="properties">The enumerable array of properties to be converted.</param>
        /// <returns>An enumerable array of <see cref="Field"/>.</returns>
        public static IEnumerable<Field> AsFields(this PropertyInfo[] properties)
        {
            foreach (var property in properties)
            {
                yield return property.AsField();
            }
        }
    }
}
