using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Interfaces;
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

        private static ClientTypeToDbTypeResolver clientTypeToDbTypeResolver = new();

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
            DbType? dbType) =>
            CreateParameter(command, name, value, dbType, null);

        /// <summary>
        /// Creates a parameter for a command object.
        /// </summary>
        /// <param name="command">The command object instance to be used.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="dbType">The database type of the parameter.</param>
        /// <param name="parameterDirection">The direction of the parameter.</param>
        /// <returns>An instance of the newly created parameter object.</returns>
        public static IDbDataParameter CreateParameter(this IDbCommand command,
            string name,
            object value,
            DbType? dbType,
            ParameterDirection? parameterDirection)
        {
            // Create the parameter
            var parameter = command.CreateParameter();

            // Set the values
            parameter.ParameterName = name.AsParameter(DbSettingMapper.Get(command.Connection));
            parameter.Value = value ?? DBNull.Value;

            // The DB Type is auto set when setting the values (so check properly Time/DateTime problem)
            if (dbType != null) // && parameter.DbType != dbType.Value)
            {
                // Prepare() requires an explicit assignment, weird Microsoft
                parameter.DbType = dbType.Value;
            }

            // Set the direction
            if (parameterDirection != null)
            {
                parameter.Direction = parameterDirection.Value;
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

            if (values.Length == 0)
            {
                command.Parameters.Add(command.CreateParameter(commandArrayParameter.ParameterName.AsParameter(dbSetting), null, null));
            }
            else
            {
                for (var i = 0; i < values.Length; i++)
                {
                    var name = string.Concat(commandArrayParameter.ParameterName, i.ToString()).AsParameter(dbSetting);
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
            CreateParameters(command, param, param?.GetType());

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
            HashSet<string> propertiesToSkip,
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
            else if (param is QueryField field)
            {
                CreateParameters(command, field, propertiesToSkip, entityType, dbFields);
            }

            // IEnumerable<QueryField>
            else if (param is IEnumerable<QueryField> fields)
            {
                CreateParameters(command, fields, propertiesToSkip, entityType, dbFields);
            }

            // QueryGroup
            else if (param is QueryGroup group)
            {
                CreateParameters(command, group, propertiesToSkip, entityType, dbFields);
            }

            // Other
            else
            {
                CreateParametersInternal(command, param, propertiesToSkip, entityType, dbFields);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="size"></param>
        /// <param name="classProperty"></param>
        /// <param name="dbField"></param>
        /// <param name="parameterDirection"></param>
        /// <param name="fallbackType"></param>
        /// <returns></returns>
        private static IDbDataParameter CreateParameter(IDbCommand command,
            string name,
            object value,
            int? size,
            ClassProperty classProperty,
            DbField dbField,
            ParameterDirection? parameterDirection,
            Type fallbackType)
        {
            var valueType = (value?.GetType() ?? classProperty?.PropertyInfo.PropertyType).GetUnderlyingType();

            if (valueType?.IsEnum == true)
            {
                return CreateParameterForEnum(command,
                    valueType,
                    name,
                    value,
                    size,
                    classProperty,
                    dbField,
                    parameterDirection);
            }
            else
            {
                return CreateParameterForNonEnum(command,
                    valueType,
                    name,
                    value,
                    size,
                    classProperty,
                    dbField,
                    parameterDirection,
                    fallbackType);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="valueType"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="size"></param>
        /// <param name="classProperty"></param>
        /// <param name="dbField"></param>
        /// <param name="parameterDirection"></param>
        /// <param name="fallbackType"></param>
        /// <returns></returns>
        private static IDbDataParameter CreateParameterForNonEnum(IDbCommand command,
            Type valueType,
            string name,
            object value,
            int? size,
            ClassProperty classProperty,
            DbField dbField,
            ParameterDirection? parameterDirection,
            Type fallbackType)
        {
            // Property Handler
            InvokePropertyHandler(classProperty, ref valueType, ref value);

            // Auto conversion
            EnsureAutomaticConversion(dbField, ref valueType, ref value);

            // The reason for not using classProperty.GetDbType() is to avoid getting the type level mapping.

            // DbType
            var dbType = classProperty != null ? TypeMapCache.Get(classProperty.GetDeclaringType(), classProperty.PropertyInfo) :
                null;

            valueType = (valueType ??= dbField?.Type.GetUnderlyingType() ?? fallbackType);
            if (dbType == null && valueType != null)
            {
                dbType =
                    valueType.GetDbType() ??                                        // Type level, use TypeMapCache
                    clientTypeToDbTypeResolver.Resolve(valueType) ??                // Type level, primitive mapping
                    (dbField?.Type != null ?
                        clientTypeToDbTypeResolver.Resolve(dbField?.Type) : null);  // Fallback to the database type
            }

            // Create the parameter
            var parameter = command.CreateParameter(name, value, dbType, parameterDirection);

            // Set the size
            var parameterSize = GetSize(size, dbField);
            parameter.Size = (parameterSize > 0) ? parameterSize : parameter.Size;

            // Parameter values
            InvokeParameterPropertyValueSetterAttributes(parameter, classProperty);

            // Return the parameter
            return parameter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="valueType"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="size"></param>
        /// <param name="classProperty"></param>
        /// <param name="dbField"></param>
        /// <param name="parameterDirection"></param>
        /// <returns></returns>
        private static IDbDataParameter CreateParameterForEnum(IDbCommand command,
            Type valueType,
            string name,
            object value,
            int? size,
            ClassProperty classProperty,
            DbField dbField,
            ParameterDirection? parameterDirection)
        {
            // Property handler
            InvokePropertyHandler(classProperty, ref valueType, ref value);

            // DbType
            var dbType = IsPostgreSqlUserDefined(dbField) ? default :
                classProperty.GetDbType() ??
                valueType.GetDbType() ??
                (dbField != null ? clientTypeToDbTypeResolver.Resolve(dbField.Type) : null) ??
                (DbType?)Converter.EnumDefaultDatabaseType;

            // Create the parameter
            var parameter = command.CreateParameter(name, value, dbType, parameterDirection);

            // Set the size
            parameter.Size = GetSize(size, dbField);

            // Type map attributes
            InvokeParameterPropertyValueSetterAttributes(parameter, classProperty);

            // Return the parameter
            return parameter;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="command"></param>
        /// <param name="param"></param>
        /// <param name="propertiesToSkip"></param>
        /// <param name="entityType"></param>
        /// <param name="dbFields"></param>
        private static void CreateParametersInternal(IDbCommand command,
            object param,
            HashSet<string> propertiesToSkip,
            Type entityType,
            IEnumerable<DbField> dbFields = null)
        {
            var type = param.GetType();

            // Check
            if (type.IsGenericType && type.GetGenericTypeDefinition() == StaticType.Dictionary)
            {
                throw new InvalidParameterException("The supported type of dictionary object must be of type IDictionary<string, object>.");
            }

            // Variables
            var entityClassProperties = entityType != null ? PropertyCache.Get(entityType) : default;
            var paramClassProperties = type.IsClassType() ? PropertyCache.Get(type) : type.GetClassProperties();

            // Skip
            if (propertiesToSkip != null)
            {
                paramClassProperties = paramClassProperties?.Where(p => propertiesToSkip.Contains(p.PropertyInfo.Name) == false);
            }

            // Iterate
            foreach (var paramClassProperty in paramClassProperties)
            {
                var entityClassProperty = (entityType == paramClassProperty.GetDeclaringType()) ?
                    paramClassProperty :
                    entityClassProperties?
                        .FirstOrDefault(e => string.Equals(e.GetMappedName(), paramClassProperty.GetMappedName()));
                var name = paramClassProperty
                    .GetMappedName()
                    .AsUnquoted(command.Connection.GetDbSetting());
                var dbField = GetDbField(name, dbFields);
                var value = paramClassProperty.PropertyInfo.GetValue(param);
                var parameter = CreateParameter(command,
                    name,
                    value,
                    dbField?.Size,
                    (entityClassProperty ?? paramClassProperty),
                    dbField,
                    null,
                    null);
                command.Parameters.Add(parameter);
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
            HashSet<string> propertiesToSkip,
            IEnumerable<DbField> dbFields = null)
        {
            var kvps = dictionary.Where(kvp =>
                propertiesToSkip?.Contains(kvp.Key) != true);

            // Iterate the key value pairs
            foreach (var kvp in kvps)
            {
                var dbField = GetDbField(kvp.Key, dbFields);
                var value = kvp.Value;
                var classProperty = (ClassProperty)null;

                // CommandParameter
                if (kvp.Value is CommandParameter commandParameter)
                {
                    value = commandParameter.Value;
                    dbField ??= GetDbField(commandParameter.Field.Name, dbFields);
                    classProperty = PropertyCache.Get(commandParameter.MappedToType, commandParameter.Field.Name);
                }
                var parameter = CreateParameter(command,
                    kvp.Key,
                    value,
                    dbField?.Size,
                    classProperty,
                    dbField,
                    null,
                    null);
                command.Parameters.Add(parameter);
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
            HashSet<string> propertiesToSkip,
            Type entityType,
            IEnumerable<DbField> dbFields = null)
        {
            if (queryGroup == null)
            {
                return;
            }
            CreateParameters(command, queryGroup.GetFields(true), propertiesToSkip, entityType, dbFields);
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
            HashSet<string> propertiesToSkip,
            Type entityType,
            IEnumerable<DbField> dbFields = null)
        {
            if (queryFields == null)
            {
                return;
            }

            // Filter the query fields
            var filteredQueryFields = queryFields
                .Where(qf => propertiesToSkip?.Contains(qf.Field.Name) != true);

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
            HashSet<string> propertiesToSkip,
            Type entityType,
            IEnumerable<DbField> dbFields = null)
        {
            if (queryField == null)
            {
                return;
            }

            var fieldName = queryField.Field.Name;

            // Skip
            if (propertiesToSkip?.Contains(fieldName) == true)
            {
                return;
            }

            // Variables
            var dbField = GetDbField(fieldName, dbFields);
            var value = queryField.Parameter.Value;
            var classProperty = PropertyCache.Get(entityType, queryField.Field);
            var (direction, fallbackType, size) = queryField is DirectionalQueryField n ?
                ((ParameterDirection?)n.Direction, n.Type, n.Size ?? dbField?.Size) : default;

            // Create the parameter
            var parameter = CreateParameter(command,
                queryField.Parameter.Name,
                value,
                size,
                classProperty,
                dbField,
                direction,
                fallbackType);
            command.Parameters.Add(parameter);

            // Set the parameter
            queryField.DbParameter = parameter;
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
            var values = (queryField?.Parameter.Value as System.Collections.IEnumerable)?
                        .WithType<object>()
                        .AsList();
            if (values?.Count > 0)
            {
                for (var i = 0; i < values.Count; i++)
                {
                    var name = string.Concat(queryField.Parameter.Name, "_In_", i.ToString());
                    var parameter = CreateParameter(command,
                        name,
                        values[i],
                        dbField?.Size,
                        null,
                        dbField,
                        null,
                        null);
                    command.Parameters.Add(parameter);
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
            var values = (queryField?.Parameter.Value as System.Collections.IEnumerable)?
                        .WithType<object>()
                        .AsList();
            if (values?.Count == 2)
            {
                // Left
                var leftParameter = CreateParameter(command,
                    string.Concat(queryField.Parameter.Name, "_Left"),
                    values[0],
                    dbField?.Size,
                    null, dbField,
                    null,
                    null);
                command.Parameters.Add(leftParameter);

                // Right
                var rightParameter = CreateParameter(command,
                    string.Concat(queryField.Parameter.Name, "_Right"),
                    values[1],
                    dbField?.Size,
                    null,
                    dbField,
                    null,
                    null);
                command.Parameters.Add(rightParameter);
            }
            else
            {
                throw new InvalidParameterException("The values for 'Between' and 'NotBetween' operations must be 2.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classProperty"></param>
        /// <param name="valueType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object InvokePropertyHandler(ClassProperty classProperty,
            ref Type valueType,
            ref object value)
        {
            var propertyHandler = classProperty?.GetPropertyHandler() ??
                (valueType == null ? null : PropertyHandlerCache.Get<object>(valueType));

            if (propertyHandler != null)
            {
                var propertyHandlerSetMethod = propertyHandler.GetType().GetMethod("Set");
                value = propertyHandlerSetMethod.Invoke(propertyHandler, new[] { value, classProperty });
                valueType = propertyHandlerSetMethod.ReturnType.GetUnderlyingType();
            }

            return propertyHandler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbField"></param>
        /// <returns></returns>
        private static bool IsPostgreSqlUserDefined(DbField dbField) =>
            string.Equals(dbField?.DatabaseType, "USER-DEFINED", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(dbField?.Provider, "PGSQL", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <param name="dbField"></param>
        private static int GetSize(int? size,
            DbField dbField) =>
            size.HasValue ? size.Value :
                 dbField?.Size.HasValue == true ? dbField.Size.Value : default;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="classProperty"></param>
        private static void InvokeParameterPropertyValueSetterAttributes(IDbDataParameter parameter,
            ClassProperty classProperty)
        {
            var attributes = GetParameterPropertyValueSetterAttributes(classProperty);
            if (attributes?.Any() != true)
            {
                return;
            }

            foreach (var attribute in attributes)
            {
                attribute.SetValue(parameter);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classProperty"></param>
        /// <returns></returns>
        private static IEnumerable<ParameterPropertyValueSetterAttribute> GetParameterPropertyValueSetterAttributes(ClassProperty classProperty) =>
            classProperty?
                .PropertyInfo?
                .GetCustomAttributes()?
                .Where(e =>
                    StaticType.ParameterPropertyValueSetterAttribute.IsAssignableFrom(e.GetType()))
                .Select(e =>
                    (ParameterPropertyValueSetterAttribute)e);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbField"></param>
        /// <param name="valueType"></param>
        /// <param name="value"></param>
        private static void EnsureAutomaticConversion(DbField dbField,
            ref Type valueType,
            ref object value)
        {
            if (valueType != null && dbField != null && IsAutomaticConversion(dbField))
            {
                var dbFieldType = dbField.Type.GetUnderlyingType();
                if (dbFieldType != valueType)
                {
                    if (value != null)
                    {
                        value = AutomaticConvert(value, valueType, dbFieldType);
                    }
                    valueType = dbFieldType;
                }
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
            if (fieldName.Contains("_In_", StringComparison.OrdinalIgnoreCase))
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
                return Convert.ChangeType(value, targetType);
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
        private static object AutomaticConvertGuidToString(object value) =>
            value?.ToString();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static object ConvertEnumValueToType(object value,
            Type type) =>
            value != null ?
                type == StaticType.String ?
                    value?.ToString() : Convert.ChangeType(Convert.ToInt32(value), type) : null;

        #endregion
    }
}
