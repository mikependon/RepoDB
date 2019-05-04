using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Extensions;
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
        private static ClientTypeToSqlDbTypeResolver m_clientTypeToSqlDbTypeResolver = new ClientTypeToSqlDbTypeResolver();
        private static Type m_bytesType = typeof(byte[]);

        #region GetDataReaderToDataEntityFunction<TEntity>

        /// <summary>
        /// Gets a compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of data entity objects.
        /// </summary>
        /// <typeparam name="TEntity">The data entity object to convert to.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <returns>A compiled function that is used to cover the <see cref="DbDataReader"/> object into a list of data entity objects.</returns>
        public static Func<DbDataReader, TEntity> GetDataReaderToDataEntityFunction<TEntity>(DbDataReader reader,
            IDbConnection connection)
            where TEntity : class
        {
            // Expression variables
            var readerParameterExpression = Expression.Parameter(typeof(DbDataReader), "reader");
            var newEntityExpression = Expression.New(typeof(TEntity));

            // Matching the fields
            var readerFields = Enumerable.Range(0, reader.FieldCount)
                .Select(reader.GetName)
                .Select((name, ordinal) => new DataReaderFieldDefinition
                {
                    Name = name.ToLower(),
                    Ordinal = ordinal,
                    Type = reader.GetFieldType(ordinal)
                });

            // Get the member assignments
            var memberAssignments = GetMemberAssignmentsForDataEntity<TEntity>(newEntityExpression, readerParameterExpression, readerFields, connection);

            // Throw an error if there are no matching atleast one
            if (memberAssignments.Any() != true)
            {
                throw new NoMatchedFieldsException($"There are no matching fields between the result set of the data reader and the type '{typeof(TEntity).FullName}'.");
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
            IEnumerable<DataReaderFieldDefinition> readerFields,
            IDbConnection connection)
            where TEntity : class
        {
            // Initialize variables
            var memberAssignments = new List<MemberAssignment>();
            var dataReaderType = typeof(DbDataReader);
            var tableFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<TEntity>());
            var isDefaultConversion = TypeMapper.ConversionType == ConversionType.Default;

            // Iterate each properties
            foreach (var property in PropertyCache.Get<TEntity>().Where(property => property.PropertyInfo.CanWrite))
            {
                // Gets the mapped name and the ordinal
                var mappedName = property.GetUnquotedMappedName().ToLower();
                var ordinal = readerFields?.Select(f => f.Name).ToList().IndexOf(mappedName);

                // Process only if there is a correct ordinal
                if (ordinal >= 0)
                {
                    // Variables needed for the iteration
                    var tableField = tableFields?.FirstOrDefault(f => f.UnquotedName.ToLower() == mappedName);
                    var readerField = readerFields.First(f => f.Name.ToLower() == mappedName);
                    var isTableFieldNullable = tableField == null || tableField?.IsNullable == true;
                    var underlyingType = Nullable.GetUnderlyingType(property.PropertyInfo.PropertyType);
                    var propertyType = underlyingType ?? property.PropertyInfo.PropertyType;
                    var convertType = readerField.Type;
                    var isConversionNeeded = readerField.Type != propertyType;

                    // Get the correct method info, if the reader.Get<Type> is not found, then use the default GetValue() method
                    var readerGetValueMethod = dataReaderType.GetTypeInfo().GetMethod(string.Concat("Get", readerField.Type.Name));
                    if (readerGetValueMethod == null)
                    {
                        // Single value is throwing an exception in GetString(), skip it and use the GetValue() instead
                        if (isDefaultConversion == false && readerField.Type != typeof(Single))
                        {
                            readerGetValueMethod = dataReaderType.GetTypeInfo().GetMethod(string.Concat("Get", propertyType.Name));
                        }

                        // If present, then use the property type, otherwise, use the object
                        if (readerGetValueMethod != null)
                        {
                            convertType = propertyType;
                        }
                        else
                        {
                            readerGetValueMethod = dataReaderType.GetTypeInfo().GetMethod("GetValue");
                            convertType = typeof(object);
                        }

                        // Force the conversion flag
                        isConversionNeeded = true;
                    }

                    // Expressions
                    var ordinalExpression = Expression.Constant(ordinal);
                    var valueExpression = (Expression)null;

                    // Check for nullables
                    if (isTableFieldNullable == true)
                    {
                        var isDbNullExpression = Expression.Call(readerParameterExpression, dataReaderType.GetTypeInfo().GetMethod("IsDBNull"), ordinalExpression);

                        // True expression
                        var trueExpression = (Expression)null;
                        if (underlyingType != null && underlyingType.GetTypeInfo().IsValueType == true)
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
                                falseExpression = Expression.Convert(falseExpression, propertyType);
                            }
                            else
                            {
                                falseExpression = ConvertValueExpressionForDataEntity(falseExpression, readerField, propertyType, convertType);
                            }
                        }
                        if (underlyingType != null && underlyingType.GetTypeInfo().IsValueType == true)
                        {
                            var nullableConstructorExpression = typeof(Nullable<>).MakeGenericType(propertyType).GetTypeInfo().GetConstructor(new[] { propertyType });
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
                        if (underlyingType != null && underlyingType.GetTypeInfo().IsValueType == true)
                        {
                            var nullableConstructorExpression = typeof(Nullable<>).MakeGenericType(propertyType).GetTypeInfo().GetConstructor(new[] { propertyType });
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
            DataReaderFieldDefinition readerField,
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
                    expression = Expression.New(typeof(Guid).GetTypeInfo().GetConstructor(new[] { typeof(string) }), expression);
                }
                else if (propertyType == typeof(string) && readerField.Type == typeof(Guid))
                {
                    // This is Guid.ToString()
                    targetMethod = typeof(Guid).GetTypeInfo().GetMethod("ToString", new Type[0]);
                    targetInstance = expression;
                    targetParameter = null;
                }
                else
                {
                    // This System.Convert.To<Type>()
                    targetMethod = typeof(Convert).GetTypeInfo().GetMethod(string.Concat("To", propertyType.Name), new[] { convertType });
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

        #region GetDataReaderToExpandoObjectFunction

        /// <summary>
        /// Gets a compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of dynamic objects.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <returns>A compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of dynamic objects.</returns>
        public static Func<DbDataReader, ExpandoObject> GetDataReaderToExpandoObjectFunction(DbDataReader reader)
        {
            // Expression variables
            var readerParameterExpression = Expression.Parameter(typeof(DbDataReader), "reader");
            var newObjectExpression = Expression.New(typeof(ExpandoObject));

            // Matching the fields
            var readerFields = Enumerable.Range(0, reader.FieldCount)
                .Select(reader.GetName)
                .Select((name, ordinal) => new DataReaderFieldDefinition
                {
                    Name = name,
                    Ordinal = ordinal,
                    Type = reader.GetFieldType(ordinal)
                });

            // Initialize the elements
            var elementInits = GetElementInitsForDictionary(readerParameterExpression, readerFields?.ToList());

            // Throw an error if there are no matching atleast one
            if (elementInits.Any() != true)
            {
                throw new NoMatchedFieldsException($"There are no elements initialization found.");
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
        private static IEnumerable<ElementInit> GetElementInitsForDictionary(ParameterExpression readerParameterExpression, List<DataReaderFieldDefinition> readerFields)
        {
            // Initialize variables
            var elementInits = new List<ElementInit>();
            var dataReaderType = typeof(DbDataReader);
            var addMethod = typeof(IDictionary<string, object>).GetTypeInfo().GetMethod("Add", new[] { typeof(string), typeof(object) });

            // Iterate each properties
            for (var ordinal = 0; ordinal < readerFields?.Count(); ordinal++)
            {
                // Field variable
                var field = readerFields[ordinal];
                var isConversionNeeded = false;

                // Get the correct method info, if the reader.Get<Type> is not found, then use the default GetValue
                var readerGetValueMethod = dataReaderType.GetTypeInfo().GetMethod(string.Concat("Get", field?.Type.Name));
                if (readerGetValueMethod == null)
                {
                    readerGetValueMethod = dataReaderType.GetTypeInfo().GetMethod("GetValue");
                    isConversionNeeded = true;
                }

                // Expressions
                var ordinalExpression = Expression.Constant(ordinal);
                var valueExpression = (Expression)null;

                // Check for nullables
                var isDbNullExpression = Expression.Call(readerParameterExpression, dataReaderType.GetTypeInfo().GetMethod("IsDBNull"), ordinalExpression);
                var trueExpression = Expression.Default(field.Type);
                var falseExpression = (Expression)Expression.Call(readerParameterExpression, readerGetValueMethod, ordinalExpression);
                if (isConversionNeeded == true)
                {
                    falseExpression = Expression.Convert(falseExpression, field?.Type);
                }
                valueExpression = Expression.Condition(isDbNullExpression, trueExpression, falseExpression);

                // Add to the bindings
                var values = new[]
                {
                    Expression.Constant(field.Name),
                    (Expression)Expression.Convert(valueExpression, typeof(object))
                };
                elementInits.Add(Expression.ElementInit(addMethod, values));
            }

            // Return the result
            return elementInits;
        }

        #endregion

        #region GetDataCommandParameterSetterFunction<TEntity>

        /// <summary>
        /// Gets a compiled function that is used to set the <see cref="DbParameter"/> objects of the <see cref="DbCommand"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="inputFields">The list of the input <see cref="Field"/> objects.</param>
        /// <param name="outputFields">The list of the input <see cref="Field"/> objects.</param>
        /// <param name="batchSize">The batch size of the entity to be passed.</param>
        /// <returns>A compiled function that is used to set the <see cref="DbParameter"/> objects of the <see cref="DbCommand"/> object.</returns>
        public static Action<DbCommand, IList<TEntity>> GetDataCommandParameterSetterFunction<TEntity>(IEnumerable<Field> inputFields,
            IEnumerable<Field> outputFields,
            int batchSize)
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

            // Expression arguments
            var commandParameter = Expression.Parameter(typeOfDbCommand, "command");
            var entitiesParameter = Expression.Parameter(typeOfListEntity, "entities");

            // Expression variables
            var dbCommandParametersProperty = typeOfDbCommand.GetProperty("Parameters");
            var dbCommandCreateParameterMethod = typeOfDbCommand.GetMethod("CreateParameter");

            // Get the parameter collection
            var parameterCollection = Expression.Property(commandParameter, dbCommandParametersProperty);

            // Set the body
            var body = new List<Expression>();

            // Get the necessary methods of the parameter collection
            var dbParameterCollectionAddMethod = typeOfDbParameterCollection.GetMethod("Add", new[] { typeOfObject });
            var dbParameterCollectionClearMethod = typeOfDbParameterCollection.GetMethod("Clear");

            // Get the necessary methods for the DbParameter
            var dbParameterParameterNameSetMethod = typeOfDbParameter.GetProperty("ParameterName").SetMethod;
            var dbParameterValueSetMethod = typeOfDbParameter.GetProperty("Value").SetMethod;
            var dbParameterDbTypeSetMethod = typeOfDbParameter.GetProperty("DbType").SetMethod;
            var dbParameterDirectionSetMethod = typeOfDbParameter.GetProperty("Direction").SetMethod;

            // Get the necessary methods of the 'Dynamic|Object' object
            var objectGetTypeMethod = typeOfObject.GetMethod("GetType");
            var typeGetPropertyMethod = typeOfType.GetMethod("GetProperty", new[] { typeOfString });
            var propertyInfoGetValueMethod = typeOfPropertyInfo.GetMethod("GetValue", new[] { typeOfObject });

            // Get the necessary methods of the List<T>
            var listIndexerMethod = typeOfListEntity.GetMethod("get_Item", new[] { typeOfInt });

            // Clear the parameter collection first
            // TODO: Performance, can we use the AddRange instead?
            body.Add(Expression.Call(parameterCollection, dbParameterCollectionClearMethod));

            // Reusable function for input/output fields
            var func = new Func<int, Expression, Expression, Field, bool, ParameterDirection, Expression>((int index,
                Expression instance,
                Expression instanceType,
                Field field,
                bool skipValueAssignment,
                ParameterDirection direction) =>
            {
                // Parameters for the block
                var parameterAssignments = new List<Expression>();

                // Create the parameter
                var createParameter = Expression.Call(commandParameter, dbCommandCreateParameterMethod);
                var parameterVariable = Expression.Variable(typeOfDbParameter,
                    string.Concat("var", field.UnquotedName, index > 0 ? index.ToString() : string.Empty));

                // Assign the newly created parameter to a variable
                var parameterAssignment = Expression.Assign(parameterVariable, createParameter);
                parameterAssignments.Add(parameterAssignment);

                // Set the name
                var nameAssignment = Expression.Call(parameterVariable, dbParameterParameterNameSetMethod,
                    Expression.Constant(index > 0 ? string.Concat(field.UnquotedName, "_", index) : field.UnquotedName));
                parameterAssignments.Add(nameAssignment);

                // Set the value
                var instanceProperty = typeOfEntity.GetProperty(field.UnquotedName);

                // Set the value
                if (skipValueAssignment == false)
                {
                    var value = (Expression)null;

                    // If the property is missing directly, then it could be a dynamic object
                    if (instanceProperty == null)
                    {
                        var instanceVariable = Expression.Variable(typeOfObject,
                           string.Concat("objectValue", field.UnquotedName, index > 0 ? index.ToString() : string.Empty));
                        var objectType = Expression.Call(instanceVariable, objectGetTypeMethod);
                        var objectProperty = Expression.Call(objectType, typeGetPropertyMethod, Expression.Constant(field.UnquotedName));
                        value = Expression.Block(new[] { instanceVariable },
                            Expression.Assign(instanceVariable, instance),
                            Expression.Call(objectProperty, propertyInfoGetValueMethod, instanceVariable));
                    }
                    else
                    {
                        value = Expression.Convert(Expression.Property(instance, instanceProperty), typeOfObject);
                    }

                    var valueVariable = Expression.Variable(typeOfObject,
                        string.Concat("valueOf", field.UnquotedName, index > 0 ? index.ToString() : string.Empty));
                    var valueBlock = (Expression)null;

                    // Check if the property is nullable
                    if (instanceProperty != null && Nullable.GetUnderlyingType(instanceProperty.PropertyType) != null)
                    {
                        var dbNullValue = Expression.Convert(Expression.Constant(DBNull.Value), typeOfObject);
                        var valueIsNull = Expression.Equal(valueVariable, Expression.Constant(null));
                        valueBlock = Expression.Block(new[] { valueVariable },
                            Expression.Assign(valueVariable, value),
                            Expression.Condition(valueIsNull, dbNullValue, valueVariable));
                    }
                    else
                    {
                        valueBlock = value;
                    }

                    // Add to the collection
                    if (valueBlock != null)
                    {
                        var valueAssignment = Expression.Call(parameterVariable, dbParameterValueSetMethod, valueBlock);
                        parameterAssignments.Add(valueAssignment);
                    }
                }

                // Identity the DB Type
                var dbType = TypeMapper.Get(field.Type?.GetUnderlyingType() ?? instanceProperty?.PropertyType.GetUnderlyingType())?.DbType;
                if (dbType == null)
                {
                    dbType = m_clientTypeToSqlDbTypeResolver.Resolve(field.Type.GetUnderlyingType() ?? instanceProperty?.PropertyType.GetUnderlyingType());
                }

                // Set the DB Type
                if (dbType != null)
                {
                    var dbTypeAssignment = Expression.Call(parameterVariable, dbParameterDbTypeSetMethod, Expression.Constant(dbType));
                    parameterAssignments.Add(dbTypeAssignment);
                }

                // Set the Parameter Direction
                if (direction != ParameterDirection.Input)
                {
                    var directionAssignment = Expression.Call(parameterVariable, dbParameterDirectionSetMethod, Expression.Constant(direction));
                    parameterAssignments.Add(directionAssignment);
                }

                // Add the actual addition
                parameterAssignments.Add(Expression.Call(parameterCollection, dbParameterCollectionAddMethod, parameterVariable));

                // Add the current block
                var parameterBlock = Expression.Block(new[] { parameterVariable }, parameterAssignments);

                // Return the expression
                return parameterBlock;
            });

            // Iterate by batch size
            for (var index = 0; index < batchSize; index++)
            {
                // Get the current instance
                var instance = Expression.Call(entitiesParameter, listIndexerMethod, Expression.Constant(index));
                var objectType = Expression.Call(instance, objectGetTypeMethod);

                // Iterate the input fields
                if (inputFields != null)
                {
                    foreach (var field in inputFields)
                    {
                        var inputFieldsExpression = func(index, instance, objectType, field, false, ParameterDirection.Input);
                        body.Add(inputFieldsExpression);
                    }
                }

                // Iterate the output fields
                if (outputFields != null)
                {
                    foreach (var field in outputFields)
                    {
                        var outputFieldsExpression = func(index, instance, objectType, field, true, ParameterDirection.Output);
                        body.Add(outputFieldsExpression);
                    }
                }
            }

            // Set the function value
            return Expression
                .Lambda<Action<DbCommand, IList<TEntity>>>(Expression.Block(body), commandParameter, entitiesParameter)
                .Compile();
        }

        #endregion

        #region GetDataCommandParameterSetterFunction(TableName)

        /// <summary>
        /// Gets a compiled function that is used to set the <see cref="DbParameter"/> objects of the <see cref="DbCommand"/> object.
        /// </summary>
        /// <param name="fields">The list of the <see cref="Field"/> objects.</param>
        /// <returns>A compiled function that is used to set the <see cref="DbParameter"/> objects of the <see cref="DbCommand"/> object.</returns>
        public static Action<DbCommand, object> GetDataCommandParameterSetterFunction(IEnumerable<Field> fields)
        {
            // Expression arguments
            var commandParameterExpression = Expression.Parameter(typeof(DbCommand), "command");
            var entityParameterExpression = Expression.Parameter(typeof(object), "entity");

            // Expression variables
            var parametersProperty = typeof(DbCommand).GetProperty("Parameters");
            var indexerProperty = typeof(DbParameterCollection).GetProperty("Item", new[] { typeof(int) });
            var valueProperty = typeof(DbParameter).GetProperty("Value").SetMethod;
            var getTypeMethod = typeof(object).GetMethod("GetType");
            var getPropertyMethod = typeof(Type).GetMethod("GetProperty", new[] { typeof(string) });
            var getPropertyValueMethod = typeof(PropertyInfo).GetMethod("GetValue", new[] { typeof(object) });

            // Get the parameter collection
            var parameterCollection = Expression.Property(commandParameterExpression, parametersProperty);

            // Set the body
            var body = (Expression)null;
            var index = 0;

            // Iterate the properties
            foreach (var field in fields)
            {
                // Set the values
                var getTypeExpression = Expression.Call(entityParameterExpression, getTypeMethod);
                var getPropertyExpression = Expression.Call(getTypeExpression, getPropertyMethod, Expression.Constant(field.UnquotedName));
                var valueExpression = Expression.Call(getPropertyExpression, getPropertyValueMethod, entityParameterExpression);
                var convertedValueExpression = Expression.Convert(valueExpression, typeof(object));
                var parameterExpression = Expression.Property(parameterCollection, indexerProperty, Expression.Constant(index));

                // Set the value
                if (body == null)
                {
                    body = Expression.Call(parameterExpression, valueProperty, valueExpression);
                }
                else
                {
                    body = Expression.Block(body, Expression.Call(parameterExpression, valueProperty, valueExpression));
                }

                // Increment the indexer
                index++;
            }

            // Set the function value
            return Expression
                .Lambda<Action<DbCommand, object>>(body, commandParameterExpression, entityParameterExpression)
                .Compile();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Create the <see cref="DbCommand"/> parameters based on the list of <see cref="Field"/> objects.
        /// </summary>
        /// <param name="command">The target <see cref="DbCommand"/> object.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects.</param>
        internal static void CreateDbCommandParametersFromFields(DbCommand command,
            IEnumerable<Field> fields)
        {
            // Check first the presence
            if (fields?.Any() != true)
            {
                return;
            }

            // Variables needed
            var bytesType = typeof(byte[]);

            // Clear the parameters
            command.Parameters.Clear();

            // Iterate and recreate
            foreach (var field in fields)
            {
                // Create the parameter
                var parameter = command.CreateParameter();

                // Set the property
                parameter.ParameterName = field.UnquotedName;

                // Set the DB Type
                var dbType = TypeMapper.Get(field.Type?.GetUnderlyingType())?.DbType;

                // Ensure the type mapping
                if (dbType == null)
                {
                    if (field.Type == bytesType)
                    {
                        dbType = DbType.Binary;
                    }
                }

                // Resolve manually
                if (dbType == null)
                {
                    dbType = m_clientTypeToSqlDbTypeResolver.Resolve(field.Type);
                }

                // Set the DB Type if present
                if (dbType != null)
                {
                    parameter.DbType = dbType.Value;
                }

                // Add the parameter
                command.Parameters.Add(parameter);
            }
        }

        #endregion
    }
}
