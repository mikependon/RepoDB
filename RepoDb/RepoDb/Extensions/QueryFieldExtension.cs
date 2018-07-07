using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using RepoDb.Enumerations;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <i>RepoDb.QueryField</i> object.
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
            return new[] { queryField };
        }

        // AsField
        internal static string AsField(this QueryField queryField)
        {
            return queryField.Field.Name.AsQuoted(true);;
        }

        // AsParameter
        internal static string AsParameter(this QueryField queryField)
        {
            return $"@{queryField.Parameter.Name}";
        }

        // AsParameterAsField
        internal static string AsParameterAsField(this QueryField queryField)
        {
            return $"@{queryField.Parameter.Name} AS {queryField.Field.Name.AsQuoted(true)}";
        }

        // AsBetweenParameter
        internal static string AsBetweenParameter(this QueryField queryField)
        {
            return $"@{queryField.Parameter.Name}_{StringConstant.BetweenLeft} {StringConstant.And.ToUpper()} @{queryField.Parameter.Name}_{StringConstant.BetweenRight}";
        }

        // AsInParameter
        internal static string AsInParameter(this QueryField queryField)
        {
            var values = ((Array)queryField.Parameter.Value)?.OfType<object>().ToList();
            var parameters = new List<string>();
            for (var i = 0; i < values.Count; i++)
            {
                parameters.Add($"@{queryField.Parameter.Name}_{StringConstant.In}_{i}");
            }
            return $"({parameters.Join(", ")})";
        }

        // AsFieldAndParameter
        internal static string AsFieldAndParameter(this QueryField queryField)
        {
            if (queryField.Operation == Operation.Equal && queryField.Parameter.Value == null)
            {
                return $"{queryField.AsField()} IS NULL";
            }
            else if (queryField.Operation == Operation.NotEqual && queryField.Parameter.Value == null)
            {
                return $"{queryField.AsField()} IS NOT NULL";
            }
            else
            {
                var operationText = queryField.GetOperationText();
                if (queryField.Operation == Operation.Between || queryField.Operation == Operation.NotBetween)
                {
                    return $"{queryField.AsField()} {operationText} {queryField.AsBetweenParameter()}";
                }
                else if (queryField.Operation == Operation.In || queryField.Operation == Operation.NotIn)
                {
                    return $"{queryField.AsField()} {operationText} {queryField.AsInParameter()}";
                }
                else
                {
                    return $"{queryField.AsField()} {operationText} {queryField.AsParameter()}";
                }
            }
        }

        // AsDbParameter
        internal static IDbDataParameter AsDbParameter(this QueryField queryField, IDbCommand command)
        {
            return AsDbParameter(queryField, command.CreateParameter());
        }

        internal static IDbDataParameter AsDbParameter(this QueryField queryField, IDbDataParameter parameter)
        {
            parameter.ParameterName = queryField.Field.Name;
            parameter.Value = queryField.Parameter.Value ?? DBNull.Value;
            return parameter;
        }

        /* IEnumerable<QueryField> */

        // AsFieldsAndParameters
        internal static IEnumerable<string> AsFieldsAndParameters(this IEnumerable<QueryField> queryFields)
        {
            return queryFields.Select(field => field.AsFieldAndParameter());
        }

        // Has
        internal static bool Has(this IEnumerable<QueryField> queryFields, string name, StringComparison comparisonType)
        {
            return queryFields.FirstOrDefault(queryField => string.Equals(queryField.Field.Name, name, comparisonType)) != null;
        }

        internal static bool Has(this IEnumerable<QueryField> queryFields, string name)
        {
            return Has(queryFields, name, StringComparison.CurrentCultureIgnoreCase);
        }

        // AsObject
        internal static object AsObject(this IEnumerable<QueryField> queryFields)
        {
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;
            queryFields.ToList().ForEach(queryField =>
            {
                expandoObject.Add(queryField.Field.Name, queryField.Parameter.Value);
            });
            return expandoObject;
        }

    }
}
 