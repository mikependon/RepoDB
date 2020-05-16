using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Resolvers;
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
        #region Privates

        internal static Type m_bytesType = typeof(byte[]);
        internal static Type m_dictionaryType = typeof(Dictionary<,>);
        internal static ClientTypeToDbTypeResolver m_clientTypeToDbTypeResolver = new ClientTypeToDbTypeResolver();

        #endregion

        #region CreateParameters

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
            DbType? dbType)
        {
            // Create the parameter
            var parameter = command.CreateParameter();

            // Set the values
            parameter.ParameterName = name.AsParameter(DbSettingMapper.Get(command.Connection.GetType()));
            parameter.Value = value ?? DBNull.Value;

            // The DB Type is auto set when setting the values (so check properly Time/DateTime problem)
            if (dbType != null && parameter.DbType != dbType.Value)
            {
                parameter.DbType = dbType.Value;
            }

            // Return the parameter
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
            if (commandArrayParameters?.Any() != true)
            {
                return;
            }
            var dbSetting = command.Connection.GetDbSetting();
            for (var i = 0; i < commandArrayParameters.Count(); i++)
            {
                var commandArrayParameter = commandArrayParameters.ElementAt(i);
                for (var c = 0; c < commandArrayParameter.Values.Count(); c++)
                {
                    var name = string.Concat(commandArrayParameter.ParameterName, c).AsParameter(dbSetting);
                    var value = commandArrayParameter.Values.ElementAt(c);
                    command.Parameters.Add(command.CreateParameter(name, value, null));
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
        /// <param name="propertiesToSkip">The list of the properties to be skipped.</param>
        internal static void CreateParameters(this IDbCommand command,
            object param,
            IEnumerable<string> propertiesToSkip)
        {
            // Check for presence
            if (param == null)
            {
                return;
            }

            // Supporting the IDictionary<string, object>
            if (param is ExpandoObject || param is IDictionary<string, object>)
            {
                CreateParameters(command, (IDictionary<string, object>)param, propertiesToSkip);
            }

            // Supporting the QueryField
            else if (param is QueryField)
            {
                CreateParameters(command, (QueryField)param, propertiesToSkip);
            }

            // Supporting the IEnumerable<QueryField>
            else if (param is IEnumerable<QueryField>)
            {
                CreateParameters(command, (IEnumerable<QueryField>)param, propertiesToSkip);
            }

            // Supporting the QueryGroup
            else if (param is QueryGroup)
            {
                CreateParameters(command, (QueryGroup)param, propertiesToSkip);
            }

            // Otherwise, iterate the properties
            else
            {
                var type = param.GetType();

                // Check the validity of the type
                if (type.IsGenericType && type.GetGenericTypeDefinition() == m_dictionaryType)
                {
                    throw new InvalidParameterException("The supported type of dictionary object must be of type IDictionary<string, object>.");
                }

                // Variables for properties
                var properties = (IEnumerable<ClassProperty>)null;

                // Add this check for performance
                if (type.IsGenericType == true)
                {
                    properties = type.GetClassProperties();
                }
                else
                {
                    properties = PropertyCache.Get(type);
                }

                // Skip some properties
                if (propertiesToSkip != null)
                {
                    properties = properties?
                        .Where(p => propertiesToSkip?.Contains(p.PropertyInfo.Name,
                            StringComparer.OrdinalIgnoreCase) == false);
                }

                // Iterate the properties
                foreach (var property in properties)
                {
                    // Get the property vaues
                    var name = property.GetMappedName();
                    var value = property.PropertyInfo.GetValue(param);

                    #region DbType

                    // Get property db type
                    var dbType = property.GetDbType();

                    // Ensure mapping based on the value type
                    if (dbType == null)
                    {
                        dbType = TypeMapCache.Get(value?.GetType().GetUnderlyingType());
                    }

                    // Check for specialized
                    if (dbType == null)
                    {
                        var propertyType = property.PropertyInfo.PropertyType.GetUnderlyingType();
                        if (propertyType?.IsEnum == true)
                        {
                            dbType = DbType.String;
                        }
                        else if (propertyType == m_bytesType)
                        {
                            dbType = DbType.Binary;
                        }
                    }

                    #endregion

                    #region PropertyHandler

                    // Check the property handler
                    var propertyHandler = PropertyHandlerCache.Get<object>(type, property.PropertyInfo);

                    if (propertyHandler != null)
                    {
                        // TODO: Ensure to reuse the existing PropertyHandler (if given)
                    }

                    #endregion

                    // Add the new parameter
                    command.Parameters.Add(command.CreateParameter(name, value, dbType));
                }
            }
        }

        /// <summary>
        /// Create the command parameters from the <see cref="IDictionary{TKey, TValue}"/> object.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="dictionary">The parameters from the <see cref="Dictionary{TKey, TValue}"/> object.</param>
        /// <param name="propertiesToSkip">The list of the properties to be skipped.</param>
        internal static void CreateParameters(this IDbCommand command,
            IDictionary<string, object> dictionary,
            IEnumerable<string> propertiesToSkip)
        {
            // Variables needed
            var kvps = dictionary.Where(kvp => propertiesToSkip?.Contains(kvp.Key, StringComparer.OrdinalIgnoreCase) != true);

            // Iterate the key value pairs
            foreach (var kvp in kvps)
            {
                var dbType = (DbType?)null;
                var value = kvp.Value;
                var valueType = (Type)null;
                var property = (PropertyInfo)null;

                // Cast the proper object and identify the properties
                if (kvp.Value is CommandParameter)
                {
                    var commandParameter = (CommandParameter)kvp.Value;

                    // Get the property and value
                    property = commandParameter.MappedToType.GetProperty(kvp.Key);
                    value = commandParameter.Value;

                    // Set the value type
                    valueType = value?.GetType()?.GetUnderlyingType();
                }
                else
                {
                    // In this area, it could be a 'dynamic' object
                    valueType = kvp.Value?.GetType()?.GetUnderlyingType();
                }

                // Get the property-level mapping
                if (property != null)
                {
                    #region PropertyHandler

                    // Check the property handler
                    var propertyHandler = PropertyHandlerCache.Get<object>(property.DeclaringType, property);
                    if (propertyHandler != null)
                    {
                        // It is hard to pre-compile this as the property handler is dynamic
                        var setMethod = propertyHandler.GetType().GetMethod("Set");
                        var returnType = setMethod.ReturnType;
                        var classProperty = PropertyCache.Get(property.DeclaringType, property);
                        dbType = m_clientTypeToDbTypeResolver.Resolve(returnType);
                        value = setMethod.Invoke(propertyHandler, new[] { value, classProperty });
                    }

                    #endregion

                    else
                    {
                        #region DbType

                        dbType = TypeMapCache.Get(property.DeclaringType, property);

                        // Check for the specialized types
                        if (dbType == null)
                        {
                            if (valueType?.IsEnum == true)
                            {
                                dbType = DbType.String;
                            }
                            else if (valueType == m_bytesType)
                            {
                                dbType = DbType.Binary;
                            }
                        }

                        #endregion
                    }
                }

                // Add the parameter
                command.Parameters.Add(command.CreateParameter(kvp.Key, value, dbType));
            }
        }

        /// <summary>
        /// Create the command parameters from the <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="queryGroup">The value of the <see cref="QueryGroup"/> object.</param>
        /// <param name="propertiesToSkip">The list of the properties to be skipped.</param>
        internal static void CreateParameters(this IDbCommand command,
            QueryGroup queryGroup,
            IEnumerable<string> propertiesToSkip)
        {
            // Call the overloaded methods for the query fields
            CreateParameters(command, queryGroup?.GetFields(true), propertiesToSkip);
        }

        /// <summary>
        /// Create the command parameters from the list of <see cref="QueryField"/> objects.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="queryFields">The list of <see cref="QueryField"/> objects.</param>
        /// <param name="propertiesToSkip">The list of the properties to be skipped.</param>
        internal static void CreateParameters(this IDbCommand command,
            IEnumerable<QueryField> queryFields,
            IEnumerable<string> propertiesToSkip)
        {
            // Filter the query fields
            var filteredQueryFields = queryFields
                .Where(qf => propertiesToSkip?.Contains(qf.Field.Name, StringComparer.OrdinalIgnoreCase) != true);

            // Iterate the filtered query fields
            foreach (var queryField in filteredQueryFields)
            {
                CreateParameters(command, queryField, null);
            }
        }

        /// <summary>
        /// Creates a command parameter from the <see cref="QueryField"/> object.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="queryField">The value of <see cref="QueryField"/> object.</param>
        /// <param name="propertiesToSkip">The list of the properties to be skipped.</param>
        internal static void CreateParameters(this IDbCommand command,
            QueryField queryField,
            IEnumerable<string> propertiesToSkip)
        {
            // Exclude those to be skipped
            if (propertiesToSkip?.Contains(queryField.Field.Name, StringComparer.OrdinalIgnoreCase) == true)
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
            var valueType = value?.GetType()?.GetUnderlyingType();
            var dbType = (DbType?)null;

            #region DbType

            dbType = TypeMapCache.Get(valueType);

            // Check for the specialized types
            if (dbType == null)
            {
                if (valueType?.IsEnum == true)
                {
                    dbType = DbType.String;
                }
                else if (valueType == m_bytesType)
                {
                    dbType = DbType.Binary;
                }
            }

            #endregion

            #region PropertyHandler

            // Check the property handler
            var typeHandler = PropertyHandlerCache.Get<object>(valueType);

            if (typeHandler != null)
            {
                // TODO: Ensure to reuse the existing PropertyHandler (if given)
            }

            #endregion

            // Create the parameter
            command.Parameters.Add(command.CreateParameter(queryField.Parameter.Name, value, dbType));
        }

        #endregion
    }
}
