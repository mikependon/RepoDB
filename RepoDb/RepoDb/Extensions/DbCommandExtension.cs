using RepoDb.Attributes;
using RepoDb.Enumerations;
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
        public static IDbDataParameter CreateParameter(this IDbCommand command,
            string name,
            object value,
            DbType? dbType = null)
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
        internal static void CreateParametersFromArray(this IDbCommand command,
            IEnumerable<CommandArrayParameter> commandArrayParameters)
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
        public static void CreateParameters(this IDbCommand command,
            object param)
        {
            CreateParameters(command, param, null);
        }

        /// <summary>
        /// Creates a parameter from object by mapping the property from the target entity type.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="param">The object to be used when creating the parameters.</param>
        /// <param name="skip">The list of the properties to be skpped.</param>
        public static void CreateParameters(this IDbCommand command,
            object param,
            IEnumerable<string> skip)
        {
            // Check for presence
            if (param == null)
            {
                return;
            }

            // Supporting the IEnumerable<PropertyValue>
            if (param is IEnumerable<PropertyValue>)
            {
                CreateParameters(command, (IEnumerable<PropertyValue>)param);
            }

            // Supporting the IDictionary<string, object>
            else if (param is ExpandoObject || param is IDictionary<string, object>)
            {
                CreateParameters(command, (IDictionary<string, object>)param, skip);
            }

            // Supporting the QueryField
            else if (param is QueryField)
            {
                CreateParameters(command, (QueryField)param, skip);
            }

            // Supporting the IEnumerable<QueryField>
            else if (param is IEnumerable<QueryField>)
            {
                CreateParameters(command, (IEnumerable<QueryField>)param, skip);
            }

            // Supporting the QueryGroup
            else if (param is QueryGroup)
            {
                CreateParameters(command, (QueryGroup)param, skip);
            }

            // Otherwise, iterate the properties
            else
            {
                var type = param.GetType();
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    throw new InvalidOperationException("Invalid parameters passed. The supported type of dictionary object must be typeof(IDictionary<string, object>).");
                }

                var properties = (IEnumerable<PropertyInfo>)null;

                // Add this check for performance
                if (skip == null)
                {
                    properties = type.GetProperties();
                }
                else
                {
                    properties = type
                        .GetProperties()
                        .Where(p => skip?.Contains(p.Name, StringComparer.CurrentCultureIgnoreCase) == false);
                }

                // Iterate the properties
                foreach (var property in properties)
                {
                    var dbType = property.GetCustomAttribute<TypeMapAttribute>()?.DbType ??
                        TypeMapper.Get(GetUnderlyingType(property.PropertyType))?.DbType;
                    command.Parameters.Add(command.CreateParameter(
                        PropertyMappedNameCache.Get(property, false), property.GetValue(param), dbType));
                }
            }
        }

        /// <summary>
        /// Create the command parameters from the <see cref="IDictionary{TKey, TValue}"/> object.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="dictionary">The parameters from the <see cref="Dictionary{TKey, TValue}"/> object.</param>
        /// <param name="skip">The list of the properties to be skpped.</param>
        private static void CreateParameters(this IDbCommand command,
            IDictionary<string, object> dictionary,
            IEnumerable<string> skip)
        {
            var dbType = (DbType?)null;

            foreach (var item in dictionary)
            {
                // Exclude those to be skipped
                if (skip?.Contains(item.Key, StringComparer.CurrentCultureIgnoreCase) == true)
                {
                    continue;
                }

                var value = item.Value;

                // Cast the proper object and identify the properties
                if (item.Value is CommandParameter)
                {
                    var commandParameter = (CommandParameter)item.Value;
                    var property = commandParameter.MappedToType.GetProperty(item.Key);
                    dbType = property?.GetCustomAttribute<TypeMapAttribute>()?.DbType ??
                        TypeMapper.Get(GetUnderlyingType(property?.PropertyType))?.DbType;
                    value = commandParameter.Value;
                }
                else
                {
                    dbType = TypeMapper.Get(GetUnderlyingType(item.Value?.GetType()))?.DbType;
                }

                // Add the parameter
                command.Parameters.Add(command.CreateParameter(item.Key, value, dbType));
            }
        }

        /// <summary>
        /// Create the command parameters from the list of <see cref="PropertyValue"/> objects.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="propertyValues">The list <see cref="PropertyValue"/> to be added.</param>
        private static void CreateParameters(this IDbCommand command,
            IEnumerable<PropertyValue> propertyValues)
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
        /// Create the command parameters from the <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="queryGroup">The value of the <see cref="QueryGroup"/> object.</param>
        /// <param name="skip">The list of the properties to be skpped.</param>
        private static void CreateParameters(this IDbCommand command,
            QueryGroup queryGroup,
            IEnumerable<string> skip)
        {
            CreateParameters(command, queryGroup?.GetFields(true), skip);
        }

        /// <summary>
        /// Create the command parameters from the list of <see cref="QueryField"/> objects.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="queryFields">The list of <see cref="QueryField"/> objects.</param>
        /// <param name="skip">The list of the properties to be skpped.</param>
        private static void CreateParameters(this IDbCommand command,
            IEnumerable<QueryField> queryFields,
            IEnumerable<string> skip)
        {
            foreach (var queryField in queryFields)
            {
                CreateParameters(command, queryField, skip);
            }
        }

        /// <summary>
        /// Creates a command parameter from the <see cref="QueryField"/> object.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="queryField">The value of <see cref="QueryField"/> object.</param>
        /// <param name="skip">The list of the properties to be skpped.</param>
        private static void CreateParameters(this IDbCommand command,
            QueryField queryField,
            IEnumerable<string> skip)
        {
            // Exclude those to be skipped
            if (skip?.Contains(queryField.Field.UnquotedName, StringComparer.CurrentCultureIgnoreCase) == true)
            {
                return;
            }

            // Validate, make sure to only have the proper operation
            if (queryField.Operation != Operation.Equal)
            {
                throw new InvalidOperationException($"Operation must only be '{nameof(Operation.Equal)}' when calling the 'Execute' methods.");
            }

            // Get the values
            var value = queryField.Parameter.Value;
            var dbType = TypeMapper.Get(GetUnderlyingType(value?.GetType()))?.DbType;

            // Create the parameter
            command.Parameters.Add(command.CreateParameter(queryField.Parameter.Name, value, dbType));
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
