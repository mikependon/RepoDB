using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;
using RepoDb.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RepoDb.Entity;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A static factory class used to create a custom function.
    /// </summary>
    internal static class FunctionFactory2
    {
        #region SubClasses

        /// <summary>
        /// A helper class for type enum.
        /// </summary>
        private class EnumHelper
        {
            /// <summary>
            /// Parses the string value to a desired enum. It uses the method <see cref="Enum.Parse(Type, string)"/> underneath.
            /// </summary>
            /// <param name="enumType">The type of enum.</param>
            /// <param name="value">The value to parse.</param>
            /// <param name="ignoreCase">The case sensitivity of the parse operation.</param>
            /// <returns>The enum value.</returns>
            public static object Parse(Type enumType,
                string value,
                bool ignoreCase)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    return Enum.Parse(enumType?.GetUnderlyingType(), value, ignoreCase);
                }
                if (enumType.IsNullable())
                {
                    var nullable = typeof(Nullable<>).MakeGenericType(new[] { enumType });
                    return Activator.CreateInstance(nullable);
                }
                else
                {
                    return Activator.CreateInstance(enumType);
                }
            }


            /// <summary>
            /// Converts the value using the desired convert method (of type <see cref="MethodInfo"/>). If not given, it will use the <see cref="Convert"/> class.
            /// </summary>
            /// <param name="sourceType">The source type.</param>
            /// <param name="targetType">The target type.</param>
            /// <param name="value">The value to parse.</param>
            /// <param name="converterMethod">The converter method to be checked and used.</param>
            /// <returns>The converted value value.</returns>
            public static object Convert(Type sourceType,
                Type targetType,
                object value,
                MethodInfo converterMethod)
            {
                if (value == null)
                {
                    return sourceType.IsNullable() ? null :
                        targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
                }
                if (converterMethod != null)
                {
                    return converterMethod.Invoke(null, new[] { value });
                }
                else
                {
                    return Converter.DbNullToNull(value) == null ? Activator.CreateInstance(targetType) :
                        System.Convert.ChangeType(value, targetType);
                }
            }
        }

        private static class Types
        {
            // Get the types
            public static readonly Type TypeOfDbCommand = typeof(DbCommand);
            public static readonly Type TypeOfObject = typeof(object);
            public static readonly Type TypeOfDbParameter = typeof(DbParameter);
            public static readonly Type TypeOfInt = typeof(int);
            public static readonly Type TypeOfString = typeof(string);
            public static readonly Type TypeOfType = typeof(Type);
            public static readonly Type TypeOfPropertyInfo = typeof(PropertyInfo);
            public static readonly Type TypeOfBytes = typeof(byte[]);
            public static readonly Type TypeOfTimeSpan = typeof(TimeSpan);
            public static readonly Type TypeOfGuid = typeof(Guid);
            public static readonly Type TypeOfDateTime = typeof(DateTime);
            public static readonly Type TypeOfDecimal = typeof(Decimal);
            public static readonly Type TypeOfFloat = typeof(float);
            public static readonly Type TypeOfLong = typeof(long);
            public static readonly Type TypeOfDouble = typeof(Double);
            public static readonly Type TypeOfShort = typeof(short);
            public static readonly Type TypeOfBoolean = typeof(bool);
            public static readonly Type TypeOfConvert = typeof(Convert);

            public static readonly PropertyInfo DbCommandParametersProperty = TypeOfDbCommand.GetProperty("Parameters");
            public static readonly MethodInfo DbCommandCreateParameterMethod = TypeOfDbCommand.GetMethod("CreateParameter");
            public static readonly MethodInfo DbParameterCollectionAddMethod = typeof(DbParameterCollection).GetMethod("Add", new[] { TypeOfObject });
            public static readonly MethodInfo DbParameterCollectionClearMethod = typeof(DbParameterCollection).GetMethod("Clear");

            // MethodInfo for 'Dynamic|Object' object
            public static readonly MethodInfo ObjectGetTypeMethod = TypeOfObject.GetMethod("GetType");
            public static readonly MethodInfo TypeGetPropertyMethod = TypeOfType.GetMethod("GetProperty", new[] { Types.TypeOfString, typeof(BindingFlags) });
            public static readonly MethodInfo PropertyInfoGetValueMethod = TypeOfPropertyInfo.GetMethod("GetValue", new[] { Types.TypeOfObject });

            public static readonly MethodInfo DbParameterDbTypeSetMethod = TypeOfDbParameter.GetProperty("DbType")?.SetMethod ?? throw new PropertyNotFoundException("DbType");
            public static readonly MethodInfo DbParameterDirectionSetMethod = TypeOfDbParameter.GetProperty("Direction")?.SetMethod ?? throw new PropertyNotFoundException("Direction");
            public static readonly MethodInfo DbParameterSizeSetMethod = TypeOfDbParameter.GetProperty("Size")?.SetMethod ?? throw new PropertyNotFoundException("Size");
            public static readonly MethodInfo DbParameterParameterNameSetMethod = TypeOfDbParameter.GetProperty("ParameterName")?.SetMethod ?? throw new PropertyNotFoundException("ParameterName");
            public static readonly MethodInfo DbParameterPrecisionSetMethod = TypeOfDbParameter.GetProperty("Precision")?.SetMethod ?? throw new PropertyNotFoundException("Precision");
            public static readonly MethodInfo DbParameterScaleSetMethod = TypeOfDbParameter.GetProperty("Scale")?.SetMethod ?? throw new PropertyNotFoundException("Scale");
            public static readonly MethodInfo DbParameterValueSetMethod = TypeOfDbParameter.GetProperty("Value")?.SetMethod ?? throw new PropertyNotFoundException("Value");
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
            // Variables for arguments
            var typeOfEntity = typeof(TEntity);
            var commandParameterExpression = Expression.Parameter(Types.TypeOfDbCommand, "command");
            var entityParameterExpression = Expression.Parameter(typeOfEntity, "entity");

            // Variables for types
            var entityProperties = PropertyCache.Get<TEntity>();

            // Variables for DbParameterCollection
            var dbParameterCollection = Expression.Property(commandParameterExpression, Types.DbCommandParametersProperty);

            // Other variables
            var dbTypeResolver = new ClientTypeToDbTypeResolver();

            // Variables for the object instance
            var propertyVariableList = new List<dynamic>();
            var instanceVariable = Expression.Variable(typeOfEntity, "instance");
            var instanceType = Expression.Constant(typeOfEntity);
            var instanceTypeVariable = Expression.Variable(Types.TypeOfType, "instanceType");

            // Input fields properties
            inputFields?.ForEach((x, i) =>
            {
                propertyVariableList.Add(new
                {
                    Index = i,
                    Field = x,
                    Direction = ParameterDirection.Input
                });
            });

            // Output fields properties
            outputFields?.ForEach(x =>
            {
                propertyVariableList.Add(new
                {
                    Index = propertyVariableList.Count,
                    Field = x,
                    Direction = ParameterDirection.Output
                });
            });

            // Variables for expression body
            var bodyExpressions = new List<Expression>
            {
                Expression.Call(dbParameterCollection, Types.DbParameterCollectionClearMethod)
            };

            // Clear the parameter collection first

            // Get the current instance
            var instanceExpressions = new List<Expression>();
            var instanceVariables = new List<ParameterExpression> {instanceVariable};

            // Entity instance
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
                if (typeOfEntity == Types.TypeOfObject)
                {
                    propertyVariable = Expression.Variable(Types.TypeOfPropertyInfo, string.Concat("property", propertyName));
                    propertyInstance = Expression.Call(Expression.Call(instanceVariable, Types.ObjectGetTypeMethod),
                        Types.TypeGetPropertyMethod,
                        new Expression[]
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

                var parameterAssignment = typeOfEntity.IsSubclassOf(typeof(BaseEntity<TEntity>))
                    ? BuildParameterAssignmentExpression2<TEntity>(
                        instanceVariable,
                        propertyVariable,
                        field,
                        classProperty,
                        (direction == ParameterDirection.Output),
                        direction,
                        dbSetting,
                        dbTypeResolver,
                        commandParameterExpression,
                        dbParameterCollection)
                    : BuildParameterAssignmentExpression<TEntity>(
                        instanceVariable,
                        propertyVariable,
                        field,
                        classProperty,
                        (direction == ParameterDirection.Output),
                        direction,
                        dbSetting,
                        dbTypeResolver,
                        commandParameterExpression,
                        dbParameterCollection);

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
        public static Action<DbCommand, IList<TEntity>> GetDataEntitiesDbCommandParameterSetterFunction<TEntity>(
            IEnumerable<DbField> inputFields,
            IEnumerable<DbField> outputFields,
            int batchSize,
            IDbSetting dbSetting) where TEntity : class
        {
            // Get the types
            var typeOfListEntity = typeof(IList<TEntity>);
            var typeOfEntity = typeof(TEntity);

            // Variables for arguments
            var commandParameterExpression = Expression.Parameter(Types.TypeOfDbCommand, "command");
            var entitiesParameterExpression = Expression.Parameter(typeOfListEntity, "entities");

            // Variables for types
            var entityProperties = PropertyCache.Get<TEntity>();

            // Variables for DbParameterCollection
            var dbParameterCollection = Expression.Property(commandParameterExpression, Types.DbCommandParametersProperty);

            // Variables for List<T>
            var listIndexerMethod = typeOfListEntity.GetMethod("get_Item", new[] { Types.TypeOfInt });

            // Other variables
            var dbTypeResolver = new ClientTypeToDbTypeResolver();

            // Variables for the object instance
            var propertyVariableList = new List<dynamic>();
            var instanceVariable = Expression.Variable(typeOfEntity, "instance");
            var instanceType = Expression.Constant(typeOfEntity); // Expression.Call(instanceVariable, objectGetTypeMethod);
            var instanceTypeVariable = Expression.Variable(Types.TypeOfType, "instanceType");

            // Input fields properties
            inputFields?.ForEach((x, i) =>
            {
                propertyVariableList.Add(new
                {
                    Index = i,
                    Field = x,
                    Direction = ParameterDirection.Input
                });
            });

            // Output fields properties
            outputFields?.ForEach(x =>
            {
                propertyVariableList.Add(new
                {
                    Index = propertyVariableList.Count,
                    Field = x,
                    Direction = ParameterDirection.Output
                });
            });

            // Variables for expression body
            var bodyExpressions = new List<Expression>
            {
                Expression.Call(dbParameterCollection, Types.DbParameterCollectionClearMethod)
            };

            // Clear the parameter collection first

            // Iterate by batch size
            for (var entityIndex = 0; entityIndex < batchSize; entityIndex++)
            {
                // Get the current instance
                var instance = Expression.Call(entitiesParameterExpression, listIndexerMethod, Expression.Constant(entityIndex));
                var instanceExpressions = new List<Expression>();
                var instanceVariables = new List<ParameterExpression> {instanceVariable};

                // Entity instance
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
                    if (typeOfEntity == Types.TypeOfObject)
                    {
                        propertyVariable = Expression.Variable(Types.TypeOfPropertyInfo, string.Concat("property", propertyName));
                        propertyInstance = Expression.Call(Expression.Call(instanceVariable, Types.ObjectGetTypeMethod),
                            Types.TypeGetPropertyMethod,
                            new Expression[]
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
                    var parameterAssignment = typeOfEntity.IsSubclassOf(typeof(BaseEntity<TEntity>))
                        ? BuildParameterAssignmentExpression2<TEntity>(
                            entityIndex /* index */,
                            instanceVariable /* instance */,
                            propertyVariable /* property */,
                            field /* field */,
                            classProperty /* classProperty */,
                            (direction == ParameterDirection.Output) /* skipValueAssignment */,
                            direction /* direction */,
                            dbSetting,
                            dbTypeResolver,
                            commandParameterExpression,
                            dbParameterCollection)
                        : BuildParameterAssignmentExpression<TEntity>(
                            entityIndex /* index */,
                            instanceVariable /* instance */,
                            propertyVariable /* property */,
                            field /* field */,
                            classProperty /* classProperty */,
                            (direction == ParameterDirection.Output) /* skipValueAssignment */,
                            direction /* direction */,
                            dbSetting,
                            dbTypeResolver,
                            commandParameterExpression,
                            dbParameterCollection);

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
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
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
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The target <see cref="Field"/>.</param>
        /// <returns>A compiled function that is used to set the data entity object property value.</returns>
        public static Action<TEntity, object> GetDataEntityPropertyValueSetterFunction<TEntity>(Field field)
            where TEntity : class
        {
            // Variables for type
            var typeOfEntity = typeof(TEntity);
            var typeOfObject = typeof(object);
            var typeOfConverter = typeof(Converter);

            // Variables for argument
            var entityParameter = Expression.Parameter(typeOfEntity, "entity");
            var valueParameter = Expression.Parameter(typeOfObject, "value");

            // Get the entity property
            var property = (typeOfEntity.GetProperty(field.Name) ?? typeOfEntity.GetPropertyByMapping(field.Name)?.PropertyInfo)?.SetMethod;

            // Get the converter
            var toTypeMethod = typeOfConverter.GetMethod("ToType", new[] { typeOfObject }).MakeGenericMethod(field.Type.GetUnderlyingType());

            // Assign the value into DataEntity.Property
            var propertyAssignment = Expression.Call(entityParameter, property,
                Expression.Convert(Expression.Call(toTypeMethod, valueParameter), field.Type));

            // Return function
            return Expression.Lambda<Action<TEntity, object>>(propertyAssignment,
                entityParameter, valueParameter).Compile();
        }

        #endregion

        #region Helpers

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
                if (dbSetting.IsDirectionSupported)
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

        #region Other Data Providers Helpers

        #region SqlServer (System)

        /// <summary>
        /// Gets the SystemSqlServerTypeMapAttribute if present.
        /// </summary>
        /// <param name="property">The instance of propery to inspect.</param>
        /// <returns>The instance of SystemSqlServerTypeMapAttribute.</returns>
        internal static Attribute GetSystemSqlServerTypeMapAttribute(ClassProperty property)
        {
            return property?
                .PropertyInfo
                .GetCustomAttributes()?
                .FirstOrDefault(e =>
                    e.GetType().FullName.Equals("RepoDb.Attributes.SystemSqlServerTypeMapAttribute"));
        }

        /// <summary>
        /// Gets the value represented by the SystemSqlServerTypeMapAttribute.DbType property.
        /// </summary>
        /// <param name="attribute">The instance of SystemSqlServerTypeMapAttribute to extract.</param>
        /// <returns>The value represented by the SystemSqlServerTypeMapAttribute.DbType property.</returns>
        internal static object GetSystemSqlServerDbTypeFromAttribute(Attribute attribute)
        {
            if (attribute == null)
            {
                return null;
            }
            var type = attribute.GetType();
            return type
                .GetProperty("DbType")?
                .GetValue(attribute);
        }

        /// <summary>
        /// Gets the system type of System.Data.SqlClient.SqlParameter represented by SystemSqlServerTypeMapAttribute.ParameterType property.
        /// </summary>
        /// <param name="attribute">The instance of SystemSqlServerTypeMapAttribute to extract.</param>
        /// <returns>The type of System.Data.SqlClient.SqlParameter represented by SystemSqlServerTypeMapAttribute.ParameterType property.</returns>
        internal static Type GetSystemSqlServerParameterTypeFromAttribute(Attribute attribute)
        {
            if (attribute == null)
            {
                return null;
            }
            return (Type)attribute
                .GetType()
                .GetProperty("ParameterType")?
                .GetValue(attribute);
        }

        /// <summary>
        /// Gets the instance of <see cref="MethodInfo"/> represented by the SystemSqlServerTypeMapAttribute.DbType property.
        /// </summary>
        /// <param name="attribute">The instance of SystemSqlServerTypeMapAttribute to extract.</param>
        /// <returns>The instance of <see cref="MethodInfo"/> represented by the SystemSqlServerTypeMapAttribute.DbType property.</returns>
        internal static MethodInfo GetSystemSqlServerDbTypeFromAttributeSetMethod(Attribute attribute)
        {
            if (attribute == null)
            {
                return null;
            }
            return GetSystemSqlServerParameterTypeFromAttribute(attribute)?
                .GetProperty("SqlDbType")?
                .SetMethod;
        }

        #endregion

        #region SqlServer (Microsoft)

        /// <summary>
        /// Gets the MicrosoftSqlServerTypeMapAttribute if present.
        /// </summary>
        /// <param name="property">The instance of propery to inspect.</param>
        /// <returns>The instance of MicrosoftSqlServerTypeMapAttribute.</returns>
        internal static Attribute GetMicrosoftSqlServerTypeMapAttribute(ClassProperty property)
        {
            return property?
                .PropertyInfo
                .GetCustomAttributes()?
                .FirstOrDefault(e =>
                    e.GetType().FullName.Equals("RepoDb.Attributes.MicrosoftSqlServerTypeMapAttribute"));
        }

        /// <summary>
        /// Gets the value represented by the MicrosoftSqlServerTypeMapAttribute.DbType property.
        /// </summary>
        /// <param name="attribute">The instance of MicrosoftSqlServerTypeMapAttribute to extract.</param>
        /// <returns>The value represented by the MicrosoftSqlServerTypeMapAttribute.DbType property.</returns>
        internal static object GetMicrosoftSqlServerDbTypeFromAttribute(Attribute attribute)
        {
            if (attribute == null)
            {
                return null;
            }
            var type = attribute.GetType();
            return type
                .GetProperty("DbType")?
                .GetValue(attribute);
        }

        /// <summary>
        /// Gets the system type of Microsoft.Data.SqlClient.SqlParameter represented by MicrosoftSqlServerTypeMapAttribute.ParameterType property.
        /// </summary>
        /// <param name="attribute">The instance of MicrosoftSqlServerTypeMapAttribute to extract.</param>
        /// <returns>The type of Microsoft.Data.SqlClient.SqlParameter represented by MicrosoftSqlServerTypeMapAttribute.ParameterType property.</returns>
        internal static Type GetMicrosoftSqlServerParameterTypeFromAttribute(Attribute attribute)
        {
            if (attribute == null)
            {
                return null;
            }
            return (Type)attribute
                .GetType()
                .GetProperty("ParameterType")?
                .GetValue(attribute);
        }

        /// <summary>
        /// Gets the instance of <see cref="MethodInfo"/> represented by the MicrosoftSqlServerTypeMapAttribute.DbType property.
        /// </summary>
        /// <param name="attribute">The instance of MicrosoftSqlServerTypeMapAttribute to extract.</param>
        /// <returns>The instance of <see cref="MethodInfo"/> represented by the MicrosoftSqlServerTypeMapAttribute.DbType property.</returns>
        internal static MethodInfo GetMicrosoftSqlServerDbTypeFromAttributeSetMethod(Attribute attribute)
        {
            if (attribute == null)
            {
                return null;
            }
            return GetMicrosoftSqlServerParameterTypeFromAttribute(attribute)?
                .GetProperty("SqlDbType")?
                .SetMethod;
        }

        #endregion

        #region MySql

        /// <summary>
        /// Gets the MySqlTypeMapAttribute if present.
        /// </summary>
        /// <param name="property">The instance of propery to inspect.</param>
        /// <returns>The instance of MySqlTypeMapAttribute.</returns>
        internal static Attribute GetMySqlDbTypeTypeMapAttribute(ClassProperty property)
        {
            return property?
                .PropertyInfo
                .GetCustomAttributes()?
                .FirstOrDefault(e =>
                    e.GetType().FullName.Equals("RepoDb.Attributes.MySqlTypeMapAttribute"));
        }

        /// <summary>
        /// Gets the value represented by the MySqlTypeMapAttribute.DbType property.
        /// </summary>
        /// <param name="attribute">The instance of MySqlTypeMapAttribute to extract.</param>
        /// <returns>The value represented by the MySqlTypeMapAttribute.DbType property.</returns>
        internal static object GetMySqlDbTypeFromAttribute(Attribute attribute)
        {
            if (attribute == null)
            {
                return null;
            }
            var type = attribute.GetType();
            return type
                .GetProperty("DbType")?
                .GetValue(attribute);
        }

        /// <summary>
        /// Gets the system type of MySql.Data.MySqlClient.MySqlParameter represented by MySqlTypeMapAttribute.ParameterType property.
        /// </summary>
        /// <param name="attribute">The instance of MySqlTypeMapAttribute to extract.</param>
        /// <returns>The type of MySql.Data.MySqlClient.MySqlParameter represented by MySqlTypeMapAttribute.ParameterType property.</returns>
        internal static Type GetMySqlParameterTypeFromAttribute(Attribute attribute)
        {
            if (attribute == null)
            {
                return null;
            }
            return (Type)attribute
                .GetType()
                .GetProperty("ParameterType")?
                .GetValue(attribute);
        }

        /// <summary>
        /// Gets the instance of <see cref="MethodInfo"/> represented by the MySqlTypeMapAttribute.DbType property.
        /// </summary>
        /// <param name="attribute">The instance of MySqlTypeMapAttribute to extract.</param>
        /// <returns>The instance of <see cref="MethodInfo"/> represented by the MySqlTypeMapAttribute.DbType property.</returns>
        internal static MethodInfo GetMySqlDbTypeFromAttributeSetMethod(Attribute attribute)
        {
            if (attribute == null)
            {
                return null;
            }
            return GetMySqlParameterTypeFromAttribute(attribute)?
                .GetProperty("MySqlDbType")?
                .SetMethod;
        }

        #endregion

        #region Npgsql

        /// <summary>
        /// Gets the NpgsqlDbTypeMapAttribute if present.
        /// </summary>
        /// <param name="property">The instance of propery to inspect.</param>
        /// <returns>The instance of NpgsqlDbTypeMapAttribute.</returns>
        internal static Attribute GetNpgsqlDbTypeTypeMapAttribute(ClassProperty property)
        {
            return property?
                .PropertyInfo
                .GetCustomAttributes()?
                .FirstOrDefault(e =>
                    e.GetType().FullName.Equals("RepoDb.Attributes.NpgsqlTypeMapAttribute"));
        }

        /// <summary>
        /// Gets the value represented by the NpgsqlDbTypeMapAttribute.DbType property.
        /// </summary>
        /// <param name="attribute">The instance of NpgsqlDbTypeMapAttribute to extract.</param>
        /// <returns>The value represented by the NpgsqlDbTypeMapAttribute.DbType property.</returns>
        internal static object GetNpgsqlDbTypeFromAttribute(Attribute attribute)
        {
            if (attribute == null)
            {
                return null;
            }
            var type = attribute.GetType();
            return type
                .GetProperty("DbType")?
                .GetValue(attribute);
        }

        /// <summary>
        /// Gets the system type of NpgsqlTypes.NpgsqlParameter represented by NpgsqlDbTypeMapAttribute.ParameterType property.
        /// </summary>
        /// <param name="attribute">The instance of NpgsqlDbTypeMapAttribute to extract.</param>
        /// <returns>The type of NpgsqlTypes.NpgsqlParameter represented by NpgsqlDbTypeMapAttribute.ParameterType property.</returns>
        internal static Type GetNpgsqlParameterTypeFromAttribute(Attribute attribute)
        {
            if (attribute == null)
            {
                return null;
            }
            return (Type)attribute
                .GetType()
                .GetProperty("ParameterType")?
                .GetValue(attribute);
        }

        /// <summary>
        /// Gets the instance of <see cref="MethodInfo"/> represented by the NpgsqlDbTypeMapAttribute.DbType property.
        /// </summary>
        /// <param name="attribute">The instance of NpgsqlDbTypeMapAttribute to extract.</param>
        /// <returns>The instance of <see cref="MethodInfo"/> represented by the NpgsqlDbTypeMapAttribute.DbType property.</returns>
        internal static MethodInfo GetNpgsqlDbTypeFromAttributeSetMethod(Attribute attribute)
        {
            if (attribute == null)
            {
                return null;
            }
            return GetNpgsqlParameterTypeFromAttribute(attribute)?
                .GetProperty("NpgsqlDbType")?
                .SetMethod;
        }

        #endregion

        #endregion

        private static Expression BuildParameterAssignmentExpression<TEntity>
        (
            Expression instance,
            Expression property,
            DbField dbField,
            ClassProperty classProperty,
            bool skipValueAssignment,
            ParameterDirection direction,
            IDbSetting dbSetting,
            ClientTypeToDbTypeResolver dbTypeResolver,
            Expression commandParameterExpression,
            Expression dbParameterCollection
        ) where TEntity : class
        {
            // Parameters for the block
            var typeOfEntity = typeof(TEntity);
            var parameterAssignments = new List<Expression>();

            // Parameter variables
            var parameterName = dbField.Name.AsUnquoted(true, dbSetting).AsAlphaNumeric();
            var parameterVariable = Expression.Variable(Types.TypeOfDbParameter, string.Concat("parameter", parameterName));
            var parameterInstance = Expression.Call(commandParameterExpression, Types.DbCommandCreateParameterMethod);
            parameterAssignments.Add(Expression.Assign(parameterVariable, parameterInstance));

            // Set the name
            var nameAssignment = Expression.Call(parameterVariable, Types.DbParameterParameterNameSetMethod, Expression.Constant(parameterName));
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
            if (!skipValueAssignment)
            {
                // Set the value

                // Check the proper type of the entity
                if (typeOfEntity != Types.TypeOfObject && typeOfEntity.IsGenericType == false)
                {
                    instanceProperty = classProperty.PropertyInfo; // typeOfEntity.GetProperty(classProperty.PropertyInfo.Name);
                }

                #region PropertyHandler

                var propertyHandlerAttribute = instanceProperty?.GetCustomAttribute<PropertyHandlerAttribute>();

                if (propertyHandlerAttribute != null)
                {
                    // Get from the attribute
                    handlerInstance = PropertyHandlerCache.Get<TEntity, object>(classProperty.PropertyInfo);
                    handlerSetMethod = propertyHandlerAttribute.HandlerType.GetMethod("Set");
                }
                else
                {
                    // Get from the type level mappings (DB type)
                    handlerInstance = PropertyHandlerMapper.Get<object>(dbField.Type.GetUnderlyingType());
                    if (handlerInstance != null)
                    {
                        handlerSetMethod = handlerInstance.GetType().GetMethod("Set");
                    }
                }

                #endregion

                #region Instance.Property or PropertyInfo.GetValue()

                // Set the value
                var value = (Expression)null;

                // If the property is missing directly, then it could be a dynamic object
                if (instanceProperty == null)
                {
                    value = Expression.Call(property, Types.PropertyInfoGetValueMethod, instance);
                }
                else
                {
                    propertyType = instanceProperty.PropertyType.GetUnderlyingType();

                    if (handlerInstance == null)
                    {
                        if (Converter.ConversionType == ConversionType.Automatic)
                        {
                            var valueToConvert = Expression.Property(instance, instanceProperty);

                            #region StringToGuid

                            // Create a new guid here
                            if (propertyType == Types.TypeOfString && fieldType == Types.TypeOfGuid /* StringToGuid */)
                            {
                                value = Expression.New(Types.TypeOfGuid.GetConstructor(new[] { Types.TypeOfString }), new[] { valueToConvert });
                            }

                            #endregion

                            #region GuidToString

                            // Call the System.Convert conversion
                            else if (propertyType == Types.TypeOfGuid && fieldType == Types.TypeOfString/* GuidToString*/)
                            {
                                var convertMethod = typeof(Convert).GetMethod("ToString", new[] { Types.TypeOfObject });
                                value = Expression.Call(convertMethod, Expression.Convert(valueToConvert, Types.TypeOfObject));
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
                                    convertToTypeMethod = Types.TypeOfConvert.GetMethod(string.Concat("To", mappedToType.ToString()), new[] { Types.TypeOfObject });
                                }
                            }
                            if (convertToTypeMethod == null)
                            {
                                convertToTypeMethod = Types.TypeOfConvert.GetMethod(string.Concat("To", dbField.Type.Name), new[] { Types.TypeOfObject });
                            }
                            if (convertToTypeMethod == null)
                            {
                                throw new ConverterNotFoundException($"The convert between '{propertyType.FullName}' and database type '{dbField.DatabaseType}' (of .NET CLR '{dbField.Type.FullName}') is not found.");
                            }
                            else
                            {
                                var converterMethod = typeof(EnumHelper).GetMethod("Convert");
                                if (converterMethod != null)
                                {
                                    value = Expression.Call(converterMethod,
                                        Expression.Constant(instanceProperty.PropertyType),
                                        Expression.Constant(dbField.Type),
                                        Expression.Convert(value, Types.TypeOfObject),
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
                    value = Expression.Convert(value, Types.TypeOfObject);
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
                var dbNullValue = Expression.Convert(Expression.Constant(DBNull.Value), Types.TypeOfObject);

                // Check if the property is nullable
                if (isNullable == true)
                {
                    // Identification of the DBNull
                    var valueVariable = Expression.Variable(Types.TypeOfObject, string.Concat("valueOf", parameterName));
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
                var valueAssignment = Expression.Call(parameterVariable, Types.DbParameterValueSetMethod, valueBlock);

                #endregion

                // Check if it is a direct assignment or not
                if (typeOfEntity != Types.TypeOfObject)
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
                        dbNullValueAssignment = Expression.Call(parameterVariable, Types.DbParameterValueSetMethod,
                            Expression.Convert(Expression.Default(dbField.Type), Types.TypeOfObject));
                    }

                    // Set the DBNull value
                    if (dbNullValueAssignment == null)
                    {
                        dbNullValueAssignment = Expression.Call(parameterVariable, Types.DbParameterValueSetMethod, dbNullValue);
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
            //if (fieldOrPropertyType != typeOfTimeSpan)
            //{
            // Identify the DB Type
            var fieldOrPropertyType = (Type)null;
            var dbType = (DbType?)null;

            // Identify the conversion
            if (Converter.ConversionType == ConversionType.Automatic)
            {
                // Identity the conversion
                if (propertyType == Types.TypeOfDateTime && fieldType == Types.TypeOfString /* DateTimeToString */ ||
                    propertyType == Types.TypeOfDecimal && (fieldType == Types.TypeOfFloat || fieldType == Types.TypeOfDouble) /* DecimalToFloat/DecimalToDouble */ ||
                    propertyType == Types.TypeOfDouble && fieldType == Types.TypeOfLong /* DoubleToBigint */||
                    propertyType == Types.TypeOfDouble && fieldType == Types.TypeOfInt /* DoubleToBigint */ ||
                    propertyType == Types.TypeOfDouble && fieldType == Types.TypeOfShort /* DoubleToShort */||
                    propertyType == Types.TypeOfFloat && fieldType == Types.TypeOfLong /* FloatToBigint */ ||
                    propertyType == Types.TypeOfFloat && fieldType == Types.TypeOfShort /* FloatToShort */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfDateTime /* StringToDate */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfShort /* StringToShort */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfInt /* StringToInt */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfLong /* StringToLong */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfDouble /* StringToDouble */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfDecimal /* StringToDecimal */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfFloat /* StringToFloat */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfBoolean /* StringToBoolean */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfGuid /* StringToGuid */ ||
                    propertyType == Types.TypeOfGuid && fieldType == Types.TypeOfString /* GuidToString */)
                {
                    fieldOrPropertyType = fieldType;
                }
                else if (propertyType == Types.TypeOfGuid && fieldType == Types.TypeOfString /* UniqueIdentifierToString */)
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
                if (fieldOrPropertyType != typeof(SqlVariant) && !string.Equals(dbField.DatabaseType, "sql_variant", StringComparison.OrdinalIgnoreCase))
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
                var dbTypeAssignment = Expression.Call(parameterVariable, Types.DbParameterDbTypeSetMethod, Expression.Constant(dbType));
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
                var directionAssignment = Expression.Call(parameterVariable, Types.DbParameterDirectionSetMethod, Expression.Constant(direction));
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
                var sizeAssignment = Expression.Call(parameterVariable, Types.DbParameterSizeSetMethod, Expression.Constant(dbField.Size.Value));
                parameterAssignments.Add(sizeAssignment);
            }
            //}

            #endregion

            #region Precision

            // Set the Precision
            if (dbField.Precision != null)
            {
                var precisionAssignment = Expression.Call(parameterVariable, Types.DbParameterPrecisionSetMethod, Expression.Constant(dbField.Precision.Value));
                parameterAssignments.Add(precisionAssignment);
            }

            #endregion

            #region Scale

            // Set the Scale
            if (dbField.Scale != null)
            {
                var scaleAssignment = Expression.Call(parameterVariable, Types.DbParameterScaleSetMethod, Expression.Constant(dbField.Scale.Value));
                parameterAssignments.Add(scaleAssignment);
            }

            #endregion

            // Add the actual addition
            parameterAssignments.Add(Expression.Call(dbParameterCollection, Types.DbParameterCollectionAddMethod, parameterVariable));

            // Return the value
            return Expression.Block(new[] { parameterVariable }, parameterAssignments);
        }

        private static Expression BuildParameterAssignmentExpression2<TEntity>
        (
            Expression instance,
            Expression property,
            DbField dbField,
            ClassProperty classProperty,
            bool skipValueAssignment,
            ParameterDirection direction,
            IDbSetting dbSetting,
            ClientTypeToDbTypeResolver dbTypeResolver,
            Expression commandParameterExpression,
            Expression dbParameterCollection
        ) where TEntity : class
        {
            // Parameters for the block
            var typeOfEntity = typeof(TEntity);
            var parameterAssignments = new List<Expression>();

            // Parameter variables
            var parameterName = dbField.Name.AsUnquoted(true, dbSetting).AsAlphaNumeric();
            var parameterVariable = Expression.Variable(Types.TypeOfDbParameter, string.Concat("parameter", parameterName));
            var parameterInstance = Expression.Call(commandParameterExpression, Types.DbCommandCreateParameterMethod);
            parameterAssignments.Add(Expression.Assign(parameterVariable, parameterInstance));

            // Set the name
            var nameAssignment = Expression.Call(parameterVariable, Types.DbParameterParameterNameSetMethod, Expression.Constant(parameterName));
            parameterAssignments.Add(nameAssignment);

            // Property instance
            var instanceProperty = (PropertyInfo)null;
            var propertyType = (Type)null;
            var fieldType = dbField.Type?.GetUnderlyingType();

            //  Property handlers
            var propertyMap = (EntityPropertyMap<TEntity>)BaseEntity<TEntity>.PropertyMap;
            var converterMethod = (Delegate)((EntityPropertyConverter)propertyMap?.Find(classProperty.PropertyInfo))?.ToObject;

            #region Value

            // Set the value
            if (skipValueAssignment == false)
            {
                // Set the value

                // Check the proper type of the entity
                if (typeOfEntity != Types.TypeOfObject && typeOfEntity.IsGenericType == false)
                {
                    instanceProperty = classProperty.PropertyInfo; // typeOfEntity.GetProperty(classProperty.PropertyInfo.Name);
                }

                #region Instance.Property or PropertyInfo.GetValue()

                // Set the value
                Expression value;

                // If the property is missing directly, then it could be a dynamic object
                if (instanceProperty == null)
                {
                    value = Expression.Call(property, Types.PropertyInfoGetValueMethod, instance);
                }
                else
                {
                    propertyType = instanceProperty.PropertyType.GetUnderlyingType();

                    if (converterMethod == null)
                    {
                        if (Converter.ConversionType == ConversionType.Automatic)
                        {
                            var valueToConvert = Expression.Property(instance, instanceProperty);

                            #region StringToGuid

                            // Create a new guid here
                            if (propertyType == Types.TypeOfString && fieldType == Types.TypeOfGuid /* StringToGuid */)
                            {
                                value = Expression.New(Types.TypeOfGuid.GetConstructor(new[] { Types.TypeOfString }), new[] { valueToConvert });
                            }

                            #endregion

                            #region GuidToString

                            // Call the System.Convert conversion
                            else if (propertyType == Types.TypeOfGuid && fieldType == Types.TypeOfString/* GuidToString*/)
                            {
                                var convertMethod = typeof(Convert).GetMethod("ToString", new[] { Types.TypeOfObject });
                                value = Expression.Call(convertMethod, Expression.Convert(valueToConvert, Types.TypeOfObject));
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
                                    convertToTypeMethod = Types.TypeOfConvert.GetMethod(string.Concat("To", mappedToType.ToString()), new[] { Types.TypeOfObject });
                                }
                            }
                            if (convertToTypeMethod == null)
                            {
                                convertToTypeMethod = Types.TypeOfConvert.GetMethod(string.Concat("To", dbField.Type.Name), new[] { Types.TypeOfObject });
                            }
                            if (convertToTypeMethod == null)
                            {
                                throw new ConverterNotFoundException($"The convert between '{propertyType.FullName}' and database type '{dbField.DatabaseType}' (of .NET CLR '{dbField.Type.FullName}') is not found.");
                            }
                            else
                            {
                                var convertMethod = typeof(EnumHelper).GetMethod("Convert");
                                if (convertMethod != null)
                                {
                                    value = Expression.Call(convertMethod,
                                        Expression.Constant(instanceProperty.PropertyType),
                                        Expression.Constant(dbField.Type),
                                        Expression.Convert(value, Types.TypeOfObject),
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
                        value = Expression.Call(Expression.Constant(converterMethod.Target), converterMethod.Method, value);
                    }

                    // Convert to object
                    value = Expression.Convert(value, Types.TypeOfObject);
                }

                // Declare the variable for the value assignment
                Expression valueBlock;
                var isNullable = dbField.IsNullable 
                                 || instanceProperty == null 
                                 || instanceProperty.PropertyType.IsValueType == false 
                                 || Nullable.GetUnderlyingType(instanceProperty.PropertyType) != null;

                // The value for DBNull.Value
                var dbNullValue = Expression.Convert(Expression.Constant(DBNull.Value), Types.TypeOfObject);

                // Check if the property is nullable
                if (isNullable)
                {
                    // Identification of the DBNull
                    var valueVariable = Expression.Variable(Types.TypeOfObject, string.Concat("valueOf", parameterName));
                    var valueIsNull = Expression.Equal(valueVariable, Expression.Constant(null));

                    // Set the property value
                    valueBlock = Expression.Block(new[] { valueVariable },
                        Expression.Assign(valueVariable, value),
                        Expression.Condition(valueIsNull, dbNullValue, valueVariable));
                }
                else
                {
                    valueBlock = value;
                }

                // Add to the collection
                var valueAssignment = Expression.Call(parameterVariable, Types.DbParameterValueSetMethod, valueBlock);

                #endregion

                // Check if it is a direct assignment or not
                if (typeof(TEntity) != Types.TypeOfObject)
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
                        dbNullValueAssignment = Expression.Call(parameterVariable, Types.DbParameterValueSetMethod,
                            Expression.Convert(Expression.Default(dbField.Type), Types.TypeOfObject));
                    }

                    // Set the DBNull value
                    if (dbNullValueAssignment == null)
                    {
                        dbNullValueAssignment = Expression.Call(parameterVariable, Types.DbParameterValueSetMethod, dbNullValue);
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
            //if (fieldOrPropertyType != typeOfTimeSpan)
            //{
            // Identify the DB Type
            var fieldOrPropertyType = (Type)null;
            var dbType = (DbType?)null;

            // Identify the conversion
            if (Converter.ConversionType == ConversionType.Automatic)
            {
                // Identity the conversion
                if (propertyType == Types.TypeOfDateTime && fieldType == Types.TypeOfString /* DateTimeToString */ ||
                    propertyType == Types.TypeOfDecimal && (fieldType == Types.TypeOfFloat || fieldType == Types.TypeOfDouble) /* DecimalToFloat/DecimalToDouble */ ||
                    propertyType == Types.TypeOfDouble && fieldType == Types.TypeOfLong /* DoubleToBigint */||
                    propertyType == Types.TypeOfDouble && fieldType == Types.TypeOfInt /* DoubleToBigint */ ||
                    propertyType == Types.TypeOfDouble && fieldType == Types.TypeOfShort /* DoubleToShort */||
                    propertyType == Types.TypeOfFloat && fieldType == Types.TypeOfLong /* FloatToBigint */ ||
                    propertyType == Types.TypeOfFloat && fieldType == Types.TypeOfShort /* FloatToShort */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfDateTime /* StringToDate */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfShort /* StringToShort */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfInt /* StringToInt */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfLong /* StringToLong */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfDouble /* StringToDouble */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfDecimal /* StringToDecimal */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfFloat /* StringToFloat */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfBoolean /* StringToBoolean */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfGuid /* StringToGuid */ ||
                    propertyType == Types.TypeOfGuid && fieldType == Types.TypeOfString /* GuidToString */)
                {
                    fieldOrPropertyType = fieldType;
                }
                else if (propertyType == Types.TypeOfGuid && fieldType == Types.TypeOfString /* UniqueIdentifierToString */)
                {
                    fieldOrPropertyType = propertyType;
                }
                if (fieldOrPropertyType != null)
                {
                    dbType = dbTypeResolver.Resolve(fieldOrPropertyType);
                }
            }

            // Get the class property
            if (dbType == null && converterMethod == null)
            {
                if (fieldOrPropertyType != typeof(SqlVariant) && !string.Equals(dbField.DatabaseType, "sql_variant", StringComparison.OrdinalIgnoreCase))
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
                var dbTypeAssignment = Expression.Call(parameterVariable, Types.DbParameterDbTypeSetMethod, Expression.Constant(dbType));
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
                var directionAssignment = Expression.Call(parameterVariable, Types.DbParameterDirectionSetMethod, Expression.Constant(direction));
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
                var sizeAssignment = Expression.Call(parameterVariable, Types.DbParameterSizeSetMethod, Expression.Constant(dbField.Size.Value));
                parameterAssignments.Add(sizeAssignment);
            }
            //}

            #endregion

            #region Precision

            // Set the Precision
            if (dbField.Precision != null)
            {
                var precisionAssignment = Expression.Call(parameterVariable, Types.DbParameterPrecisionSetMethod, Expression.Constant(dbField.Precision.Value));
                parameterAssignments.Add(precisionAssignment);
            }

            #endregion

            #region Scale

            // Set the Scale
            if (dbField.Scale != null)
            {
                var scaleAssignment = Expression.Call(parameterVariable, Types.DbParameterScaleSetMethod, Expression.Constant(dbField.Scale.Value));
                parameterAssignments.Add(scaleAssignment);
            }

            #endregion

            // Add the actual addition
            parameterAssignments.Add(Expression.Call(dbParameterCollection, Types.DbParameterCollectionAddMethod, parameterVariable));

            // Return the value
            return Expression.Block(new[] { parameterVariable }, parameterAssignments);
        }

        private static Expression BuildParameterAssignmentExpression<TEntity>
        (
            int entityIndex,
            Expression instance,
            Expression property,
            DbField dbField,
            ClassProperty classProperty,
            bool skipValueAssignment,
            ParameterDirection direction,
            IDbSetting dbSetting,
            ClientTypeToDbTypeResolver dbTypeResolver,
            Expression commandParameterExpression,
            Expression dbParameterCollection
        ) where TEntity : class
        {
            // Parameters for the block
            var parameterAssignments = new List<Expression>();
            var typeOfEntity = typeof(TEntity);

            // Parameter variables
            var parameterName = dbField.Name.AsUnquoted(true, dbSetting).AsAlphaNumeric();
            var parameterVariable = Expression.Variable(Types.TypeOfDbParameter, string.Concat("parameter", parameterName));
            var parameterInstance = Expression.Call(commandParameterExpression, Types.DbCommandCreateParameterMethod);
            parameterAssignments.Add(Expression.Assign(parameterVariable, parameterInstance));

            // Set the name
            var nameAssignment = Expression.Call(parameterVariable, Types.DbParameterParameterNameSetMethod,
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
                if (typeOfEntity != Types.TypeOfObject && typeOfEntity.IsGenericType == false)
                {
                    instanceProperty = classProperty.PropertyInfo; // typeOfEntity.GetProperty(classProperty.PropertyInfo.Name);
                }

                #region PropertyHandler

                var propertyHandlerAttribute = instanceProperty?.GetCustomAttribute<PropertyHandlerAttribute>();

                if (propertyHandlerAttribute != null)
                {
                    // Get from the attribute
                    handlerInstance = PropertyHandlerCache.Get<TEntity, object>(classProperty.PropertyInfo);
                    handlerSetMethod = propertyHandlerAttribute.HandlerType.GetMethod("Set");
                }
                else
                {
                    // Get from the type level mappings (DB type)
                    handlerInstance = PropertyHandlerMapper.Get<object>(dbField.Type.GetUnderlyingType());
                    if (handlerInstance != null)
                    {
                        handlerSetMethod = handlerInstance.GetType().GetMethod("Set");
                    }
                }

                #endregion

                #region Instance.Property or PropertyInfo.GetValue()

                // Set the value
                var value = (Expression)null;

                // If the property is missing directly, then it could be a dynamic object
                if (instanceProperty == null)
                {
                    value = Expression.Call(property, Types.PropertyInfoGetValueMethod, instance);
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
                            if (propertyType == Types.TypeOfString && fieldType == Types.TypeOfGuid /* StringToGuid */)
                            {
                                value = Expression.New(Types.TypeOfGuid.GetConstructor(new[] { Types.TypeOfString }), valueToConvert);
                            }

                            #endregion

                            #region GuidToString

                            // Call the System.Convert conversion
                            else if (propertyType == Types.TypeOfGuid && fieldType == Types.TypeOfString/* GuidToString*/)
                            {
                                var convertMethod = typeof(Convert).GetMethod("ToString", new[] { Types.TypeOfObject });
                                value = Expression.Call(convertMethod, Expression.Convert(valueToConvert, Types.TypeOfObject));
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
                                    convertToTypeMethod = Types.TypeOfConvert.GetMethod(string.Concat("To", mappedToType.ToString()), new[] { Types.TypeOfObject });
                                }
                            }
                            if (convertToTypeMethod == null)
                            {
                                convertToTypeMethod = Types.TypeOfConvert.GetMethod(string.Concat("To", dbField.Type.Name), new[] { Types.TypeOfObject });
                            }
                            if (convertToTypeMethod == null)
                            {
                                throw new ConverterNotFoundException($"The convert between '{propertyType.FullName}' and database type '{dbField.DatabaseType}' (of .NET CLR '{dbField.Type.FullName}') is not found.");
                            }
                            else
                            {
                                var converterMethod = typeof(EnumHelper).GetMethod("Convert");
                                if (converterMethod != null)
                                {
                                    value = Expression.Call(converterMethod,
                                        Expression.Constant(instanceProperty.PropertyType),
                                        Expression.Constant(dbField.Type),
                                        Expression.Convert(value, Types.TypeOfObject),
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
                    value = Expression.Convert(value, Types.TypeOfObject);
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
                var dbNullValue = Expression.Convert(Expression.Constant(DBNull.Value), Types.TypeOfObject);

                // Check if the property is nullable
                if (isNullable == true)
                {
                    // Identification of the DBNull
                    var valueVariable = Expression.Variable(Types.TypeOfObject, string.Concat("valueOf", parameterName));
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
                valueAssignment = Expression.Call(parameterVariable, Types.DbParameterValueSetMethod, valueBlock);

                #endregion

                // Check if it is a direct assignment or not
                if (typeOfEntity != Types.TypeOfObject)
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
                        dbNullValueAssignment = Expression.Call(parameterVariable, Types.DbParameterValueSetMethod,
                            Expression.Convert(Expression.Default(dbField.Type), Types.TypeOfObject));
                    }

                    // Set the DBNull value
                    if (dbNullValueAssignment == null)
                    {
                        dbNullValueAssignment = Expression.Call(parameterVariable, Types.DbParameterValueSetMethod, dbNullValue);
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
            //if (fieldOrPropertyType != typeOfTimeSpan)
            //{
            // Identify the DB Type
            var fieldOrPropertyType = (Type)null;
            var dbType = (DbType?)null;

            // Identify the conversion
            if (Converter.ConversionType == ConversionType.Automatic)
            {
                // Identity the conversion
                if (propertyType == Types.TypeOfDateTime && fieldType == Types.TypeOfString /* DateTimeToString */ ||
                    propertyType == Types.TypeOfDecimal && (fieldType == Types.TypeOfFloat || fieldType == Types.TypeOfDouble) /* DecimalToFloat/DecimalToDouble */ ||
                    propertyType == Types.TypeOfDouble && fieldType == Types.TypeOfLong /* DoubleToBigint */||
                    propertyType == Types.TypeOfDouble && fieldType == Types.TypeOfInt /* DoubleToBigint */ ||
                    propertyType == Types.TypeOfDouble && fieldType == Types.TypeOfShort /* DoubleToShort */||
                    propertyType == Types.TypeOfFloat && fieldType == Types.TypeOfLong /* FloatToBigint */ ||
                    propertyType == Types.TypeOfFloat && fieldType == Types.TypeOfShort /* FloatToShort */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfDateTime /* StringToDate */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfShort /* StringToShort */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfInt /* StringToInt */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfLong /* StringToLong */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfDouble /* StringToDouble */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfDecimal /* StringToDecimal */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfFloat /* StringToFloat */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfBoolean /* StringToBoolean */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfGuid /* StringToGuid */ ||
                    propertyType == Types.TypeOfGuid && fieldType == Types.TypeOfString /* GuidToString */)
                {
                    fieldOrPropertyType = fieldType;
                }
                else if (propertyType == Types.TypeOfGuid && fieldType == Types.TypeOfString /* UniqueIdentifierToString */)
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
                if (fieldOrPropertyType != typeof(SqlVariant) && !string.Equals(dbField.DatabaseType, "sql_variant", StringComparison.OrdinalIgnoreCase))
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
                var dbTypeAssignment = Expression.Call(parameterVariable, Types.DbParameterDbTypeSetMethod, Expression.Constant(dbType));
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
                var directionAssignment = Expression.Call(parameterVariable, Types.DbParameterDirectionSetMethod, Expression.Constant(direction));
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
                var sizeAssignment = Expression.Call(parameterVariable, Types.DbParameterSizeSetMethod, Expression.Constant(dbField.Size.Value));
                parameterAssignments.Add(sizeAssignment);
            }
            //}

            #endregion

            #region Precision

            // Set the Precision
            if (dbField.Precision != null)
            {
                var precisionAssignment = Expression.Call(parameterVariable, Types.DbParameterPrecisionSetMethod, Expression.Constant(dbField.Precision.Value));
                parameterAssignments.Add(precisionAssignment);
            }

            #endregion

            #region Scale

            // Set the Scale
            if (dbField.Scale != null)
            {
                var scaleAssignment = Expression.Call(parameterVariable, Types.DbParameterScaleSetMethod, Expression.Constant(dbField.Scale.Value));
                parameterAssignments.Add(scaleAssignment);
            }

            #endregion

            // Add the actual addition
            parameterAssignments.Add(Expression.Call(dbParameterCollection, Types.DbParameterCollectionAddMethod, parameterVariable));

            // Return the value
            return Expression.Block(new[] { parameterVariable }, parameterAssignments);
        }

        private static Expression BuildParameterAssignmentExpression2<TEntity>
        (
            int entityIndex,
            Expression instance,
            Expression property,
            DbField dbField,
            ClassProperty classProperty,
            bool skipValueAssignment,
            ParameterDirection direction,
            IDbSetting dbSetting,
            ClientTypeToDbTypeResolver dbTypeResolver,
            Expression commandParameterExpression,
            Expression dbParameterCollection
        ) where TEntity : class
        {
            // Parameters for the block
            var parameterAssignments = new List<Expression>();
            var typeOfEntity = typeof(TEntity);

            // Parameter variables
            var parameterName = dbField.Name.AsUnquoted(true, dbSetting).AsAlphaNumeric();
            var parameterVariable = Expression.Variable(Types.TypeOfDbParameter, string.Concat("parameter", parameterName));
            var parameterInstance = Expression.Call(commandParameterExpression, Types.DbCommandCreateParameterMethod);
            parameterAssignments.Add(Expression.Assign(parameterVariable, parameterInstance));

            // Set the name
            var nameAssignment = Expression.Call(parameterVariable, Types.DbParameterParameterNameSetMethod,
                Expression.Constant(entityIndex > 0 ? string.Concat(parameterName, "_", entityIndex) : parameterName));
            parameterAssignments.Add(nameAssignment);

            // Property instance
            var instanceProperty = (PropertyInfo)null;
            var propertyType = (Type)null;
            var fieldType = dbField.Type?.GetUnderlyingType();

            //  Property handlers
            var propertyMap = (EntityPropertyMap<TEntity>)BaseEntity<TEntity>.PropertyMap;
            var converterMethod = (Delegate)((EntityPropertyConverter)propertyMap?.Find(classProperty.PropertyInfo))?.ToObject;


            #region Value

            // Set the value
            if (skipValueAssignment == false)
            {
                // Set the value
                var valueAssignment = (Expression)null;

                // Check the proper type of the entity
                if (typeOfEntity != Types.TypeOfObject && typeOfEntity.IsGenericType == false)
                {
                    instanceProperty = classProperty.PropertyInfo; // typeOfEntity.GetProperty(classProperty.PropertyInfo.Name);
                }

                #region Instance.Property or PropertyInfo.GetValue()

                // Set the value
                var value = (Expression)null;

                // If the property is missing directly, then it could be a dynamic object
                if (instanceProperty == null)
                {
                    value = Expression.Call(property, Types.PropertyInfoGetValueMethod, instance);
                }
                else
                {
                    propertyType = instanceProperty?.PropertyType.GetUnderlyingType();

                    if (converterMethod == null)
                    {
                        if (Converter.ConversionType == ConversionType.Automatic)
                        {
                            var valueToConvert = Expression.Property(instance, instanceProperty);

                            #region StringToGuid

                            // Create a new guid here
                            if (propertyType == Types.TypeOfString && fieldType == Types.TypeOfGuid /* StringToGuid */)
                            {
                                value = Expression.New(Types.TypeOfGuid.GetConstructor(new[] { Types.TypeOfString }), valueToConvert);
                            }

                            #endregion

                            #region GuidToString

                            // Call the System.Convert conversion
                            else if (propertyType == Types.TypeOfGuid && fieldType == Types.TypeOfString/* GuidToString*/)
                            {
                                var convertMethod = typeof(Convert).GetMethod("ToString", new[] { Types.TypeOfObject });
                                value = Expression.Call(convertMethod, Expression.Convert(valueToConvert, Types.TypeOfObject));
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
                                    convertToTypeMethod = Types.TypeOfConvert.GetMethod(string.Concat("To", mappedToType.ToString()), new[] { Types.TypeOfObject });
                                }
                            }
                            if (convertToTypeMethod == null)
                            {
                                convertToTypeMethod = Types.TypeOfConvert.GetMethod(string.Concat("To", dbField.Type.Name), new[] { Types.TypeOfObject });
                            }
                            if (convertToTypeMethod == null)
                            {
                                throw new ConverterNotFoundException($"The convert between '{propertyType.FullName}' and database type '{dbField.DatabaseType}' (of .NET CLR '{dbField.Type.FullName}') is not found.");
                            }
                            else
                            {
                                var convertMethod = typeof(EnumHelper).GetMethod("Convert");
                                if (convertMethod != null)
                                {
                                    value = Expression.Call(convertMethod,
                                        Expression.Constant(instanceProperty.PropertyType),
                                        Expression.Constant(dbField.Type),
                                        Expression.Convert(value, Types.TypeOfObject),
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

                        // Get the value directly from the property
                        value = Expression.Property(instance, instanceProperty);
                        value = Expression.Call(Expression.Constant(converterMethod.Target), converterMethod.Method, value);
                    }

                    // Convert to object
                    value = Expression.Convert(value, Types.TypeOfObject);
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
                var dbNullValue = Expression.Convert(Expression.Constant(DBNull.Value), Types.TypeOfObject);

                // Check if the property is nullable
                if (isNullable == true)
                {
                    // Identification of the DBNull
                    var valueVariable = Expression.Variable(Types.TypeOfObject, string.Concat("valueOf", parameterName));
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
                valueAssignment = Expression.Call(parameterVariable, Types.DbParameterValueSetMethod, valueBlock);

                #endregion

                // Check if it is a direct assignment or not
                if (typeOfEntity != Types.TypeOfObject)
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
                        dbNullValueAssignment = Expression.Call(parameterVariable, Types.DbParameterValueSetMethod,
                            Expression.Convert(Expression.Default(dbField.Type), Types.TypeOfObject));
                    }

                    // Set the DBNull value
                    if (dbNullValueAssignment == null)
                    {
                        dbNullValueAssignment = Expression.Call(parameterVariable, Types.DbParameterValueSetMethod, dbNullValue);
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
            //if (fieldOrPropertyType != typeOfTimeSpan)
            //{
            // Identify the DB Type
            var fieldOrPropertyType = (Type)null;
            var dbType = (DbType?)null;

            // Identify the conversion
            if (Converter.ConversionType == ConversionType.Automatic)
            {
                // Identity the conversion
                if (propertyType == Types.TypeOfDateTime && fieldType == Types.TypeOfString /* DateTimeToString */ ||
                    propertyType == Types.TypeOfDecimal && (fieldType == Types.TypeOfFloat || fieldType == Types.TypeOfDouble) /* DecimalToFloat/DecimalToDouble */ ||
                    propertyType == Types.TypeOfDouble && fieldType == Types.TypeOfLong /* DoubleToBigint */||
                    propertyType == Types.TypeOfDouble && fieldType == Types.TypeOfInt /* DoubleToBigint */ ||
                    propertyType == Types.TypeOfDouble && fieldType == Types.TypeOfShort /* DoubleToShort */||
                    propertyType == Types.TypeOfFloat && fieldType == Types.TypeOfLong /* FloatToBigint */ ||
                    propertyType == Types.TypeOfFloat && fieldType == Types.TypeOfShort /* FloatToShort */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfDateTime /* StringToDate */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfShort /* StringToShort */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfInt /* StringToInt */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfLong /* StringToLong */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfDouble /* StringToDouble */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfDecimal /* StringToDecimal */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfFloat /* StringToFloat */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfBoolean /* StringToBoolean */ ||
                    propertyType == Types.TypeOfString && fieldType == Types.TypeOfGuid /* StringToGuid */ ||
                    propertyType == Types.TypeOfGuid && fieldType == Types.TypeOfString /* GuidToString */)
                {
                    fieldOrPropertyType = fieldType;
                }
                else if (propertyType == Types.TypeOfGuid && fieldType == Types.TypeOfString /* UniqueIdentifierToString */)
                {
                    fieldOrPropertyType = propertyType;
                }
                if (fieldOrPropertyType != null)
                {
                    dbType = dbTypeResolver.Resolve(fieldOrPropertyType);
                }
            }

            // Get the class property
            if (dbType == null && converterMethod == null)
            {
                if (fieldOrPropertyType != typeof(SqlVariant) && !string.Equals(dbField.DatabaseType, "sql_variant", StringComparison.OrdinalIgnoreCase))
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
                var dbTypeAssignment = Expression.Call(parameterVariable, Types.DbParameterDbTypeSetMethod, Expression.Constant(dbType));
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
                var directionAssignment = Expression.Call(parameterVariable, Types.DbParameterDirectionSetMethod, Expression.Constant(direction));
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
                var sizeAssignment = Expression.Call(parameterVariable, Types.DbParameterSizeSetMethod, Expression.Constant(dbField.Size.Value));
                parameterAssignments.Add(sizeAssignment);
            }
            //}

            #endregion

            #region Precision

            // Set the Precision
            if (dbField.Precision != null)
            {
                var precisionAssignment = Expression.Call(parameterVariable, Types.DbParameterPrecisionSetMethod, Expression.Constant(dbField.Precision.Value));
                parameterAssignments.Add(precisionAssignment);
            }

            #endregion

            #region Scale

            // Set the Scale
            if (dbField.Scale != null)
            {
                var scaleAssignment = Expression.Call(parameterVariable, Types.DbParameterScaleSetMethod, Expression.Constant(dbField.Scale.Value));
                parameterAssignments.Add(scaleAssignment);
            }

            #endregion

            // Add the actual addition
            parameterAssignments.Add(Expression.Call(dbParameterCollection, Types.DbParameterCollectionAddMethod, parameterVariable));

            // Return the value
            return Expression.Block(new[] { parameterVariable }, parameterAssignments);
        }

    }
}
