using System.Collections.Generic;
using System;
using System.Dynamic;
using System.Linq;
using RepoDb.Enumerations;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="QueryGroup"/> object.
    /// </summary>
    public static class QueryGroupExtension
    {
        /// <summary>
        /// Convert an instance of query group into an enumerable list of query groups.
        /// </summary>
        /// <param name="queryGroup">The query group to be converted.</param>
        /// <returns>An enumerable list of query groups.</returns>
        public static IEnumerable<QueryGroup> AsEnumerable(this QueryGroup queryGroup)
        {
            return new[] { queryGroup };
        }

        // AsObject
        internal static object AsObject(this QueryGroup queryGroup, bool fixParameters = true)
        {
            var expandObject = new ExpandoObject() as IDictionary<string, object>;
            if (fixParameters)
            {
                queryGroup?.FixParameters();
            }
            queryGroup?
                .GetAllQueryFields()?
                .ToList()
                .ForEach(queryField =>
            {
                if (queryField.Operation == Operation.Between || queryField.Operation == Operation.NotBetween)
                {
                    var betweenLeftParameterName = string.Concat(queryField.Parameter.Name, "_", StringConstant.BetweenLeft);
                    var betweenRightParameterName = string.Concat(queryField.Parameter.Name, "_", StringConstant.BetweenRight);
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
                    if (!expandObject.ContainsKey(betweenLeftParameterName))
                    {
                        var leftValue = values.Count > 0 ? values[0] : null;
                        expandObject.Add(betweenLeftParameterName, leftValue);
                    }
                    if (!expandObject.ContainsKey(betweenRightParameterName))
                    {
                        var rightValue = values.Count > 1 ? values[1] : null;
                        expandObject.Add(betweenRightParameterName, rightValue);
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
                        var parameterName = string.Concat(queryField.Parameter.Name, "_", StringConstant.In, "_", i);
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
