using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <i>System.Data.IDbCommand</i> object.
    /// </summary>
    public static class DbCommandExtension
    {
        // CreateParameter
        internal static IDbDataParameter CreateParameter(this IDbCommand command, string parameterName, object value, DbType? dbType = null)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;
            if (dbType != null)
            {
                parameter.DbType = dbType.Value;
            }
            return parameter;
        }

        // CreateParameters
        internal static void CreateParameters(this IDbCommand command, object param)
        {
            var obj = param as ExpandoObject;
            if (obj != null)
            {
                var dictionary = (IDictionary<string, object>)param;
                foreach (var item in dictionary)
                {
                    var typeMap = TypeMapper.Get(item.Value?.GetType());
                    command.Parameters.Add(command.CreateParameter(item.Key, item.Value, typeMap?.DbType));
                }
            }
            else
            {
                param?.GetType()
                    .GetProperties()
                    .ToList()
                    .ForEach(property =>
                    {
                        var typeMap = TypeMapper.Get(property.PropertyType);
                        command.Parameters.Add(command.CreateParameter(property.Name, property.GetValue(param), typeMap?.DbType));
                    });
            }
        }
    }
}