using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.Reflection
{
    /// <summary>
    /// The compiler class of the library.
    /// </summary>
    internal partial class Compiler
    {
        #region SubClasses

        /// <summary>
        /// A helper class for type enum.
        /// </summary>
        internal class EnumHelper
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

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handlerInstance"></param>
        /// <returns></returns>
        internal static MethodInfo GetPropertyHandlerGetMethod(object handlerInstance)
        {
            return handlerInstance?.GetType().GetMethod("Get");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classProperty"></param>
        /// <returns></returns>
        internal static ParameterInfo GetPropertyHandlerGetParameter(ClassProperty classProperty) =>
            GetPropertyHandlerGetParameter(classProperty.GetPropertyHandler());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handlerInstance"></param>
        /// <returns></returns>
        internal static ParameterInfo GetPropertyHandlerGetParameter(object handlerInstance) =>
            GetPropertyHandlerGetParameter(GetPropertyHandlerGetMethod(handlerInstance));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="getMethod"></param>
        /// <returns></returns>
        internal static ParameterInfo GetPropertyHandlerGetParameter(MethodInfo getMethod)
        {
            return getMethod?.GetParameters()?.First();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        internal static Type GetParameterUnderlyingType(ParameterInfo parameter)
        {
            return parameter != null ?
                Nullable.GetUnderlyingType(parameter.ParameterType) : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classProperty"></param>
        /// <returns></returns>
        internal static Type GetTargetType(ClassProperty classProperty)
        {
            return classProperty.PropertyInfo.PropertyType.GetUnderlyingType();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerField"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        internal static Type GetConvertType(DataReaderField readerField,
            Type targetType)
        {
            var methodInfo = GetNonTimeSpanReaderGetValueMethod(readerField);
            var value = (Type)null;

            if (methodInfo == null)
            {
                methodInfo = GetNonSingleReaderGetValueMethod(readerField);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="connectionString"></param>
        /// <param name="transaction"></param>
        /// <param name="enableValidation"></param>
        /// <returns></returns>
        internal static IEnumerable<DbField> GetDbFields(IDbConnection connection,
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="dbFields"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static IEnumerable<DataReaderField> GetDataReaderFields(DbDataReader reader,
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classProperty"></param>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static object GetHandlerInstance(ClassProperty classProperty,
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static MethodInfo GetNonTimeSpanReaderGetValueMethod(DataReaderField readerField)
        {
            if (readerField?.Type == StaticType.TimeSpan)
            {
                return null;
            }
            return GetDbReaderGetValueMethod(readerField);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static MethodInfo GetNonSingleReaderGetValueMethod(DataReaderField readerField)
        {
            if (readerField?.Type == StaticType.Single)
            {
                return null;
            }
            return GetDbReaderGetValueMethod(readerField);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static MethodInfo GetDbReaderGetValueMethod(DataReaderField readerField) =>
            GetDbReaderGetValueMethod(readerField.Type);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        internal static MethodInfo GetDbReaderGetValueMethod(Type targetType)
        {
            return StaticType.DbDataReader.GetMethod(string.Concat("Get", targetType?.Name));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal static MethodInfo GetDbReaderGetValueMethod()
        {
            return StaticType.DbDataReader.GetMethod("GetValue");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static MethodInfo GetDbReaderGetValueOrDefaultMethod(DataReaderField readerField) =>
            GetDbReaderGetValueOrDefaultMethod(readerField.Type);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        internal static MethodInfo GetDbReaderGetValueOrDefaultMethod(Type targetType) =>
            GetDbReaderGetValueMethod(targetType) ?? GetDbReaderGetValueMethod();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static MethodInfo GetDbReaderGetValueTargettedMethod(DataReaderField readerField)
        {
            return GetNonTimeSpanReaderGetValueMethod(readerField) ??
                GetNonSingleReaderGetValueMethod(readerField) ??
                GetDbReaderGetValueMethod();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerField"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        internal static bool? CheckIfConversionIsNeeded(DataReaderField readerField,
            Type targetType)
        {
            var methodInfo = GetNonTimeSpanReaderGetValueMethod(readerField);
            return ((methodInfo == null) ? (bool?)true : null) ?? readerField.Type != targetType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="propertyType"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToStringToEnumExpression(Expression expression,
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="propertyType"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToTypeToEnumExpression(Expression expression,
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToGuidToStringExpression(Expression expression)
        {
            return Expression.New(StaticType.Guid.GetConstructor(new[] { StaticType.String }), expression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToStringToGuidExpression(Expression expression)
        {
            var methodInfo = StaticType.Guid.GetMethod("ToString", new Type[0]);
            return Expression.Call(expression, methodInfo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="propertyType"></param>
        /// <param name="convertType"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToSystemConvertExpression(Expression expression,
            Type propertyType,
            Type convertType)
        {
            var methodInfo = StaticType.Convert.GetMethod(string.Concat("To", propertyType.Name),
                new[] { convertType });

            return (methodInfo != null) ? (Expression)Expression.Call(null, methodInfo, expression) :
                (Expression)Expression.Convert(expression, propertyType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToEnumExpression(Expression expression,
            DataReaderField readerField) =>
            ConvertExpressionToEnumExpression(expression, readerField, readerField.Type, readerField.Type);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="readerField"></param>
        /// <param name="propertyType"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToEnumExpression(Expression expression,
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="readerField"></param>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionWithDefaultConversion(Expression expression,
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

        #endregion

        #region Common

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerParameterExpression"></param>
        /// <param name="classProperty"></param>
        /// <param name="readerField"></param>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        internal static Expression GetClassPropertyValueExpression(ParameterExpression readerParameterExpression,
            ClassProperty classProperty,
            DataReaderField readerField,
            int ordinal)
        {
            var isNullable = readerField.DbField == null || readerField.DbField?.IsNullable == true;

            if (isNullable == true)
            {
                // Expression for Nullables
                return GetNullableDbFieldValueExpression(readerParameterExpression,
                    classProperty, readerField, ordinal);
            }
            else
            {
                // Expression for Non-Nullables
                return GetNonNullableDbFieldValueExpression(readerParameterExpression,
                    classProperty, readerField, ordinal);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerParameterExpression"></param>
        /// <param name="classProperty"></param>
        /// <param name="readerField"></param>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        internal static Expression GetNullableDbFieldValueExpression(ParameterExpression readerParameterExpression,
            ClassProperty classProperty,
            DataReaderField readerField,
            int ordinal)
        {
            // IsDbNull Check
            var isDbNullExpression = Expression.Call(readerParameterExpression,
                StaticType.DbDataReader.GetMethod("IsDBNull"), Expression.Constant(ordinal));

            // True Expression
            var trueExpression = GetNullableTrueExpression(classProperty,
                readerField);

            // False expression
            var falseExpression = GetNullableFalseExpression(readerParameterExpression,
                classProperty, readerField, ordinal);

            // Set the value
            return Expression.Condition(isDbNullExpression, trueExpression, falseExpression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerParameterExpression"></param>
        /// <param name="classProperty"></param>
        /// <param name="readerField"></param>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        internal static Expression GetNonNullableDbFieldValueExpression(ParameterExpression readerParameterExpression,
            ClassProperty classProperty,
            DataReaderField readerField,
            int ordinal)
        {
            var propertyUnderlyingType = Nullable.GetUnderlyingType(classProperty.PropertyInfo.PropertyType);
            var targetType = GetTargetType(classProperty);
            var readerGetValueMethod = GetDbReaderGetValueMethod(readerField);

            // Verify the get value if present
            if (readerGetValueMethod == null)
            {
                throw new MissingMethodException($"The data reader 'Get<Type>()' method is not found for type '{classProperty.PropertyInfo.PropertyType.FullName}'.");
            }

            // Variables needed
            var convertType = GetConvertType(readerField, targetType);
            var isConversionNeeded = CheckIfConversionIsNeeded(readerField, targetType);
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
                var nullableExpression = ConvertExpressionForNullablePropertyType(expression,
                    classProperty,
                    readerField,
                    targetType);
                if (nullableExpression != null)
                {
                    expression = nullableExpression;
                    isNullableAlreadySet = true;
                }
            }

            // Property Handler
            expression = ConvertValueExpressionViaPropertyHandler(expression,
                classProperty,
                isNullableAlreadySet);

            // Return the value
            return expression;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classProperty"></param>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static Expression GetNullableTrueExpression(ClassProperty classProperty,
            DataReaderField readerField)
        {
            var trueExpression = (Expression)null;
            var propertyUnderlyingType = Nullable.GetUnderlyingType(classProperty.PropertyInfo.PropertyType);
            var targetType = GetTargetType(classProperty);
            var handlerInstance = GetHandlerInstance(classProperty, readerField);
            var getParameter = GetPropertyHandlerGetParameter(handlerInstance);
            var getParameterUnderlyingType = GetParameterUnderlyingType(getParameter);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerParameterExpression"></param>
        /// <param name="classProperty"></param>
        /// <param name="readerField"></param>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        internal static Expression GetNullableFalseExpression(ParameterExpression readerParameterExpression,
            ClassProperty classProperty,
            DataReaderField readerField,
            int ordinal)
        {
            var propertyUnderlyingType = Nullable.GetUnderlyingType(classProperty.PropertyInfo.PropertyType);
            var targetType = GetTargetType(classProperty);
            var ordinalExpression = Expression.Constant(ordinal);
            var readerGetValueMethod = GetDbReaderGetValueTargettedMethod(readerField);
            var expression = (Expression)Expression.Call(readerParameterExpression,
                readerGetValueMethod,
                ordinalExpression);
            var isNullableAlreadySet = false;

            // Nullable DB Field Expression
            expression = ConvertNullableFalseExpression(expression, classProperty, readerField);

            // Nullable Property
            if (propertyUnderlyingType != null && propertyUnderlyingType.IsValueType == true)
            {
                var nullableExpression = ConvertExpressionForNullablePropertyType(expression,
                    classProperty,
                    readerField,
                    targetType);
                if (nullableExpression != null)
                {
                    expression = nullableExpression;
                    isNullableAlreadySet = true;
                }
            }

            // Property Handler
            expression = ConvertValueExpressionViaPropertyHandler(expression,
                classProperty,
                isNullableAlreadySet);

            // Return the value
            return expression;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="classProperty"></param>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static Expression ConvertNullableFalseExpressionWithDefaultConversion(Expression expression,
            ClassProperty classProperty,
            DataReaderField readerField)
        {
            var getParameter = GetPropertyHandlerGetParameter(classProperty);
            var targetType = GetTargetType(classProperty);

            if (targetType.IsEnum)
            {
                expression = ConvertExpressionToEnumExpression(expression, readerField,
                    classProperty.PropertyInfo.PropertyType, targetType);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="readerField"></param>
        /// <param name="propertyType"></param>
        /// <param name="convertType"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionWithAutomaticConversion(Expression expression,
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="classProperty"></param>
        /// <param name="isNullableAlreadySet"></param>
        /// <returns></returns>
        internal static Expression ConvertValueExpressionViaPropertyHandler(Expression expression,
            ClassProperty classProperty,
            bool isNullableAlreadySet)
        {
            var handlerInstance = classProperty.GetPropertyHandler();
            if (handlerInstance == null)
            {
                return expression;
            }

            var targetType = GetTargetType(classProperty);
            var handlerGetMethod = GetPropertyHandlerGetMethod(handlerInstance);
            var getParameter = GetPropertyHandlerGetParameter(handlerGetMethod);
            var getParameterUnderlyingType = GetParameterUnderlyingType(getParameter);

            if (targetType != getParameterUnderlyingType)
            {
                expression = Expression.Convert(expression, getParameter.ParameterType.GetUnderlyingType());
            }

            if (isNullableAlreadySet == false && getParameterUnderlyingType != null)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="classProperty"></param>
        /// <param name="readerField"></param>
        /// <param name="propertyType"></param>
        /// <param name="convertType"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionForDataEntity(Expression expression,
            ClassProperty classProperty,
            DataReaderField readerField,
            Type propertyType,
            Type convertType)
        {
            var getParameter = GetPropertyHandlerGetParameter(classProperty);
            propertyType = GetParameterUnderlyingType(getParameter) ?? propertyType;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trueExpression"></param>
        /// <param name="classProperty"></param>
        /// <param name="readerField"></param>
        /// <param name="considerNullable"></param>
        /// <returns></returns>
        internal static Expression ConvertTrueExpressionViaPropertyHandler(Expression trueExpression,
            ClassProperty classProperty,
            DataReaderField readerField,
            bool considerNullable)
        {
            var handlerInstance = classProperty.GetPropertyHandler();
            if (handlerInstance == null)
            {
                return trueExpression;
            }

            var handlerGetMethod = GetPropertyHandlerGetMethod(handlerInstance);
            var getParameter = GetPropertyHandlerGetParameter(handlerGetMethod);
            var getParameterUnderlyingType = GetParameterUnderlyingType(getParameter);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="classProperty"></param>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static Expression ConvertNullableFalseExpression(Expression expression,
            ClassProperty classProperty,
            DataReaderField readerField)
        {
            var targetType = GetTargetType(classProperty);
            var isConversionNeeded = CheckIfConversionIsNeeded(readerField, targetType);

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
                    expression = ConvertNullableFalseExpressionWithDefaultConversion(expression, classProperty, readerField);
                }
            }
            else
            {
                var convertType = GetConvertType(readerField, targetType);

                // Automatic
                expression = ConvertExpressionWithAutomaticConversion(expression,
                    readerField,
                    classProperty.PropertyInfo.PropertyType,
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="classProperty"></param>
        /// <param name="readerField"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionForNullablePropertyType(Expression expression,
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
            return null;
        }

        /// <summary>
        /// Returns the list of the bindings for the entity.
        /// </summary>
        /// <typeparam name="TEntity">The target entity type.</typeparam>
        /// <param name="readerParameterExpression">The data reader parameter.</param>
        /// <param name="readerFields">The list of fields to be bound from the data reader.</param>
        /// <param name="dbSetting">The database setting that is being used.</param>
        /// <returns>The enumerable list of member assignment and bindings.</returns>
        internal static IEnumerable<MemberAssignment> GetMemberBindingsForDataEntity<TEntity>(ParameterExpression readerParameterExpression,
            IEnumerable<DataReaderField> readerFields,
            IDbSetting dbSetting)
            where TEntity : class
        {
            var fieldNames = readerFields.Select(f => f.Name.ToLower()).AsList();
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
                var ordinal = fieldNames.IndexOf(mappedName.ToLowerInvariant());
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

        /// <summary>
        /// Returns the list of the bindings for the object.
        /// </summary>
        /// <param name="readerParameterExpression">The data reader parameter.</param>
        /// <param name="readerFields">The list of fields to be bound from the data reader.</param>
        /// <returns>The enumerable list of child elements initializations.</returns>
        internal static IEnumerable<ElementInit> GetMemberBindingsForDictionary(ParameterExpression readerParameterExpression,
            IList<DataReaderField> readerFields)
        {
            // Initialize variables
            var elementInits = new List<ElementInit>();
            var dataReaderType = StaticType.DbDataReader;
            var addMethod = StaticType.IDictionaryStringObject.GetMethod("Add", new[] { StaticType.String, StaticType.Object });

            // Iterate each properties
            for (var ordinal = 0; ordinal < readerFields?.Count; ordinal++)
            {
                var readerField = readerFields[ordinal];
                var readerGetValueMethod = GetDbReaderGetValueOrDefaultMethod(readerField);
                var isConversionNeeded = readerGetValueMethod.ReturnType == StaticType.Object;
                var ordinalExpression = Expression.Constant(ordinal);
                var valueExpression = (Expression)Expression.Call(readerParameterExpression,
                    readerGetValueMethod, ordinalExpression);

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
                var values = new Expression[]
                {
                    Expression.Constant(readerField.Name),
                    Expression.Convert(valueExpression, StaticType.Object)
                };
                elementInits.Add(Expression.ElementInit(addMethod, values));
            }

            // Return the result
            return elementInits;
        }

        #endregion
    }
}
