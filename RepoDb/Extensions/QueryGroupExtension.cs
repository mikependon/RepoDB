using System.Collections.Generic;
using RepoDb.Interfaces;
using System;
using System.Dynamic;
using System.Linq;
using RepoDb.Enumerations;

namespace RepoDb.Extensions
{
    public static class QueryGroupExtension
    {
        // AsEnumerable
        public static IEnumerable<IQueryGroup> AsEnumerable(this IQueryGroup queryGroup)
        {
            return new[] { queryGroup };
        }

        // AsObject
        internal static object AsObject(this IQueryGroup queryGroup)
        {
            queryGroup?.FixParameters();
            var expandObject = new ExpandoObject() as IDictionary<string, object>;
            var queryFields = queryGroup?.GetAllQueryFields();
            if (queryFields!=null && queryFields.Any())
            {
                queryFields.ToList().ForEach(queryField =>
                {
                    if (queryField.Operation == Operation.Between || queryField.Operation == Operation.NotBetween)
                    {
                        var leftParameterName = $"{queryField.Parameter.Name}_{Constant.LeftValue}";
                        var rightParameterName = $"{queryField.Parameter.Name}_{Constant.RightValue}";
                        var values = (queryField.Parameter.Value as Array)?.ToObjects();
                        if (!expandObject.ContainsKey(leftParameterName))
                        {
                            var leftValue = values.Length > 0 ? values[0] : null;
                            expandObject.Add(leftParameterName, leftValue);
                        }
                        if (!expandObject.ContainsKey(rightParameterName))
                        {
                            var rightValue = values.Length > 1 ? values[1] : null;
                            expandObject.Add(rightParameterName, rightValue);
                        }
                    }
                    else
                    {
                        if (!expandObject.ContainsKey(queryField.Parameter.Name))
                        {
                            expandObject.Add(queryField.Parameter.Name, queryField.Parameter.Value);
                        }
                    }
                });
            }
            return (ExpandoObject)expandObject;
        }
    }
}
 