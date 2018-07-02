using System.Collections.Generic;
using System.Linq;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <i>RepoDb.Field</i> object.
    /// </summary>
    public static class FieldExtension
    {
        /// <summary>
        /// Converts an instance of a field into an enumerable list of field.
        /// </summary>
        /// <param name="field">The field to be converted.</param>
        /// <returns>An enumerable list of field.</returns>
        public static IEnumerable<Field> AsEnumerable(this Field field)
        {
            return new[] { field };
        }

        // AsField
        internal static string AsField(this Field field)
        {
            return field.Name.AsQuoted(true);
        }

        // AsParameter
        internal static string AsParameter(this Field field)
        {
            return $"@{field.Name.AsQuotedParameter(true)}";
        }

        // AsParameterAsField
        internal static string AsParameterAsField(this Field field)
        {
            return $"{AsParameter(field)} {StringConstant.As.ToUpper()} {AsField(field)}";
        }

        // AsFieldAndParameter
        internal static string AsFieldAndParameter(this Field field)
        {
            return $"{AsField(field)} = {AsParameter(field)}";
        }

        // AsJoinQualifier
        internal static string AsJoinQualifier(this Field field, string leftAlias, string rightAlias)
        {
            return $"{leftAlias}.{field.Name.AsQuoted(true)} = {rightAlias}.{field.Name.AsQuoted(true)}";
        }

        // AsFieldAndAliasField
        internal static string AsFieldAndAliasField(this Field field, string alias)
        {
            return $"{AsField(field)} = {alias}.{AsField(field)}";
        }

        /* IEnumerable<PropertyInfo> */

        // AsFields
        internal static IEnumerable<string> AsFields(this IEnumerable<Field> fields)
        {
            return fields?.Select(field => field.AsField());
        }

        // AsParameters
        internal static IEnumerable<string> AsParameters(this IEnumerable<Field> fields)
        {
            return fields?.Select(field => field.AsParameter());
        }

        // AsParametersAsFields
        internal static IEnumerable<string> AsParametersAsFields(this IEnumerable<Field> fields)
        {
            return fields?.Select(field => field.AsParameterAsField());
        }

        // AsFieldsAndParameters
        internal static IEnumerable<string> AsFieldsAndParameters(this IEnumerable<Field> fields)
        {
            return fields?.Select(field => field.AsFieldAndParameter());
        }

        // AsFieldsAndAliasFields
        internal static IEnumerable<string> AsFieldsAndAliasFields(this IEnumerable<Field> fields, string alias)
        {
            return fields?.Select(field => field.AsFieldAndAliasField(alias));
        }
    }
}

