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
            /// <returns>The enum value.</returns>
            public static object Parse(Type enumType,
                string value)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    return Enum.Parse(enumType?.GetUnderlyingType(), value, true);
                }
                if (enumType.IsNullable())
                {
                    var nullable = StaticType.Nullable.MakeGenericType(new[] { enumType });
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

        #endregion

        #region Helpers

        private static IEnumerable<DbField> GetDbFields(IDbConnection connection,
            string tableName,
            string connectionString,
            IDbTransaction transaction,
            bool enableValidation)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return DbFieldCache.Get(connection, tableName, transaction, enableValidation);
            }
            else
            {
                using (var dbConnection = (DbConnection)Activator.CreateInstance(connection.GetType()))
                {
                    dbConnection.ConnectionString = connectionString;
                    return DbFieldCache.Get(dbConnection, tableName, transaction, enableValidation);
                }
            }
        }

        private static IEnumerable<DataReaderField> GetDataReaderFields(DbDataReader reader,
            IEnumerable<DbField> dbFields,
            IDbSetting dbSetting)
        {
            return Enumerable.Range(0, reader.FieldCount)
                .Select(reader.GetName)
                .Select((name, ordinal) => new DataReaderField
                {
                    Name = name,
                    Ordinal = ordinal,
                    Type = reader.GetFieldType(ordinal),
                    DbField = dbFields?.FirstOrDefault(dbField => string.Equals(dbField.Name.AsUnquoted(true, dbSetting), name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase))
                });
        }

        private static object GetHandlerInstance(ClassProperty classProperty,
            DataReaderField readerField)
        {
            if (classProperty == null)
            {
                return null;
            }
            var value = classProperty.GetPropertyHandler();

            if (value == null && readerField?.Type != null)
            {
                value = PropertyHandlerCache
                    .Get<object>(readerField.Type.GetUnderlyingType());
            }

            return value;
        }

        private static MethodInfo GetNonTimeSpanReaderGetValueMethod(DataReaderField readerField,
            Type targetType)
        {
            if (targetType == StaticType.TimeSpan)
            {
                return null;
            }

            return StaticType
                .DbDataReader
                .GetMethod(string.Concat("Get", readerField?.Type?.Name));
        }

        private static MethodInfo GetNonSingleReaderGetValueMethod(ClassProperty classProperty,
            DataReaderField readerField,
            Type targetType)
        {
            var handlerInstance = GetHandlerInstance(classProperty, readerField);
            var isDefaultConversion = (Converter.ConversionType == ConversionType.Default) ?
                true : (handlerInstance != null);

            if (isDefaultConversion == false && readerField?.Type != StaticType.Single)
            {
                return StaticType
                    .DbDataReader
                    .GetMethod(string.Concat("Get", targetType.Name));
            }
            else
            {
                return null;
            }
        }

        private static MethodInfo GetDbReaderGetValueMethod(ClassProperty classProperty,
            DataReaderField readerField,
            Type targetType)
        {
            var handlerInstance = GetHandlerInstance(classProperty, readerField);
            var isDefaultConversion = (Converter.ConversionType == ConversionType.Default) ?
                true : (handlerInstance != null);

            var methodInfo = GetNonTimeSpanReaderGetValueMethod(readerField, targetType) ??
                GetNonSingleReaderGetValueMethod(classProperty, readerField, targetType) ??
                StaticType.DbDataReader.GetMethod("GetValue");

            return methodInfo;
        }

        private static bool? CheckIfConversionIsNeeded(DataReaderField readerField,
            Type targetType)
        {
            var methodInfo = GetNonTimeSpanReaderGetValueMethod(readerField, targetType);
            return (methodInfo == null) ? (bool?)true : null;
        }

        private static Type GetConvertType(ClassProperty classProperty,
            DataReaderField readerField,
            Type targetType)
        {
            var methodInfo = GetNonTimeSpanReaderGetValueMethod(readerField, targetType);
            var value = (Type)null;

            if (methodInfo == null)
            {
                methodInfo = GetNonSingleReaderGetValueMethod(classProperty, readerField, targetType);
                if (methodInfo != null)
                {
                    value = targetType;
                }
                else
                {
                    value = StaticType.Object;
                }
            }

            return value ?? readerField.Type;
        }

        private static Expression ConvertExpressionToStringToEnumExpression(Expression expression,
            Type propertyType,
            Type targetType)
        {
            var enumParseMethod = typeof(EnumHelper).GetMethod("Parse",
                new[] { StaticType.Type, StaticType.String });

            expression = Expression.Call(enumParseMethod, new[]
            {
                Expression.Constant(propertyType),
                expression
            });

            var enumPropertyType = targetType;
            if (propertyType.IsNullable())
            {
                enumPropertyType = StaticType.Nullable.MakeGenericType(targetType);
            }

            return Expression.Convert(expression, enumPropertyType);
        }

        private static Expression ConvertExpressionToTypeToEnumExpression(Expression expression,
            Type propertyType,
            Type targetType)
        {
            var enumToObjectMethod = StaticType.Enum.GetMethod("ToObject", new[]
            {
                StaticType.Type,
                propertyType
            });

            if (propertyType == StaticType.Boolean)
            {
                expression = Expression.Convert(expression, StaticType.Object);
            }

            return Expression.Call(enumToObjectMethod, new[]
            {
                Expression.Constant(targetType),
                expression
            });
        }

        private static Expression ConvertExpressionToGuidToStringExpression(Expression expression)
        {
            return Expression.New(StaticType.Guid.GetConstructor(new[] { StaticType.String }), expression);
        }

        private static Expression ConvertExpressionToStringToGuidExpression(Expression expression)
        {
            var methodInfo = StaticType.Guid.GetMethod("ToString", new Type[0]);
            return Expression.Call(expression, methodInfo);
        }

        private static Expression ConvertExpressionToSystemConvertExpression(Expression expression,
            Type propertyType,
            Type convertType)
        {
            var methodInfo = StaticType.Convert.GetMethod(string.Concat("To", propertyType.Name),
                new[] { convertType });

            return (methodInfo != null) ? (Expression)Expression.Call(null, methodInfo, expression) :
                (Expression)Expression.Convert(expression, propertyType);
        }

        private static Expression ConvertExpressionToEnumExpression(Expression expression,
            DataReaderField readerField) =>
            ConvertExpressionToEnumExpression(expression, readerField, readerField.Type, readerField.Type);

        private static Expression ConvertExpressionToEnumExpression(Expression expression,
            DataReaderField readerField,
            Type propertyType,
            Type targetType)
        {
            if (readerField.Type == StaticType.String)
            {
                return ConvertExpressionToStringToEnumExpression(expression, propertyType, targetType);
            }
            else
            {
                expression = ConvertExpressionToTypeToEnumExpression(expression, readerField.Type, targetType);
                // TODO: How to eliminate this conversion?
                return Expression.Convert(expression, targetType);
            }
        }

        private static Expression ConvertExpressionWithDefaultConversion(Expression expression,
            DataReaderField readerField,
            Type propertyType)
        {
            if (propertyType.IsEnum)
            {
                return ConvertExpressionToEnumExpression(expression, readerField);
            }
            else
            {
                return Expression.Convert(expression, propertyType);
            }
        }

        private static Expression ConvertExpressionWithAutomaticConversion(Expression expression,
            DataReaderField readerField,
            Type propertyType,
            Type convertType)
        {
            if (propertyType == StaticType.Guid && readerField.Type == StaticType.String)
            {
                return ConvertExpressionToGuidToStringExpression(expression);
            }
            else if (propertyType == StaticType.String && readerField.Type == StaticType.Guid)
            {
                return ConvertExpressionToStringToGuidExpression(expression);
            }
            else
            {
                return ConvertExpressionToSystemConvertExpression(expression, propertyType, convertType);
            }
        }

        private static Expression ConvertValueExpressionViaPropertyHandler(Expression expression,
            ClassProperty classProperty,
            bool considerNullable)
        {
            var handlerInstance = classProperty.GetPropertyHandler();
            if (handlerInstance == null)
            {
                return null;
            }

            var handlerGetMethod = handlerInstance?.GetType().GetMethod("Get");
            var getParameter = handlerGetMethod?.GetParameters()?.First();
            var getParameterUnderlyingType = getParameter != null ?
                Nullable.GetUnderlyingType(getParameter.ParameterType) : null;

            if (considerNullable == false && getParameterUnderlyingType != null)
            {
                var nullableGetConstructor = getParameter?.ParameterType.GetConstructor(new[] { getParameterUnderlyingType });
                expression = Expression.New(nullableGetConstructor, expression);
            }

            expression = Expression.Call(Expression.Constant(handlerInstance),
                handlerGetMethod,
                expression,
                Expression.Constant(classProperty));

            if (handlerGetMethod.ReturnType != classProperty.PropertyInfo.PropertyType)
            {
                expression = Expression.Convert(expression, classProperty.PropertyInfo.PropertyType);
            }

            return expression;
        }

        private static Expression ConvertExpressionForDataEntity(Expression expression,
            ClassProperty classProperty,
            DataReaderField readerField,
            Type propertyType,
            Type convertType)
        {
            var handlerInstance = GetHandlerInstance(classProperty, readerField);
            var handlerGetMethod = handlerInstance?.GetType().GetMethod("Get");
            var getParameter = handlerGetMethod?.GetParameters()?.First();

            propertyType = getParameter?.ParameterType?.GetUnderlyingType() ?? propertyType;

            if (Converter.ConversionType == ConversionType.Default)
            {
                return ConvertExpressionWithDefaultConversion(expression,
                    readerField,
                    propertyType);
            }
            else
            {
                return ConvertExpressionWithAutomaticConversion(expression,
                    readerField,
                    propertyType,
                    convertType);
            }
        }

        /*
         * TODO: Refactor this calls.
         * var handlerGetMethod = handlerInstance?.GetType().GetMethod("Get");
         * var getParameter = handlerGetMethod?.GetParameters()?.First();
         * var getParameterUnderlyingType = getParameter != null ?
         *      Nullable.GetUnderlyingType(getParameter.ParameterType) : null;
         */

        private static Expression ConvertTrueExpressionViaPropertyHandler(Expression trueExpression,
            ClassProperty classProperty,
            DataReaderField readerField,
            bool considerNullable)
        {
            var handlerInstance = classProperty.GetPropertyHandler();
            if (handlerInstance == null)
            {
                return trueExpression;
            }

            var handlerGetMethod = handlerInstance?.GetType().GetMethod("Get");
            var getParameter = handlerGetMethod?.GetParameters()?.First();
            var getParameterUnderlyingType = getParameter != null ?
                Nullable.GetUnderlyingType(getParameter.ParameterType) : null;

            if (considerNullable == false && getParameterUnderlyingType != null)
            {
                var nullableGetConstructor = getParameter?.ParameterType.GetConstructor(new[] { getParameterUnderlyingType });
                trueExpression = Expression.New(nullableGetConstructor, trueExpression);
            }
            if (readerField.Type != getParameter.ParameterType.GetUnderlyingType())
            {
                trueExpression = Expression.Call(Expression.Constant(handlerInstance),
                    handlerGetMethod,
                    Expression.Convert(trueExpression, getParameter.ParameterType.GetUnderlyingType()),
                    Expression.Constant(classProperty));
            }
            else
            {
                trueExpression = Expression.Call(Expression.Constant(handlerInstance),
                    handlerGetMethod,
                    trueExpression,
                    Expression.Constant(classProperty));
            }
            if (handlerGetMethod.ReturnType != classProperty.PropertyInfo.PropertyType)
            {
                trueExpression = Expression.Convert(trueExpression, classProperty.PropertyInfo.PropertyType);
            }

            return trueExpression;
        }

        private static Expression GetNullableTrueExpressionForClassProperty(ClassProperty classProperty,
            DataReaderField readerField)
        {
            var trueExpression = (Expression)null;
            var propertyType = classProperty.PropertyInfo.PropertyType;
            var propertyUnderlyingType = Nullable.GetUnderlyingType(propertyType);
            var targetType = propertyUnderlyingType ?? propertyType;
            var handlerInstance = GetHandlerInstance(classProperty, readerField);
            var handlerGetMethod = handlerInstance?.GetType().GetMethod("Get");
            var getParameter = handlerGetMethod?.GetParameters()?.First();
            var getParameterUnderlyingType = getParameter != null ?
                Nullable.GetUnderlyingType(getParameter.ParameterType) : null;
            var isNullableAlreadySet = false;

            // Check for nullable
            if (propertyUnderlyingType != null && propertyUnderlyingType.IsValueType == true)
            {
                if (handlerInstance == null || (handlerInstance != null && getParameterUnderlyingType != null))
                {
                    trueExpression = Expression.New(StaticType.Nullable.MakeGenericType(getParameter?.ParameterType.GetUnderlyingType() ?? targetType));
                    isNullableAlreadySet = true;
                }
            }

            // Check if it has been set
            if (trueExpression == null)
            {
                trueExpression = Expression.Default(getParameter?.ParameterType.GetUnderlyingType() ?? targetType);
            }

            // Property Handler
            trueExpression = ConvertTrueExpressionViaPropertyHandler(trueExpression,
                classProperty,
                readerField,
                isNullableAlreadySet);

            // Return the value
            return trueExpression;
        }

        /*
         * TODO: Refactor this calls.
         * var propertyType = classProperty.PropertyInfo.PropertyType;
         * var propertyUnderlyingType = Nullable.GetUnderlyingType(propertyType);
         * var targetType = propertyUnderlyingType ?? propertyType;
         */

        private static Expression AAA(Expression expression,
            ClassProperty classProperty,
            DataReaderField readerField)
        {
            var propertyType = classProperty.PropertyInfo.PropertyType;
            var propertyUnderlyingType = Nullable.GetUnderlyingType(propertyType);
            var targetType = propertyUnderlyingType ?? propertyType;
            var handlerInstance = GetHandlerInstance(classProperty, readerField);
            var handlerGetMethod = handlerInstance?.GetType().GetMethod("Get");
            var getParameter = handlerGetMethod?.GetParameters()?.First();

            if (targetType.IsEnum)
            {
                expression = ConvertExpressionToEnumExpression(expression, readerField,
                    propertyType, targetType);
            }
            else
            {
                // TimeSpanToDateTime
                if (readerField.Type == StaticType.DateTime && targetType == StaticType.TimeSpan)
                {
                    expression = Expression.Convert(expression, StaticType.DateTime);
                }

                // Default
                else
                {
                    expression = Expression.Convert(expression,
                        getParameter?.ParameterType?.GetUnderlyingType() ?? targetType);
                }
            }

            return expression;
        }

        private static Expression ConvertNullableFalseExpressionForClassProperty(Expression expression,
            ClassProperty classProperty,
            DataReaderField readerField)
        {
            var propertyType = classProperty.PropertyInfo.PropertyType;
            var propertyUnderlyingType = Nullable.GetUnderlyingType(propertyType);
            var targetType = propertyUnderlyingType ?? propertyType;
            var isConversionNeeded = CheckIfConversionIsNeeded(readerField, targetType) ??
                readerField.Type != targetType;

            // Check if conversion is needed
            if (isConversionNeeded == false)
            {
                return expression;
            }

            // Variables needed
            var handlerInstance = GetHandlerInstance(classProperty, readerField);
            var isDefaultConversion = (Converter.ConversionType == ConversionType.Default) ?
                true : (handlerInstance != null);

            // Check the conversion
            if (isDefaultConversion == true)
            {
                // Default
                if (handlerInstance == null)
                {
                    expression = AAA(expression, classProperty, readerField);
                }
            }
            else
            {
                var convertType = GetConvertType(classProperty, readerField, targetType) ??
                    readerField.Type;

                // Automatic
                expression = ConvertExpressionWithAutomaticConversion(expression,
                    readerField,
                    propertyType,
                    convertType);
            }

            if (handlerInstance == null)
            {
                // In SqLite, the Time column is represented as System.DateTime in .NET. If in any case that the models
                // has been designed to have it as System.TimeSpan, then we should somehow be able to set it properly.
                if (readerField.Type == StaticType.DateTime && targetType == StaticType.TimeSpan)
                {
                    var timeOfDayProperty = StaticType.DateTime.GetProperty("TimeOfDay");
                    expression = Expression.Property(expression, timeOfDayProperty);
                }
            }

            // Return the value
            return expression;
        }

        private static Expression GetExpressionForNullablePropertyType(Expression expression,
            ClassProperty classProperty,
            DataReaderField readerField,
            Type targetType)
        {
            var handlerInstance = GetHandlerInstance(classProperty, readerField);
            var setNullable = (targetType.IsEnum == false) || (targetType.IsEnum && readerField.Type != StaticType.String);
            if (setNullable == true)
            {
                var nullableConstructorExpression = StaticType.Nullable.MakeGenericType(targetType).GetConstructor(new[] { targetType });
                if (handlerInstance == null)
                {
                    return Expression.New(nullableConstructorExpression, expression);
                }
            }
            return expression;
        }

        private static Expression GetNullableFalseExpressionForClassProperty(ParameterExpression readerParameterExpression,
            ClassProperty classProperty,
            DataReaderField readerField,
            int ordinal)
        {
            var propertyType = classProperty.PropertyInfo.PropertyType;
            var propertyUnderlyingType = Nullable.GetUnderlyingType(propertyType);
            var targetType = propertyUnderlyingType ?? propertyType;
            var ordinalExpression = Expression.Constant(ordinal);
            var handlerInstance = GetHandlerInstance(classProperty, readerField);
            var handlerGetMethod = handlerInstance?.GetType().GetMethod("Get");
            var getParameter = handlerGetMethod?.GetParameters()?.First();
            var getParameterUnderlyingType = getParameter != null ?
                Nullable.GetUnderlyingType(getParameter.ParameterType) : null;
            var isConversionNeeded = CheckIfConversionIsNeeded(readerField, targetType) ??
                readerField.Type != targetType;
            var isDefaultConversion = (Converter.ConversionType == ConversionType.Default) ?
                true : (handlerInstance != null);
            var isDbNullExpression = Expression.Call(readerParameterExpression,
                StaticType.DbDataReader.GetMethod("IsDBNull"), ordinalExpression);
            var readerGetValueMethod = GetDbReaderGetValueMethod(classProperty,
                readerField,
                targetType);
            var falseExpression = (Expression)Expression.Call(readerParameterExpression,
                readerGetValueMethod,
                ordinalExpression);
            var convertType = GetConvertType(classProperty, readerField, targetType) ?? readerField.Type;

            // Only if there are conversions, execute the logics inside
            falseExpression = ConvertNullableFalseExpressionForClassProperty(falseExpression, classProperty, readerField);

            // Reset nullable variable
            var isNullableAlreadySet = false;
            if (propertyUnderlyingType != null && propertyUnderlyingType.IsValueType == true)
            {
                var setNullable = (targetType.IsEnum == false) || (targetType.IsEnum && readerField.Type != StaticType.String);
                if (setNullable == true)
                {
                    var nullableConstructorExpression = StaticType.Nullable.MakeGenericType(targetType).GetConstructor(new[] { targetType });
                    if (handlerInstance == null)
                    {
                        falseExpression = Expression.New(nullableConstructorExpression, falseExpression);
                        isNullableAlreadySet = true;
                    }
                }
            }

            #region PropertyHandler (FalseExpression)

            if (handlerInstance != null)
            {
                if (targetType != getParameterUnderlyingType)
                {
                    falseExpression = Expression.Convert(falseExpression, getParameter.ParameterType.GetUnderlyingType());
                }
                if (isNullableAlreadySet == false && getParameterUnderlyingType != null)
                {
                    var nullableGetConstructor = getParameter?.ParameterType.GetConstructor(new[] { getParameterUnderlyingType });
                    falseExpression = Expression.New(nullableGetConstructor, falseExpression);
                }
                falseExpression = Expression.Call(Expression.Constant(handlerInstance),
                    handlerGetMethod,
                    falseExpression,
                    Expression.Constant(classProperty));
                if (handlerGetMethod.ReturnType != classProperty.PropertyInfo.PropertyType)
                {
                    falseExpression = Expression.Convert(falseExpression, classProperty.PropertyInfo.PropertyType);
                }
            }

            #endregion

            // Return the value
            return falseExpression;
        }

        private static Expression GetNullableDbFieldValueExpressionForClassProperty(ParameterExpression readerParameterExpression,
            ClassProperty classProperty,
            DataReaderField readerField,
            int ordinal)
        {
            // IsDbNull Check
            var isDbNullExpression = Expression.Call(readerParameterExpression,
                StaticType.DbDataReader.GetMethod("IsDBNull"), Expression.Constant(ordinal));

            // True Expression
            var trueExpression = GetNullableTrueExpressionForClassProperty(classProperty,
                readerField);

            // False expression
            var falseExpression = GetNullableFalseExpressionForClassProperty(readerParameterExpression,
                classProperty, readerField, ordinal);

            // Set the value
            return Expression.Condition(isDbNullExpression, trueExpression, falseExpression);
        }

        private static Expression GetNonNullableDbFieldValueExpressionForClassProperty(ParameterExpression readerParameterExpression,
            ClassProperty classProperty,
            DataReaderField readerField,
            int ordinal)
        {
            var propertyType = classProperty.PropertyInfo.PropertyType;
            var propertyUnderlyingType = Nullable.GetUnderlyingType(propertyType);
            var targetType = propertyUnderlyingType ?? propertyType;
            var readerGetValueMethod = GetDbReaderGetValueMethod(classProperty,
                readerField,
                targetType);

            // Verify the get value if present
            if (readerGetValueMethod == null)
            {
                throw new MissingMethodException($"The data reader 'Get<Type>()' method is not found for type '{propertyType.FullName}'.");
            }

            // Variables needed
            var convertType = GetConvertType(classProperty, readerField, targetType) ??
                readerField.Type;
            var isConversionNeeded = CheckIfConversionIsNeeded(readerField, targetType) ??
                readerField.Type != targetType;
            var isNullableAlreadySet = false;

            // DbDataReader.Get<Type>()
            var expression = (Expression)Expression.Call(readerParameterExpression,
                readerGetValueMethod,
                Expression.Constant(ordinal));

            // Convert to Target Type
            if (isConversionNeeded == true)
            {
                expression = ConvertExpressionForDataEntity(expression,
                    classProperty,
                    readerField,
                    targetType,
                    convertType);
            }

            // Nullable Property
            if (propertyUnderlyingType != null && propertyUnderlyingType.IsValueType == true)
            {
                var nullableTypeExpression = GetExpressionForNullablePropertyType(expression,
                    classProperty,
                    readerField,
                    targetType);
                if (nullableTypeExpression != null)
                {
                    expression = nullableTypeExpression;
                    isNullableAlreadySet = true;
                }
            }

            // Property Handler
            expression = ConvertValueExpressionViaPropertyHandler(expression,
                classProperty, isNullableAlreadySet) ?? expression;

            // Return the value
            return expression;
        }

        private static Expression GetClassPropertyValueExpression(ParameterExpression readerParameterExpression,
            ClassProperty classProperty,
            DataReaderField readerField,
            int ordinal)
        {
            var isNullable = readerField.DbField == null || readerField.DbField?.IsNullable == true;

            if (isNullable == true)
            {
                // Expression for Nullables
                return GetNullableDbFieldValueExpressionForClassProperty(readerParameterExpression,
                    classProperty,
                    readerField,
                    ordinal);
            }
            else
            {
                // Expression for Non-Nullables
                return GetNonNullableDbFieldValueExpressionForClassProperty(readerParameterExpression,
                    classProperty,
                    readerField,
                    ordinal);
            }
        }

        #endregion

        #region GetDataReaderToDataEntityConverterFunction

        /// <summary>
        /// Gets a compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of data entity objects.
        /// </summary>
        /// <typeparam name="TEntity">The data entity object to convert to.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="connectionString">The raw connection string.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <param name="enableValidation">Enables the validation after retrieving the database fields.</param>
        /// <returns>A compiled function that is used to cover the <see cref="DbDataReader"/> object into a list of data entity objects.</returns>
        public static Func<DbDataReader, TEntity> GetDataReaderToDataEntityConverterFunction<TEntity>(DbDataReader reader,
            IDbConnection connection,
            string connectionString,
            IDbTransaction transaction,
            bool enableValidation)
            where TEntity : class
        {
            // Expression variables
            var readerParameterExpression = Expression.Parameter(StaticType.DbDataReader, "reader");
            var newEntityExpression = Expression.New(typeof(TEntity));

            // Ensure the fields
            var dbFields = GetDbFields(connection,
                ClassMappedNameCache.Get<TEntity>(),
                connectionString,
                transaction,
                enableValidation);

            // Get the reader fields
            var readerFields = GetDataReaderFields(reader, dbFields, connection?.GetDbSetting());

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
            var fieldNames = readerFields.Select(f => f.Name.ToLower()).AsList();
            var dbSetting = connection?.GetDbSetting();
            var classProperties = PropertyCache
                .Get<TEntity>()
                .Where(property => property.PropertyInfo.CanWrite)
                .Where(property =>
                    fieldNames.FirstOrDefault(field =>
                        string.Equals(field.AsUnquoted(true, dbSetting), property.GetMappedName().AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) != null);

            // Check the presence
            if (classProperties?.Any() != true)
            {
                return default;
            }

            // Variables needed
            var memberAssignments = new List<MemberAssignment>();

            // Iterate each properties
            foreach (var classProperty in classProperties)
            {
                var mappedName = classProperty
                    .GetMappedName()
                    .AsUnquoted(true, dbSetting);

                // Skip if not found
                var ordinal = fieldNames.IndexOf(mappedName.ToLower());
                if (ordinal < 0)
                {
                    continue;
                }

                // Get the value expression
                var readerField = readerFields.First(f => string.Equals(f.Name.AsUnquoted(true, dbSetting), mappedName.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase));
                var valueExpression = GetClassPropertyValueExpression(readerParameterExpression,
                    classProperty, readerField, ordinal);

                // Add the bindings
                memberAssignments.Add(Expression.Bind(classProperty.PropertyInfo, valueExpression));
            }

            // Return the value
            return memberAssignments;
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
            var readerParameterExpression = Expression.Parameter(StaticType.DbDataReader, "reader");
            var newObjectExpression = Expression.New(StaticType.ExpandoObject);

            // DB Variables
            var dbFields = tableName != null ? DbFieldCache.Get(connection, tableName, transaction) : null;

            // Get the reader fields
            var readerFields = GetDataReaderFields(reader, dbFields, connection?.GetDbSetting());

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
            var dataReaderType = StaticType.DbDataReader;
            var addMethod = StaticType.IDictionaryStringObject.GetMethod("Add", new[] { StaticType.String, StaticType.Object });

            // Iterate each properties
            for (var ordinal = 0; ordinal < readerFields?.Count; ordinal++)
            {
                // Field variable
                var readerField = readerFields[ordinal];
                var isConversionNeeded = false;

                // Get the correct method info, if the reader.Get<Type> is not found, then use the default GetValue
                var readerGetValueMethod = dataReaderType.GetMethod(string.Concat("Get", readerField.Type?.Name));
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
                    if (readerField.Type?.IsValueType != true)
                    {
                        trueExpression = Expression.Default(readerField.Type ?? StaticType.Object);
                        if (isConversionNeeded == true)
                        {
                            valueExpression = Expression.Convert(valueExpression, readerField.Type ?? StaticType.Object);
                        }
                    }
                    else
                    {
                        trueExpression = Expression.Constant(null, StaticType.Object);
                        valueExpression = Expression.Convert(valueExpression, StaticType.Object);
                    }
                    valueExpression = Expression.Condition(isDbNullExpression, trueExpression, valueExpression);
                }

                // Add to the bindings
                var values = new[]
                {
                    Expression.Constant(readerField.Name),
                    (Expression)Expression.Convert(valueExpression, StaticType.Object)
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
            var typeOfEntity = typeof(TEntity);

            // Variables for arguments
            var commandParameterExpression = Expression.Parameter(StaticType.DbCommand, "command");
            var entityParameterExpression = Expression.Parameter(typeOfEntity, "entity");

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

            // Other variables
            var dbTypeResolver = new ClientTypeToDbTypeResolver();

            // Reusable function for input/output fields
            var func = new Func<Expression, ParameterExpression, DbField, ClassProperty, bool, ParameterDirection, Expression>((Expression instance,
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
                var nameAssignment = Expression.Call(parameterVariable, dbParameterParameterNameSetMethod, Expression.Constant(parameterName));
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
                        propertyType = instanceProperty.PropertyType.GetUnderlyingType();

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
                                    var converterMethod = typeof(EnumHelper).GetMethod("Convert");
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
            var instanceType = Expression.Constant(typeOfEntity);
            var instanceTypeVariable = Expression.Variable(StaticType.Type, "instanceType");

            // Input fields properties
            if (inputFields?.Any() == true)
            {
                propertyVariableList.AddRange(inputFields.Select((value, index) =>
                    new
                    {
                        Index = index,
                        Field = value,
                        Direction = ParameterDirection.Input
                    }));
            }

            // Output fields properties
            if (outputFields?.Any() == true)
            {
                propertyVariableList.AddRange(outputFields.Select((value, index) =>
                    new
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

            // Reusable function for input/output fields
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
                                    var converterMethod = typeof(EnumHelper).GetMethod("Convert");
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

            // Variables for argument
            var entityParameterExpression = Expression.Parameter(typeOfEntity, "entity");
            var dbCommandParameterExpression = Expression.Parameter(StaticType.DbCommand, "command");

            // Variables for DbCommand
            var dbCommandParametersProperty = StaticType.DbCommand.GetProperty("Parameters");

            // Variables for DbParameterCollection
            var dbParameterCollectionIndexerMethod = StaticType.DbParameterCollection.GetMethod("get_Item", new[] { StaticType.String });

            // Variables for DbParameter
            var dbParameterValueProperty = StaticType.DbParameter.GetProperty("Value");

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

            // Variables for argument
            var entityParameter = Expression.Parameter(typeOfEntity, "entity");
            var valueParameter = Expression.Parameter(StaticType.Object, "value");

            // Get the entity property
            var property = (typeOfEntity.GetProperty(field.Name) ?? typeOfEntity.GetPropertyByMapping(field.Name)?.PropertyInfo)?.SetMethod;

            // Get the converter
            var toTypeMethod = StaticType.Converter.GetMethod("ToType", new[] { StaticType.Object }).MakeGenericMethod(field.Type.GetUnderlyingType());

            // Assign the value into DataEntity.Property
            var propertyAssignment = Expression.Call(entityParameter, property,
                Expression.Convert(Expression.Call(toTypeMethod, valueParameter), field.Type));

            // Return function
            return Expression.Lambda<Action<TEntity, object>>(propertyAssignment,
                entityParameter, valueParameter).Compile();
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
    }
}
