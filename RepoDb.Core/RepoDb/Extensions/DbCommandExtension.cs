using RepoDb.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="IDbCommand"/> object.
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
        /// <param name="commandArrayParameters">The list of <see cref="CommandArrayParameter"/> to be used for replacement.</param>
        internal static void CreateParametersFromArray(this IDbCommand command, IEnumerable<CommandArrayParameter> commandArrayParameters)
        {
            if (commandArrayParameters == null)
            {
                return;
            }
            for (var i = 0; i < commandArrayParameters.Count(); i++)
            {
                var commandArrayParameter = commandArrayParameters.ElementAt(i);
                for (var c = 0; c < commandArrayParameter.Values.Count(); c++)
                {
                    var name = string.Concat(commandArrayParameter.ParameterName, c).AsParameter();
                    var value = commandArrayParameter.Values.ElementAt(c);
                    command.Parameters.Add(command.CreateParameter(name, value));
                }
            }
        }

        /// <summary>
        /// Creates a parameter from object by mapping the property from the target entity type.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="param">The object to be used when creating the parameters.</param>
        public static void CreateParameters(this IDbCommand command, object param)
        {
            if (param == null)
            {
                return;
            }
            if (param is ExpandoObject || param is IDictionary<string, object>)
            {
                CreateParameters(command, (IDictionary<string, object>)param);
            }
            else if (param is IEnumerable<PropertyValue>)
            {
                CreateParameters(command, (IEnumerable<PropertyValue>)param);
            }
            else
            {
                var type = param.GetType();
                foreach (var property in type.GetProperties())
                {
                    if (property.PropertyType.IsArray)
                    {
                        continue;
                    }
                    var dbType = property.GetCustomAttribute<TypeMapAttribute>()?.DbType ??
                        TypeMapper.Get(GetUnderlyingType(property.PropertyType))?.DbType;
                    command.Parameters.Add(command.CreateParameter(
                        PropertyMappedNameCache.Get(property), property.GetValue(param), dbType));
                }
            }
        }

        /// <summary>
        /// Creates a parameter from object by mapping the property from the target entity type.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="dictionary">The parameters from the <see cref="Dictionary{TKey, TValue}"/> object.</param>
        private static void CreateParameters(this IDbCommand command, IDictionary<string, object> dictionary)
        {
            var dbType = (DbType?)null;
            foreach (var item in dictionary)
            {
                var value = item.Value;
                if (item.Value is CommandParameter)
                {
                    var commandParameter = (CommandParameter)item.Value;
                    var property = commandParameter.MappedToType.GetTypeInfo().GetProperty(item.Key);
                    dbType = property?.GetCustomAttribute<TypeMapAttribute>()?.DbType ??
                        TypeMapper.Get(GetUnderlyingType(property?.PropertyType))?.DbType;
                    value = commandParameter.Value;
                }
                else
                {
                    dbType = TypeMapper.Get(GetUnderlyingType(item.Value?.GetType()))?.DbType;
                }
                command.Parameters.Add(command.CreateParameter(item.Key, value, dbType));
            }
        }

        /// <summary>
        /// Creates a parameter from object by mapping the property from the target entity type.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="propertyValues">The list <see cref="PropertyValue"/> to be added.</param>
        private static void CreateParameters(this IDbCommand command, IEnumerable<PropertyValue> propertyValues)
        {
            foreach (var propertyValue in propertyValues)
            {
                var dbType = propertyValue.Property.GetDbType();
                if (dbType == null)
                {
                    if (propertyValue.Value == null && propertyValue.Property.PropertyInfo.PropertyType == typeof(byte[]))
                    {
                        dbType = DbType.Binary;
                    }
                }
                command.Parameters.Add(command.CreateParameter(propertyValue.Name, propertyValue.Value, dbType));
            }
        }

        /// <summary>
        /// Gets the underlying type if present.
        /// </summary>
        /// <param name="type">The type where to get the underlying type.</param>
        /// <returns>The underlying type.</returns>
        private static Type GetUnderlyingType(Type type)
        {
            return type != null ? Nullable.GetUnderlyingType(type) ?? type : null;
        }
    }
}
