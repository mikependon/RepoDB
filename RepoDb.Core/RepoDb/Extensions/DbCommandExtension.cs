using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="IDbCommand"/> object.
    /// </summary>
    public static class DbCommandExtension
    {
        #region Privates

        private static ClientTypeToDbTypeResolver clientTypeToDbTypeResolver = new ClientTypeToDbTypeResolver();

        #endregion

        #region SubClasses

        /// <summary>
        /// 
        /// </summary>
        private class PropertyHandlerSetReturnDefinition
        {
            public object Value { get; set; }
            public Type ReturnType { get; set; }
        }

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

            // Table-Valued Parameter
            EnsureTableValueParameter(parameter);

            // Return the parameter
            return parameter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        private static void EnsureTableValueParameter(IDbDataParameter parameter)
        {
            if (parameter == null || parameter.Value is DataTable == false)
            {
                return;
            }

            var property = parameter.GetType().GetProperty("TypeName");
            if (property == null)
            {
                return;
            }

            var table = ((DataTable)parameter.Value);
            property.SetValue(parameter, table.TableName);
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
                CreateParametersFromArray(command, commandArrayParameter, dbSetting);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="commandArrayParameter"></param>
        /// <param name="dbSetting"></param>
        private static void CreateParametersFromArray(this IDbCommand command,
            CommandArrayParameter commandArrayParameter,
            IDbSetting dbSetting)
        {
            var values = commandArrayParameter.Values.AsArray();

            for (var i = 0; i < values.Length; i++)
            {
                var name = string.Concat(commandArrayParameter.ParameterName, i).AsParameter(dbSetting);
                command.Parameters.Add(command.CreateParameter(name, values[i], null));
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
            // Check
            if (param == null)
            {
                return;
            }

            // IDictionary<string, object>
            if (param is ExpandoObject || param is IDictionary<string, object>)
            {
                CreateParameters(command, (IDictionary<string, object>)param, propertiesToSkip);
            }

            // QueryField
            else if (param is QueryField)
            {
                CreateParameters(command, (QueryField)param, propertiesToSkip, entityType);
            }

            // IEnumerable<QueryField>
            else if (param is IEnumerable<QueryField>)
            {
                CreateParameters(command, (IEnumerable<QueryField>)param, propertiesToSkip, entityType);
            }

            // QueryGroup
            else if (param is QueryGroup)
            {
                CreateParameters(command, (QueryGroup)param, propertiesToSkip, entityType);
            }

            // Other
            else
            {
                CreateParametersInternal(command, param, propertiesToSkip);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="param"></param>
        /// <param name="propertiesToSkip"></param>
        private static void CreateParametersInternal(IDbCommand command,
            object param,
            IEnumerable<string> propertiesToSkip)
        {
            var type = param.GetType();

            // Check
            if (type.IsGenericType && type.GetGenericTypeDefinition() == StaticType.Dictionary)
            {
                throw new InvalidParameterException("The supported type of dictionary object must be of type IDictionary<string, object>.");
            }

            // Variables
            var classProperties = type.IsClassType() ? PropertyCache.Get(type) : type.GetClassProperties();

            // Skip
            if (propertiesToSkip != null)
            {
                classProperties = classProperties?
                    .Where(p => propertiesToSkip?.Contains(p.PropertyInfo.Name,
                        StringComparer.OrdinalIgnoreCase) == false);
            }

            // Iterate
            foreach (var classProperty in classProperties)
            {
                var name = classProperty.GetMappedName();
                var value = classProperty.PropertyInfo.GetValue(param);
                var returnType = (Type)null;
                var dbType = (DbType?)null;

                // Propertyhandler
                var definition = InvokePropertyHandlerSetMethod(classProperty.GetPropertyHandler(), value, classProperty);
                if (definition != null)
                {
                    returnType = definition.ReturnType;
                    value = definition.Value;
                }

                // DbType
                if (returnType != null)
                {
                    dbType = clientTypeToDbTypeResolver.Resolve(returnType);
                }
                else
                {
                    dbType = GetSpecializedDbType(classProperty.PropertyInfo.PropertyType.GetUnderlyingType()) ??
                        classProperty.GetDbType() ??
                        value?.GetType()?.GetDbType();
                }

                // Add the parameter
                command.Parameters.Add(command.CreateParameter(name, value, dbType));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="dictionary"></param>
        /// <param name="propertiesToSkip"></param>
        private static void CreateParameters(IDbCommand command,
            IDictionary<string, object> dictionary,
            IEnumerable<string> propertiesToSkip)
        {
            var kvps = dictionary.Where(kvp =>
                propertiesToSkip?.Contains(kvp.Key, StringComparer.OrdinalIgnoreCase) != true);

            // Iterate the key value pairs
            foreach (var kvp in kvps)
            {
                var value = kvp.Value;
                var classProperty = (ClassProperty)null;
                var valueType = (Type)null;

                // CommandParameter
                if (kvp.Value is CommandParameter)
                {
                    var commandParameter = (CommandParameter)kvp.Value;
                    classProperty = PropertyCache.Get(commandParameter.MappedToType, kvp.Key);
                    value = commandParameter.Value;
                    valueType = value?.GetType()?.GetUnderlyingType();
                }
                else
                {
                    valueType = kvp.Value?.GetType()?.GetUnderlyingType();
                }

                // Propertyhandler
                var propertyHandler = classProperty?.GetPropertyHandler() ?? PropertyHandlerCache.Get<object>(valueType);
                var definition = InvokePropertyHandlerSetMethod(propertyHandler, value, classProperty);
                if (definition != null)
                {
                    valueType = definition.ReturnType;
                    value = definition.Value;
                }

                // DbType
                var dbType = clientTypeToDbTypeResolver.Resolve(valueType) ??
                    GetSpecializedDbType(classProperty?.PropertyInfo?.PropertyType?.GetUnderlyingType()) ??
                    classProperty?.GetDbType() ??
                    value?.GetType()?.GetDbType();

                // Add the parameter
                command.Parameters.Add(command.CreateParameter(kvp.Key, value, dbType));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="queryGroup"></param>
        /// <param name="propertiesToSkip"></param>
        /// <param name="entityType"></param>
        internal static void CreateParameters(IDbCommand command,
            QueryGroup queryGroup,
            IEnumerable<string> propertiesToSkip,
            Type entityType) =>
            CreateParameters(command, queryGroup?.GetFields(true), propertiesToSkip, entityType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="queryFields"></param>
        /// <param name="propertiesToSkip"></param>
        /// <param name="entityType"></param>
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
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="queryField"></param>
        /// <param name="propertiesToSkip"></param>
        /// <param name="entityType"></param>
        private static void CreateParameters(this IDbCommand command,
            QueryField queryField,
            IEnumerable<string> propertiesToSkip,
            Type entityType)
        {
            // Skip
            if (propertiesToSkip?.Contains(queryField.Field.Name, StringComparer.OrdinalIgnoreCase) == true)
            {
                return;
            }

            // Variables
            var value = queryField.Parameter.Value;
            var valueType = value?.GetType()?.GetUnderlyingType();

            // PropertyHandler
            var classProperty = PropertyCache.Get(entityType, queryField.Field);
            var propertyHandler = classProperty?.GetPropertyHandler<object>() ?? PropertyHandlerCache.Get<object>(valueType);
            var definition = InvokePropertyHandlerSetMethod(propertyHandler, value, classProperty);
            if (definition != null)
            {
                valueType = definition.ReturnType;
                value = definition.Value;
            }

            // DbType
            var dbType = clientTypeToDbTypeResolver.Resolve(valueType) ??
                GetSpecializedDbType(classProperty?.PropertyInfo?.PropertyType?.GetUnderlyingType()) ??
                classProperty?.GetDbType() ??
                value?.GetType()?.GetDbType();

            // Add the parameter
            command.Parameters.Add(command.CreateParameter(queryField.Parameter.Name, value, dbType));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="queryField"></param>
        private static void CreateParametersForInOperation(this IDbCommand command,
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
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="queryField"></param>
        private static void CreateParametersForBetweenOperation(this IDbCommand command,
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
        /// 
        /// </summary>
        /// <param name="propertyHandler"></param>
        /// <param name="value"></param>
        /// <param name="classProperty"></param>
        /// <returns></returns>
        private static PropertyHandlerSetReturnDefinition InvokePropertyHandlerSetMethod(object propertyHandler,
            object value,
            ClassProperty classProperty)
        {
            if (propertyHandler == null)
            {
                return null;
            }

            var setMethod = propertyHandler.GetType().GetMethod("Set");
            return new PropertyHandlerSetReturnDefinition
            {
                ReturnType = setMethod.ReturnType,
                Value = setMethod.Invoke(propertyHandler, new[] { value, classProperty })
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static DbType? GetSpecializedDbType(Type type)
        {
            if (type == null)
            {
                return null;
            }

            if (type.IsEnum == true)
            {
                // TODO: Why String?
                return DbType.String;
            }
            else if (type == StaticType.ByteArray)
            {
                return DbType.Binary;
            }

            return null;
        }

        #endregion
    }
}
