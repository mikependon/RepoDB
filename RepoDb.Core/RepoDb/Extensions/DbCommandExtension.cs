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
            parameter.ParameterName = name.AsParameter(DbSettingMapper.Get(command.Connection.GetType()));
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
                CreateParametersInternal(command, param, propertiesToSkip, dbFields);
            }
        }

        /// <summary>
        /// Create Parameter, the process will handle value conversion and type conversion.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="name">Entity property's name.</param>
        /// <param name="value">Entity property's value, maybe null.</param>
        /// <param name="classProperty">
        /// The entity's class property information. <br />
        /// If the parameter is a dictionary, it will be null, otherwise it will not be null.
        /// </param>
        /// <param name="dbField">
        /// Used to get the actual field type information of the database to determine whether automatic type conversion is required. <br />
        /// When the tableName is assigned, it will be based on the database field information obtained by the tableName, so this parameter will be null in most cases.
        /// </param>
        /// <param name="parameterDirection">The direction of the parameter.</param>
        /// <param name="fallbackType">Used when none of the above parameters can determine the value of Parameter.DbType.</param>
        /// <returns>An instance of the newly created parameter object.</returns>
        private static IDbDataParameter CreateParameter(IDbCommand command,
            string name,
            object value,
            ClassProperty classProperty,
            DbField dbField,
            ParameterDirection? parameterDirection,
            Type fallbackType)
        {
            /*
             * In some cases, the value type and the classProperty type will be inconsistent, Therefore, the type gives priority to value.
             * ex: in RepoDb.MySql.IntegrationTests.TestMySqlConnectionForQueryForMySqlMapAttribute
             *    entity AttributeTable.Id = int
             *    database completetable.Id = bigint(20) AUTO_INCREMENT
             *    value id in connection.Query<AttributeTable>(id) is from connection.Insert<AttributeTable>(table) = ulong
             */
            var valueType = (value?.GetType() ?? classProperty?.PropertyInfo.PropertyType).GetUnderlyingType();

            /*
             * Try to get the propertyHandler, the order of the source of resolve is classProperty and valueType.
             * If the propertyHandler exists, the value and DbType are re-determined by the propertyHandler.
             */
            var propertyHandler =
                classProperty?.GetPropertyHandler() ??
                (valueType == null ? null : PropertyHandlerCache.Get<object>(valueType));
            if (propertyHandler != null)
            {
                var propertyHandlerSetMethod = propertyHandler.GetType().GetMethod("Set");
                value = propertyHandlerSetMethod.Invoke(propertyHandler, new[] { value, classProperty });
                valueType = propertyHandlerSetMethod.ReturnType.GetUnderlyingType();
            }

            /*
             * When the database field information exists and the field type definition is found to be different from the valueType, 
             * if automatic type conversion is activated at this time, it will be processed.
             */
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

            /*
             * Set DbType as much as possible, to avoid parameter misjudgment when Value is null.
             * order:
             *      1. attribute level, TypeMapAttribute define on class's property
             *      2. property level, assigned by TypeMapper.Add(entityType, property, dbType, ...)
             *      3. type level, use TypeMapCache, assigned by TypeMapper.Add(type, dbType, ...), not included Primitive mapping.
             *      4. type level, primitive mapping, included special type. ex: byte[] ...etc.
             *      5. if isEnum, use Converter.EnumDefaultDatabaseType.
             */

            // if classProperty exists, try get dbType from attribute level or property level, 
            // The reason for not using classProperty.GetDbType() is to avoid getting the type level mapping.
            var dbType = classProperty?.GetDbType();
            if (dbType == null && (valueType ??= dbField?.Type.GetUnderlyingType() ?? fallbackType) != null)
            {
                dbType =
                    valueType.GetDbType() ??                                        // type level, use TypeMapCache
                    clientTypeToDbTypeResolver.Resolve(valueType) ??                // type level, primitive mapping
                    (dbField?.Type != null ?
                        clientTypeToDbTypeResolver.Resolve(dbField?.Type) : null);  // fallback to the database type
            }
            if (dbType == null && valueType.IsEnum)
            {
                dbType = Converter.EnumDefaultDatabaseType;                         // use Converter.EnumDefaultDatabaseType
            }

            /*
             * Return the parameter
             */
            return command.CreateParameter(name, value, dbType, parameterDirection);
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
                classProperties = classProperties?.Where(p => propertiesToSkip.Contains(p.PropertyInfo.Name, StringComparer.OrdinalIgnoreCase) == false);
            }

            // Iterate
            foreach (var classProperty in classProperties)
            {
                var name = classProperty.GetMappedName();
                var dbField = GetDbField(name, dbFields);
                var value = classProperty.PropertyInfo.GetValue(param);
                command.Parameters.Add(CreateParameter(command, name, value, classProperty, dbField, null, null));
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

                // CommandParameter
                if (kvp.Value is CommandParameter commandParameter)
                {
                    value = commandParameter.Value;
                    dbField = dbField ?? GetDbField(commandParameter.Field.Name, dbFields);
                    classProperty = PropertyCache.Get(commandParameter.MappedToType, commandParameter.Field.Name);
                }
                command.Parameters.Add(CreateParameter(command, kvp.Key, value, classProperty, dbField, null, null));
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

            var fieldName = queryField.Field.Name;

            // Skip
            if (propertiesToSkip?.Contains(fieldName, StringComparer.OrdinalIgnoreCase) == true)
            {
                return;
            }

            // Variables
            var dbField = GetDbField(fieldName, dbFields);
            var value = queryField.Parameter.Value;
            var classProperty = PropertyCache.Get(entityType, queryField.Field);
            var (direction, fallbackType) = queryField is DirectionalQueryField n ? ((ParameterDirection?)n.Direction, n.Type) : default;
            var parameter = CreateParameter(command, queryField.Parameter.Name, value, classProperty, dbField, direction, fallbackType);
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
                    var name = string.Concat(queryField.Parameter.Name, "_In_", i);
                    command.Parameters.Add(CreateParameter(command, name, values[i], null, dbField, null, null));
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
                command.Parameters.Add(CreateParameter(command, string.Concat(queryField.Parameter.Name, "_Left"), values[0], null, dbField, null, null));
                command.Parameters.Add(CreateParameter(command, string.Concat(queryField.Parameter.Name, "_Right"), values[1], null, dbField, null, null));
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
        private static object GetPropertyHandler(ClassProperty classProperty,
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
