using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Interfaces;

namespace RepoDb.Extensions
{
    public static class PropertyInfoExtension
    {
        // AsEnumerable
        public static IEnumerable<PropertyInfo> AsEnumerable(this PropertyInfo property)
        {
            return property != null ? new[] { property } : null;
        }

        // GetMappedName
        public static string GetMappedName(this PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute(typeof(MapAttribute)) as MapAttribute;
            return (attribute?.Name ?? property.Name);
        }

        // IsIgnored
        public static bool IsIgnored(this PropertyInfo property, Command command)
        {
            var ignore = property.GetCustomAttribute<IgnoreAttribute>();
            return (ignore != null && (ignore.Command & command) == command && ignore.Command != Command.None);
        }

        // IsPrimary
        public static bool IsPrimary(this PropertyInfo property)
        {
            return (property.GetCustomAttribute<PrimaryAttribute>() != null);
        }

        // IsIdentity
        public static bool IsIdentity(this PropertyInfo property)
        {
            var primary = property.GetCustomAttribute<PrimaryAttribute>();
            return (primary?.IsIdentity).HasValue;
        }

        // AsQueryField
        public static IQueryField AsQueryField(this PropertyInfo property, object entity)
        {
            return new QueryField(property.GetMappedName(), property.GetValue(entity));
        }

        // AsDataColumn
        public static DataColumn AsDataColumn(this PropertyInfo property)
        {
            return new DataColumn(property.GetMappedName(), property.PropertyType);
        }

        // AsJoinQualifier
        public static string AsJoinQualifier(this PropertyInfo property, string leftAlias, string rightAlias)
        {
            return $"{leftAlias}.[{property.GetMappedName()}] = {rightAlias}.[{property.GetMappedName()}]";
        }

        // AsField
        public static string AsField(this PropertyInfo property)
        {
            return $"[{property.GetMappedName()}]";
        }

        // AsParameter
        public static string AsParameter(this PropertyInfo property)
        {
            return $"@{property.GetMappedName()}";
        }

        // AsParameterAsField
        public static string AsParameterAsField(this PropertyInfo property)
        {
            return $"{AsParameter(property)} {Constant.As.ToUpper()} {AsField(property)}";
        }

        // AsFieldAndParameter
        public static string AsFieldAndParameter(this PropertyInfo property)
        {
            return $"{AsField(property)} = {AsParameter(property)}";
        }

        // AsFieldAndAliasField
        public static string AsFieldAndAliasField(this PropertyInfo property, string alias)
        {
            return $"{AsField(property)} = {alias}.{AsField(property)}";
        }

        /* IEnumerable<PropertyInfo> */

        // AsFields
        public static IEnumerable<string> AsFields(this IEnumerable<PropertyInfo> properties)
        {
            return properties?.Select(property => property.AsField());
        }

        // AsParameters
        public static IEnumerable<string> AsParameters(this IEnumerable<PropertyInfo> properties)
        {
            return properties?.Select(property => property.AsParameter());
        }

        // AsParametersAsFields
        public static IEnumerable<string> AsParametersAsFields(this IEnumerable<PropertyInfo> properties)
        {
            return properties?.Select(property => property.AsParameterAsField());
        }

        // AsFieldsAndParameters
        public static IEnumerable<string> AsFieldsAndParameters(this IEnumerable<PropertyInfo> properties)
        {
            return properties?.Select(property => property.AsFieldAndParameter());
        }

        // AsFieldsAndAliasFields
        public static IEnumerable<string> AsFieldsAndAliasFields(this IEnumerable<PropertyInfo> properties, string alias)
        {
            return properties?.Select(property => property.AsFieldAndAliasField(alias));
        }
    }
}
