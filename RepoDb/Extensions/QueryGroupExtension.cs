using System.Collections.Generic;
using RepoDb.Interfaces;
using System;
using System.Dynamic;
using System.Linq;

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
                    if (!expandObject.ContainsKey(queryField.Parameter.Name))
                    {
                        expandObject.Add(queryField.Parameter.Name, queryField.Parameter.Value);
                    }
                });
            }
            return (ExpandoObject)expandObject;
        }
    }
}
 