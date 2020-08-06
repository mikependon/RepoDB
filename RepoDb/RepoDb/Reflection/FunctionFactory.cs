using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A static factory class used to create a custom function.
    /// </summary>
    internal static class FunctionFactory
    {
        #region GetDataReaderToDataEntityConverterFunction

        /// <summary>
        /// Gets a compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of data entity objects.
        /// </summary>
        /// <typeparam name="TEntity">The data entity object to convert to.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>A compiled function that is used to cover the <see cref="DbDataReader"/> object into a list of data entity objects.</returns>
        public static Func<DbDataReader, TEntity> GetDataReaderToDataEntityConverterFunction<TEntity>(DbDataReader reader,
            IDbConnection connection,
            IDbTransaction transaction)
            where TEntity : class
        {
            // Expression variables
            var readerParameterExpression = Expression.Parameter(typeof(DbDataReader), "reader");
            var newEntityExpression = Expression.New(typeof(TEntity));

            // DB Variables
            var dbSetting = connection.GetDbSetting();
            var dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<TEntity>(), transaction);

            // Matching the fields
            var readerFields = Enumerable.Range(0, reader.FieldCount)
                .Select((index) => reader.GetName(index))
                .Select((name, ordinal) => new DataReaderField
                {
                    Name = name,
                    Ordinal = ordinal,
                    Type = reader.GetFieldType(ordinal),
                    DbField = dbFields?.FirstOrDefault(dbField => string.Equals(dbField.Name.AsUnquoted(true, dbSetting), name, StringComparison.OrdinalIgnoreCase))
                });

            // Get the member assignments
            var memberAssignments = GetMemberAssignmentsForDataEntity<TEntity>(newEntityExpression, readerParameterExpression, readerFields, connection);

            // Throw an error if there are no matching atleast one
            if (memberAssignments.Any() != true)
            {
                throw new MissingFieldsException($"There are no matching fields between the result set of the data reader and the type '{typeof(TEntity).FullName}'.");
            }

            // Initialize the members
            var body = Expression.MemberInit(newEntityExpression, memberAssignments);

            // Set the function value
            return Expression
                .Lambda<Func<DbDataReader, TEntity>>(body, readerParameterExpression)
                .Compile();
        }

        /// <summary>
        /// Returns the list of the bindings for the entity.
        /// </summary>
        /// <typeparam name="TEntity">The target entity type.</typeparam>
        /// <param name="newEntityExpression">The new entity expression.</param>
        /// <param name="readerParameterExpression">The data reader parameter.</param>
        /// <param name="readerFields">The list of fields to be bound from the data reader.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <returns>The enumerable list of member assignment and bindings.</returns>
        private static IEnumerable<MemberAssignment> GetMemberAssignmentsForDataEntity<TEntity>(Expression newEntityExpression,
            ParameterExpression readerParameterExpression,
            IEnumerable<DataReaderField> readerFields,
            IDbConnection connection)
            where TEntity : class
        {
            // Initialize variables
            var memberAssignments = new List<MemberAssignment>();
            var typeOfDbDataReader = typeof(DbDataReader);
            var typeOfDateTime = typeof(DateTime);
            var typeOfTimeSpan = typeof(TimeSpan);
            var typeOfSingle = typeof(Single);
            var isDefaultConversion = TypeMapper.ConversionType == ConversionType.Default;
            var properties = PropertyCache.Get<TEntity>().Where(property => property.PropertyInfo.CanWrite);
            var fieldNames = readerFields.Select(f => f.Name.ToLowerInvariant()).AsList();
            var dbSetting = connection.GetDbSetting();

            // Filter the properties by reader fields
            properties = properties.Where(property =>
                fieldNames.FirstOrDefault(field =>
                    string.Equals(field.AsUnquoted(true, dbSetting), property.GetMappedName().AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) != null);

            // Iterate each properties
            foreach (var property in properties)
            {
                // Gets the mapped name and the ordinal
                var mappedName = property.GetMappedName().AsUnquoted(true, dbSetting);
                var ordinal = fieldNames.IndexOf(mappedName.ToLowerInvariant());

                // Process only if there is a correct ordinal
                if (ordinal >= 0)
                {
                    // Variables needed for the iteration
                    var readerField = readerFields.First(f => string.Equals(f.Name.AsUnquoted(true, dbSetting), mappedName.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase));
                    var underlyingType = Nullable.GetUnderlyingType(property.PropertyInfo.PropertyType);
                    var propertyType = underlyingType ?? property.PropertyInfo.PropertyType;
                    var convertType = readerField.Type;
                    var isConversionNeeded = readerField.Type != propertyType;
                    var isNullable = readerField.DbField == null || readerField.DbField?.IsNullable == true;

                    // Get the correct method info, if the reader.Get<Type> is not found, then use the default GetValue() method
                    var readerGetValueMethod = (MethodInfo)null;

                    // Ignore for the TimeSpan
                    if (propertyType != typeOfTimeSpan)
                    {
                        readerGetValueMethod = typeOfDbDataReader.GetMethod(string.Concat("Get", readerField.Type.Name));
                    }

                    // If null, use the object
                    if (readerGetValueMethod == null)
                    {
                        // Single value is throwing an exception in GetString(), skip it and use the GetValue() instead
                        if (isDefaultConversion == false && readerField.Type != typeOfSingle)
                        {
                            readerGetValueMethod = typeOfDbDataReader.GetMethod(string.Concat("Get", propertyType.Name));
                        }

                        // If present, then use the property type, otherwise, use the object
                        if (readerGetValueMethod != null)
                        {
                            convertType = propertyType;
                        }
                        else
                        {
                            readerGetValueMethod = typeOfDbDataReader.GetMethod("GetValue");
                            convertType = typeof(object);
                        }

                        // Force the conversion flag
                        isConversionNeeded = true;
                    }

                    // Expressions
                    var ordinalExpression = Expression.Constant(ordinal);
                    var valueExpression = (Expression)null;

                    // Check for nullables
                    if (isNullable == true)
                    {
                        var isDbNullExpression = Expression.Call(readerParameterExpression, typeOfDbDataReader.GetMethod("IsDBNull"), ordinalExpression);

                        // True expression
                        var trueExpression = (Expression)null;
                        if (underlyingType != null && underlyingType.IsValueType == true)
                        {
                            trueExpression = Expression.New(typeof(Nullable<>).MakeGenericType(propertyType));
                        }
                        else
                        {
                            trueExpression = Expression.Default(propertyType);
                        }

                        // False expression
                        var falseExpression = (Expression)Expression.Call(readerParameterExpression, readerGetValueMethod, ordinalExpression);

                        // Only if there are conversions, execute the logics inside
                        if (isConversionNeeded == true)
                        {
                            if (isDefaultConversion == true)
                            {
                                if (propertyType.IsEnum)
                                {
                                    #region StringToEnum

                                    if (readerField.Type == typeof(string))
                                    {
                                        var enumParseMethod = typeof(Enum).GetMethod("Parse", new[] { typeof(Type), typeof(string), typeof(bool) });
                                        falseExpression = Expression.Call(enumParseMethod, new[]
                                        {
                                            Expression.Constant(propertyType),
                                            falseExpression,
                                            Expression.Constant(true)
                                        });
                                        falseExpression = Expression.Convert(falseExpression, propertyType);
                                    }

                                    #endregion

                                    #region <Bool|Numbers>ToEnum

                                    else
                                    {
                                        var enumUnderlyingType = Enum.GetUnderlyingType(propertyType);
                                        var enumToObjectMethod = typeof(Enum).GetMethod("ToObject", new[] { typeof(Type), readerField.Type });
                                        if (readerField.Type == typeof(bool))
                                        {
                                            falseExpression = Expression.Convert(falseExpression, typeof(object));
                                        }
                                        falseExpression = Expression.Call(enumToObjectMethod, new[]
                                        {
                                            Expression.Constant(propertyType),
                                            falseExpression
                                        });
                                        falseExpression = Expression.Convert(falseExpression, propertyType);
                                    }

                                    #endregion
                                }
                                else
                                {
                                    #region TimeSpanToDateTime

                                    if (readerField.Type == typeOfDateTime && propertyType == typeOfTimeSpan)
                                    {
                                        falseExpression = Expression.Convert(falseExpression, typeOfDateTime);
                                    }

                                    #endregion

                                    #region Default

                                    else
                                    {
                                        falseExpression = Expression.Convert(falseExpression, propertyType);
                                    }

                                    #endregion
                                }
                            }
                            else
                            {
                                falseExpression = ConvertValueExpressionForDataEntity(falseExpression, readerField, propertyType, convertType);
                            }

                            #region DateTimeToTimeSpan

                            // In SqLite, the Time column is represented as System.DateTime in .NET. If in any case that the models
                            // has been designed to have it as System.TimeSpan, then we should somehow be able to set it properly.

                            if (readerField.Type == typeOfDateTime && propertyType == typeOfTimeSpan)
                            {
                                var timeOfDayProperty = typeof(DateTime).GetProperty("TimeOfDay");
                                falseExpression = Expression.Property(falseExpression, timeOfDayProperty);
                            }

                            #endregion
                        }
                        if (underlyingType != null && underlyingType.IsValueType == true)
                        {
                            var nullableConstructorExpression = typeof(Nullable<>).MakeGenericType(propertyType).GetConstructor(new[] { propertyType });
                            falseExpression = Expression.New(nullableConstructorExpression, falseExpression);
                        }

                        // Set the value
                        valueExpression = Expression.Condition(isDbNullExpression, trueExpression, falseExpression);
                    }
                    else
                    {
                        // Call the actual Get<Type>/GetValue method by ordinal
                        valueExpression = Expression.Call(readerParameterExpression,
                            readerGetValueMethod,
                            ordinalExpression);

                        // Convert to correct type if necessary
                        if (isConversionNeeded == true)
                        {
                            valueExpression = ConvertValueExpressionForDataEntity(valueExpression, readerField, propertyType, convertType);
                        }

                        // Set for the 'Nullable' property
                        if (underlyingType != null && underlyingType.IsValueType == true)
                        {
                            var nullableConstructorExpression = typeof(Nullable<>).MakeGenericType(propertyType).GetConstructor(new[] { propertyType });
                            valueExpression = Expression.New(nullableConstructorExpression, valueExpression);
                        }
                    }

                    // Set the actual property value
                    memberAssignments.Add(Expression.Bind(property.PropertyInfo, valueExpression));
                }
            }

            // Return the result
            return memberAssignments;
        }

        private static Expression ConvertValueExpressionForDataEntity(Expression expression,
            DataReaderField readerField,
            Type propertyType,
            Type convertType)
        {
            if (TypeMapper.ConversionType == ConversionType.Default)
            {
                return Expression.Convert(expression, propertyType);
            }
            else
            {
                var result = (Expression)null;

                // Variables needed
                var targetInstance = (Expression)null;
                var targetMethod = (MethodInfo)null;
                var targetParameter = (Expression)null;

                // Identify if the target type is 'Guid'
                if (propertyType == typeof(Guid) && readerField.Type == typeof(string))
                {
                    // This is (new Guid(string))
                    expression = Expression.New(typeof(Guid).GetConstructor(new[] { typeof(string) }), expression);
                }
                else if (propertyType == typeof(string) && readerField.Type == typeof(Guid))
                {
                    // This is Guid.ToString()
                    targetMethod = typeof(Guid).GetMethod("ToString", new Type[0]);
                    targetInstance = expression;
                    targetParameter = null;
                }
                else
                {
                    // This System.Convert.To<Type>()
                    targetMethod = typeof(Convert).GetMethod(string.Concat("To", propertyType.Name), new[] { convertType });
                    targetInstance = null;
                    targetParameter = expression;
                }

                // If there are methods found from System.Convert(), then use it, otherwise use the normal
                if (targetMethod != null)
                {
                    if (targetParameter == null)
                    {
                        result = Expression.Call(targetInstance, targetMethod);
                    }
                    else
                    {
                        result = Expression.Call(targetInstance, targetMethod, targetParameter);
                    }
                }
                else
                {
                    // There are coersion problem on certain types (i.e: Guid-to-String (vice versa))
                    result = Expression.Convert(expression, propertyType);
                }

                return result;
            }
        }

        #endregion

        #region GetDataReaderToExpandoObjectConverterFunction

        /// <summary>
        /// Gets a compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of dynamic objects.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>A compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of dynamic objects.</returns>
        public static Func<DbDataReader, ExpandoObject> GetDataReaderToExpandoObjectConverterFunction(DbDataReader reader,
            string tableName,
            IDbConnection connection,
            IDbTransaction transaction)
        {
            // Expression variables
            var readerParameterExpression = Expression.Parameter(typeof(DbDataReader), "reader");
            var newObjectExpression = Expression.New(typeof(ExpandoObject));

            // DB Variables
            var dbSetting = connection.GetDbSetting();
            var dbFields = tableName != null ? DbFieldCache.Get(connection, tableName, transaction) : null;

            // Matching the fields
            var readerFields = Enumerable.Range(0, reader.FieldCount)
                .Select(reader.GetName)
                .Select((name, ordinal) => new DataReaderField
                {
                    Name = name,
                    Ordinal = ordinal,
                    Type = reader.GetFieldType(ordinal),
                    DbField = dbFields?.FirstOrDefault(dbField => string.Equals(dbField.Name.AsUnquoted(true, dbSetting), name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase))
                });

            // Initialize the elements
            var elementInits = GetElementInitsForDictionary(readerParameterExpression, readerFields?.AsList());

            // Throw an error if there are no matching atleast one
            if (elementInits.Any() != true)
            {
                throw new EmptyException($"There are no elements initialization found.");
            }

            // Initialize the members
            var body = Expression.ListInit(newObjectExpression, elementInits);

            // Set the function value
            return Expression
                .Lambda<Func<DbDataReader, ExpandoObject>>(body, readerParameterExpression)
                .Compile();
        }

        /// <summary>
        /// Returns the list of the bindings for the object.
        /// </summary>
        /// <param name="readerParameterExpression">The data reader parameter.</param>
        /// <param name="readerFields">The list of fields to be bound from the data reader.</param>
        /// <returns>The enumerable list of child elements initializations.</returns>
        private static IEnumerable<ElementInit> GetElementInitsForDictionary(ParameterExpression readerParameterExpression,
            IList<DataReaderField> readerFields)
        {
            // Initialize variables
            var elementInits = new List<ElementInit>();
            var dataReaderType = typeof(DbDataReader);
            var addMethod = typeof(IDictionary<string, object>).GetMethod("Add", new[] { typeof(string), typeof(object) });

            // Iterate each properties
            for (var ordinal = 0; ordinal < readerFields?.Count; ordinal++)
            {
                // Field variable
                var readerField = readerFields[ordinal];
                var isConversionNeeded = false;

                // Get the correct method info, if the reader.Get<Type> is not found, then use the default GetValue
                var readerGetValueMethod = dataReaderType.GetMethod(string.Concat("Get", readerField.Type.Name));
                if (readerGetValueMethod == null)
                {
                    readerGetValueMethod = dataReaderType.GetMethod("GetValue");
                    isConversionNeeded = true;
                }

                // Expressions
                var ordinalExpression = Expression.Constant(ordinal);
                var valueExpression = (Expression)Expression.Call(readerParameterExpression, readerGetValueMethod, ordinalExpression);

                // Check for nullables
                if (readerField.DbField == null || readerField.DbField?.IsNullable == true)
                {
                    var isDbNullExpression = Expression.Call(readerParameterExpression, dataReaderType.GetMethod("IsDBNull"), ordinalExpression);
                    var trueExpression = (Expression)null;
                    if (readerField.Type.IsValueType == true)
                    {
                        trueExpression = Expression.Constant(null, typeof(object));
                        valueExpression = Expression.Convert(valueExpression, typeof(object));
                    }
                    else
                    {
                        trueExpression = Expression.Default(readerField.Type);
                        if (isConversionNeeded == true)
                        {
                            valueExpression = Expression.Convert(valueExpression, readerField.Type);
                        }
                    }
                    valueExpression = Expression.Condition(isDbNullExpression, trueExpression, valueExpression);
                }

                // Add to the bindings
                var values = new[]
                {
                    Expression.Constant(readerField.Name),
                    (Expression)Expression.Convert(valueExpression, typeof(object))
                };
                elementInits.Add(Expression.ElementInit(addMethod, values));
            }

            // Return the result
            return elementInits;
        }

        #endregion

        #region GetDataEntityDbCommandParameterSetterFunction

        /// <summary>
        /// Gets a compiled function that is used to set the <see cref="DbParameter"/> objects of the <see cref="DbCommand"/> object based from the values of the data entity/dynamic object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity objects.</typeparam>
        /// <param name="inputFields">The list of the input <see cref="DbField"/> objects.</param>
        /// <param name="outputFields">The list of the output <see cref="DbField"/> objects.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The compiled function.</returns>
        public static Action<DbCommand, TEntity> GetDataEntityDbCommandParameterSetterFunction<TEntity>(IEnumerable<DbField> inputFields,
            IEnumerable<DbField> outputFields,
            IDbSetting dbSetting)
            where TEntity : class
        {
            // Get the types
            var typeOfDbCommand = typeof(DbCommand);
            var typeOfEntity = typeof(TEntity);
            var typeOfObject = typeof(object);
            var typeOfDbParameter = typeof(DbParameter);
            var typeOfDbParameterCollection = typeof(DbParameterCollection);
            var typeOfInt = typeof(int);
            var typeOfString = typeof(string);
            var typeOfType = typeof(Type);
            var typeOfPropertyInfo = typeof(PropertyInfo);
            var typeOfBytes = typeof(byte[]);
            var typeOfTimeSpan = typeof(TimeSpan);
            var typeOfBindingFlags = typeof(BindingFlags);
            var typeOfGuid = typeof(Guid);
            var typeOfDateTime = typeof(DateTime);
            var typeOfDecimal = typeof(Decimal);
            var typeOfFloat = typeof(float);
            var typeOfLong = typeof(long);
            var typeOfDouble = typeof(Double);
            var typeOfShort = typeof(short);
            var typeOfConvert = typeof(Convert);

            // Variables for arguments
            var commandParameterExpression = Expression.Parameter(typeOfDbCommand, "command");
            var entityParameterExpression = Expression.Parameter(typeOfEntity, "entity");

            // Variables for types
            var entityProperties = PropertyCache.Get<TEntity>();

            // Variables for DbCommand
            var dbCommandParametersProperty = typeOfDbCommand.GetProperty("Parameters");
            var dbCommandCreateParameterMethod = typeOfDbCommand.GetMethod("CreateParameter");
            var dbParameterParameterNameSetMethod = typeOfDbParameter.GetProperty("ParameterName").SetMethod;
            var dbParameterValueSetMethod = typeOfDbParameter.GetProperty("Value").SetMethod;
            var dbParameterDbTypeSetMethod = typeOfDbParameter.GetProperty("DbType").SetMethod;
            var dbParameterDirectionSetMethod = typeOfDbParameter.GetProperty("Direction").SetMethod;
            var dbParameterSizeSetMethod = typeOfDbParameter.GetProperty("Size").SetMethod;
            var dbParameterPrecisionSetMethod = typeOfDbParameter.GetProperty("Precision").SetMethod;
            var dbParameterScaleSetMethod = typeOfDbParameter.GetProperty("Scale").SetMethod;

            // Variables for DbParameterCollection
            var dbParameterCollection = Expression.Property(commandParameterExpression, dbCommandParametersProperty);
            var dbParameterCollectionAddMethod = typeOfDbParameterCollection.GetMethod("Add", new[] { typeOfObject });
            var dbParameterCollectionClearMethod = typeOfDbParameterCollection.GetMethod("Clear");

            // Variables for 'Dynamic|Object' object
            var objectGetTypeMethod = typeOfObject.GetMethod("GetType");
            var typeGetPropertyMethod = typeOfType.GetMethod("GetProperty", new[] { typeOfString, typeOfBindingFlags });
            var propertyInfoGetValueMethod = typeOfPropertyInfo.GetMethod("GetValue", new[] { typeOfObject });

            // Other variables
            var dbTypeResolver = new ClientTypeToDbTypeResolver();

            // Reusable function for input/output fields
            var func = new Func<Expression, ParameterExpression, DbField, ClassProperty, bool, ParameterDirection, Expression>((Expression instance,
                ParameterExpression property,
                DbField field,
                ClassProperty classProperty,
                bool skipValueAssignment,
                ParameterDirection direction) =>
            {
                // Parameters for the block
                var parameterAssignments = new List<Expression>();

                // Parameter variables
                var parameterName = field.Name.AsUnquoted(true, dbSetting).AsAlphaNumeric();
                var parameterVariable = Expression.Variable(typeOfDbParameter, string.Concat("parameter", parameterName));
                var parameterInstance = Expression.Call(commandParameterExpression, dbCommandCreateParameterMethod);
                parameterAssignments.Add(Expression.Assign(parameterVariable, parameterInstance));

                // Set the name
                var nameAssignment = Expression.Call(parameterVariable, dbParameterParameterNameSetMethod, Expression.Constant(parameterName));
                parameterAssignments.Add(nameAssignment);

                // Property instance
                var instanceProperty = (PropertyInfo)null;
                var propertyType = (Type)null;
                var fieldType = field.Type?.GetUnderlyingType();

                #region Value

                // Set the value
                if (skipValueAssignment == false)
                {
                    // Set the value
                    var valueAssignment = (Expression)null;

                    // Check the proper type of the entity
                    if (typeOfEntity != typeOfObject && typeOfEntity.IsGenericType == false)
                    {
                        instanceProperty = classProperty.PropertyInfo; // typeOfEntity.GetProperty(classProperty.PropertyInfo.Name);
                    }

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
                        propertyType = instanceProperty.PropertyType.GetUnderlyingType();

                        // Parse with Guid if necessary
                        if (TypeMapper.ConversionType == ConversionType.Automatic)
                        {
                            #region StringToGuid

                            var valueToConvert = Expression.Property(instance, instanceProperty);

                            // Create a new guid here
                            if (propertyType == typeOfString && fieldType == typeOfGuid /* StringToGuid */)
                            {
                                value = Expression.New(typeOfGuid.GetTypeInfo().GetConstructor(new[] { typeOfString }), new[] { valueToConvert });
                            }
                            else
                            {
                                value = valueToConvert;
                            }

                            #endregion
                        }
                        else
                        {
                            // Get the Class.Property
                            value = Expression.Property(instance, instanceProperty);
                        }

                        #region EnumAsIntForString

                        if (propertyType.IsEnum)
                        {
                            var mappedToType = classProperty?.GetDbType();
                            if (mappedToType != null && mappedToType != DbType.String)
                            {
                                var convertToTypeMethod = typeOfConvert.GetMethod(string.Concat("To", mappedToType.ToString()), new[] { typeOfObject });
                                if (convertToTypeMethod != null)
                                {
                                    value = Expression.Call(convertToTypeMethod, Expression.Convert(value, typeOfObject));
                                }
                            }
                        }

                        #endregion

                        // Convert to object
                        value = Expression.Convert(value, typeOfObject);
                    }

                    // Declare the variable for the value assignment
                    var valueBlock = (Expression)null;
                    var isNullable = field.IsNullable == true ||
                        instanceProperty == null ||
                        (
                            instanceProperty != null &&
                            (
                                instanceProperty.PropertyType.IsValueType == false ||
                                Nullable.GetUnderlyingType(instanceProperty.PropertyType) != null
                            )
                        );

                    // The value for DBNull.Value
                    var dbNullValue = Expression.Convert(Expression.Constant(DBNull.Value), typeOfObject);

                    // Check if the property is nullable
                    if (isNullable == true)
                    {
                        // Identification of the DBNull
                        var valueVariable = Expression.Variable(typeOfObject, string.Concat("valueOf", parameterName));
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
                    if (typeOfEntity != typeOfObject)
                    {
                        parameterAssignments.Add(valueAssignment);
                    }
                    else
                    {
                        var dbNullValueAssignment = (Expression)null;

                        #region DBNull.Value

                        // Set the default type value
                        if (field.IsNullable == false && field.Type != null)
                        {
                            dbNullValueAssignment = Expression.Call(parameterVariable, dbParameterValueSetMethod,
                                Expression.Convert(Expression.Default(field.Type), typeOfObject));
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

                // Identify the DB Type
                var fieldOrPropertyType = (Type)null;

                // Identify the conversion
                if (TypeMapper.ConversionType == ConversionType.Automatic)
                {
                    // Identity the conversion
                    if (propertyType == typeOfDateTime && fieldType == typeOfString /* DateTimeToString */ ||
                propertyType == typeOfDecimal && fieldType == typeOfFloat /* DecimalToFloat */ ||
                propertyType == typeOfDouble && fieldType == typeOfLong /* DoubleToBigint */||
                propertyType == typeOfDouble && fieldType == typeOfInt /* DoubleToBigint */ ||
                propertyType == typeOfDouble && fieldType == typeOfShort /* DoubleToShort */||
                propertyType == typeOfFloat && fieldType == typeOfLong /* FloatToBigint */ ||
                propertyType == typeOfFloat && fieldType == typeOfShort /* FloatToShort */ ||
                propertyType == typeOfGuid && fieldType == typeOfString /* UniqueIdentifierToString */)
                    {
                        fieldOrPropertyType = propertyType;
                    }
                }

                // Set to normal if null
                if (fieldOrPropertyType == null)
                {
                    fieldOrPropertyType = field.Type?.GetUnderlyingType() ?? instanceProperty?.PropertyType.GetUnderlyingType();
                }

                // Set for non Timestamp
                if (fieldOrPropertyType != typeOfTimeSpan)
                {
                    var dbType = (DbType?)null;

                    // Get the class property
                    if (propertyType?.IsEnum == true)
                    {
                        dbType = classProperty?.GetDbType();
                    }

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

                    // Set the DB Type
                    if (dbType != null)
                    {
                        var dbTypeAssignment = Expression.Call(parameterVariable, dbParameterDbTypeSetMethod, Expression.Constant(dbType));
                        parameterAssignments.Add(dbTypeAssignment);
                    }
                }

                #endregion

                #region Direction

                if (dbSetting.IsDbParameterDirectionSettingSupported)
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
                if (!string.Equals(field.DatabaseType, "image", StringComparison.OrdinalIgnoreCase))
                {
                    // Set the Size
                    if (field.Size != null)
                    {
                        var sizeAssignment = Expression.Call(parameterVariable, dbParameterSizeSetMethod, Expression.Constant(field.Size.Value));
                        parameterAssignments.Add(sizeAssignment);
                    }
                }

                #endregion

                #region Precision

                // Set the Precision
                if (field.Precision != null)
                {
                    var precisionAssignment = Expression.Call(parameterVariable, dbParameterPrecisionSetMethod, Expression.Constant(field.Precision.Value));
                    parameterAssignments.Add(precisionAssignment);
                }

                #endregion

                #region Scale

                // Set the Scale
                if (field.Scale != null)
                {
                    var scaleAssignment = Expression.Call(parameterVariable, dbParameterScaleSetMethod, Expression.Constant(field.Scale.Value));
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
            var instanceType = Expression.Constant(typeOfEntity);
            var instanceTypeVariable = Expression.Variable(typeOfType, "instanceType");

            // Input fields properties
            if (inputFields?.Any() == true)
            {
                for (var index = 0; index < inputFields.Count(); index++)
                {
                    propertyVariableList.Add(new
                    {
                        Index = index,
                        Field = inputFields.ElementAt(index),
                        Direction = ParameterDirection.Input
                    });
                }
            }

            // Output fields properties
            if (outputFields?.Any() == true)
            {
                for (var index = 0; index < outputFields.Count(); index++)
                {
                    propertyVariableList.Add(new
                    {
                        Index = inputFields.Count() + index,
                        Field = outputFields.ElementAt(index),
                        Direction = ParameterDirection.Output
                    });
                }
            }

            // Variables for expression body
            var bodyExpressions = new List<Expression>();

            // Clear the parameter collection first
            bodyExpressions.Add(Expression.Call(dbParameterCollection, dbParameterCollectionClearMethod));

            // Get the current instance
            var instanceExpressions = new List<Expression>();
            var instanceVariables = new List<ParameterExpression>();

            // Entity instance
            instanceVariables.Add(instanceVariable);
            instanceExpressions.Add(Expression.Assign(instanceVariable, entityParameterExpression));

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
                if (typeOfEntity == typeOfObject)
                {
                    propertyVariable = Expression.Variable(typeOfPropertyInfo, string.Concat("property", propertyName));
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
                    classProperty = entityProperties.First(property => string.Equals(property.GetMappedName().AsUnquoted(true, dbSetting), propertyName.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase));
                    if (classProperty != null)
                    {
                        propertyVariable = Expression.Variable(classProperty.PropertyInfo.PropertyType, string.Concat("property", propertyName));
                        propertyInstance = Expression.Property(instanceVariable, classProperty.PropertyInfo);
                    }
                }

                // Execute the function
                var parameterAssignment = func(instanceVariable /* instance */,
                    propertyVariable /* property */,
                    field /* field */,
                    classProperty /* classProperty */,
                    (direction == ParameterDirection.Output) /* skipValueAssignment */,
                    direction /* direction */);

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

            // Set the function value
            return Expression
                .Lambda<Action<DbCommand, TEntity>>(Expression.Block(bodyExpressions), commandParameterExpression, entityParameterExpression)
                .Compile();
        }

        #endregion

        #region GetDataEntitiesDbCommandParameterSetterFunction

        /// <summary>
        /// Gets a compiled function that is used to set the <see cref="DbParameter"/> objects of the <see cref="DbCommand"/> object based from the values of the data entity/dynamic objects.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity objects.</typeparam>
        /// <param name="inputFields">The list of the input <see cref="DbField"/> objects.</param>
        /// <param name="outputFields">The list of the input <see cref="DbField"/> objects.</param>
        /// <param name="batchSize">The batch size of the entity to be passed.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The compiled function.</returns>
        public static Action<DbCommand, IList<TEntity>> GetDataEntitiesDbCommandParameterSetterFunction<TEntity>(IEnumerable<DbField> inputFields,
            IEnumerable<DbField> outputFields,
            int batchSize,
            IDbSetting dbSetting)
            where TEntity : class
        {
            // Get the types
            var typeOfDbCommand = typeof(DbCommand);
            var typeOfListEntity = typeof(IList<TEntity>);
            var typeOfEntity = typeof(TEntity);
            var typeOfObject = typeof(object);
            var typeOfDbParameter = typeof(DbParameter);
            var typeOfDbParameterCollection = typeof(DbParameterCollection);
            var typeOfInt = typeof(int);
            var typeOfString = typeof(string);
            var typeOfType = typeof(Type);
            var typeOfPropertyInfo = typeof(PropertyInfo);
            var typeOfTimeSpan = typeof(TimeSpan);
            var typeOfBindingFlags = typeof(BindingFlags);
            var typeOfGuid = typeof(Guid);
            var typeOfDateTime = typeof(DateTime);
            var typeOfDecimal = typeof(Decimal);
            var typeOfFloat = typeof(float);
            var typeOfLong = typeof(long);
            var typeOfDouble = typeof(Double);
            var typeOfShort = typeof(short);
            var typeOfConvert = typeof(Convert);

            // Variables for arguments
            var commandParameterExpression = Expression.Parameter(typeOfDbCommand, "command");
            var entitiesParameterExpression = Expression.Parameter(typeOfListEntity, "entities");

            // Variables for types
            var entityProperties = PropertyCache.Get<TEntity>();

            // Variables for DbCommand
            var dbCommandParametersProperty = typeOfDbCommand.GetProperty("Parameters");
            var dbCommandCreateParameterMethod = typeOfDbCommand.GetMethod("CreateParameter");
            var dbParameterParameterNameSetMethod = typeOfDbParameter.GetProperty("ParameterName").SetMethod;
            var dbParameterValueSetMethod = typeOfDbParameter.GetProperty("Value").SetMethod;
            var dbParameterDbTypeSetMethod = typeOfDbParameter.GetProperty("DbType").SetMethod;
            var dbParameterDirectionSetMethod = typeOfDbParameter.GetProperty("Direction").SetMethod;
            var dbParameterSizeSetMethod = typeOfDbParameter.GetProperty("Size").SetMethod;
            var dbParameterPrecisionSetMethod = typeOfDbParameter.GetProperty("Precision").SetMethod;
            var dbParameterScaleSetMethod = typeOfDbParameter.GetProperty("Scale").SetMethod;

            // Variables for DbParameterCollection
            var dbParameterCollection = Expression.Property(commandParameterExpression, dbCommandParametersProperty);
            var dbParameterCollectionAddMethod = typeOfDbParameterCollection.GetMethod("Add", new[] { typeOfObject });
            var dbParameterCollectionClearMethod = typeOfDbParameterCollection.GetMethod("Clear");

            // Variables for 'Dynamic|Object' object
            var objectGetTypeMethod = typeOfObject.GetMethod("GetType");
            var typeGetPropertyMethod = typeOfType.GetMethod("GetProperty", new[] { typeOfString, typeOfBindingFlags });
            var propertyInfoGetValueMethod = typeOfPropertyInfo.GetMethod("GetValue", new[] { typeOfObject });

            // Variables for List<T>
            var listIndexerMethod = typeOfListEntity.GetMethod("get_Item", new[] { typeOfInt });

            // Other variables
            var dbTypeResolver = new ClientTypeToDbTypeResolver();

            // Reusable function for input/output fields
            var func = new Func<int, Expression, ParameterExpression, DbField, ClassProperty, bool, ParameterDirection, Expression>((int entityIndex,
                Expression instance,
                ParameterExpression property,
                DbField field,
                ClassProperty classProperty,
                bool skipValueAssignment,
                ParameterDirection direction) =>
            {
                // Parameters for the block
                var parameterAssignments = new List<Expression>();

                // Parameter variables
                var parameterName = field.Name.AsUnquoted(true, dbSetting).AsAlphaNumeric();
                var parameterVariable = Expression.Variable(typeOfDbParameter, string.Concat("parameter", parameterName));
                var parameterInstance = Expression.Call(commandParameterExpression, dbCommandCreateParameterMethod);
                parameterAssignments.Add(Expression.Assign(parameterVariable, parameterInstance));

                // Set the name
                var nameAssignment = Expression.Call(parameterVariable, dbParameterParameterNameSetMethod,
                    Expression.Constant(entityIndex > 0 ? string.Concat(parameterName, "_", entityIndex) : parameterName));
                parameterAssignments.Add(nameAssignment);

                // Property instance
                var instanceProperty = (PropertyInfo)null;
                var propertyType = (Type)null;
                var fieldType = field.Type?.GetUnderlyingType();

                #region Value

                // Set the value
                if (skipValueAssignment == false)
                {
                    // Set the value
                    var valueAssignment = (Expression)null;

                    // Check the proper type of the entity
                    if (typeOfEntity != typeOfObject && typeOfEntity.IsGenericType == false)
                    {
                        instanceProperty = classProperty.PropertyInfo; // typeOfEntity.GetProperty(classProperty.PropertyInfo.Name);
                    }

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

                        // Parse with Guid if necessary
                        if (TypeMapper.ConversionType == ConversionType.Automatic)
                        {
                            #region StringToGuid

                            var valueToConvert = Expression.Property(instance, instanceProperty);

                            // Create a new guid here
                            if (propertyType == typeOfString && fieldType == typeOfGuid /* StringToGuid */)
                            {
                                value = Expression.New(typeOfGuid.GetTypeInfo().GetConstructor(new[] { typeOfString }), new[] { valueToConvert });
                            }
                            else
                            {
                                value = valueToConvert;
                            }

                            #endregion
                        }
                        else
                        {
                            // Get the Class.Property
                            value = Expression.Property(instance, instanceProperty);
                        }

                        #region EnumAsIntForString

                        if (propertyType.IsEnum)
                        {
                            var mappedToType = classProperty?.GetDbType();
                            if (mappedToType != null && mappedToType != DbType.String)
                            {
                                var convertToTypeMethod = typeOfConvert.GetMethod(string.Concat("To", mappedToType.ToString()), new[] { typeOfObject });
                                if (convertToTypeMethod != null)
                                {
                                    value = Expression.Call(convertToTypeMethod, Expression.Convert(value, typeOfObject));
                                }
                            }
                        }

                        #endregion

                        // Convert to object
                        value = Expression.Convert(value, typeOfObject);
                    }

                    // Declare the variable for the value assignment
                    var valueBlock = (Expression)null;
                    var isNullable = field.IsNullable == true ||
                        instanceProperty == null ||
                        (
                            instanceProperty != null &&
                            (
                                instanceProperty.PropertyType.IsValueType == false ||
                                Nullable.GetUnderlyingType(instanceProperty.PropertyType) != null
                            )
                        );

                    // The value for DBNull.Value
                    var dbNullValue = Expression.Convert(Expression.Constant(DBNull.Value), typeOfObject);

                    // Check if the property is nullable
                    if (isNullable == true)
                    {
                        // Identification of the DBNull
                        var valueVariable = Expression.Variable(typeOfObject, string.Concat("valueOf", parameterName));
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
                    if (typeOfEntity != typeOfObject)
                    {
                        parameterAssignments.Add(valueAssignment);
                    }
                    else
                    {
                        var dbNullValueAssignment = (Expression)null;

                        #region DBNull.Value

                        // Set the default type value
                        if (field.IsNullable == false && field.Type != null)
                        {
                            dbNullValueAssignment = Expression.Call(parameterVariable, dbParameterValueSetMethod,
                                Expression.Convert(Expression.Default(field.Type), typeOfObject));
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

                // Identify the DB Type
                var fieldOrPropertyType = (Type)null;

                // Identify the conversion
                if (TypeMapper.ConversionType == ConversionType.Automatic)
                {
                    // Identity the conversion
                    if (propertyType == typeOfDateTime && fieldType == typeOfString /* DateTimeToString */ ||
                propertyType == typeOfDecimal && fieldType == typeOfFloat /* DecimalToFloat */ ||
                propertyType == typeOfDouble && fieldType == typeOfLong /* DoubleToBigint */||
                propertyType == typeOfDouble && fieldType == typeOfInt /* DoubleToBigint */ ||
                propertyType == typeOfDouble && fieldType == typeOfShort /* DoubleToShort */||
                propertyType == typeOfFloat && fieldType == typeOfLong /* FloatToBigint */ ||
                propertyType == typeOfFloat && fieldType == typeOfShort /* FloatToShort */ ||
                propertyType == typeOfGuid && fieldType == typeOfString /* UniqueIdentifierToString */)
                    {
                        fieldOrPropertyType = propertyType;
                    }
                }

                // Set to normal if null
                if (fieldOrPropertyType == null)
                {
                    fieldOrPropertyType = field.Type?.GetUnderlyingType() ?? instanceProperty?.PropertyType.GetUnderlyingType();
                }

                // Set for non Timestamp
                if (fieldOrPropertyType != typeOfTimeSpan)
                {
                    var dbType = (DbType?)null;

                    // Get the class property db type
                    if (propertyType?.IsEnum == true)
                    {
                        dbType = classProperty?.GetDbType();
                    }

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

                    // Set the DB Type
                    if (dbType != null)
                    {
                        var dbTypeAssignment = Expression.Call(parameterVariable, dbParameterDbTypeSetMethod, Expression.Constant(dbType));
                        parameterAssignments.Add(dbTypeAssignment);
                    }
                }

                #endregion

                #region Direction

                if (dbSetting.IsDbParameterDirectionSettingSupported)
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
                if (!string.Equals(field.DatabaseType, "image", StringComparison.OrdinalIgnoreCase))
                {
                    // Set the Size
                    if (field.Size != null)
                    {
                        var sizeAssignment = Expression.Call(parameterVariable, dbParameterSizeSetMethod, Expression.Constant(field.Size.Value));
                        parameterAssignments.Add(sizeAssignment);
                    }
                }

                #endregion

                #region Precision

                // Set the Precision
                if (field.Precision != null)
                {
                    var precisionAssignment = Expression.Call(parameterVariable, dbParameterPrecisionSetMethod, Expression.Constant(field.Precision.Value));
                    parameterAssignments.Add(precisionAssignment);
                }

                #endregion

                #region Scale

                // Set the Scale
                if (field.Scale != null)
                {
                    var scaleAssignment = Expression.Call(parameterVariable, dbParameterScaleSetMethod, Expression.Constant(field.Scale.Value));
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
            var instanceTypeVariable = Expression.Variable(typeOfType, "instanceType");

            // Input fields properties
            if (inputFields?.Any() == true)
            {
                for (var index = 0; index < inputFields.Count(); index++)
                {
                    propertyVariableList.Add(new
                    {
                        Index = index,
                        Field = inputFields.ElementAt(index),
                        Direction = ParameterDirection.Input
                    });
                }
            }

            // Output fields properties
            if (outputFields?.Any() == true)
            {
                for (var index = 0; index < outputFields.Count(); index++)
                {
                    propertyVariableList.Add(new
                    {
                        Index = inputFields.Count() + index,
                        Field = outputFields.ElementAt(index),
                        Direction = ParameterDirection.Output
                    });
                }
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
                    if (typeOfEntity == typeOfObject)
                    {
                        propertyVariable = Expression.Variable(typeOfPropertyInfo, string.Concat("property", propertyName));
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
                    var parameterAssignment = func(entityIndex /* index */,
                        instanceVariable /* instance */,
                        propertyVariable /* property */,
                        field /* field */,
                        classProperty /* classProperty */,
                        (direction == ParameterDirection.Output) /* skipValueAssignment */,
                        direction /* direction */);

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

        #endregion

        #region GetDataEntityPropertySetterFromDbCommandParameterFunction

        /// <summary>
        /// Gets a compiled function that is used to set the data entity object property value based from the value of <see cref="DbCommand"/> parameter object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="field">The target <see cref="Field"/>.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="index">The index of the batches.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>A compiled function that is used to set the data entity object property value based from the value of <see cref="DbCommand"/> parameter object.</returns>
        public static Action<TEntity, DbCommand> GetDataEntityPropertySetterFromDbCommandParameterFunction<TEntity>(Field field,
            string parameterName,
            int index,
            IDbSetting dbSetting)
            where TEntity : class
        {
            // Variables for type
            var typeOfEntity = typeof(TEntity);
            var typeOfDbCommand = typeof(DbCommand);
            var typeOfDbParameterCollection = typeof(DbParameterCollection);
            var typeOfString = typeof(string);
            var typeOfDbParameter = typeof(DbParameter);

            // Variables for argument
            var entityParameterExpression = Expression.Parameter(typeOfEntity, "entity");
            var dbCommandParameterExpression = Expression.Parameter(typeOfDbCommand, "command");

            // Variables for DbCommand
            var dbCommandParametersProperty = typeOfDbCommand.GetProperty("Parameters");

            // Variables for DbParameterCollection
            var dbParameterCollectionIndexerMethod = typeOfDbParameterCollection.GetMethod("get_Item", new[] { typeOfString });

            // Variables for DbParameter
            var dbParameterValueProperty = typeOfDbParameter.GetProperty("Value");

            // Get the entity property
            var propertyName = field.Name.AsUnquoted(true, dbSetting).AsAlphaNumeric();
            var property = (typeOfEntity.GetProperty(propertyName) ?? typeOfEntity.GetPropertyByMapping(propertyName)?.PropertyInfo)?.SetMethod;

            // Get the command parameter
            var name = parameterName ?? propertyName;
            var parameters = Expression.Property(dbCommandParameterExpression, dbCommandParametersProperty);
            var parameter = Expression.Call(parameters, dbParameterCollectionIndexerMethod,
                Expression.Constant(index > 0 ? string.Concat(name, "_", index) : name));

            // Assign the Parameter.Value into DataEntity.Property
            var value = Expression.Property(parameter, dbParameterValueProperty);
            var propertyAssignment = Expression.Call(entityParameterExpression, property,
                Expression.Convert(value, field.Type?.GetUnderlyingType()));

            // Return function
            return Expression.Lambda<Action<TEntity, DbCommand>>(
                propertyAssignment, entityParameterExpression, dbCommandParameterExpression).Compile();
        }

        #endregion

        #region GetDataEntityPropertyValueSetterFunction

        /// <summary>
        /// Gets a compiled function that is used to set the data entity object property value.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="field">The target <see cref="Field"/>.</param>
        /// <returns>A compiled function that is used to set the data entity object property value.</returns>
        public static Action<TEntity, object> GetDataEntityPropertyValueSetterFunction<TEntity>(Field field)
            where TEntity : class
        {
            // Variables for type
            var typeOfEntity = typeof(TEntity);
            var typeOfObject = typeof(object);

            // Variables for argument
            var entityParameter = Expression.Parameter(typeOfEntity, "entity");
            var valueParameter = Expression.Parameter(typeOfObject, "value");

            // Get the entity property
            var property = (typeOfEntity.GetProperty(field.Name) ?? typeOfEntity.GetPropertyByMapping(field.Name)?.PropertyInfo)?.SetMethod;

            // Assign the value into DataEntity.Property
            var propertyAssignment = Expression.Call(entityParameter, property,
                Expression.Convert(valueParameter, field.Type));

            // Return function
            return Expression.Lambda<Action<TEntity, object>>(propertyAssignment,
                entityParameter, valueParameter).Compile();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Create the <see cref="DbCommand"/> parameters based on the list of <see cref="DbField"/> objects.
        /// </summary>
        /// <param name="command">The target <see cref="DbCommand"/> object.</param>
        /// <param name="inputFields">The list of the input <see cref="DbField"/> objects.</param>
        /// <param name="outputFields">The list of the output <see cref="DbField"/> objects.</param>
        /// <param name="batchSize">The batch size of the entities to be passed.</param>
        internal static void CreateDbCommandParametersFromFields(DbCommand command,
            IEnumerable<DbField> inputFields,
            IEnumerable<DbField> outputFields,
            int batchSize)
        {
            // Variables
            var dbTypeResolver = new ClientTypeToDbTypeResolver();
            var typeOfBytes = typeof(byte[]);
            var dbSetting = command.Connection.GetDbSetting();

            // Clear the parameters
            command.Parameters.Clear();

            // Function for each field
            var func = new Action<int, DbField, ParameterDirection>((int index,
                DbField field,
                ParameterDirection direction) =>
            {
                // Create the parameter
                var parameter = command.CreateParameter();

                // Set the property
                parameter.ParameterName = field.Name.AsParameter(index, dbSetting);

                // Set the Direction
                if (dbSetting.IsDbParameterDirectionSettingSupported)
                {
                    parameter.Direction = direction;
                }

                // Set the DB Type
                var dbType = TypeMapper.Get(field.Type?.GetUnderlyingType());

                // Ensure the type mapping
                if (dbType == null)
                {
                    if (field.Type == typeOfBytes)
                    {
                        dbType = DbType.Binary;
                    }
                }

                // Resolve manually
                if (dbType == null)
                {
                    dbType = dbTypeResolver.Resolve(field.Type);
                }

                // Set the DB Type if present
                if (dbType != null)
                {
                    parameter.DbType = dbType.Value;
                }

                // Set the Size if present
                if (field.Size != null)
                {
                    parameter.Size = field.Size.Value;
                }

                // Set the Precision if present
                if (field.Precision != null)
                {
                    parameter.Precision = field.Precision.Value;
                }

                // Set the Scale if present
                if (field.Scale != null)
                {
                    parameter.Scale = field.Scale.Value;
                }

                // Add the parameter
                command.Parameters.Add(parameter);
            });

            for (var index = 0; index < batchSize; index++)
            {
                // Iterate all the input fields
                if (inputFields?.Any() == true)
                {
                    foreach (var field in inputFields)
                    {
                        func(index, field, ParameterDirection.Input);
                    }
                }

                // Iterate all the output fields
                if (outputFields?.Any() == true)
                {
                    foreach (var field in outputFields)
                    {
                        func(index, field, ParameterDirection.Output);
                    }
                }
            }
        }

        #endregion
    }
}
