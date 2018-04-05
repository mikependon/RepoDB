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
        // ToEnumerable
        internal static IEnumerable<PropertyInfo> ToEnumerable(this PropertyInfo property)
        {
            return property != null ? new[] { property } : null;
        }

        // IsIgnored
        internal static bool IsIgnored(this PropertyInfo property, Command command)
        {
            var ignore = property.GetCustomAttribute<IgnoreAttribute>();
            return (ignore != null && (ignore.Command & command) == command && ignore.Command != Command.None);
        }

        // IsPrimary
        internal static bool IsPrimary(this PropertyInfo property)
        {
            return (property.GetCustomAttribute<PrimaryAttribute>() != null);
        }

        // IsIdentity
        internal static bool IsIdentity(this PropertyInfo property)
        {
            var primary = property.GetCustomAttribute<PrimaryAttribute>();
            return (primary?.IsIdentity).HasValue;
        }

        // AsQueryField
        internal static IQueryField AsQueryField(this PropertyInfo property, object entity)
        {
            return new QueryField(property.Name, property.GetValue(entity));
        }

        // AsDataColumn
        internal static DataColumn AsDataColumn(this PropertyInfo property)
        {
            return new DataColumn(property.Name, property.PropertyType);
        }

        // AsJoinQualifier
        internal static string AsJoinQualifier(this PropertyInfo property, string leftAlias, string rightAlias)
        {
            return $"{leftAlias}.[{property.Name}] = {rightAlias}.[{property.Name}]";
        }

        // AsField
        internal static string AsField(this PropertyInfo property)
        {
            return $"[{property.Name}]";
        }

        // AsParameter
        internal static string AsParameter(this PropertyInfo property)
        {
            return $"@{property.Name}";
        }

        // AsParameterAsField
        internal static string AsParameterAsField(this PropertyInfo property)
        {
            return $"@{property.Name} AS [{property.Name}]";
        }

        // AsFieldAndParameter
        internal static string AsFieldAndParameter(this PropertyInfo property)
        {
            return $"[{property.Name}] = @{property.Name}";
        }

        /* IEnumerable<PropertyInfo> */

        // AsFields
        internal static IEnumerable<string> AsFields(this IEnumerable<PropertyInfo> properties)
        {
            return properties.Select(property => property.AsField());
        }

        // AsParameters
        internal static IEnumerable<string> AsParameters(this IEnumerable<PropertyInfo> properties)
        {
            return properties.Select(property => property.AsParameter());
        }

        // AsParametersAsFields
        internal static IEnumerable<string> AsParametersAsFields(this IEnumerable<PropertyInfo> properties)
        {
            return properties.Select(property => property.AsParameterAsField());
        }

        // AsFieldsAndParameters
        internal static IEnumerable<string> AsFieldsAndParameters(this IEnumerable<PropertyInfo> properties)
        {
            return properties.Select(property => property.AsFieldAndParameter());
        }
    }
}
