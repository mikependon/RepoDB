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

        #region CreateParameter

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
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="commandArrayParameters"></param>
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
            CreateParameters(command, param, null, entityType, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="param"></param>
        /// <param name="propertiesToSkip"></param>
        /// <param name="entityType"></param>
        /// <param name="dbFields"></param>
        internal static void CreateParameters(this IDbCommand command,
            object param,
            IEnumerable<string> propertiesToSkip,
            Type entityType,
            IEnumerable<DbField> dbFields = null)
        {
            // Check
            if (param == null)
            {
                return;
            }

            // IDictionary<string, object>
            if (param is ExpandoObject || param is IDictionary<string, object>)
            {
                CreateParameters(command, (IDictionary<string, object>)param, propertiesToSkip, dbFields);
            }

            // QueryField
            else if (param is QueryField)
            {
                CreateParameters(command, (QueryField)param, propertiesToSkip, entityType, dbFields);
            }

            // IEnumerable<QueryField>
            else if (param is IEnumerable<QueryField>)
            {
                CreateParameters(command, (IEnumerable<QueryField>)param, propertiesToSkip, entityType, dbFields);
            }

            // QueryGroup
            else if (param is QueryGroup)
            {
                CreateParameters(command, (QueryGroup)param, propertiesToSkip, entityType, dbFields);
            }

            // Other
            else
            {
                CreateParametersInternal(command, param, propertiesToSkip, dbFields);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="param"></param>
        /// <param name="propertiesToSkip"></param>
        /// <param name="dbFields"></param>
        private static void CreateParametersInternal(IDbCommand command,
            object param,
            IEnumerable<string> propertiesToSkip,
            IEnumerable<DbField> dbFields = null)
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
                var dbField = GetDbField(name, dbFields);
                var value = classProperty.PropertyInfo.GetValue(param);
                var returnType = (Type)null;

                // Propertyhandler
                var propertyHandler = GetProperyHandler(classProperty, value?.GetType());
                var definition = InvokePropertyHandlerSetMethod(propertyHandler, value, classProperty);
                if (definition != null)
                {
                    returnType = definition.ReturnType.GetUnderlyingType();
                    value = definition.Value;
                }

                // Automatic
                if (IsAutomaticConversion(dbField))
                {
                    var underlyingType = dbField.Type.GetUnderlyingType();
                    value = AutomaticConvert(value, classProperty.PropertyInfo.PropertyType.GetUnderlyingType(), underlyingType);
                    returnType = underlyingType;
                }

                // DbType
                var dbType = (returnType != null ? clientTypeToDbTypeResolver.Resolve(returnType) : null) ??
                    classProperty.GetDbType() ??
                    value?.GetType()?.GetDbType();

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
        /// <param name="dbFields"></param>
        private static void CreateParameters(IDbCommand command,
            IDictionary<string, object> dictionary,
            IEnumerable<string> propertiesToSkip,
            IEnumerable<DbField> dbFields = null)
        {
            var kvps = dictionary.Where(kvp =>
                propertiesToSkip?.Contains(kvp.Key, StringComparer.OrdinalIgnoreCase) != true);

            // Iterate the key value pairs
            foreach (var kvp in kvps)
            {
                var dbField = GetDbField(kvp.Key, dbFields);
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
                var propertyHandler = GetProperyHandler(classProperty, valueType);
                var definition = InvokePropertyHandlerSetMethod(propertyHandler, value, classProperty);
                if (definition != null)
                {
                    valueType = definition.ReturnType.GetUnderlyingType();
                    value = definition.Value;
                }

                // Automatic
                if (IsAutomaticConversion(dbField))
                {
                    var dbFieldType = dbField.Type.GetUnderlyingType();
                    value = AutomaticConvert(value, valueType, dbFieldType);
                    valueType = dbFieldType;
                }

                // DbType
                var dbType = (valueType != null ? clientTypeToDbTypeResolver.Resolve(valueType) : null) ??
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
        /// <param name="dbFields"></param>
        internal static void CreateParameters(IDbCommand command,
            QueryGroup queryGroup,
            IEnumerable<string> propertiesToSkip,
            Type entityType,
            IEnumerable<DbField> dbFields = null)
        {
            if (queryGroup == null)
            {
                return;
            }
            CreateParameters(command, queryGroup?.GetFields(true), propertiesToSkip, entityType, dbFields);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="queryFields"></param>
        /// <param name="propertiesToSkip"></param>
        /// <param name="entityType"></param>
        /// <param name="dbFields"></param>
        internal static void CreateParameters(this IDbCommand command,
            IEnumerable<QueryField> queryFields,
            IEnumerable<string> propertiesToSkip,
            Type entityType,
            IEnumerable<DbField> dbFields = null)
        {
            if (queryFields == null)
            {
                return;
            }

            // Filter the query fields
            var filteredQueryFields = queryFields
                .Where(qf => propertiesToSkip?.Contains(qf.Field.Name, StringComparer.OrdinalIgnoreCase) != true);

            // Iterate the filtered query fields
            foreach (var queryField in filteredQueryFields)
            {
                if (queryField.Operation == Operation.In || queryField.Operation == Operation.NotIn)
                {
                    var dbField = GetDbField(queryField.Field.Name, dbFields);
                    CreateParametersForInOperation(command, queryField, dbField);
                }
                else if (queryField.Operation == Operation.Between || queryField.Operation == Operation.NotBetween)
                {
                    var dbField = GetDbField(queryField.Field.Name, dbFields);
                    CreateParametersForBetweenOperation(command, queryField, dbField);
                }
                else
                {
                    CreateParameters(command, queryField, null, entityType, dbFields);
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
        /// <param name="dbFields"></param>
        private static void CreateParameters(this IDbCommand command,
            QueryField queryField,
            IEnumerable<string> propertiesToSkip,
            Type entityType,
            IEnumerable<DbField> dbFields = null)
        {
            if (queryField == null)
            {
                return;
            }

            // Skip
            if (propertiesToSkip?.Contains(queryField.Field.Name, StringComparer.OrdinalIgnoreCase) == true)
            {
                return;
            }

            // Variables
            var dbField = GetDbField(queryField.Field.Name, dbFields);
            var value = queryField.Parameter.Value;
            var valueType = value?.GetType()?.GetUnderlyingType();

            // PropertyHandler
            var classProperty = PropertyCache.Get(entityType, queryField.Field);
            var propertyHandler = GetProperyHandler(classProperty, valueType);
            var definition = InvokePropertyHandlerSetMethod(propertyHandler, value, classProperty);
            if (definition != null)
            {
                valueType = definition.ReturnType.GetUnderlyingType();
                value = definition.Value;
            }

            // Automatic
            if (IsAutomaticConversion(dbField))
            {
                var underlyingType = dbField.Type.GetUnderlyingType();
                value = AutomaticConvert(value, classProperty?.PropertyInfo?.PropertyType?.GetUnderlyingType(), underlyingType);
                valueType = underlyingType;
            }

            // DbType
            var dbType = (valueType != null ? clientTypeToDbTypeResolver.Resolve(valueType) : null) ??
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
        /// <param name="dbField"></param>
        private static void CreateParametersForInOperation(this IDbCommand command,
            QueryField queryField,
            DbField dbField = null)
        {
            if (queryField == null)
            {
                return;
            }
            var values = (queryField.Parameter.Value as System.Collections.IEnumerable)?
                        .WithType<object>()
                        .AsList();
            if (values.Any() == true)
            {
                for (var i = 0; i < values.Count; i++)
                {
                    var name = string.Concat(queryField.Parameter.Name, "_In_", i);
                    var value = values[i];
                    var valueType = value?.GetType()?.GetUnderlyingType();

                    // Propertyhandler
                    var properyHandler = GetProperyHandler(null, valueType);
                    var definition = InvokePropertyHandlerSetMethod(properyHandler, value, null);
                    if (definition != null)
                    {
                        valueType = definition.ReturnType.GetUnderlyingType();
                        value = definition.Value;
                    }

                    // Automatic
                    if (IsAutomaticConversion(dbField))
                    {
                        var underlyingType = dbField.Type.GetUnderlyingType();
                        value = AutomaticConvert(value, value?.GetType()?.GetUnderlyingType(), underlyingType);
                        valueType = underlyingType;
                    }

                    // DbType
                    var dbType = (valueType != null ? clientTypeToDbTypeResolver.Resolve(valueType) : null);

                    // Create
                    command.Parameters.Add(CreateParameter(command, name, values[i], dbType));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="queryField"></param>
        /// <param name="dbField"></param>
        private static void CreateParametersForBetweenOperation(this IDbCommand command,
            QueryField queryField,
            DbField dbField = null)
        {
            if (queryField == null)
            {
                return;
            }
            var values = (queryField.Parameter.Value as System.Collections.IEnumerable)?
                        .WithType<object>()
                        .AsList();
            if (values?.Count == 2)
            {
                var leftValue = values[0];
                var rightValue = values[1];
                var leftValueType = leftValue?.GetType()?.GetUnderlyingType();
                var rightValueType = rightValue?.GetType()?.GetUnderlyingType();

                // Propertyhandler (Left)
                var leftPropertyHandler = GetProperyHandler(null, leftValueType);
                var leftdefinition = InvokePropertyHandlerSetMethod(leftPropertyHandler, leftValue, null);
                if (leftdefinition != null)
                {
                    leftValueType = leftdefinition.ReturnType.GetUnderlyingType();
                    leftValue = leftdefinition.Value;
                }

                // Propertyhandler (Right)
                var rightPropertyHandler = GetProperyHandler(null, rightValueType);
                var rightDefinition = InvokePropertyHandlerSetMethod(rightPropertyHandler, rightValue, null);
                if (rightDefinition != null)
                {
                    rightValueType = rightDefinition.ReturnType.GetUnderlyingType();
                    rightValue = rightDefinition.Value;
                }

                // Automatic
                if (IsAutomaticConversion(dbField))
                {
                    leftValueType = dbField.Type.GetUnderlyingType();
                    rightValueType = leftValueType;
                    leftValue = AutomaticConvert(leftValue, leftValue?.GetType(), leftValueType);
                    rightValue = AutomaticConvert(rightValue, leftValue?.GetType(), rightValueType);
                }

                // DbType
                var leftDbType = (leftValueType != null ? clientTypeToDbTypeResolver.Resolve(leftValueType) : null);
                var rightDbType = (rightValueType != null ? clientTypeToDbTypeResolver.Resolve(rightValueType) : null);

                // Add
                command.Parameters.Add(
                    CreateParameter(command, string.Concat(queryField.Parameter.Name, "_Left"), leftValue, leftDbType));
                command.Parameters.Add(
                    CreateParameter(command, string.Concat(queryField.Parameter.Name, "_Right"), rightValue, rightDbType));
            }
            else
            {
                throw new InvalidParameterException("The values for 'Between' and 'NotBetween' operations must be 2.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbField"></param>
        /// <returns></returns>
        private static bool IsAutomaticConversion(DbField dbField) =>
            (
                Converter.ConversionType == ConversionType.Automatic ||
                dbField?.IsPrimary == true ||
                dbField?.IsIdentity == true
            ) &&
            dbField?.Type != null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="dbFields"></param>
        /// <returns></returns>
        private static DbField GetDbField(string fieldName,
            IEnumerable<DbField> dbFields)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                return null;
            }
            if (fieldName.Contains("_In_"))
            {
                return dbFields?.FirstOrDefault(df =>
                    fieldName.StartsWith(df.Name, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                return dbFields?.FirstOrDefault(df =>
                    string.Equals(df.Name, fieldName, StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyHandler"></param>
        /// <param name="value"></param>
        /// <param name="classProperty"></param>
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
        /// <param name="value"></param>
        /// <param name="fromType"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private static object AutomaticConvert(object value,
            Type fromType,
            Type targetType)
        {
            if (fromType == null || targetType == null || fromType == targetType)
            {
                return value;
            }
            if (fromType == StaticType.String && targetType == StaticType.Guid)
            {
                return AutomaticConvertStringToGuid(value);
            }
            else if (fromType == StaticType.Guid && targetType == StaticType.String)
            {
                return AutomaticConvertGuidToString(value);
            }
            else
            {
                var method = StaticType.Convert.GetMethod(string.Concat("To", targetType.Name), new[] { fromType });
                if (method != null)
                {
                    value = method.Invoke(null, new[] { value });
                }
                return value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object AutomaticConvertStringToGuid(object value)
        {
            if (value != null)
            {
                value = StaticType.Guid
                    .GetMethod("Parse", new[] { StaticType.String })
                    .Invoke(null, new[] { value });

            }
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object AutomaticConvertGuidToString(object value)
        {
            return value?.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classProperty"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private static object GetProperyHandler(ClassProperty classProperty,
            Type targetType)
        {
            var propertyHandler = classProperty?.GetPropertyHandler();
            if (propertyHandler == null && targetType != null)
            {
                propertyHandler = PropertyHandlerCache.Get<object>(targetType);
            }
            return propertyHandler;
        }

        #endregion
    }
}
