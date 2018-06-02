using System.Collections.Generic;
using RepoDb.Interfaces;
using System;
using System.Dynamic;
using System.Linq;
using RepoDb.Enumerations;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <i>RepoDb.Interfaces.IQueryGroup</i> object.
    /// </summary>
    public static class QueryGroupExtension
    {
        /// <summary>
        /// Convert an instance of query group into an enumerable list of query groups.
        /// </summary>
        /// <param name="queryGroup">The query group to be converted.</param>
        /// <returns>An enumerable list of query groups.</returns>
        public static IEnumerable<IQueryGroup> AsEnumerable(this IQueryGroup queryGroup)
        {
            return new[] { queryGroup };
        }

        // AsObject
        internal static object AsObject(this IQueryGroup queryGroup)
        {
            var expandObject = new ExpandoObject() as IDictionary<string, object>;
            queryGroup?
                .Fix()
                .GetAllQueryFields()?
                .ToList()
                .ForEach(queryField =>
            {
                if (queryField.Operation == Operation.Between || queryField.Operation == Operation.NotBetween)
                {
                    var leftParameterName = $"{queryField.Parameter.Name}_{Constant.LeftValue}";
                    var rightParameterName = $"{queryField.Parameter.Name}_{Constant.RightValue}";
                    var values = new List<object>();
                    if (queryField.Parameter.Value != null)
                    {
                        if (queryField.Parameter.Value is Array)
                        {
                            values.AddRange(((Array)queryField.Parameter.Value).AsEnumerable());
                        }
                        else
                        {
                            values.Add(queryField.Parameter.Value);
                        }
                    }
                    if (!expandObject.ContainsKey(leftParameterName))
                    {
                        var leftValue = values.Count > 0 ? values[0] : null;
                        expandObject.Add(leftParameterName, leftValue);
                    }
                    if (!expandObject.ContainsKey(rightParameterName))
                    {
                        var rightValue = values.Count > 1 ? values[1] : null;
                        expandObject.Add(rightParameterName, rightValue);
                    }
                }
                else if (queryField.Operation == Operation.In || queryField.Operation == Operation.NotIn)
                {
                    var values = new List<object>();
                    if (queryField.Parameter.Value != null)
                    {
                        if (queryField.Parameter.Value is Array)
                        {
                            values.AddRange(((Array)queryField.Parameter.Value).AsEnumerable());
                        }
                        else
                        {
                            values.Add(queryField.Parameter.Value);
                        }
                    }
                    for (var i = 0; i < values.Count; i++)
                    {
                        var parameterName = $"{queryField.Parameter.Name}_{Constant.In}_{i}";
                        if (!expandObject.ContainsKey(parameterName))
                        {
                            expandObject.Add(parameterName, values[i]);
                        }
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
            return (ExpandoObject)expandObject;
        }
    }
}
