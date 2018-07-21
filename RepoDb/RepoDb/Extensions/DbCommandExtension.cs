using RepoDb.Attributes;
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
        /// <summary>
        /// Creates a parameter for a command object.
        /// </summary>
        /// <param name="command">The command object instance to be used.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="dbType">The database type of the parameter.</param>
        /// <returns>An instance of the newly created parameter object.</returns>
        public static IDbDataParameter CreateParameter(this IDbCommand command, string name, object value, DbType? dbType = null)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            if (dbType != null)
            {
                parameter.DbType = dbType.Value;
            }
            return parameter;
        }

        /// <summary>
        /// Creates a parameter for a command object.
        /// </summary>
        /// <param name="command">The command object instance to be used.</param>
        /// <param name="param">The object to be used when creating the parameters.</param>
        public static void CreateParameters(this IDbCommand command, object param)
        {
            CreateParameters(command, param, null);
        }

        /// <summary>
        /// Creates a parameter from object by mapping the property from the target entity type.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="param">The object to be used when creating the parameters.</param>
        /// <param name="mappedToEntityType">The target type where to map the parameters.</param>
        internal static void CreateParameters(this IDbCommand command, object param, Type mappedToEntityType)
        {
            var obj = param as ExpandoObject;
            if (obj != null)
            {
                var dictionary = (IDictionary<string, object>)param;
                var dbType = (DbType?)null;
                foreach (var item in dictionary)
                {
                    if (mappedToEntityType != null)
                    {
                        var property = mappedToEntityType.GetProperty(item.Key);
                        dbType = property?.GetCustomAttribute<TypeMapAttribute>()?.DbType ??
                            TypeMapper.Get(property?.PropertyType)?.DbType; ;
                    }
                    else
                    {
                        dbType = TypeMapper.Get(item.Value?.GetType())?.DbType;
                    }
                    command.Parameters.Add(command.CreateParameter(item.Key, item.Value, dbType));
                }
            }
            else
            {
                param?.GetType()
                    .GetProperties()
                    .ToList()
                    .ForEach(property =>
                    {
                        var dbType = property.GetCustomAttribute<TypeMapAttribute>()?.DbType ??
                            TypeMapper.Get(property.PropertyType)?.DbType;
                        command.Parameters.Add(command.CreateParameter(property.Name, property.GetValue(param), dbType));
                    });
            }
        }
    }
}
