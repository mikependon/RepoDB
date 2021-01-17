using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using RepoDb.Interfaces;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="PropertyInfo"/> object.
    /// </summary>
    public static class PropertyInfoExtension
    {
        /// <summary>
        /// Gets a custom attribute defined on the property.
        /// </summary>
        /// <typeparam name="T">The custom attribute that is defined into the property.</typeparam>
        /// <param name="property">The type of where the custom attribute is defined.</param>
        /// <returns>The custom attribute.</returns>
        public static T GetCustomAttribute<T>(this PropertyInfo property)
            where T : Attribute =>
            (T)GetCustomAttribute(property, typeof(T));

        /// <summary>
        /// Gets a custom attribute defined on the property.
        /// </summary>
        /// <param name="property">The type of where the custom attribute is defined.</param>
        /// <param name="type">The custom attribute that is defined into the property.</param>
        /// <returns>The custom attribute.</returns>
        public static Attribute GetCustomAttribute(this PropertyInfo property,
            Type type)
        {
            var attributes = property.GetCustomAttributes(type, false).WithType<Attribute>();
            return attributes?.FirstOrDefault(a => a.GetType() == type);
        }

        /// <summary>
        /// Gets the mapped name of the property.
        /// </summary>
        /// <param name="property">The property where the mapped name will be retrieved.</param>
        /// <returns>A string containing the mapped name.</returns>
        public static string GetMappedName(this PropertyInfo property) =>
            GetMappedName(property, property.DeclaringType);

        /// <summary>
        /// Gets the mapped name of the property.
        /// </summary>
        /// <param name="property">The property where the mapped name will be retrieved.</param>
        /// <param name="declaringType">The declaring type of the property.</param>
        /// <returns>A string containing the mapped name.</returns>
        internal static string GetMappedName(this PropertyInfo property,
            Type declaringType)
        {
            var attributeName = ((MapAttribute)GetCustomAttribute(property, StaticType.MapAttribute))?.Name ??
                ((ColumnAttribute)GetCustomAttribute(property, StaticType.ColumnAttribute))?.Name;
            return attributeName ??
                PropertyMapper.Get(declaringType, property) ??
                property.Name;
        }

        /// <summary>
        /// Converts a <see cref="PropertyInfo"/> into a query field object.
        /// </summary>
        /// <param name="property">The instance of <see cref="PropertyInfo"/> to be converted.</param>
        /// <param name="entity">The entity object where the value of the property will be retrieved.</param>
        /// <returns>An instance of query field object that holds the converted name and values of the property.</returns>
        public static QueryField AsQueryField(this PropertyInfo property,
            object entity) =>
            AsQueryField(property, entity, false);

        /// <summary>
        /// Converts a <see cref="PropertyInfo"/> into a query field object.
        /// </summary>
        /// <param name="property">The instance of <see cref="PropertyInfo"/> to be converted.</param>
        /// <param name="entity">The entity object where the value of the property will be retrieved.</param>
        /// <returns>An instance of query field object that holds the converted name and values of the property.</returns>
        /// <param name="appendUnderscore">The value to identify whether the underscore prefix will be appended to the parameter name.</param>
        internal static QueryField AsQueryField(this PropertyInfo property,
            object entity,
            bool appendUnderscore) =>
            new QueryField(property.AsField(), Operation.Equal, property.GetHandledValue(entity), appendUnderscore);

        /// <summary>
        /// Converts a <see cref="PropertyInfo"/> into a mapped name.
        /// </summary>
        /// <param name="property">The instance of the property to be converted.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>A instance of string containing the value of a mapped name.</returns>
        internal static string AsFieldAsString(this PropertyInfo property,
            IDbSetting dbSetting) =>
            PropertyMappedNameCache.Get(property).AsQuoted(true, dbSetting);

        /// <summary>
        /// Converts a <see cref="PropertyInfo"/> into a parameterized name.
        /// </summary>
        /// <param name="property">The instance of the property to be converted.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>A instance of string containing the value of a parameterized name.</returns>
        internal static string AsParameterAsString(this PropertyInfo property,
            IDbSetting dbSetting) =>
            string.Concat(dbSetting.ParameterPrefix, PropertyMappedNameCache.Get(property));

        /// <summary>
        /// Converts a <see cref="PropertyInfo"/> into a parameterized (as field) name.
        /// </summary>
        /// <param name="property">The instance of the property to be converted.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>A instance of string containing the value of a parameterized (as field) name.</returns>
        internal static string AsParameterAsFieldAsString(this PropertyInfo property,
            IDbSetting dbSetting) =>
            string.Concat(AsParameterAsString(property, dbSetting), " AS ", AsFieldAsString(property, dbSetting));

        /// <summary>
        /// Converts a <see cref="PropertyInfo"/> into a field and parameter name.
        /// </summary>
        /// <param name="property">The instance of the property to be converted.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>A instance of string containing the value of a field and parameter name.</returns>
        internal static string AsFieldAndParameterAsString(this PropertyInfo property,
            IDbSetting dbSetting) =>
            string.Concat(AsFieldAsString(property, dbSetting), " = ", AsParameterAsString(property, dbSetting));

        /// <summary>
        /// Converts a <see cref="PropertyInfo"/> into a field (and its alias) name.
        /// </summary>
        /// <param name="property">The instance of the property to be converted.</param>
        /// <param name="alias">The alias to be used.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>A instance of string containing the value of a field (and its alias) name.</returns>
        internal static string AsFieldAndAliasField(this PropertyInfo property,
            string alias,
            IDbSetting dbSetting) =>
            string.Concat(AsFieldAsString(property, dbSetting), " = ", alias, StringConstant.Period, AsFieldAsString(property, dbSetting));

        /// <summary>
        /// Generates a hashcode of the <see cref="PropertyInfo"/> object based on the parent class name and its own name.
        /// </summary>
        /// <param name="property">The instance of the <see cref="PropertyInfo"/> object.</param>
        /// <param name="declaringType">The declaring type of the <see cref="PropertyInfo"/> object. This refers to the derived class if present.</param>
        /// <returns>The generated hashcode.</returns>
        internal static int GenerateCustomizedHashCode(this PropertyInfo property,
            Type declaringType) =>
            (declaringType ?? property.DeclaringType).GetHashCode() ^ property.Name.GetHashCode() ^ property.PropertyType.GetHashCode();

        /// <summary>
        /// Converts an instance of <see cref="PropertyInfo"/> object into <see cref="Field"/> object.
        /// </summary>
        /// <param name="property">The instance of <see cref="PropertyInfo"/> object to be converted.</param>
        /// <returns>The converted instance of <see cref="Field"/> object.</returns>
        public static Field AsField(this PropertyInfo property) =>
            new Field(PropertyMappedNameCache.Get(property), property.PropertyType.GetUnderlyingType());

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
        public static IEnumerable<Field> AsFields(this PropertyInfo[] properties) =>
            AsFields(properties.AsEnumerable<PropertyInfo>());

        /// <summary>
        /// Returns the value of the data entity property. If the property handler is defined in the property, then the
        /// handled value will be returned.
        /// </summary>
        /// <param name="property">The target <see cref="PropertyInfo"/> object.</param>
        /// <param name="entity">The instance of the data entity object.</param>
        /// <returns>The handled value of the data entity property.</returns>
        public static object GetHandledValue(this PropertyInfo property,
            object entity) =>
            GetHandledValue(property, entity, property.DeclaringType);

        /// <summary>
        /// Returns the value of the data entity property. If the property handler is defined in the property, then the
        /// handled value will be returned.
        /// </summary>
        /// <param name="property">The target <see cref="PropertyInfo"/> object.</param>
        /// <param name="entity">The instance of the data entity object.</param>
        /// <param name="declaringType">The customized declaring type of the <see cref="PropertyInfo"/> object.</param>
        /// <returns>The handled value of the data entity property.</returns>
        public static object GetHandledValue(this PropertyInfo property,
            object entity,
            Type declaringType)
        {
            var classProperty = PropertyCache.Get((declaringType ?? property?.DeclaringType), property);
            var propertyHandler = classProperty?.GetPropertyHandler();
            var value = property?.GetValue(entity);
            if (propertyHandler != null)
            {
                var setMethod = propertyHandler.GetType().GetMethod("Set");
                return setMethod.Invoke(propertyHandler, new[] { value, classProperty });
            }
            return value;
        }
    }
}
