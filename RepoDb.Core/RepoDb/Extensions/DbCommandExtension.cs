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

        internal static Type bytesType = StaticType.ByteArray;
        internal static Type dictionaryType = StaticType.Dictionary;
        internal static ClientTypeToDbTypeResolver clientTypeToDbTypeResolver = new ClientTypeToDbTypeResolver();

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
            if (commandArrayParameters.Any() != true)
            {
                return;
            }
            var dbSetting = command.Connection.GetDbSetting();
            foreach (var commandArrayParameter in commandArrayParameters)
            {
                var values = commandArrayParameter.Values.AsArray();
                for (var i = 0; i < values.Length; i++)
                {
                    var name = string.Concat(commandArrayParameter.ParameterName, i).AsParameter(dbSetting);
                    command.Parameters.Add(command.CreateParameter(name, values[i], null));
                }
            }
        }

        /// <summary>
        /// Creates a parameter from object by mapping the property from the target entity type.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="param">The object to be used when creating the parameters.</param>
        public static void CreateParameters(this IDbCommand command,
            object param) =>
            CreateParameters(command, param, null);

        /// <summary>
        /// Creates a parameter from object by mapping the property from the target entity type.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="param">The object to be used when creating the parameters.</param>
        /// <param name="entityType">The type of the data entity.</param>
        public static void CreateParameters(this IDbCommand command,
            object param,
            Type entityType) =>
            CreateParameters(command, param, null, entityType);

        /// <summary>
        /// Creates a parameter from object by mapping the property from the target entity type.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="param">The object to be used when creating the parameters.</param>
        /// <param name="propertiesToSkip">The list of the properties to be skipped.</param>
        /// <param name="entityType">The type of the data entity.</param>
        internal static void CreateParameters(this IDbCommand command,
            object param,
            IEnumerable<string> propertiesToSkip,
            Type entityType)
        {
            // Check for presence
            if (param == null)
            {
                return;
            }

            // Supporting the IDictionary<string, object>
            if (param is ExpandoObject || param is IDictionary<string, object>)
            {
                CreateParameters(command, (IDictionary<string, object>)param, propertiesToSkip, entityType);
            }

            // Supporting the QueryField
            else if (param is QueryField)
            {
                CreateParameters(command, (QueryField)param, propertiesToSkip, entityType);
            }

            // Supporting the IEnumerable<QueryField>
            else if (param is IEnumerable<QueryField>)
            {
                CreateParameters(command, (IEnumerable<QueryField>)param, propertiesToSkip, entityType);
            }

            // Supporting the QueryGroup
            else if (param is QueryGroup)
            {
                CreateParameters(command, (QueryGroup)param, propertiesToSkip, entityType);
            }

            // Otherwise, iterate the properties
            else
            {
                var type = param.GetType();

                // Check the validity of the type
                if (type.IsGenericType && type.GetGenericTypeDefinition() == dictionaryType)
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
                    var dbType = (DbType?)null;

                    #region PropertyHandler

                    // Check the property handler
                    var propertyHandler = property.GetPropertyHandler();
                    if (propertyHandler != null)
                    {
                        var setMethod = propertyHandler.GetType().GetMethod("Set");
                        var returnType = setMethod.ReturnType;
                        var classProperty = PropertyCache.Get(property.GetDeclaringType(), property.PropertyInfo);
                        dbType = clientTypeToDbTypeResolver.Resolve(returnType);
                        value = setMethod.Invoke(propertyHandler, new[] { value, classProperty });
                    }

                    #endregion

                    #region DbType

                    else
                    {
                        // Get property db type
                        dbType = property.GetDbType();

                        // Ensure mapping based on the value type
                        if (dbType == null && value != null)
                        {
                            dbType = TypeMapCache.Get(value.GetType().GetUnderlyingType());
                        }

                        // Check for specialized
                        if (dbType == null)
                        {
                            var propertyType = property.PropertyInfo.PropertyType.GetUnderlyingType();
                            if (propertyType?.IsEnum == true)
                            {
                                dbType = DbType.String;
                            }
                            else if (propertyType == bytesType)
                            {
                                dbType = DbType.Binary;
                            }
                        }
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
        /// <param name="entityType">The type of the data entity.</param>
        internal static void CreateParameters(this IDbCommand command,
            IDictionary<string, object> dictionary,
            IEnumerable<string> propertiesToSkip,
            Type entityType)
        {
            // Variables needed
            var kvps = dictionary.Where(kvp => propertiesToSkip?.Contains(kvp.Key, StringComparer.OrdinalIgnoreCase) != true);

            // Iterate the key value pairs
            foreach (var kvp in kvps)
            {
                var dbType = (DbType?)null;
                var value = kvp.Value;
                var valueType = (Type)null;
                var declaringType = entityType;
                var property = (PropertyInfo)null;

                // Cast the proper object and identify the properties
                if (kvp.Value is CommandParameter)
                {
                    var commandParameter = (CommandParameter)kvp.Value;

                    // Get the property and value
                    property = commandParameter.MappedToType.GetProperty(kvp.Key);
                    declaringType = commandParameter.MappedToType ?? property.DeclaringType;
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
                    var propertyHandler = PropertyHandlerCache.Get<object>(declaringType, property);
                    if (propertyHandler != null)
                    {
                        var setMethod = propertyHandler.GetType().GetMethod("Set");
                        var returnType = setMethod.ReturnType;
                        var classProperty = PropertyCache.Get(declaringType, property);
                        dbType = clientTypeToDbTypeResolver.Resolve(returnType);
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
                            else if (valueType == bytesType)
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
        /// <param name="entityType">The type of the data entity.</param>
        internal static void CreateParameters(this IDbCommand command,
            QueryGroup queryGroup,
            IEnumerable<string> propertiesToSkip,
            Type entityType) =>
            CreateParameters(command, queryGroup?.GetFields(true), propertiesToSkip, entityType);

        /// <summary>
        /// Create the command parameters from the list of <see cref="QueryField"/> objects.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="queryFields">The list of <see cref="QueryField"/> objects.</param>
        /// <param name="propertiesToSkip">The list of the properties to be skipped.</param>
        /// <param name="entityType">The type of the data entity.</param>
        internal static void CreateParameters(this IDbCommand command,
            IEnumerable<QueryField> queryFields,
            IEnumerable<string> propertiesToSkip,
            Type entityType)
        {
            // Filter the query fields
            var filteredQueryFields = queryFields
                .Where(qf => propertiesToSkip?.Contains(qf.Field.Name, StringComparer.OrdinalIgnoreCase) != true);

            // Iterate the filtered query fields
            foreach (var queryField in filteredQueryFields)
            {
                if (queryField.Operation == Operation.In || queryField.Operation == Operation.NotIn)
                {
                    CreateParametersForInOperation(command, queryField);
                }
                else if (queryField.Operation == Operation.Between || queryField.Operation == Operation.NotBetween)
                {
                    CreateParametersForBetweenOperation(command, queryField);
                }
                else
                {
                    CreateParameters(command, queryField, null, entityType);
                }
            }
        }

        /// <summary>
        /// Creates a command parameter from the <see cref="QueryField"/> with <see cref="Operation.In"/>.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="queryField">The value of <see cref="QueryField"/> object.</param>
        internal static void CreateParametersForInOperation(this IDbCommand command,
            QueryField queryField)
        {
            var values = (queryField.Parameter.Value as System.Collections.IEnumerable)?
                        .OfType<object>()
                        .AsList();
            if (values.Any() == true)
            {
                for (var i = 0; i < values.Count; i++)
                {
                    var name = string.Concat(queryField.Parameter.Name, "_In_", i);
                    command.Parameters.Add(CreateParameter(command, name, values[i], null));
                }
            }
        }

        /// <summary>
        /// Creates a command parameter from the <see cref="QueryField"/> with <see cref="Operation.In"/>.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="queryField">The value of <see cref="QueryField"/> object.</param>
        internal static void CreateParametersForBetweenOperation(this IDbCommand command,
            QueryField queryField)
        {
            var values = (queryField.Parameter.Value as System.Collections.IEnumerable)?
                        .OfType<object>()
                        .AsList();
            if (values?.Count == 2)
            {
                command.Parameters.Add(CreateParameter(command, string.Concat(queryField.Parameter.Name, "_Left"), values[0], null));
                command.Parameters.Add(CreateParameter(command, string.Concat(queryField.Parameter.Name, "_Right"), values[1], null));
            }
            else
            {
                throw new InvalidParameterException("The values for 'Between' and 'NotBetween' operations must be 2.");
            }
        }

        /// <summary>
        /// Creates a command parameter from the <see cref="QueryField"/> object.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="queryField">The value of <see cref="QueryField"/> object.</param>
        /// <param name="propertiesToSkip">The list of the properties to be skipped.</param>
        /// <param name="entityType">The type of the data entity.</param>
        internal static void CreateParameters(this IDbCommand command,
            QueryField queryField,
            IEnumerable<string> propertiesToSkip,
            Type entityType)
        {
            // Exclude those to be skipped
            if (propertiesToSkip?.Contains(queryField.Field.Name, StringComparer.OrdinalIgnoreCase) == true)
            {
                return;
            }

            // Get the values
            var value = queryField.Parameter.Value;
            var valueType = value?.GetType()?.GetUnderlyingType();
            var dbType = (DbType?)null;

            #region PropertyHandler

            // Check the property handler
            var typeHandler = PropertyHandlerCache.Get<object>(valueType);
            if (typeHandler != null)
            {
                var classProperty = (ClassProperty)null;
                var setMethod = typeHandler.GetType().GetMethod("Set");
                var returnType = setMethod.ReturnType;
                if (entityType != null)
                {
                    classProperty = PropertyCache.Get(entityType, queryField.Field);
                }
                dbType = clientTypeToDbTypeResolver.Resolve(returnType);
                value = setMethod.Invoke(typeHandler, new[] { value, classProperty });
            }

            #endregion

            #region DbType

            else
            {
                dbType = TypeMapCache.Get(valueType);

                // Check for the specialized types
                if (dbType == null)
                {
                    if (valueType?.IsEnum == true)
                    {
                        dbType = DbType.String;
                    }
                    else if (valueType == bytesType)
                    {
                        dbType = DbType.Binary;
                    }
                }
            }

            #endregion

            // Create the parameter
            command.Parameters.Add(command.CreateParameter(queryField.Parameter.Name, value, dbType));
        }

        #endregion
    }
}
