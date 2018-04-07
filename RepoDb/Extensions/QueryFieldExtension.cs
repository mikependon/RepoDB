using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using RepoDb.Attributes;
using RepoDb.Interfaces;
using RepoDb.Enumerations;

namespace RepoDb.Extensions
{
    public static class QueryFieldExtension
    {
        // AsEnumerable
        public static IEnumerable<IQueryField> AsEnumerable(this IQueryField queryField)
        {
            return new[] { queryField };
        }

        // AsField
        internal static string AsField(this IQueryField queryField)
        {
            return $"[{queryField.Field.Name}]";
        }

        // AsParameter
        internal static string AsParameter(this IQueryField queryField)
        {
            return $"@{queryField.Parameter.Name}";
        }

        // AsParameterAsField
        internal static string AsParameterAsField(this IQueryField queryField)
        {
            return $"@{queryField.Parameter.Name} AS [{queryField.Field.Name}]";
        }

        // AsBetweenParameter
        internal static string AsBetweenParameter(this IQueryField queryField)
        {
            return $"@{queryField.Parameter.Name}_{Constant.LeftValue} {Constant.And.ToUpper()} @{queryField.Parameter.Name}_{Constant.RightValue}";
        }

        // AsInParameter
        internal static string AsInParameter(this IQueryField queryField)
        {
            var values = ((Array)queryField.Parameter.Value)?.AsEnumerable().ToList();
            var parameters = new List<string>();
            for (var i = 0; i < values.Count; i++)
            {
                parameters.Add($"@{queryField.Parameter.Name}_{Constant.In}_{i}");
            }
            return $"({parameters.Join(", ")})";
        }

        // AsFieldAndParameter
        internal static string AsFieldAndParameter(this IQueryField queryField)
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
                var textAttribute = queryField.Operation
                    .GetType()
                    .GetMembers()
                    .First(member => string.Equals(member.Name, queryField.Operation.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    .GetCustomAttribute<TextAttribute>();
                if (queryField.Operation == Operation.Between || queryField.Operation == Operation.NotBetween)
                {
                    return $"{queryField.AsField()} {textAttribute.Text} {queryField.AsBetweenParameter()}";
                }
                else if (queryField.Operation == Operation.In || queryField.Operation == Operation.NotIn)
                {
                    return $"{queryField.AsField()} {textAttribute.Text} {queryField.AsInParameter()}";
                }
                else
                {
                    return $"{queryField.AsField()} {textAttribute.Text} {queryField.AsParameter()}";
                }
            }
        }

        // AsDbParameter
        internal static IDbDataParameter AsDbParameter(this IQueryField queryField, IDbCommand command)
        {
            return AsDbParameter(queryField, command.CreateParameter());
        }

        internal static IDbDataParameter AsDbParameter(this IQueryField queryField, IDbDataParameter parameter)
        {
            parameter.ParameterName = queryField.Field.Name;
            parameter.Value = queryField.Parameter.Value ?? DBNull.Value;
            return parameter;
        }

        /* IEnumerable<IQueryField> */

        // AsFieldsAndParameters
        internal static IEnumerable<string> AsFieldsAndParameters(this IEnumerable<IQueryField> queryFields)
        {
            return queryFields.Select(field => field.AsFieldAndParameter());
        }

        // Has
        internal static bool Has(this IEnumerable<IQueryField> queryFields, string name, StringComparison comparisonType)
        {
            return queryFields.FirstOrDefault(queryField => string.Equals(queryField.Field.Name, name, comparisonType)) != null;
        }

        internal static bool Has(this IEnumerable<IQueryField> queryFields, string name)
        {
            return Has(queryFields, name, StringComparison.InvariantCultureIgnoreCase);
        }

        // AsObject
        internal static object AsObject(this IEnumerable<IQueryField> queryFields)
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
 