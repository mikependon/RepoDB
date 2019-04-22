using System.Collections.Generic;
using System.Linq;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="Field"/> object.
    /// </summary>
    public static class FieldExtension
    {
        /// <summary>
        /// Converts an instance of a <see cref="Field"/> into an <see cref="IEnumerable{T}"/> of <see cref="Field"/> object.
        /// </summary>
        /// <param name="field">The <see cref="Field"/> to be converted.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> list of <see cref="Field"/> object.</returns>
        public static IEnumerable<Field> AsEnumerable(this Field field)
        {
            yield return field;
        }

        // AsField
        internal static string AsField(this Field field)
        {
            return field.Name;
        }

        // AsParameter
        internal static string AsParameter(this Field field, string prefix = Constant.DefaultParameterPrefix)
        {
            return field.Name.AsQuotedParameter(true, prefix);
        }

        // AsAliasField
        internal static string AsAliasField(this Field field, string alias)
        {
            return string.Concat(alias, ".", field.Name);
        }

        // AsParameterAsField
        internal static string AsParameterAsField(this Field field, string prefix = Constant.DefaultParameterPrefix)
        {
            return string.Concat(AsParameter(field, prefix), " AS ", AsField(field));
        }

        // AsFieldAndParameter
        internal static string AsFieldAndParameter(this Field field, string prefix = Constant.DefaultParameterPrefix)
        {
            return string.Concat(AsField(field), " = ", AsParameter(field, prefix));
        }

        // AsJoinQualifier
        internal static string AsJoinQualifier(this Field field, string leftAlias, string rightAlias)
        {
            return string.Concat(leftAlias, ".", field.Name, " = ", rightAlias, ".", field.Name);
        }

        // AsFieldAndAliasField
        internal static string AsFieldAndAliasField(this Field field, string alias)
        {
            return string.Concat(AsField(field), " = ", alias, ".", AsField(field));
        }

        /* IEnumerable<PropertyInfo> */

        // AsFields
        internal static IEnumerable<string> AsFields(this IEnumerable<Field> fields)
        {
            return fields?.Select(field => field.AsField());
        }

        // AsParameters
        internal static IEnumerable<string> AsParameters(this IEnumerable<Field> fields, string prefix = Constant.DefaultParameterPrefix)
        {
            return fields?.Select(field => field.AsParameter(prefix));
        }

        // AsAliasFields
        internal static IEnumerable<string> AsAliasFields(this IEnumerable<Field> fields, string alias)
        {
            return fields?.Select(field => field.AsAliasField(alias));
        }

        // AsParametersAsFields
        internal static IEnumerable<string> AsParametersAsFields(this IEnumerable<Field> fields, string prefix = Constant.DefaultParameterPrefix)
        {
            return fields?.Select(field => field.AsParameterAsField(prefix));
        }

        // AsFieldsAndParameters
        internal static IEnumerable<string> AsFieldsAndParameters(this IEnumerable<Field> fields, string prefix = Constant.DefaultParameterPrefix)
        {
            return fields?.Select(field => field.AsFieldAndParameter(prefix));
        }

        // AsFieldsAndAliasFields
        internal static IEnumerable<string> AsFieldsAndAliasFields(this IEnumerable<Field> fields, string alias)
        {
            return fields?.Select(field => field.AsFieldAndAliasField(alias));
        }
    }
}

