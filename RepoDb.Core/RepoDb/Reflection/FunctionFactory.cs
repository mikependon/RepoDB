using RepoDb.Enumerations;
using RepoDb.Exceptions;
using System;
using System.Collections.Generic;
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
        /// Gets a designated expression function for converting the <see cref="DbDataReader"/> object into class entity.
        /// </summary>
        /// <typeparam name="TEntity">The target entity type.</typeparam>
        /// <param name="reader">The data reader object.</param>
        /// <returns>The entity instance with the converted values from the data reader.</returns>
        public static Func<DbDataReader, TEntity> GetDataReaderToDataEntityFunction<TEntity>(DbDataReader reader)
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
            var memberAssignments = GetMemberAssignments<TEntity>(newEntityExpression, readerParameterExpression, readerFields);

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
        /// <returns>The enumerable list of member assignment and bindings.</returns>
        private static IEnumerable<MemberAssignment> GetMemberAssignments<TEntity>(Expression newEntityExpression,
            ParameterExpression readerParameterExpression, IEnumerable<DataReaderFieldDefinition> readerFields)
            where TEntity : class
        {
            // Initialize variables
            var memberAssignments = new List<MemberAssignment>();
            var dataReaderType = typeof(DbDataReader);
            var tableFields = FieldDefinitionCache.Get<TEntity>(Command.Query);
            var properties = PropertyCache.Get<TEntity>(Command.Query)
                .Where(property => property.PropertyInfo.CanWrite)
                .ToList();

            // Iterate each properties
            properties?.ForEach(property =>
            {
                // Get the mapped name, definition, and ordinal
                var mappedName = property.GetMappedName().ToLower();
                var tableField = tableFields?.FirstOrDefault(fd => fd.Name.ToLower() == mappedName);
                var readerField = readerFields.FirstOrDefault(f => f.Name == mappedName);
                var ordinal = readerFields.Select(f => f.Name).ToList().IndexOf(mappedName);

                // Process only if there is a correct ordinal
                if (ordinal >= 0)
                {
                    // Identify the value for null check
                    var isNullable = tableField == null || tableField?.IsNullable == true;

                    // Get the type
                    var underlyingType = Nullable.GetUnderlyingType(property.PropertyInfo.PropertyType);
                    var propertyType = underlyingType ?? property.PropertyInfo.PropertyType;
                    var isConversionNeeded = readerField?.Type != propertyType;

                    // Get the correct method info, if the reader.Get<Type> is not found, then use the default GetValue
                    var readerGetValueMethod = dataReaderType.GetTypeInfo().GetMethod($"Get{readerField?.Type.Name}");
                    if (readerGetValueMethod == null)
                    {
                        readerGetValueMethod = dataReaderType.GetTypeInfo().GetMethod($"Get{propertyType.Name}") ??
                            dataReaderType.GetTypeInfo().GetMethod("GetValue");
                        isConversionNeeded = true; // Force
                    }

                    // Expressions
                    var ordinalExpression = Expression.Constant(ordinal);
                    var valueExpression = (Expression)null;

                    // Check for nullables
                    if (isNullable == true)
                    {
                        var isDbNullExpression = Expression.Call(readerParameterExpression, dataReaderType.GetTypeInfo().GetMethod("IsDBNull"), ordinalExpression);
                        var trueExpression = Expression.Default(propertyType);
                        var falseExpression = (Expression)Expression.Call(readerParameterExpression, readerGetValueMethod, ordinalExpression);
                        if (isConversionNeeded == true)
                        {
                            falseExpression = Expression.Convert(falseExpression, propertyType);
                        }
                        valueExpression = Expression.Condition(isDbNullExpression, trueExpression, falseExpression);
                    }
                    else
                    {
                        // Call the actual Get<Type>/GetValue method by ordinal
                        valueExpression = Expression.Call(readerParameterExpression,
                            readerGetValueMethod,
                            ordinalExpression);
                        if (isConversionNeeded == true)
                        {
                            valueExpression = Expression.Convert(valueExpression, propertyType);
                        }
                    }

                    // Check if the property is a 'Nullable' property
                    if (underlyingType != null && underlyingType.GetTypeInfo().IsValueType == true)
                    {
                        // Create a new instance of nullable
                        var nullableConstructorExpression = typeof(Nullable<>).MakeGenericType(propertyType).GetTypeInfo().GetConstructor(new[] { propertyType });
                        valueExpression = Expression.New(nullableConstructorExpression, valueExpression);
                    }

                    // Set the actual property value
                    memberAssignments.Add(Expression.Bind(property.PropertyInfo, valueExpression));
                }
            });

            // Return the result
            return memberAssignments;
        }

        /// <summary>
        /// Gets a compiled function that is used to convert the <see cref="DbDataReader"/> object into data entity object.
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
                var readerGetValueMethod = dataReaderType.GetTypeInfo().GetMethod($"Get{field?.Type.Name}");
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
