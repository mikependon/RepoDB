using RepoDb.Enumerations;
using RepoDb.Exceptions;
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
    public static class FunctionFactory
    {
        /// <summary>
        /// Gets a compiled function that is used to convert the <see cref="DbDataReader"/> object into data entity object.
        /// </summary>
        /// <typeparam name="TEntity">The target entity type.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <returns>The entity instance with the converted values from the data reader.</returns>
        public static Func<DbDataReader, TEntity> GetDataReaderToDataEntityFunction<TEntity>(DbDataReader reader, IDbConnection connection)
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
            var memberAssignments = GetMemberAssignments<TEntity>(newEntityExpression, readerParameterExpression, readerFields, connection);

            // Throw an error if there are no matching atleast one
            if (memberAssignments.Any() == false)
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
        private static IEnumerable<MemberAssignment> GetMemberAssignments<TEntity>(Expression newEntityExpression,
            ParameterExpression readerParameterExpression,
            IEnumerable<DataReaderFieldDefinition> readerFields,
            IDbConnection connection)
            where TEntity : class
        {
            // Initialize variables
            var memberAssignments = new List<MemberAssignment>();
            var dataReaderType = typeof(DbDataReader);
            var tableFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<TEntity>());
            var strict = TypeMapper.ConversionType == ConversionType.Default;

            // Iterate each properties
            foreach (var property in PropertyCache.Get<TEntity>().Where(property => property.PropertyInfo.CanWrite))
            {
                // Gets the mapped name and the ordinal
                var mappedName = property.GetMappedName().ToLower();
                var ordinal = readerFields?.Select(f => f.Name).ToList().IndexOf(mappedName);

                // Process only if there is a correct ordinal
                if (ordinal >= 0)
                {
                    // Variables needed for the iteration
                    var tableField = tableFields?.FirstOrDefault(f => f.Name.ToLower() == mappedName);
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
                        if (strict == false && readerField.Type != typeof(Single))
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
                            if (strict == true)
                            {
                                falseExpression = Expression.Convert(falseExpression, propertyType);
                            }
                            else
                            {
                                falseExpression = Convert(falseExpression, readerField, propertyType, convertType, strict);
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
                            valueExpression = Convert(valueExpression, readerField, propertyType, convertType, strict);
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

        private static Expression Convert(Expression expression,
            DataReaderFieldDefinition readerField,
            Type propertyType,
            Type convertType,
            bool strict)
        {
            if (strict == true)
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
                    // This is Guid.Parse()
                    targetMethod = typeof(Guid).GetTypeInfo().GetMethod("Parse", new[] { typeof(string) });
                    targetInstance = null;
                    targetParameter = expression;
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

        /// <summary>
        /// Gets a compiled function that is used to convert the <see cref="DbDataReader"/> object into dynamic object (<see cref="ExpandoObject"/>).
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <returns>An instance of data entity object.</returns>
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
            var elementInits = GetElementInits(readerParameterExpression, readerFields?.ToList());

            // Throw an error if there are no matching atleast one
            if (elementInits.Any() == false)
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
        private static IEnumerable<ElementInit> GetElementInits(ParameterExpression readerParameterExpression, List<DataReaderFieldDefinition> readerFields)
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
    }
}
