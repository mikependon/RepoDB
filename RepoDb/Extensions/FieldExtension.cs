using System.Collections.Generic;
using System.Linq;
using RepoDb.Interfaces;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <i>RepoDb.Interfaces.IField</i> object.
    /// </summary>
    public static class FieldExtension
    {
        /// <summary>
        /// Converts an instance of a field into an enumerable list of field.
        /// </summary>
        /// <param name="field">The field to be converted.</param>
        /// <returns>An enumerable list of field.</returns>
        public static IEnumerable<IField> AsEnumerable(this IField field)
        {
            return new[] { field };
        }

        // AsField
        internal static string AsField(this IField field)
        {
            return $"[{field.Name}]";
        }

        // AsParameter
        internal static string AsParameter(this IField field)
        {
            return $"@{field.Name}";
        }

        // AsParameterAsField
        internal static string AsParameterAsField(this IField field)
        {
            return $"{AsParameter(field)} {Constant.As.ToUpper()} {AsField(field)}";
        }

        // AsFieldAndParameter
        internal static string AsFieldAndParameter(this IField field)
        {
            return $"{AsField(field)} = {AsParameter(field)}";
        }

        // AsJoinQualifier
        internal static string AsJoinQualifier(this IField field, string leftAlias, string rightAlias)
        {
            return $"{leftAlias}.[{field.Name}] = {rightAlias}.[{field.Name}]";
        }

        // AsFieldAndAliasField
        internal static string AsFieldAndAliasField(this IField field, string alias)
        {
            return $"{AsField(field)} = {alias}.{AsField(field)}";
        }

        /* IEnumerable<PropertyInfo> */

        // AsFields
        internal static IEnumerable<string> AsFields(this IEnumerable<IField> fields)
        {
            return fields?.Select(field => field.AsField());
        }

        // AsParameters
        internal static IEnumerable<string> AsParameters(this IEnumerable<IField> fields)
        {
            return fields?.Select(field => field.AsParameter());
        }

        // AsParametersAsFields
        internal static IEnumerable<string> AsParametersAsFields(this IEnumerable<IField> fields)
        {
            return fields?.Select(field => field.AsParameterAsField());
        }

        // AsFieldsAndParameters
        internal static IEnumerable<string> AsFieldsAndParameters(this IEnumerable<IField> fields)
        {
            return fields?.Select(field => field.AsFieldAndParameter());
        }

        // AsFieldsAndAliasFields
        internal static IEnumerable<string> AsFieldsAndAliasFields(this IEnumerable<IField> fields, string alias)
        {
            return fields?.Select(field => field.AsFieldAndAliasField(alias));
        }
    }
}
 
 