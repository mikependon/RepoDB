using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;

namespace RepoDb.Reflection
{
    internal partial class Compiler
    {
        /// <summary>
        /// Gets a compiled function that is used to set the <see cref="DbParameter"/> objects of the <see cref="DbCommand"/> object based from the values of the data entity/dynamic objects.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity objects.</typeparam>
        /// <param name="inputFields">The list of the input <see cref="DbField"/> objects.</param>
        /// <param name="outputFields">The list of the input <see cref="DbField"/> objects.</param>
        /// <param name="batchSize">The batch size of the entity to be passed.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The compiled function.</returns>
        public static Action<DbCommand, IList<TEntity>> CompileDataEntityListDbParameterSetter<TEntity>(IEnumerable<DbField> inputFields,
            IEnumerable<DbField> outputFields,
            int batchSize,
            IDbSetting dbSetting)
            where TEntity : class
        {
            // Get the types
            var typeOfListEntity = typeof(IList<TEntity>);
            var typeOfEntity = typeof(TEntity);

            // Variables for arguments
            var commandParameterExpression = Expression.Parameter(StaticType.DbCommand, "command");
            var entitiesParameterExpression = Expression.Parameter(typeOfListEntity, "entities");

            // Variables for types
            var entityProperties = PropertyCache.Get<TEntity>();

            // Variables for DbCommand
            var dbCommandParametersProperty = StaticType.DbCommand.GetProperty("Parameters");
            var dbCommandCreateParameterMethod = StaticType.DbCommand.GetMethod("CreateParameter");
            var dbParameterParameterNameSetMethod = StaticType.DbParameter.GetProperty("ParameterName").SetMethod;
            var dbParameterValueSetMethod = StaticType.DbParameter.GetProperty("Value").SetMethod;
            var dbParameterDbTypeSetMethod = StaticType.DbParameter.GetProperty("DbType").SetMethod;
            var dbParameterDirectionSetMethod = StaticType.DbParameter.GetProperty("Direction").SetMethod;
            var dbParameterSizeSetMethod = StaticType.DbParameter.GetProperty("Size").SetMethod;
            var dbParameterPrecisionSetMethod = StaticType.DbParameter.GetProperty("Precision").SetMethod;
            var dbParameterScaleSetMethod = StaticType.DbParameter.GetProperty("Scale").SetMethod;

            // Variables for DbParameterCollection
            var dbParameterCollection = Expression.Property(commandParameterExpression, dbCommandParametersProperty);
            var dbParameterCollectionAddMethod = StaticType.DbParameterCollection.GetMethod("Add", new[] { StaticType.Object });
            var dbParameterCollectionClearMethod = StaticType.DbParameterCollection.GetMethod("Clear");

            // Variables for 'Dynamic|Object' object
            var objectGetTypeMethod = StaticType.Object.GetMethod("GetType");
            var typeGetPropertyMethod = StaticType.Type.GetMethod("GetProperty", new[] { StaticType.String, StaticType.BindingFlags });
            var propertyInfoGetValueMethod = StaticType.PropertyInfo.GetMethod("GetValue", new[] { StaticType.Object });

            // Variables for List<T>
            var listIndexerMethod = typeOfListEntity.GetMethod("get_Item", new[] { StaticType.Int32 });

            // Other variables
            var dbTypeResolver = new ClientTypeToDbTypeResolver();

            // Reusable function for input / output fields

            var func = new Func<int, Expression, ParameterExpression, DbField, ClassProperty, bool, ParameterDirection, Expression>((int entityIndex,
                Expression instance,
                ParameterExpression property,
                DbField dbField,
                ClassProperty classProperty,
                bool skipValueAssignment,
                ParameterDirection direction) =>
            {
                // Parameters for the block
                var parameterAssignments = new List<Expression>();

                // Parameter variables
                var parameterName = dbField.Name.AsUnquoted(true, dbSetting).AsAlphaNumeric();
                var parameterVariable = Expression.Variable(StaticType.DbParameter, string.Concat("parameter", parameterName));
                var parameterInstance = Expression.Call(commandParameterExpression, dbCommandCreateParameterMethod);
                parameterAssignments.Add(Expression.Assign(parameterVariable, parameterInstance));

                // Set the name
                var nameAssignment = Expression.Call(parameterVariable, dbParameterParameterNameSetMethod,
                Expression.Constant(entityIndex > 0 ? string.Concat(parameterName, "_", entityIndex) : parameterName));
                parameterAssignments.Add(nameAssignment);

                // Property instance
                var instanceProperty = (PropertyInfo)null;
                var propertyType = (Type)null;
                var fieldType = dbField.Type?.GetUnderlyingType();

                //  Property handlers
                var handlerInstance = (object)null;
                var handlerSetMethod = (MethodInfo)null;

                #region Value

                // Set the value
                if (skipValueAssignment == false)
                {
                    // Set the value
                    var valueAssignment = (Expression)null;

                    // Check the proper type of the entity
                    if (typeOfEntity != StaticType.Object && typeOfEntity.IsGenericType == false)
                    {
                        instanceProperty = classProperty.PropertyInfo;
                    }

                    #region PropertyHandler

                    if (classProperty != null)
                    {
                        handlerInstance = classProperty.GetPropertyHandler();
                    }
                    if (handlerInstance == null && dbField.Type != null)
                    {
                        handlerInstance = PropertyHandlerCache.Get<object>(dbField.Type.GetUnderlyingType());
                    }
                    if (handlerInstance != null)
                    {
                        handlerSetMethod = handlerInstance.GetType().GetMethod("Set");
                    }

                    #endregion

                    #region Instance.Property or PropertyInfo.GetValue()

                    // Set the value
                    var value = (Expression)null;

                    // If the property is missing directly, then it could be a dynamic object
                    if (instanceProperty == null)
                    {
                        value = Expression.Call(property, propertyInfoGetValueMethod, instance);
                    }
                    else
                    {
                        propertyType = instanceProperty?.PropertyType.GetUnderlyingType();

                        if (handlerInstance == null)
                        {
                            if (Converter.ConversionType == ConversionType.Automatic)
                            {
                                var valueToConvert = Expression.Property(instance, instanceProperty);

                                #region StringToGuid

                                // Create a new guid here
                                if (propertyType == StaticType.String && fieldType == StaticType.Guid /* StringToGuid */)
                                {
                                    value = Expression.New(StaticType.Guid.GetConstructor(new[] { StaticType.String }), new[] { valueToConvert });
                                }

                                #endregion

                                #region GuidToString

                                // Call the System.Convert conversion
                                else if (propertyType == StaticType.Guid && fieldType == StaticType.String/* GuidToString*/)
                                {
                                    var convertMethod = StaticType.Convert.GetMethod("ToString", new[] { StaticType.Object });
                                    value = Expression.Call(convertMethod, Expression.Convert(valueToConvert, StaticType.Object));
                                    value = Expression.Convert(value, fieldType);
                                }

                                #endregion

                                else
                                {
                                    value = valueToConvert;
                                }
                            }
                            else
                            {
                                // Get the Class.Property
                                value = Expression.Property(instance, instanceProperty);
                            }

                            #region EnumAsIntForString

                            if (propertyType.IsEnum)
                            {
                                var convertToTypeMethod = (MethodInfo)null;
                                if (convertToTypeMethod == null)
                                {
                                    var mappedToType = classProperty?.GetDbType();
                                    if (mappedToType == null)
                                    {
                                        mappedToType = new ClientTypeToDbTypeResolver().Resolve(dbField.Type);
                                    }
                                    if (mappedToType != null)
                                    {
                                        convertToTypeMethod = StaticType.Convert.GetMethod(string.Concat("To", mappedToType.ToString()), new[] { StaticType.Object });
                                    }
                                }
                                if (convertToTypeMethod == null)
                                {
                                    convertToTypeMethod = StaticType.Convert.GetMethod(string.Concat("To", dbField.Type.Name), new[] { StaticType.Object });
                                }
                                if (convertToTypeMethod == null)
                                {
                                    throw new ConverterNotFoundException($"The convert between '{propertyType.FullName}' and database type '{dbField.DatabaseType}' (of .NET CLR '{dbField.Type.FullName}') is not found.");
                                }
                                else
                                {
                                    var converterMethod = typeof(Compiler.EnumHelper).GetMethod("Convert");
                                    if (converterMethod != null)
                                    {
                                        value = Expression.Call(converterMethod,
                                            Expression.Constant(instanceProperty.PropertyType),
                                            Expression.Constant(dbField.Type),
                                            Expression.Convert(value, StaticType.Object),
                                            Expression.Constant(convertToTypeMethod));
                                    }
                                }
                            }

                            #endregion
                        }
                        else
                        {
                            // Get the value directly from the property
                            value = Expression.Property(instance, instanceProperty);

                            #region PropertyHandler

                            if (handlerInstance != null)
                            {
                                var setParameter = handlerSetMethod.GetParameters().First();
                                value = Expression.Call(Expression.Constant(handlerInstance),
                                    handlerSetMethod,
                                    Expression.Convert(value, setParameter.ParameterType),
                                    Expression.Constant(classProperty));
                            }

                            #endregion
                        }

                        // Convert to object
                        value = Expression.Convert(value, StaticType.Object);
                    }

                    // Declare the variable for the value assignment
                    var valueBlock = (Expression)null;
                    var isNullable = dbField.IsNullable == true ||
                        instanceProperty == null ||
                        (
                            instanceProperty != null &&
                            (
                                instanceProperty.PropertyType.IsValueType == false ||
                                Nullable.GetUnderlyingType(instanceProperty.PropertyType) != null
                            )
                        );

                    // The value for DBNull.Value
                    var dbNullValue = Expression.Convert(Expression.Constant(DBNull.Value), StaticType.Object);

                    // Check if the property is nullable
                    if (isNullable == true)
                    {
                        // Identification of the DBNull
                        var valueVariable = Expression.Variable(StaticType.Object, string.Concat("valueOf", parameterName));
                        var valueIsNull = Expression.Equal(valueVariable, Expression.Constant(null));

                        // Set the propert value
                        valueBlock = Expression.Block(new[] { valueVariable },
                            Expression.Assign(valueVariable, value),
                            Expression.Condition(valueIsNull, dbNullValue, valueVariable));
                    }
                    else
                    {
                        valueBlock = value;
                    }

                    // Add to the collection
                    valueAssignment = Expression.Call(parameterVariable, dbParameterValueSetMethod, valueBlock);

                    #endregion

                    // Check if it is a direct assignment or not
                    if (typeOfEntity != StaticType.Object)
                    {
                        parameterAssignments.Add(valueAssignment);
                    }
                    else
                    {
                        var dbNullValueAssignment = (Expression)null;

                        #region DBNull.Value

                        // Set the default type value
                        if (dbField.IsNullable == false && dbField.Type != null)
                        {
                            dbNullValueAssignment = Expression.Call(parameterVariable, dbParameterValueSetMethod,
                                Expression.Convert(Expression.Default(dbField.Type), StaticType.Object));
                        }

                        // Set the DBNull value
                        if (dbNullValueAssignment == null)
                        {
                            dbNullValueAssignment = Expression.Call(parameterVariable, dbParameterValueSetMethod, dbNullValue);
                        }

                        #endregion

                        // Check the presence of the property
                        var propertyIsNull = Expression.Equal(property, Expression.Constant(null));

                        // Add to parameter assignment
                        parameterAssignments.Add(Expression.Condition(propertyIsNull, dbNullValueAssignment, valueAssignment));
                    }
                }

                #endregion

                #region DbType

                #region DbType

                // Set for non Timestamp, not-working in System.Data.SqlClient but is working at Microsoft.Data.SqlClient
                // It is actually me who file this issue to Microsoft :)
                //if (fieldOrPropertyType != StaticType.TimeSpan)
                //{
                // Identify the DB Type
                var fieldOrPropertyType = (Type)null;
                var dbType = (DbType?)null;

                // Identify the conversion
                if (Converter.ConversionType == ConversionType.Automatic)
                {
                    // Identity the conversion
                    if (propertyType == StaticType.DateTime && fieldType == StaticType.String /* DateTimeToString */ ||
                        propertyType == StaticType.Decimal && (fieldType == StaticType.Single || fieldType == StaticType.Double) /* DecimalToFloat/DecimalToDouble */ ||
                        propertyType == StaticType.Double && fieldType == StaticType.Int64 /* DoubleToBigint */||
                        propertyType == StaticType.Double && fieldType == StaticType.Int32 /* DoubleToBigint */ ||
                        propertyType == StaticType.Double && fieldType == StaticType.Int16 /* DoubleToShort */||
                        propertyType == StaticType.Single && fieldType == StaticType.Int64 /* FloatToBigint */ ||
                        propertyType == StaticType.Single && fieldType == StaticType.Int16 /* FloatToShort */ ||
                        propertyType == StaticType.String && fieldType == StaticType.DateTime /* StringToDate */ ||
                        propertyType == StaticType.String && fieldType == StaticType.Int16 /* StringToShort */ ||
                        propertyType == StaticType.String && fieldType == StaticType.Int32 /* StringToInt */ ||
                        propertyType == StaticType.String && fieldType == StaticType.Int64 /* StringToLong */ ||
                        propertyType == StaticType.String && fieldType == StaticType.Double /* StringToDouble */ ||
                        propertyType == StaticType.String && fieldType == StaticType.Decimal /* StringToDecimal */ ||
                        propertyType == StaticType.String && fieldType == StaticType.Single /* StringToFloat */ ||
                        propertyType == StaticType.String && fieldType == StaticType.Boolean /* StringToBoolean */ ||
                        propertyType == StaticType.String && fieldType == StaticType.Guid /* StringToGuid */ ||
                        propertyType == StaticType.Guid && fieldType == StaticType.String /* GuidToString */)
                    {
                        fieldOrPropertyType = fieldType;
                    }
                    else if (propertyType == StaticType.Guid && fieldType == StaticType.String /* UniqueIdentifierToString */)
                    {
                        fieldOrPropertyType = propertyType;
                    }
                    if (fieldOrPropertyType != null)
                    {
                        dbType = dbTypeResolver.Resolve(fieldOrPropertyType);
                    }
                }

                // Get the class property
                if (dbType == null && handlerInstance == null)
                {
                    if (fieldOrPropertyType != StaticType.SqlVariant && !string.Equals(dbField.DatabaseType, "sql_variant", StringComparison.OrdinalIgnoreCase))
                    {
                        dbType = classProperty?.GetDbType();
                    }
                }

                // Set to normal if null
                if (fieldOrPropertyType == null)
                {
                    fieldOrPropertyType = dbField.Type?.GetUnderlyingType() ?? instanceProperty?.PropertyType.GetUnderlyingType();
                }

                if (fieldOrPropertyType != null)
                {
                    // Get the type mapping
                    if (dbType == null)
                    {
                        dbType = TypeMapper.Get(fieldOrPropertyType);
                    }

                    // Use the resolver
                    if (dbType == null)
                    {
                        dbType = dbTypeResolver.Resolve(fieldOrPropertyType);
                    }
                }

                // Set the DB Type
                if (dbType != null)
                {
                    var dbTypeAssignment = Expression.Call(parameterVariable, dbParameterDbTypeSetMethod, Expression.Constant(dbType));
                    parameterAssignments.Add(dbTypeAssignment);
                }
                //}

                #endregion

                #region SqlDbType (System)

                // Get the SqlDbType value from SystemSqlServerTypeMapAttribute
                var systemSqlServerTypeMapAttribute = GetSystemSqlServerTypeMapAttribute(classProperty);
                if (systemSqlServerTypeMapAttribute != null)
                {
                    var systemSqlDbTypeValue = GetSystemSqlServerDbTypeFromAttribute(systemSqlServerTypeMapAttribute);
                    var systemSqlParameterType = GetSystemSqlServerParameterTypeFromAttribute(systemSqlServerTypeMapAttribute);
                    var dbParameterSystemSqlDbTypeSetMethod = GetSystemSqlServerDbTypeFromAttributeSetMethod(systemSqlServerTypeMapAttribute);
                    var systemSqlDbTypeAssignment = Expression.Call(
                        Expression.Convert(parameterVariable, systemSqlParameterType),
                        dbParameterSystemSqlDbTypeSetMethod,
                        Expression.Constant(systemSqlDbTypeValue));
                    parameterAssignments.Add(systemSqlDbTypeAssignment);
                }

                #endregion

                #region SqlDbType (Microsoft)

                // Get the SqlDbType value from MicrosoftSqlServerTypeMapAttribute
                var microsoftSqlServerTypeMapAttribute = GetMicrosoftSqlServerTypeMapAttribute(classProperty);
                if (microsoftSqlServerTypeMapAttribute != null)
                {
                    var microsoftSqlDbTypeValue = GetMicrosoftSqlServerDbTypeFromAttribute(microsoftSqlServerTypeMapAttribute);
                    var microsoftSqlParameterType = GetMicrosoftSqlServerParameterTypeFromAttribute(microsoftSqlServerTypeMapAttribute);
                    var dbParameterMicrosoftSqlDbTypeSetMethod = GetMicrosoftSqlServerDbTypeFromAttributeSetMethod(microsoftSqlServerTypeMapAttribute);
                    var microsoftSqlDbTypeAssignment = Expression.Call(
                        Expression.Convert(parameterVariable, microsoftSqlParameterType),
                        dbParameterMicrosoftSqlDbTypeSetMethod,
                        Expression.Constant(microsoftSqlDbTypeValue));
                    parameterAssignments.Add(microsoftSqlDbTypeAssignment);
                }

                #endregion

                #region MySqlDbType

                // Get the MySqlDbType value from MySqlDbTypeAttribute
                var mysqlDbTypeTypeMapAttribute = GetMySqlDbTypeTypeMapAttribute(classProperty);
                if (mysqlDbTypeTypeMapAttribute != null)
                {
                    var mySqlDbTypeValue = GetMySqlDbTypeFromAttribute(mysqlDbTypeTypeMapAttribute);
                    var mySqlParameterType = GetMySqlParameterTypeFromAttribute(mysqlDbTypeTypeMapAttribute);
                    var dbParameterMySqlDbTypeSetMethod = GetMySqlDbTypeFromAttributeSetMethod(mysqlDbTypeTypeMapAttribute);
                    var mySqlDbTypeAssignment = Expression.Call(
                        Expression.Convert(parameterVariable, mySqlParameterType),
                        dbParameterMySqlDbTypeSetMethod,
                        Expression.Constant(mySqlDbTypeValue));
                    parameterAssignments.Add(mySqlDbTypeAssignment);
                }

                #endregion

                #region NpgsqlDbType

                // Get the NpgsqlDbType value from NpgsqlTypeMapAttribute
                var npgsqlDbTypeTypeMapAttribute = GetNpgsqlDbTypeTypeMapAttribute(classProperty);
                if (npgsqlDbTypeTypeMapAttribute != null)
                {
                    var npgsqlDbTypeValue = GetNpgsqlDbTypeFromAttribute(npgsqlDbTypeTypeMapAttribute);
                    var npgsqlParameterType = GetNpgsqlParameterTypeFromAttribute(npgsqlDbTypeTypeMapAttribute);
                    var dbParameterNpgsqlDbTypeSetMethod = GetNpgsqlDbTypeFromAttributeSetMethod(npgsqlDbTypeTypeMapAttribute);
                    var npgsqlDbTypeAssignment = Expression.Call(
                        Expression.Convert(parameterVariable, npgsqlParameterType),
                        dbParameterNpgsqlDbTypeSetMethod,
                        Expression.Constant(npgsqlDbTypeValue));
                    parameterAssignments.Add(npgsqlDbTypeAssignment);
                }

                #endregion

                #endregion

                #region Direction

                if (dbSetting.IsDirectionSupported)
                {
                    // Set the Parameter Direction
                    var directionAssignment = Expression.Call(parameterVariable, dbParameterDirectionSetMethod, Expression.Constant(direction));
                    parameterAssignments.Add(directionAssignment);
                }

                #endregion

                #region Size

                // Set only for non-image
                // By default, SQL Server only put (16 size), and that would fail if the user
                // used this type for their binary columns and assign a much longer values
                //if (!string.Equals(field.DatabaseType, "image", StringComparison.OrdinalIgnoreCase))
                //{
                // Set the Size
                if (dbField.Size != null)
                {
                    var sizeAssignment = Expression.Call(parameterVariable, dbParameterSizeSetMethod, Expression.Constant(dbField.Size.Value));
                    parameterAssignments.Add(sizeAssignment);
                }
                //}

                #endregion

                #region Precision

                // Set the Precision
                if (dbField.Precision != null)
                {
                    var precisionAssignment = Expression.Call(parameterVariable, dbParameterPrecisionSetMethod, Expression.Constant(dbField.Precision.Value));
                    parameterAssignments.Add(precisionAssignment);
                }

                #endregion

                #region Scale

                // Set the Scale
                if (dbField.Scale != null)
                {
                    var scaleAssignment = Expression.Call(parameterVariable, dbParameterScaleSetMethod, Expression.Constant(dbField.Scale.Value));
                    parameterAssignments.Add(scaleAssignment);
                }

                #endregion

                // Add the actual addition
                parameterAssignments.Add(Expression.Call(dbParameterCollection, dbParameterCollectionAddMethod, parameterVariable));

                // Return the value
                return Expression.Block(new[] { parameterVariable }, parameterAssignments);
            });

            // Variables for the object instance
            var propertyVariableList = new List<dynamic>();
            var instanceVariable = Expression.Variable(typeOfEntity, "instance");
            var instanceType = Expression.Constant(typeOfEntity); // Expression.Call(instanceVariable, objectGetTypeMethod);
            var instanceTypeVariable = Expression.Variable(StaticType.Type, "instanceType");

            // Input fields properties
            if (inputFields?.Any() == true)
            {
                propertyVariableList.AddRange(inputFields.Select((value, index) => new
                {
                    Index = index,
                    Field = value,
                    Direction = ParameterDirection.Input
                }));
            }

            // Output fields properties
            if (outputFields?.Any() == true)
            {
                propertyVariableList.AddRange(outputFields.Select((value, index) => new
                {
                    Index = index,
                    Field = value,
                    Direction = ParameterDirection.Output
                }));
            }

            // Variables for expression body
            var bodyExpressions = new List<Expression>();

            // Clear the parameter collection first
            bodyExpressions.Add(Expression.Call(dbParameterCollection, dbParameterCollectionClearMethod));

            // Iterate by batch size
            for (var entityIndex = 0; entityIndex < batchSize; entityIndex++)
            {
                // Get the current instance
                var instance = Expression.Call(entitiesParameterExpression, listIndexerMethod, Expression.Constant(entityIndex));
                var instanceExpressions = new List<Expression>();
                var instanceVariables = new List<ParameterExpression>();

                // Entity instance
                instanceVariables.Add(instanceVariable);
                instanceExpressions.Add(Expression.Assign(instanceVariable, instance));

                // Iterate the input fields
                foreach (var item in propertyVariableList)
                {
                    #region Field Expressions

                    // Property variables
                    var propertyExpressions = new List<Expression>();
                    var propertyVariables = new List<ParameterExpression>();
                    var field = (DbField)item.Field;
                    var direction = (ParameterDirection)item.Direction;
                    var propertyIndex = (int)item.Index;
                    var propertyVariable = (ParameterExpression)null;
                    var propertyInstance = (Expression)null;
                    var classProperty = (ClassProperty)null;
                    var propertyName = field.Name.AsUnquoted(true, dbSetting);

                    // Set the proper assignments (property)
                    if (typeOfEntity == StaticType.Object)
                    {
                        propertyVariable = Expression.Variable(StaticType.PropertyInfo, string.Concat("property", propertyName));
                        propertyInstance = Expression.Call(Expression.Call(instanceVariable, objectGetTypeMethod),
                            typeGetPropertyMethod,
                            new[]
                            {
                                Expression.Constant(propertyName),
                                Expression.Constant(BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase)
                            });
                    }
                    else
                    {
                        classProperty = entityProperties.FirstOrDefault(property =>
                            string.Equals(property.GetMappedName().AsUnquoted(true, dbSetting), propertyName.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase));
                        if (classProperty != null)
                        {
                            propertyVariable = Expression.Variable(classProperty.PropertyInfo.PropertyType, string.Concat("property", propertyName));
                            propertyInstance = Expression.Property(instanceVariable, classProperty.PropertyInfo);
                        }
                    }

                    // Execute the function
                    var parameterAssignment = GetParameterAssignmentExpression<TEntity>(commandParameterExpression,
                        entityIndex /* index */,
                        instanceVariable /* instance */,
                        propertyVariable /* property */,
                        field /* field */,
                        classProperty /* classProperty */,
                        direction /* direction */,
                        dbSetting /* dbSetting */);

                    // Add the necessary variables
                    if (propertyVariable != null)
                    {
                        propertyVariables.Add(propertyVariable);
                    }

                    // Add the necessary expressions
                    if (propertyVariable != null)
                    {
                        propertyExpressions.Add(Expression.Assign(propertyVariable, propertyInstance));
                    }
                    propertyExpressions.Add(parameterAssignment);

                    // Add the property block
                    var propertyBlock = Expression.Block(propertyVariables, propertyExpressions);

                    // Add to instance expression
                    instanceExpressions.Add(propertyBlock);

                    #endregion
                }

                // Add to the instance block
                var instanceBlock = Expression.Block(instanceVariables, instanceExpressions);

                // Add to the body
                bodyExpressions.Add(instanceBlock);
            }

            // Set the function value
            return Expression
                .Lambda<Action<DbCommand, IList<TEntity>>>(Expression.Block(bodyExpressions), commandParameterExpression, entitiesParameterExpression)
                .Compile();
        }

    }
}
