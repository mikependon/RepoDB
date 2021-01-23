using System.Collections;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace RepoDb
{
    public partial class QueryGroup
    {
        /// <summary>
        /// Converts every <see cref="QueryGroup"/> object of the list of <see cref="QueryGroupTypeMap"/> into an <see cref="object"/> 
        /// with all the child <see cref="QueryField"/>s as the property/value to that object. The value of every property of the created
        /// object will be an instance of the <see cref="CommandParameter"/> with the proper type, name and value.
        /// </summary>
        /// <param name="queryGroupTypeMaps">The list of <see cref="QueryGroupTypeMap"/> objects to be converted.</param>
        /// <param name="fixParameters">A boolean value whether to fix the parameter name before converting.</param>
        /// <returns>An instance of an object that contains all the definition of the converted underlying <see cref="QueryFields"/>s.</returns>
        internal static object AsMappedObject(QueryGroupTypeMap[] queryGroupTypeMaps,
            bool fixParameters = true)
        {
            var dictionary = new ExpandoObject() as IDictionary<string, object>;

            foreach (var queryGroupTypeMap in queryGroupTypeMaps)
            {
                AsMappedObject(dictionary, queryGroupTypeMap, fixParameters);
            }

            return (ExpandoObject)dictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="queryGroupTypeMap"></param>
        /// <param name="fixParameters"></param>
        private static void AsMappedObject(IDictionary<string, object> dictionary,
            QueryGroupTypeMap queryGroupTypeMap,
            bool fixParameters = true)
        {
            var queryFields = queryGroupTypeMap
                .QueryGroup
                .GetFields(true);

            // Identify if there are fields to count
            if (queryFields.Any() != true)
            {
                return;
            }

            // Fix the variables for the parameters
            if (fixParameters == true)
            {
                queryGroupTypeMap.QueryGroup.Fix();
            }

            // Iterate all the query fields
            AsMappedObjectForQueryFields(dictionary, queryGroupTypeMap, queryFields);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="queryGroupTypeMap"></param>
        /// <param name="queryFields"></param>
        private static void AsMappedObjectForQueryFields(IDictionary<string, object> dictionary,
            QueryGroupTypeMap queryGroupTypeMap,
            IEnumerable<QueryField> queryFields)
        {
            foreach (var queryField in queryFields)
            {
                if (queryField.Operation == Operation.Between ||
                    queryField.Operation == Operation.NotBetween)
                {
                    AsMappedObjectForBetweenQueryField(dictionary, queryGroupTypeMap, queryField);
                }
                else if (queryField.Operation == Operation.In ||
                    queryField.Operation == Operation.NotIn)
                {
                    AsMappedObjectForInQueryField(dictionary, queryGroupTypeMap, queryField);
                }
                else
                {
                    AsMappedObjectForNormalQueryField(dictionary, queryGroupTypeMap, queryField);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="queryGroupTypeMap"></param>
        /// <param name="queryField"></param>
        private static void AsMappedObjectForBetweenQueryField(IDictionary<string, object> dictionary,
            QueryGroupTypeMap queryGroupTypeMap,
            QueryField queryField)
        {
            var values = GetValueList(queryField.Parameter.Value);

            // Left
            var left = string.Concat(queryField.Parameter.Name, "_Left");
            if (!dictionary.ContainsKey(left))
            {
                var leftValue = values.Count > 0 ? values[0] : null;
                if (queryGroupTypeMap.MappedType != null)
                {
                    dictionary.Add(left,
                        new CommandParameter(queryField.Field, leftValue, queryGroupTypeMap.MappedType));
                }
                else
                {
                    dictionary.Add(left, leftValue);
                }
            }

            // Right
            var right = string.Concat(queryField.Parameter.Name, "_Right");
            if (!dictionary.ContainsKey(right))
            {
                var rightValue = values.Count > 1 ? values[1] : null;
                if (queryGroupTypeMap.MappedType != null)
                {
                    dictionary.Add(right,
                        new CommandParameter(queryField.Field, rightValue, queryGroupTypeMap.MappedType));
                }
                else
                {
                    dictionary.Add(right, rightValue);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="queryGroupTypeMap"></param>
        /// <param name="queryField"></param>
        private static void AsMappedObjectForInQueryField(IDictionary<string, object> dictionary,
            QueryGroupTypeMap queryGroupTypeMap,
            QueryField queryField)
        {
            var values = GetValueList(queryField.Parameter.Value);

            for (var i = 0; i < values.Count; i++)
            {
                var parameterName = string.Concat(queryField.Parameter.Name, "_In_", i);
                if (dictionary.ContainsKey(parameterName))
                {
                    continue;
                }

                if (queryGroupTypeMap.MappedType != null)
                {
                    dictionary.Add(parameterName,
                        new CommandParameter(queryField.Field, values[i], queryGroupTypeMap.MappedType));
                }
                else
                {
                    dictionary.Add(parameterName, values[i]);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="queryGroupTypeMap"></param>
        /// <param name="queryField"></param>
        private static void AsMappedObjectForNormalQueryField(IDictionary<string, object> dictionary,
            QueryGroupTypeMap queryGroupTypeMap,
            QueryField queryField)
        {
            if (dictionary.ContainsKey(queryField.Parameter.Name))
            {
                return;
            }

            if (queryGroupTypeMap.MappedType != null)
            {
                dictionary.Add(queryField.Parameter.Name,
                    new CommandParameter(queryField.Field, queryField.Parameter.Value, queryGroupTypeMap.MappedType));
            }
            else
            {
                dictionary.Add(queryField.Parameter.Name, queryField.Parameter.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private static IList<T> GetValueList<T>(T value)
        {
            var list = new List<T>();

            if (value is IEnumerable enumerable)
            {
                var items = enumerable
                    .WithType<T>()
                    .AsList();
                list.AddRange(items);
            }
            else
            {
                list.AddIfNotNull(value);
            }

            return list;
        }
    }
}
