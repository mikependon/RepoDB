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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string AsField(this Field field,
            IDbSetting dbSetting) =>
            field.Name.AsQuoted(true, dbSetting);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="index"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string AsParameter(this Field field,
            int index,
            IDbSetting dbSetting) =>
            field.Name.AsParameter(index, dbSetting);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="alias"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string AsAliasField(this Field field,
            string alias,
            IDbSetting dbSetting) =>
            field.Name.AsAliasField(alias, dbSetting);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="index"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string AsParameterAsField(this Field field,
            int index,
            IDbSetting dbSetting) =>
            field.Name.AsParameterAsField(index, dbSetting);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="index"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string AsFieldAndParameter(this Field field,
            int index,
            IDbSetting dbSetting) =>
            field.Name.AsFieldAndParameter(index, dbSetting);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="leftAlias"></param>
        /// <param name="rightAlias"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string AsFieldAndAliasField(this Field field,
            string leftAlias,
            string rightAlias,
            IDbSetting dbSetting) =>
            field.Name.AsFieldAndAliasField(leftAlias, rightAlias, dbSetting);

        /// <summary>
        /// Creates a string representation of JOIN statement for the target <see cref="Field"/> object.
        /// </summary>
        /// <param name="field">The instance of the <see cref="Field"/> object.</param>
        /// <param name="leftAlias">The left alias.</param>
        /// <param name="rightAlias">The right alias.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The currently in used database setting.</returns>
        public static string AsJoinQualifier(this Field field,
            string leftAlias,
            string rightAlias,
            IDbSetting dbSetting) =>
            field.Name.AsJoinQualifier(leftAlias, rightAlias, dbSetting);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static IEnumerable<string> AsFields(this IEnumerable<Field> fields,
            IDbSetting dbSetting) =>
            fields?.Select(field => field.AsField(dbSetting));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="index"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static IEnumerable<string> AsParameters(this IEnumerable<Field> fields,
            int index,
            IDbSetting dbSetting) =>
            fields?.Select(field => field.AsParameter(index, dbSetting));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="alias"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static IEnumerable<string> AsAliasFields(this IEnumerable<Field> fields,
            string alias,
            IDbSetting dbSetting) =>
            fields?.Select(field => field.AsAliasField(alias, dbSetting));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="index"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static IEnumerable<string> AsParametersAsFields(this IEnumerable<Field> fields,
            int index,
            IDbSetting dbSetting) =>
            fields?.Select(field => field.AsParameterAsField(index, dbSetting));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="index"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static IEnumerable<string> AsFieldsAndParameters(this IEnumerable<Field> fields,
            int index,
            IDbSetting dbSetting) =>
            fields?.Select(field => field.AsFieldAndParameter(index, dbSetting));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="leftAlias"></param>
        /// <param name="rightAlias"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static IEnumerable<string> AsFieldsAndAliasFields(this IEnumerable<Field> fields,
            string leftAlias,
            string rightAlias,
            IDbSetting dbSetting) =>
            fields?.Select(field => field.AsFieldAndAliasField(leftAlias, rightAlias, dbSetting));
    }
}

