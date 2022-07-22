using System.Collections.Generic;
using System.Linq;
using RepoDb.Interfaces;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="QueryField"/> object.
    /// </summary>
    public static class QueryFieldExtension
    {
        /// <summary>
        /// Converts an instance of a query field into an enumerable list of query fields.
        /// </summary>
        /// <param name="queryField">The query field to be converted.</param>
        /// <returns>An enumerable list of query fields.</returns>
        public static IEnumerable<QueryField> AsEnumerable(this QueryField queryField)
        {
            yield return queryField;
        }

        /// <summary>
        /// Resets all the instances of <see cref="QueryField"/>.
        /// </summary>
        /// <param name="queryFields">The list of <see cref="QueryField"/> objects.</param>
        public static void ResetAll(this IEnumerable<QueryField> queryFields)
        {
            foreach (var queryField in queryFields)
            {
                queryField.Reset();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal static void PrependAnUnderscoreAtParameter(this QueryField queryField)
        {
            queryField.Parameter?.PrependAnUnderscore();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryField"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static string AsField(this QueryField queryField,
            IDbSetting dbSetting) =>
            AsField(queryField, null, dbSetting);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryField"></param>
        /// <param name="functionFormat"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static string AsField(this QueryField queryField,
            string functionFormat,
            IDbSetting dbSetting) =>
            queryField.Field.Name.AsField(functionFormat, dbSetting);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryField"></param>
        /// <param name="index"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static string AsParameter(this QueryField queryField,
            int index,
            IDbSetting dbSetting) =>
            queryField.Parameter.Name.AsParameter(index, dbSetting);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryField"></param>
        /// <param name="index"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static string AsParameterAsField(this QueryField queryField,
            int index,
            IDbSetting dbSetting) =>
            string.Concat(queryField.AsParameter(index, dbSetting), " AS ", queryField.AsField(dbSetting));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryField"></param>
        /// <param name="index"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static string AsBetweenParameter(this QueryField queryField,
            int index,
            IDbSetting dbSetting) =>
            AsBetweenParameter(queryField, index, null, dbSetting);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryField"></param>
        /// <param name="index"></param>
        /// <param name="functionFormat"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static string AsBetweenParameter(this QueryField queryField,
            int index,
            string functionFormat,
            IDbSetting dbSetting) =>
            string.IsNullOrWhiteSpace(functionFormat) ?
                string.Concat(queryField.Parameter.Name.AsParameter(index, dbSetting), "_Left AND ", queryField.Parameter.Name.AsParameter(index, dbSetting), "_Right") :
                string.Concat(
                    string.Format(functionFormat, string.Concat(queryField.Parameter.Name.AsParameter(index, dbSetting), "_Left")),
                    " AND ",
                    string.Format(functionFormat, string.Concat(queryField.Parameter.Name.AsParameter(index, dbSetting), "_Right")));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryField"></param>
        /// <param name="index"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static string AsInParameter(this QueryField queryField,
            int index,
            IDbSetting dbSetting) =>
            AsInParameter(queryField, index, null, dbSetting);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryField"></param>
        /// <param name="index"></param>
        /// <param name="functionFormat"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static string AsInParameter(this QueryField queryField,
            int index,
            string functionFormat,
            IDbSetting dbSetting)
        {
            var enumerable = (System.Collections.IEnumerable)queryField.Parameter.Value;
            var values = enumerable
                .OfType<object>()
                .Select((_, valueIndex) =>
                    string.IsNullOrWhiteSpace(functionFormat) ?
                        string.Concat(queryField.Parameter.Name.AsParameter(index, dbSetting), "_In_", valueIndex.ToString()) :
                        string.Format(functionFormat, string.Concat(queryField.Parameter.Name.AsParameter(index, dbSetting), "_In_", valueIndex.ToString())))
                .Join(", ");
            return string.Concat("(", values, ")");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryField"></param>
        /// <param name="index"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static string AsFieldAndParameterForBetween(this QueryField queryField,
            int index,
            IDbSetting dbSetting) =>
            AsFieldAndParameterForBetween(queryField, index, dbSetting);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryField"></param>
        /// <param name="index"></param>
        /// <param name="functionFormat"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static string AsFieldAndParameterForBetween(this QueryField queryField,
            int index,
            string functionFormat,
            IDbSetting dbSetting) =>
            string.Concat(queryField.AsField(functionFormat, dbSetting), " ", queryField.Operation.GetText(), " ", queryField.AsBetweenParameter(index /*, functionFormat */, dbSetting));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryField"></param>
        /// <param name="index"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static string AsFieldAndParameterForIn(this QueryField queryField,
            int index,
            IDbSetting dbSetting) =>
            AsFieldAndParameterForIn(queryField, index, null, dbSetting);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryField"></param>
        /// <param name="index"></param>
        /// <param name="functionFormat"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static string AsFieldAndParameterForIn(this QueryField queryField,
            int index,
            string functionFormat,
            IDbSetting dbSetting)
        {
            var enumerable = (queryField.Parameter.Value as System.Collections.IEnumerable)?.WithType<object>();
            if (enumerable?.Any() != true)
            {
                return "1 = 0";
            }
            else
            {
                return string.Concat(queryField.AsField(functionFormat, dbSetting), " ", queryField.Operation.GetText(), " ", queryField.AsInParameter(index /*, functionFormat*/, dbSetting));
            }
        }
    }
}
