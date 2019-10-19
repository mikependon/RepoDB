using RepoDb.Interfaces;
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
        private static string AsField(this Field field,
            IDbSetting dbSetting)
        {
            return field.Name.AsQuoted(true, dbSetting);
        }

        // AsParameter
        private static string AsParameter(this Field field,
            int index,
            IDbSetting dbSetting)
        {
            return field.Name.AsParameter(index, dbSetting);
        }

        // AsAliasField
        private static string AsAliasField(this Field field,
            string alias,
            IDbSetting dbSetting)
        {
            return field.Name.AsAliasField(alias, dbSetting);
        }

        // AsParameterAsField
        private static string AsParameterAsField(this Field field,
            int index,
            IDbSetting dbSetting)
        {
            return field.Name.AsParameterAsField(index, dbSetting);
        }

        // AsFieldAndParameter
        private static string AsFieldAndParameter(this Field field,
            int index,
            IDbSetting dbSetting)
        {
            return field.Name.AsFieldAndParameter(index, dbSetting);
        }

        // AsFieldAndAliasField
        private static string AsFieldAndAliasField(this Field field,
            string alias,
            IDbSetting dbSetting)
        {
            return field.Name.AsFieldAndAliasField(alias, dbSetting);
        }

        // AsJoinQualifier
        internal static string AsJoinQualifier(this Field field,
            string leftAlias,
            string rightAlias,
            IDbSetting dbSetting)
        {
            return field.Name.AsJoinQualifier(leftAlias, rightAlias, dbSetting);
        }

        /* IEnumerable<PropertyInfo> */

        // AsFields
        internal static IEnumerable<string> AsFields(this IEnumerable<Field> fields,
            IDbSetting dbSetting)
        {
            return fields?.Select(field => field.AsField(dbSetting));
        }

        // AsParameters
        internal static IEnumerable<string> AsParameters(this IEnumerable<Field> fields,
            int index,
            IDbSetting dbSetting)
        {
            return fields?.Select(field => field.AsParameter(index, dbSetting));
        }

        // AsAliasFields
        internal static IEnumerable<string> AsAliasFields(this IEnumerable<Field> fields,
            string alias,
            IDbSetting dbSetting)
        {
            return fields?.Select(field => field.AsAliasField(alias, dbSetting));
        }

        // AsParametersAsFields
        internal static IEnumerable<string> AsParametersAsFields(this IEnumerable<Field> fields,
            int index,
            IDbSetting dbSetting)
        {
            return fields?.Select(field => field.AsParameterAsField(index, dbSetting));
        }

        // AsFieldsAndParameters
        internal static IEnumerable<string> AsFieldsAndParameters(this IEnumerable<Field> fields,
            int index,
            IDbSetting dbSetting)
        {
            return fields?.Select(field => field.AsFieldAndParameter(index, dbSetting));
        }

        // AsFieldsAndAliasFields
        internal static IEnumerable<string> AsFieldsAndAliasFields(this IEnumerable<Field> fields,
            string alias,
            IDbSetting dbSetting)
        {
            return fields?.Select(field => field.AsFieldAndAliasField(alias, dbSetting));
        }
    }
}

