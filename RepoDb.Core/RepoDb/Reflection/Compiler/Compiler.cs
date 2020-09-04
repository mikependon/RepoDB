using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;

namespace RepoDb.Reflection
{
    /// <summary>
    /// The compiler class of the library.
    /// </summary>
    internal partial class Compiler
    {
        #region SubClasses/SubStructs

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
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return Enum.Parse(enumType?.GetUnderlyingType(), value, true);
                }
                if (enumType.IsNullable())
                {
                    var nullable = StaticType.Nullable.MakeGenericType(new[] { enumType?.GetUnderlyingType() });
                    return Activator.CreateInstance(nullable);
                }
                else
                {
                    return Activator.CreateInstance(enumType);
                }
            }

            /// <summary>
            /// Converts the value using the desired convert method (of type <see cref="MethodInfo"/>). If not given, it will use the <see cref="System.Convert"/> class.
            /// </summary>
            /// <param name="dbField">The target <see cref="DbField"/> object.</param>
            /// <param name="value">The value to parse.</param>
            /// <param name="converterMethod">The converter method to be checked and used.</param>
            /// <returns>The converted value value.</returns>
            public static object Convert(DbField dbField,
                object value,
                MethodInfo converterMethod)
            {
                if (value == null)
                {
                    return dbField.IsNullable ? null :
                        dbField.Type.IsValueType ? Activator.CreateInstance(dbField.Type) : null;
                }
                if (converterMethod != null)
                {
                    return converterMethod.Invoke(null, new[] { value });
                }
                else
                {
                    return Converter.DbNullToNull(value) == null ? Activator.CreateInstance(dbField.Type) :
                        System.Convert.ChangeType(value, dbField.Type);
                }
            }
        }

        /// <summary>
        /// A class that contains both the instance of <see cref="RepoDb.ClassProperty"/> and <see cref="System.Reflection.ParameterInfo"/> objects.
        /// </summary>
        internal class ClassPropertyParameterInfo
        {
            /// <summary>
            /// Gets the instance of <see cref="RepoDb.ClassProperty"/> object in used.
            /// </summary>
            public ClassProperty ClassProperty { get; set; }

            /// <summary>
            /// Gets the instance of <see cref="System.Reflection.ParameterInfo"/> object in used.
            /// </summary>
            public ParameterInfo ParameterInfo { get; set; }

            /// <summary>
            /// Returns the string that represents this object.
            /// </summary>
            /// <returns>The presented string.</returns>
            public override string ToString() =>
                string.Concat("ClassProperty = ", ClassProperty?.ToString(), ", ParameterInfo = ", ParameterInfo?.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        internal struct FieldDirection
        {
            public int Index { get; set; }
            public DbField DbField { get; set; }
            public ParameterDirection Direction { get; set; }
        }

        /// <summary>
        /// A class that contains both the property <see cref="MemberAssignment"/> object and the constructor argument <see cref="Expression"/> value.
        /// </summary>
        internal struct MemberBinding
        {
            /// <summary>
            /// Gets the instance of <see cref="ClassProperty"/> object in used.
            /// </summary>
            public ClassProperty ClassProperty { get; set; }

            /// <summary>
            /// Gets the current member assignment of the defined property.
            /// </summary>
            public MemberAssignment MemberAssignment { get; set; }

            /// <summary>
            /// Gets the corresponding constructor argument of the defined property.
            /// </summary>
            public Expression Argument { get; set; }

            /// <summary>
            /// Returns the string that represents this object.
            /// </summary>
            /// <returns>The presented string.</returns>
            public override string ToString() =>
                ClassProperty.ToString();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        internal static IEnumerable<FieldDirection> GetInputFieldDirections(IEnumerable<DbField> fields)
        {
            if (fields?.Any() != true)
            {
                return Enumerable.Empty<FieldDirection>();
            }
            return fields?.Select((value, index) => new FieldDirection
            {
                Index = index,
                DbField = value,
                Direction = ParameterDirection.Input
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        internal static IEnumerable<FieldDirection> GetOutputFieldDirections(IEnumerable<DbField> fields)
        {
            if (fields?.Any() != true)
            {
                return Enumerable.Empty<FieldDirection>();
            }
            return fields?.Select((value, index) => new FieldDirection
            {
                Index = index,
                DbField = value,
                Direction = ParameterDirection.Output
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromType"></param>
        /// <param name="toType"></param>
        /// <returns></returns>
        internal static MethodInfo GetSystemConvertGetTypeMethod(Type fromType,
            Type toType) =>
            GetSystemConvertGetTypeMethod(fromType, toType.Name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromType"></param>
        /// <param name="toTypeName"></param>
        /// <returns></returns>
        internal static MethodInfo GetSystemConvertGetTypeMethod(Type fromType,
            string toTypeName) =>
            StaticType.Convert.GetMethod(string.Concat("To", toTypeName), new[] { fromType });

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static object GetClassHandler(Type type) =>
            ClassHandlerCache.Get<object>(type);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static MethodInfo GetClassHandlerGetMethod(Type type) =>
            GetClassHandlerGetMethod(GetClassHandler(type));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handlerInstance"></param>
        /// <returns></returns>
        internal static MethodInfo GetClassHandlerGetMethod(object handlerInstance) =>
            handlerInstance?.GetType().GetMethod("Get");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static MethodInfo GetClassHandlerSetMethod(Type type) =>
            GetClassHandlerSetMethod(GetClassHandler(type));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handlerInstance"></param>
        /// <returns></returns>
        internal static MethodInfo GetClassHandlerSetMethod(object handlerInstance) =>
            GetClassHandlerSetMethod(handlerInstance, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handlerInstance"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        internal static MethodInfo GetClassHandlerSetMethod(object handlerInstance, params Type[] types) =>
            handlerInstance?.GetType().GetMethod("Set", types);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="getMethod"></param>
        /// <returns></returns>
        internal static ParameterInfo GetClassHandlerGetParameter(MethodInfo getMethod) =>
            getMethod?.GetParameters()?.First();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="setMethod"></param>
        /// <returns></returns>
        internal static ParameterInfo GetClassHandlerSetParameter(MethodInfo setMethod) =>
            setMethod?.GetParameters()?.First();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classProperty"></param>
        /// <returns></returns>
        internal static MethodInfo GetPropertyHandlerGetMethod(ClassProperty classProperty) =>
            GetPropertyHandlerGetMethod(classProperty.GetPropertyHandler());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handlerInstance"></param>
        /// <returns></returns>
        internal static MethodInfo GetPropertyHandlerGetMethod(object handlerInstance) =>
            handlerInstance?.GetType().GetMethod("Get");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classProperty"></param>
        /// <returns></returns>
        internal static MethodInfo GetPropertyHandlerSetMethod(ClassProperty classProperty) =>
            GetPropertyHandlerSetMethod(classProperty.GetPropertyHandler());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handlerInstance"></param>
        /// <returns></returns>
        internal static MethodInfo GetPropertyHandlerSetMethod(object handlerInstance) =>
            handlerInstance?.GetType().GetMethod("Set");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classPropertyParameterInfo"></param>
        /// <returns></returns>
        internal static ParameterInfo GetPropertyHandlerGetParameter(ClassPropertyParameterInfo classPropertyParameterInfo) =>
            GetPropertyHandlerGetParameter(classPropertyParameterInfo.ClassProperty);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classProperty"></param>
        /// <returns></returns>
        internal static ParameterInfo GetPropertyHandlerGetParameter(ClassProperty classProperty) =>
            GetPropertyHandlerGetParameter(classProperty?.GetPropertyHandler());

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
        internal static ParameterInfo GetPropertyHandlerGetParameter(MethodInfo getMethod) =>
            getMethod?.GetParameters()?.First();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classProperty"></param>
        /// <returns></returns>
        internal static ParameterInfo GetPropertyHandlerSetParameter(ClassProperty classProperty) =>
            GetPropertyHandlerSetParameter(classProperty.GetPropertyHandler());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handlerInstance"></param>
        /// <returns></returns>
        internal static ParameterInfo GetPropertyHandlerSetParameter(object handlerInstance) =>
            GetPropertyHandlerSetParameter(GetPropertyHandlerSetMethod(handlerInstance));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="setMethod"></param>
        /// <returns></returns>
        internal static ParameterInfo GetPropertyHandlerSetParameter(MethodInfo setMethod) =>
            setMethod?.GetParameters()?.First();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        internal static Type GetParameterUnderlyingType(ParameterInfo parameter) =>
            parameter != null ? Nullable.GetUnderlyingType(parameter.ParameterType) : null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classPropertyParameterInfo"></param>
        /// <returns></returns>
        internal static Type GetTargetType(ClassPropertyParameterInfo classPropertyParameterInfo) =>
            (classPropertyParameterInfo.ParameterInfo?.ParameterType ??
                classPropertyParameterInfo.ClassProperty?.PropertyInfo.PropertyType)?.GetUnderlyingType();

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
            if (string.IsNullOrWhiteSpace(connectionString))
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
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="connectionString"></param>
        /// <param name="transaction"></param>
        /// <param name="enableValidation"></param>
        /// <returns></returns>
        internal static async Task<IEnumerable<DbField>> GetDbFieldsAsync(IDbConnection connection,
            string tableName,
            string connectionString,
            IDbTransaction transaction,
            bool enableValidation)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return await DbFieldCache.GetAsync(connection, tableName, transaction, enableValidation);
            }
            else
            {
                using (var dbConnection = (DbConnection)Activator.CreateInstance(connection.GetType()))
                {
                    dbConnection.ConnectionString = connectionString;
                    return await DbFieldCache.GetAsync(dbConnection, tableName, transaction, enableValidation);
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
                    Type = reader.GetFieldType(ordinal) ?? StaticType.Object,
                    DbField = dbFields?.FirstOrDefault(dbField => string.Equals(dbField.Name.AsUnquoted(true, dbSetting), name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase))
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classPropertyParameterInfo"></param>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static object GetHandlerInstance(ClassPropertyParameterInfo classPropertyParameterInfo,
            DataReaderField readerField) =>
            GetHandlerInstance(classPropertyParameterInfo.ClassProperty, readerField);

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
        internal static MethodInfo GetNonTimeSpanReaderGetValueMethod(DataReaderField readerField) =>
            (readerField?.Type == StaticType.TimeSpan) ? null : GetDbReaderGetValueMethod(readerField);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static MethodInfo GetNonSingleReaderGetValueMethod(DataReaderField readerField) =>
            (readerField?.Type == StaticType.Single) ? null : GetDbReaderGetValueMethod(readerField);

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
        internal static MethodInfo GetDbReaderGetValueMethod(Type targetType) =>
            StaticType.DbDataReader.GetMethod(string.Concat("Get", targetType?.Name));

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal static MethodInfo GetDbReaderGetValueMethod() =>
            StaticType.DbDataReader.GetMethod("GetValue");

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
        internal static MethodInfo GetDbReaderGetValueTargettedMethod(DataReaderField readerField) =>
            GetNonTimeSpanReaderGetValueMethod(readerField) ??
            GetNonSingleReaderGetValueMethod(readerField) ??
            GetDbReaderGetValueMethod();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal static MethodInfo GetDbParameterValueSetMethod() =>
            StaticType.DbParameter.GetProperty("Value").SetMethod;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerField"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        internal static bool? CheckIfConversionIsNeeded(DataReaderField readerField,
            Type targetType) =>
            ((GetNonTimeSpanReaderGetValueMethod(readerField) == null) ? (bool?)true : null) ?? readerField.Type != targetType;

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

            return ConvertValueExpressionToTypeExpression(expression, enumPropertyType);
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
                expression = ConvertValueExpressionToTypeExpression(expression, StaticType.Object);
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
        internal static Expression ConvertExpressionToGuidToStringExpression(Expression expression) =>
            Expression.New(StaticType.Guid.GetConstructor(new[] { StaticType.String }), expression);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToStringToGuidExpression(Expression expression) =>
            Expression.Call(expression, StaticType.Guid.GetMethod("ToString", new Type[0]));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="toType"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToSystemConvertExpression(Expression expression,
            Type toType)
        {
            if (expression.Type == toType)
            {
                return expression;
            }

            if (toType.IsAssignableFrom(expression.Type) == false)
            {
                var methodInfo = StaticType.Convert.GetMethod(string.Concat("To", toType.Name),
                    new[] { expression.Type });

                if (methodInfo != null)
                {
                    return Expression.Call(null, methodInfo, expression);
                }
            }

            return ConvertValueExpressionToTypeExpression(expression, toType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueExpression"></param>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static Expression ConvertValueExpressionToEnumExpression(Expression valueExpression,
            DataReaderField readerField) =>
            ConvertValueExpressionToEnumExpression(valueExpression, readerField, readerField.Type, readerField.Type);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueExpression"></param>
        /// <param name="readerField"></param>
        /// <param name="parameterOrPropertyType"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        internal static Expression ConvertValueExpressionToEnumExpression(Expression valueExpression,
            DataReaderField readerField,
            Type parameterOrPropertyType,
            Type targetType)
        {
            if (readerField.Type == StaticType.String)
            {
                return ConvertExpressionToStringToEnumExpression(valueExpression, parameterOrPropertyType, targetType);
            }
            else
            {
                valueExpression = ConvertExpressionToTypeToEnumExpression(valueExpression, readerField.Type, targetType);
                return ConvertValueExpressionToTypeExpression(valueExpression, targetType);
            }
        }

        #endregion

        #region Common

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerParameterExpression"></param>
        /// <param name="classPropertyParameterInfo"></param>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static Expression GetClassPropertyParameterInfoValueExpression(ParameterExpression readerParameterExpression,
            ClassPropertyParameterInfo classPropertyParameterInfo,
            DataReaderField readerField)
        {
            var isNullable = readerField.DbField == null || readerField.DbField?.IsNullable == true;

            if (isNullable == true)
            {
                // Expression for Nullables
                return GetNullableDbFieldValueExpression(readerParameterExpression,
                    classPropertyParameterInfo, readerField);
            }
            else
            {
                // Expression for Non-Nullables
                return GetNonNullableDbFieldValueExpression(readerParameterExpression,
                    classPropertyParameterInfo, readerField);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerParameterExpression"></param>
        /// <param name="classPropertyParameterInfo"></param>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static Expression GetNullableDbFieldValueExpression(ParameterExpression readerParameterExpression,
            ClassPropertyParameterInfo classPropertyParameterInfo,
            DataReaderField readerField)
        {
            // IsDbNull Check
            var isDbNullExpression = Expression.Call(readerParameterExpression,
                StaticType.DbDataReader.GetMethod("IsDBNull"), Expression.Constant(readerField.Ordinal));

            // True Expression
            var trueExpression = GetNullableTrueExpression(classPropertyParameterInfo,
                readerField);

            // False expression
            var falseExpression = GetNullableFalseExpression(readerParameterExpression,
                classPropertyParameterInfo, readerField);

            // Set the value
            return Expression.Condition(isDbNullExpression, trueExpression, falseExpression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerParameterExpression"></param>
        /// <param name="classPropertyParameterInfo"></param>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static Expression GetNonNullableDbFieldValueExpression(ParameterExpression readerParameterExpression,
            ClassPropertyParameterInfo classPropertyParameterInfo,
            DataReaderField readerField)
        {
            var underlyingType = Nullable.GetUnderlyingType(classPropertyParameterInfo.ParameterInfo?.ParameterType ??
                classPropertyParameterInfo.ClassProperty?.PropertyInfo?.PropertyType);
            var targetType = GetTargetType(classPropertyParameterInfo);
            var readerGetValueMethod = GetDbReaderGetValueMethod(readerField);

            // Verify the get value if present
            if (readerGetValueMethod == null)
            {
                throw new MissingMethodException($"The data reader 'Get<Type>()' method is not found for type '{readerField.Type.FullName}'.");
            }

            // Variables needed
            var isConversionNeeded = CheckIfConversionIsNeeded(readerField, targetType);
            var isNullableAlreadySet = false;

            // DbDataReader.Get<Type>()
            var expression = (Expression)Expression.Call(readerParameterExpression,
                readerGetValueMethod,
                Expression.Constant(readerField.Ordinal));

            // Convert to Target Type
            if (isConversionNeeded == true)
            {
                expression = ConvertValueExpressionForDataEntity(expression, classPropertyParameterInfo, readerField);
            }

            // Nullable Property
            if (underlyingType != null && underlyingType.IsValueType == true)
            {
                var nullableExpression = ConvertValueExpressionForNullablePropertyType(expression,
                    classPropertyParameterInfo, readerField);
                if (nullableExpression != null)
                {
                    expression = nullableExpression;
                    isNullableAlreadySet = true;
                }
            }

            // Property Handler
            expression = ConvertValueExpressionToPropertyHandlerGetExpression(expression,
                classPropertyParameterInfo, isNullableAlreadySet);

            // Return the value
            return expression;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classPropertyParameterInfo"></param>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static Expression GetNullableTrueExpression(ClassPropertyParameterInfo classPropertyParameterInfo,
            DataReaderField readerField)
        {
            var trueExpression = (Expression)null;
            var underlyingType = Nullable.GetUnderlyingType(classPropertyParameterInfo.ParameterInfo?.ParameterType ??
                classPropertyParameterInfo.ClassProperty?.PropertyInfo?.PropertyType);
            var targetType = GetTargetType(classPropertyParameterInfo);
            var handlerInstance = GetHandlerInstance(classPropertyParameterInfo, readerField);
            var getParameter = GetPropertyHandlerGetParameter(handlerInstance);
            var getParameterUnderlyingType = GetParameterUnderlyingType(getParameter);
            var isNullableAlreadySet = false;

            // Check for nullable
            if (underlyingType != null && underlyingType.IsValueType == true)
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
                classPropertyParameterInfo,
                readerField,
                isNullableAlreadySet);

            // Return the value
            return trueExpression;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerParameterExpression"></param>
        /// <param name="classPropertyParameterInfo"></param>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static Expression GetNullableFalseExpression(ParameterExpression readerParameterExpression,
            ClassPropertyParameterInfo classPropertyParameterInfo,
            DataReaderField readerField)
        {
            var underlyingType = Nullable.GetUnderlyingType(classPropertyParameterInfo.ParameterInfo?.ParameterType ??
                classPropertyParameterInfo.ClassProperty?.PropertyInfo?.PropertyType);
            var ordinalExpression = Expression.Constant(readerField.Ordinal);
            var readerGetValueMethod = GetDbReaderGetValueTargettedMethod(readerField);
            var expression = (Expression)Expression.Call(readerParameterExpression,
                readerGetValueMethod,
                ordinalExpression);
            var isNullableAlreadySet = false;

            // Nullable DB Field Expression
            expression = ConvertValueExpressionToNullableFalseExpression(expression, classPropertyParameterInfo, readerField);

            // Nullable Property
            if (underlyingType != null && underlyingType.IsValueType == true)
            {
                var nullableExpression = ConvertValueExpressionForNullablePropertyType(expression,
                    classPropertyParameterInfo, readerField);
                if (nullableExpression != null)
                {
                    expression = nullableExpression;
                    isNullableAlreadySet = true;
                }
            }

            // Property Handler
            expression = ConvertValueExpressionToPropertyHandlerGetExpression(expression,
                classPropertyParameterInfo,
                isNullableAlreadySet);

            // Return the value
            return expression;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueExpression"></param>
        /// <param name="toType"></param>
        /// <returns></returns>
        internal static Expression ConvertValueExpressionToTypeExpression(Expression valueExpression,
            Type toType) =>
            (valueExpression.Type != toType) ? Expression.Convert(valueExpression, toType) : valueExpression;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="classPropertyParameterInfo"></param>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static Expression ConvertNullableFalseExpressionWithDefaultConversion(Expression expression,
            ClassPropertyParameterInfo classPropertyParameterInfo,
            DataReaderField readerField)
        {
            var getParameter = GetPropertyHandlerGetParameter(classPropertyParameterInfo);
            var targetType = GetTargetType(classPropertyParameterInfo);

            if (targetType.IsEnum)
            {
                var parameterOrPropertyType = (classPropertyParameterInfo.ParameterInfo?.ParameterType ?? classPropertyParameterInfo.ClassProperty?.PropertyInfo?.PropertyType);
                expression = ConvertValueExpressionToEnumExpression(expression,
                    readerField, parameterOrPropertyType, targetType);
            }
            else
            {
                // TimeSpanToDateTime
                if (readerField.Type == StaticType.DateTime && targetType == StaticType.TimeSpan)
                {
                    expression = ConvertValueExpressionToTypeExpression(expression, StaticType.DateTime);
                }

                // Default
                else
                {
                    expression = ConvertValueExpressionToTypeExpression(expression,
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
        /// <returns></returns>
        internal static Expression ConvertExpressionWithAutomaticConversion(Expression expression,
            DataReaderField readerField,
            Type propertyType) =>
            ConvertExpressionWithAutomaticConversion(expression, propertyType, readerField.Type);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="toType"></param>
        /// <param name="fromType"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionWithAutomaticConversion(Expression expression,
            Type fromType,
            Type toType)
        {
            if (fromType == StaticType.Guid && toType == StaticType.String)
            {
                expression = ConvertExpressionToGuidToStringExpression(expression);
            }
            else if (fromType == StaticType.String && toType == StaticType.Guid)
            {
                expression = ConvertExpressionToStringToGuidExpression(expression);
            }
            else
            {
                expression = ConvertExpressionToSystemConvertExpression(expression, fromType);
            }
            return expression;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueExpression"></param>
        /// <param name="classPropertyParameterInfo"></param>
        /// <param name="isNullableAlreadySet"></param>
        /// <returns></returns>
        internal static Expression ConvertValueExpressionToPropertyHandlerGetExpression(Expression valueExpression,
            ClassPropertyParameterInfo classPropertyParameterInfo,
            bool isNullableAlreadySet)
        {
            var handlerInstance = classPropertyParameterInfo.ClassProperty?.GetPropertyHandler();
            if (handlerInstance == null)
            {
                return valueExpression;
            }

            var targetType = GetTargetType(classPropertyParameterInfo);
            var handlerGetMethod = GetPropertyHandlerGetMethod(handlerInstance);
            var getParameter = GetPropertyHandlerGetParameter(handlerGetMethod);
            var getParameterUnderlyingType = GetParameterUnderlyingType(getParameter);

            if (targetType != getParameterUnderlyingType)
            {
                valueExpression = ConvertValueExpressionToTypeExpression(valueExpression, getParameter.ParameterType.GetUnderlyingType());
            }

            if (isNullableAlreadySet == false && getParameterUnderlyingType != null)
            {
                var nullableGetConstructor = getParameter?.ParameterType.GetConstructor(new[] { getParameterUnderlyingType });
                valueExpression = Expression.New(nullableGetConstructor, valueExpression);
            }

            valueExpression = Expression.Call(Expression.Constant(handlerInstance),
                handlerGetMethod,
                valueExpression,
                Expression.Constant(classPropertyParameterInfo.ClassProperty));

            if (handlerGetMethod.ReturnType != classPropertyParameterInfo.ClassProperty.PropertyInfo.PropertyType)
            {
                valueExpression = ConvertValueExpressionToTypeExpression(valueExpression, classPropertyParameterInfo.ClassProperty.PropertyInfo.PropertyType);
            }

            return valueExpression;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueExpression"></param>
        /// <param name="classPropertyParameterInfo"></param>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static Expression ConvertValueExpressionForDataEntity(Expression valueExpression,
            ClassPropertyParameterInfo classPropertyParameterInfo,
            DataReaderField readerField)
        {
            var getParameter = GetPropertyHandlerGetParameter(classPropertyParameterInfo);
            var targetType = GetParameterUnderlyingType(getParameter) ?? GetTargetType(classPropertyParameterInfo);

            if (Converter.ConversionType == ConversionType.Default)
            {
                if (targetType.IsEnum)
                {
                    return ConvertValueExpressionToEnumExpression(valueExpression, readerField);
                }
                else
                {
                    return ConvertValueExpressionToTypeExpression(valueExpression, targetType);
                }
            }
            else
            {
                return ConvertExpressionWithAutomaticConversion(valueExpression,
                    readerField, targetType);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trueExpression"></param>
        /// <param name="classPropertyParameterInfo"></param>
        /// <param name="readerField"></param>
        /// <param name="considerNullable"></param>
        /// <returns></returns>
        internal static Expression ConvertTrueExpressionViaPropertyHandler(Expression trueExpression,
            ClassPropertyParameterInfo classPropertyParameterInfo,
            DataReaderField readerField,
            bool considerNullable)
        {
            var handlerInstance = classPropertyParameterInfo.ClassProperty?.GetPropertyHandler();
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
                    ConvertValueExpressionToTypeExpression(trueExpression, getParameter.ParameterType.GetUnderlyingType()),
                    Expression.Constant(classPropertyParameterInfo.ClassProperty));
            }
            else
            {
                trueExpression = Expression.Call(Expression.Constant(handlerInstance),
                    handlerGetMethod,
                    trueExpression,
                    Expression.Constant(classPropertyParameterInfo.ClassProperty));
            }
            if (handlerGetMethod.ReturnType != classPropertyParameterInfo.ClassProperty.PropertyInfo.PropertyType)
            {
                trueExpression = ConvertValueExpressionToTypeExpression(trueExpression, classPropertyParameterInfo.ClassProperty.PropertyInfo.PropertyType);
            }

            return trueExpression;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueExpression"></param>
        /// <param name="classPropertyParameterInfo"></param>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static Expression ConvertValueExpressionToNullableFalseExpression(Expression valueExpression,
            ClassPropertyParameterInfo classPropertyParameterInfo,
            DataReaderField readerField)
        {
            var targetType = GetTargetType(classPropertyParameterInfo);
            var isConversionNeeded = CheckIfConversionIsNeeded(readerField, targetType);

            // Check if conversion is needed
            if (isConversionNeeded == false)
            {
                return valueExpression;
            }

            // Variables needed
            var handlerInstance = GetHandlerInstance(classPropertyParameterInfo, readerField);
            var isDefaultConversion = (Converter.ConversionType == ConversionType.Default) || (handlerInstance != null);

            // Check the conversion
            if (isDefaultConversion == true)
            {
                // Default
                if (handlerInstance == null)
                {
                    valueExpression = ConvertNullableFalseExpressionWithDefaultConversion(valueExpression,
                        classPropertyParameterInfo, readerField);
                }
            }
            else
            {
                // Automatic
                valueExpression = ConvertExpressionWithAutomaticConversion(valueExpression,
                    readerField,
                    classPropertyParameterInfo.ParameterInfo?.ParameterType ?? classPropertyParameterInfo.ClassProperty?.PropertyInfo?.PropertyType);
            }

            if (handlerInstance == null)
            {
                // In SqLite, the Time column is represented as System.DateTime in .NET. If in any case that the models
                // has been designed to have it as System.TimeSpan, then we should somehow be able to set it properly.
                if (readerField.Type == StaticType.DateTime && targetType == StaticType.TimeSpan)
                {
                    var timeOfDayProperty = StaticType.DateTime.GetProperty("TimeOfDay");
                    valueExpression = Expression.Property(valueExpression, timeOfDayProperty);
                }
            }

            // Return the value
            return valueExpression;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueExpression"></param>
        /// <param name="classPropertyParameterInfo"></param>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static Expression ConvertValueExpressionForNullablePropertyType(Expression valueExpression,
            ClassPropertyParameterInfo classPropertyParameterInfo,
            DataReaderField readerField)
        {
            var handlerInstance = GetHandlerInstance(classPropertyParameterInfo, readerField);
            var targetType = GetTargetType(classPropertyParameterInfo);
            var setNullable = (targetType.IsEnum == false) || (targetType.IsEnum && readerField.Type != StaticType.String);
            if (setNullable == true)
            {
                var nullableConstructorExpression = StaticType.Nullable.MakeGenericType(targetType).GetConstructor(new[] { targetType });
                if (handlerInstance == null)
                {
                    return Expression.New(nullableConstructorExpression, valueExpression);
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="readerFieldsName"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static IEnumerable<ClassPropertyParameterInfo> GetClassPropertyParameterInfos<TEntity>(IEnumerable<string> readerFieldsName,
            IDbSetting dbSetting)
            where TEntity : class
        {
            var typeOfEntity = typeof(TEntity);
            var list = new List<ClassPropertyParameterInfo>();

            // Parameter information
            var constructorInfo = typeOfEntity.GetConstructorWithMostArguments();
            var parameterInfos = constructorInfo?.GetParameters().AsList();

            // Class properties
            var classProperties = PropertyCache
                .Get(typeOfEntity)?
                .Where(property => property.PropertyInfo.CanWrite)
                .Where(property =>
                    readerFieldsName?.FirstOrDefault(field =>
                        string.Equals(field.AsUnquoted(true, dbSetting), property.GetMappedName().AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) != null)
                .AsList();

            // Get the matches, check the lengths
            if (parameterInfos?.Count >= classProperties?.Count)
            {
                foreach (var parameterInfo in parameterInfos)
                {
                    var classProperty = classProperties?.
                        FirstOrDefault(item =>
                            string.Equals(item.PropertyInfo.Name, parameterInfo.Name, StringComparison.OrdinalIgnoreCase));
                    list.Add(new ClassPropertyParameterInfo
                    {
                        ClassProperty = classProperty,
                        ParameterInfo = parameterInfo
                    });
                }
            }
            else if (classProperties?.Any() == true)
            {
                foreach (var classProperty in classProperties)
                {
                    var parameterInfo = parameterInfos?.
                        FirstOrDefault(item =>
                            string.Equals(item.Name, classProperty.PropertyInfo.Name, StringComparison.OrdinalIgnoreCase));
                    list.Add(new ClassPropertyParameterInfo
                    {
                        ClassProperty = classProperty,
                        ParameterInfo = parameterInfo
                    });
                }
            }

            // Unmatch within the parameter infos
            if (parameterInfos?.Any() == true)
            {
                foreach (var parameterInfo in parameterInfos)
                {
                    var listItem = list.FirstOrDefault(item => item.ParameterInfo == parameterInfo);
                    if (listItem != null)
                    {
                        continue;
                    }
                    var classProperty = classProperties?.FirstOrDefault(property =>
                       string.Equals(property.PropertyInfo.Name, parameterInfo.Name, StringComparison.OrdinalIgnoreCase));
                    if (classProperty != null)
                    {
                        continue;
                    }
                    list.Add(new ClassPropertyParameterInfo { ParameterInfo = parameterInfo });
                }
            }

            // Unmatch within the class properties
            if (classProperties?.Any() == true)
            {
                foreach (var classProperty in classProperties)
                {
                    var listItem = list.FirstOrDefault(item => item.ClassProperty == classProperty);
                    if (listItem != null)
                    {
                        continue;
                    }
                    var parameterInfo = parameterInfos?.FirstOrDefault(parameter =>
                       string.Equals(parameter.Name, classProperty.PropertyInfo.Name, StringComparison.OrdinalIgnoreCase));
                    if (parameterInfo != null)
                    {
                        continue;
                    }
                    list.Add(new ClassPropertyParameterInfo { ClassProperty = classProperty });
                }
            }

            // Return the list
            return list;
        }

        /// <summary>
        /// Returns the list of the bindings for the entity.
        /// </summary>
        /// <typeparam name="TEntity">The target entity type.</typeparam>
        /// <param name="readerParameterExpression">The data reader parameter.</param>
        /// <param name="readerFields">The list of fields to be bound from the data reader.</param>
        /// <param name="dbSetting">The database setting that is being used.</param>
        /// <returns>The enumerable list of <see cref="MemberBinding"/> objects.</returns>
        internal static IEnumerable<MemberBinding> GetMemberBindingsForDataEntity<TEntity>(ParameterExpression readerParameterExpression,
            IEnumerable<DataReaderField> readerFields,
            IDbSetting dbSetting)
            where TEntity : class
        {
            // Variables needed
            var readerFieldsName = readerFields.Select(f => f.Name.ToLowerInvariant()).AsList();
            var classPropertyParameterInfos = GetClassPropertyParameterInfos<TEntity>(readerFieldsName, dbSetting);

            // Check the presence
            if (classPropertyParameterInfos?.Any() != true)
            {
                return default;
            }

            // Variables needed
            var memberBindings = new List<MemberBinding>();

            // Iterate each properties
            foreach (var classPropertyParameterInfo in classPropertyParameterInfos)
            {
                var mappedName = classPropertyParameterInfo.ParameterInfo?.Name.AsUnquoted(true, dbSetting) ??
                    classPropertyParameterInfo.ClassProperty?.GetMappedName().AsUnquoted(true, dbSetting);

                // Skip if not found
                var ordinal = readerFieldsName.IndexOf(mappedName?.ToLowerInvariant());
                if (ordinal < 0)
                {
                    continue;
                }

                // Get the value expression
                var readerField = readerFields.First(f => string.Equals(f.Name.AsUnquoted(true, dbSetting), mappedName.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase));
                var valueExpression = GetClassPropertyParameterInfoValueExpression(readerParameterExpression,
                    classPropertyParameterInfo, readerField);

                // Member values
                var memberAssignment = classPropertyParameterInfo.ClassProperty != null ?
                    Expression.Bind(classPropertyParameterInfo.ClassProperty.PropertyInfo, valueExpression) : null;
                var argument = classPropertyParameterInfo.ParameterInfo != null ? valueExpression : null;

                // Add the bindings
                memberBindings.Add(new MemberBinding
                {
                    ClassProperty = classPropertyParameterInfo.ClassProperty,
                    MemberAssignment = memberAssignment,
                    Argument = argument
                });
            }

            // Return the value
            return memberBindings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerParameterExpression"></param>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        internal static Expression GetDbNullExpression(ParameterExpression readerParameterExpression,
            int ordinal) =>
            GetDbNullExpression(readerParameterExpression, Expression.Constant(ordinal));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerParameterExpression"></param>
        /// <param name="ordinalExpression"></param>
        /// <returns></returns>
        internal static Expression GetDbNullExpression(ParameterExpression readerParameterExpression,
            ConstantExpression ordinalExpression) =>
            Expression.Call(readerParameterExpression, StaticType.DbDataReader.GetMethod("IsDBNull"), ordinalExpression);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerParameterExpression"></param>
        /// <param name="readerGetValueMethod"></param>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbReaderGetValueExpression(ParameterExpression readerParameterExpression,
            MethodInfo readerGetValueMethod,
            int ordinal) =>
            Expression.Call(readerParameterExpression, readerGetValueMethod, Expression.Constant(ordinal));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerParameterExpression"></param>
        /// <param name="readerGetValueMethod"></param>
        /// <param name="ordinalExpression"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbReaderGetValueExpression(ParameterExpression readerParameterExpression,
            MethodInfo readerGetValueMethod,
            ConstantExpression ordinalExpression) =>
            Expression.Call(readerParameterExpression, readerGetValueMethod, ordinalExpression);

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
            var addMethod = StaticType.IDictionaryStringObject.GetMethod("Add", new[] { StaticType.String, StaticType.Object });

            // Iterate each properties
            for (var ordinal = 0; ordinal < readerFields?.Count; ordinal++)
            {
                var readerField = readerFields[ordinal];
                var readerGetValueMethod = GetDbReaderGetValueOrDefaultMethod(readerField);
                var valueExpression = (Expression)GetDbReaderGetValueExpression(readerParameterExpression, readerGetValueMethod, ordinal);

                // Check for nullables
                if (readerField.DbField == null || readerField.DbField?.IsNullable == true)
                {
                    var isDbNullExpression = GetDbNullExpression(readerParameterExpression, ordinal);
                    var toType = (readerField.Type?.IsValueType != true) ? (readerField.Type ?? StaticType.Object) : StaticType.Object;
                    valueExpression = Expression.Condition(isDbNullExpression, Expression.Default(toType),
                        ConvertValueExpressionToTypeExpression(valueExpression, toType));
                }

                // Add to the bindings
                var values = new Expression[]
                {
                    Expression.Constant(readerField.Name),
                    ConvertValueExpressionToTypeExpression(valueExpression, StaticType.Object)
                };
                elementInits.Add(Expression.ElementInit(addMethod, values));
            }

            // Return the result
            return elementInits;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityExpression"></param>
        /// <param name="classProperty"></param>
        /// <param name="dbField"></param>
        /// <returns></returns>
        internal static Expression GetPropertyValueWithAutomaticConversionExpression(Expression entityExpression,
            ClassProperty classProperty,
            DbField dbField)
        {
            var instanceProperty = classProperty.PropertyInfo;
            var expression = (Expression)Expression.Property(entityExpression, instanceProperty);

            // Must be opposite (for setters)
            var fieldType = instanceProperty.PropertyType.GetUnderlyingType();
            var propertyType = dbField.Type?.GetUnderlyingType();

            // Handle the auto conversion
            expression = ConvertExpressionWithAutomaticConversion(expression,
                propertyType, fieldType);

            // Return the value
            return expression;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueExpression"></param>
        /// <param name="dbField"></param>
        /// <param name="instanceProperty"></param>
        /// <returns></returns>
        internal static Expression ConvertPropertyValueForEnumHandlingExpression(Expression valueExpression,
            DbField dbField,
            PropertyInfo instanceProperty)
        {
            var propertyType = instanceProperty.PropertyType.GetUnderlyingType();
            var method = GetSystemConvertGetTypeMethod(StaticType.Object, dbField.Type);

            if (method == null)
            {
                throw new ConverterNotFoundException($"The conversion between '{propertyType.FullName}' and database type '{dbField.DatabaseType}' (of .NET CLR '{dbField.Type.FullName}') is not found.");
            }
            else
            {
                valueExpression = Expression.Call(typeof(EnumHelper).GetMethod("Convert"),
                    Expression.Constant(dbField),
                    ConvertValueExpressionToTypeExpression(valueExpression, StaticType.Object),
                    Expression.Constant(method));
            }

            return valueExpression;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueExpression"></param>
        /// <param name="classProperty"></param>
        /// <param name="dbField"></param>
        /// <returns></returns>
        internal static Expression ConvertValueExpressionToPropertyHandlerSetExpression(Expression valueExpression,
            ClassProperty classProperty,
            DbField dbField) =>
            ConvertValueExpressionToPropertyHandlerSetExpression(valueExpression, classProperty, dbField?.Type.GetUnderlyingType());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueExpression"></param>
        /// <param name="classProperty"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        internal static Expression ConvertValueExpressionToPropertyHandlerSetExpression(Expression valueExpression,
            ClassProperty classProperty,
            Type targetType)
        {
            var handlerInstance = classProperty?.GetPropertyHandler() ??
                PropertyHandlerCache.Get<object>(targetType);

            if (handlerInstance != null)
            {
                var setMethod = GetPropertyHandlerSetMethod(handlerInstance);
                var setParameter = GetPropertyHandlerSetParameter(setMethod);
                valueExpression = Expression.Call(Expression.Constant(handlerInstance),
                    setMethod,
                    ConvertValueExpressionToTypeExpression(valueExpression, setParameter.ParameterType),
                    Expression.Constant(classProperty));
            }

            return valueExpression;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityExpression"></param>
        /// <param name="classProperty"></param>
        /// <param name="dbField"></param>
        /// <returns></returns>
        internal static Expression GetEntityInstancePropertyValueExpression(Expression entityExpression,
            ClassProperty classProperty,
            DbField dbField)
        {
            var instanceProperty = classProperty.PropertyInfo;
            var propertyType = instanceProperty.PropertyType.GetUnderlyingType();
            var handlerInstance = classProperty?.GetPropertyHandler() ??
                PropertyHandlerCache.Get<object>(dbField.Type.GetUnderlyingType());
            var value = (Expression)null;

            if (handlerInstance == null)
            {
                value = (Converter.ConversionType == ConversionType.Automatic) ?
                    GetPropertyValueWithAutomaticConversionExpression(entityExpression, classProperty, dbField) :
                    Expression.Property(entityExpression, instanceProperty);

                // Enum Handling
                if (propertyType.IsEnum)
                {
                    value = ConvertPropertyValueForEnumHandlingExpression(value,
                        dbField,
                        instanceProperty);
                }
            }
            else
            {
                value = Expression.Property(entityExpression, instanceProperty);

                // Property Handler
                value = ConvertValueExpressionToPropertyHandlerSetExpression(value, classProperty, dbField);
            }

            // Convert to object
            return ConvertValueExpressionToTypeExpression(value, StaticType.Object);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyExpression"></param>
        /// <param name="entityInstance"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetObjectInstancePropertyValueExpression(ParameterExpression propertyExpression,
            Expression entityInstance)
        {
            var methodInfo = StaticType.PropertyInfo.GetMethod("GetValue", new[] { StaticType.Object });
            return Expression.Call(propertyExpression, methodInfo, entityInstance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterVariableExpression"></param>
        /// <param name="valueExpression"></param>
        /// <param name="dbField"></param>
        /// <param name="instanceProperty"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetNullablePropertyValueAssignmentExpression(ParameterExpression parameterVariableExpression,
            Expression valueExpression,
            DbField dbField,
            PropertyInfo instanceProperty,
            IDbSetting dbSetting)
        {
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

            // Check if the property is nullable
            if (isNullable == true)
            {
                // Identification of the DBNull
                var parameterName = dbField.Name.AsUnquoted(true, dbSetting).AsAlphaNumeric();
                var valueVariable = Expression.Variable(StaticType.Object, string.Concat("valueOf", parameterName));
                var valueIsNull = Expression.Equal(valueVariable, Expression.Constant(null));
                var dbNullValue = ConvertValueExpressionToTypeExpression(Expression.Constant(DBNull.Value), StaticType.Object);

                // Set the propert value
                valueBlock = Expression.Block(new[] { valueVariable },
                    Expression.Assign(valueVariable, valueExpression),
                    Expression.Condition(valueIsNull, dbNullValue, valueVariable));
            }
            else
            {
                valueBlock = valueExpression;
            }

            // Set the value
            return Expression.Call(parameterVariableExpression, GetDbParameterValueSetMethod(), valueBlock);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterVariableExpression"></param>
        /// <param name="propertyExpression"></param>
        /// <param name="existingValue"></param>
        /// <param name="dbField"></param>
        /// <returns></returns>
        internal static ConditionalExpression GetDbNullPropertyValueAssignmentExpression(ParameterExpression parameterVariableExpression,
            ParameterExpression propertyExpression,
            Expression existingValue,
            DbField dbField)
        {
            var dbParameterValueSetMethod = GetDbParameterValueSetMethod();
            var dbNullValueAssignment = (Expression)null;

            // Set the default type value
            if (dbField.IsNullable == false && dbField.Type != null)
            {
                dbNullValueAssignment = Expression.Call(parameterVariableExpression, dbParameterValueSetMethod,
                    ConvertValueExpressionToTypeExpression(Expression.Default(dbField.Type), StaticType.Object));
            }

            // Set the DBNull value
            if (dbNullValueAssignment == null)
            {
                var dbNullValue = ConvertValueExpressionToTypeExpression(Expression.Constant(DBNull.Value), StaticType.Object);
                dbNullValueAssignment = Expression.Call(parameterVariableExpression, dbParameterValueSetMethod, dbNullValue);
            }

            // Check the presence of the property
            var propertyIsNull = Expression.Equal(propertyExpression, Expression.Constant(null));

            // Set the condition
            return Expression.Condition(propertyIsNull, dbNullValueAssignment, existingValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterVariableExpression"></param>
        /// <param name="entityExpression"></param>
        /// <param name="propertyExpression"></param>
        /// <param name="classProperty"></param>
        /// <param name="dbField"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static Expression GetDbParameterValueAssignmentExpression(ParameterExpression parameterVariableExpression,
            Expression entityExpression,
            ParameterExpression propertyExpression,
            ClassProperty classProperty,
            DbField dbField,
            IDbSetting dbSetting)
        {
            // Get the property value
            var value = (propertyExpression.Type == StaticType.PropertyInfo) ? GetObjectInstancePropertyValueExpression(propertyExpression, entityExpression) :
                GetEntityInstancePropertyValueExpression(entityExpression, classProperty, dbField);

            // Ensure the nullable
            var valueAssignment = (Expression)GetNullablePropertyValueAssignmentExpression(parameterVariableExpression,
                value,
                dbField,
                classProperty?.PropertyInfo,
                dbSetting);

            // Check if it is a direct assignment or not
            if (entityExpression.Type == StaticType.Object)
            {
                valueAssignment = GetDbNullPropertyValueAssignmentExpression(parameterVariableExpression, propertyExpression, valueAssignment, dbField);
            }

            // Return
            return valueAssignment;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterVariableExpression"></param>
        /// <param name="dbField"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbParameterDbTypeAssignmentExpression(ParameterExpression parameterVariableExpression,
            DbField dbField)
        {
            var expression = (MethodCallExpression)null;
            var dbParameterDbTypeSetMethod = StaticType.DbParameter.GetProperty("DbType").SetMethod;
            var underlyingType = dbField.Type?.GetUnderlyingType();
            var dbType = TypeMapper.Get(underlyingType) ??
                new ClientTypeToDbTypeResolver().Resolve(underlyingType);

            // Set the DB Type
            if (dbType != null)
            {
                expression = Expression.Call(parameterVariableExpression, dbParameterDbTypeSetMethod, Expression.Constant(dbType));
            }

            // Return the expression
            return expression;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandParameterExpression"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbCommandCreateParameterExpression(ParameterExpression commandParameterExpression)
        {
            var dbCommandCreateParameterMethod = StaticType.DbCommand.GetMethod("CreateParameter");
            return Expression.Call(commandParameterExpression, dbCommandCreateParameterMethod);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterVariableExpresion"></param>
        /// <param name="dbField"></param>
        /// <param name="entityIndex"></param>
        /// <param name="dbSetting"></param>
        internal static MethodCallExpression GetDbParameterNameAssignmentExpression(ParameterExpression parameterVariableExpresion,
            DbField dbField,
            int entityIndex,
            IDbSetting dbSetting)
        {
            var parameterName = dbField.Name.AsUnquoted(true, dbSetting).AsAlphaNumeric();
            var dbParameterParameterNameSetMethod = StaticType.DbParameter.GetProperty("ParameterName").SetMethod;
            return Expression.Call(parameterVariableExpresion, dbParameterParameterNameSetMethod,
                Expression.Constant(entityIndex > 0 ? string.Concat(parameterName, "_", entityIndex) : parameterName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterVariableExpression"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbParameterDirectionAssignmentExpression(ParameterExpression parameterVariableExpression,
            ParameterDirection direction)
        {
            var dbParameterDirectionSetMethod = StaticType.DbParameter.GetProperty("Direction").SetMethod;
            return Expression.Call(parameterVariableExpression, dbParameterDirectionSetMethod, Expression.Constant(direction));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterVariableExpression"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbParameterSizeAssignmentExpression(ParameterExpression parameterVariableExpression,
            int size)
        {
            var dbParameterSizeSetMethod = StaticType.DbParameter.GetProperty("Size").SetMethod;
            return Expression.Call(parameterVariableExpression, dbParameterSizeSetMethod, Expression.Constant(size));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterVariableExpression"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbParameterPrecisionAssignmentExpression(ParameterExpression parameterVariableExpression,
            byte precision)
        {
            var dbParameterPrecisionSetMethod = StaticType.DbParameter.GetProperty("Precision").SetMethod;
            return Expression.Call(parameterVariableExpression, dbParameterPrecisionSetMethod, Expression.Constant(precision));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterVariableExpression"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbParameterScaleAssignmentExpression(ParameterExpression parameterVariableExpression,
            byte scale)
        {
            var dbParameterScaleSetMethod = StaticType.DbParameter.GetProperty("Scale").SetMethod;
            return Expression.Call(parameterVariableExpression, dbParameterScaleSetMethod, Expression.Constant(scale));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandParameterExpression"></param>
        /// <param name="parameterVariable"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbCommandParametersAddExpression(ParameterExpression commandParameterExpression,
            ParameterExpression parameterVariable)
        {
            var dbCommandParametersProperty = StaticType.DbCommand.GetProperty("Parameters");
            var dbParameterCollection = Expression.Property(commandParameterExpression, dbCommandParametersProperty);
            var dbParameterCollectionAddMethod = StaticType.DbParameterCollection.GetMethod("Add", new[] { StaticType.Object });
            return Expression.Call(dbParameterCollection, dbParameterCollectionAddMethod, parameterVariable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbParameterCollectionExpression"></param>
        /// <returns></returns>
        internal static Expression GetDbParameterCollectionClearMethodExpression(MemberExpression dbParameterCollectionExpression)
        {
            var dbParameterCollectionClearMethod = StaticType.DbParameterCollection.GetMethod("Clear");
            return Expression.Call(dbParameterCollectionExpression, dbParameterCollectionClearMethod);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandParameterExpression"></param>
        /// <param name="entityIndex"></param>
        /// <param name="entityExpression"></param>
        /// <param name="propertyExpression"></param>
        /// <param name="dbField"></param>
        /// <param name="classProperty"></param>
        /// <param name="direction"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static Expression GetParameterAssignmentExpression(ParameterExpression commandParameterExpression,
            int entityIndex,
            Expression entityExpression,
            ParameterExpression propertyExpression,
            DbField dbField,
            ClassProperty classProperty,
            ParameterDirection direction,
            IDbSetting dbSetting)
        {
            var parameterAssignmentExpressions = new List<Expression>();
            var parameterVariableExpression = Expression.Variable(StaticType.DbParameter,
                string.Concat("parameter", dbField.Name.AsUnquoted(true, dbSetting).AsAlphaNumeric()));

            // Variable
            var createParameterExpression = GetDbCommandCreateParameterExpression(commandParameterExpression);
            parameterAssignmentExpressions.AddIfNotNull(Expression.Assign(parameterVariableExpression, createParameterExpression));

            // DbParameter.Name
            var nameAssignmentExpression = GetDbParameterNameAssignmentExpression(parameterVariableExpression,
                dbField,
                entityIndex,
                dbSetting);
            parameterAssignmentExpressions.AddIfNotNull(nameAssignmentExpression);

            // DbParameter.Value
            if (direction != ParameterDirection.Output)
            {
                var valueAssignmentExpression = GetDbParameterValueAssignmentExpression(parameterVariableExpression,
                    entityExpression,
                    propertyExpression,
                    classProperty,
                    dbField,
                    dbSetting);
                parameterAssignmentExpressions.AddIfNotNull(valueAssignmentExpression);
            }

            // DbParameter.DbType
            var dbTypeAssignmentExpression = GetDbParameterDbTypeAssignmentExpression(parameterVariableExpression,
                dbField);
            parameterAssignmentExpressions.AddIfNotNull(dbTypeAssignmentExpression);

            // DbParameter.SqlDbType (System)
            var systemSqlDbTypeAssignmentExpression = GetDbParameterSystemSqlDbTypeAssignmentExpression(parameterVariableExpression,
                classProperty);
            parameterAssignmentExpressions.AddIfNotNull(systemSqlDbTypeAssignmentExpression);

            // DbParameter.SqlDbType (Microsoft)
            var microsoftSqlDbTypeAssignmentExpression = GetDbParameterMicrosoftSqlDbTypeAssignmentExpression(parameterVariableExpression,
                classProperty);
            parameterAssignmentExpressions.AddIfNotNull(microsoftSqlDbTypeAssignmentExpression);

            // DbParameter.MySqlDbType
            var mySqlDbTypeAssignmentExpression = GetDbParameterMySqlDbTypeAssignmentExpression(parameterVariableExpression,
                classProperty);
            parameterAssignmentExpressions.AddIfNotNull(mySqlDbTypeAssignmentExpression);

            // DbParameter.NpgsqlDbType
            var npgsqlDbTypeAssignmentExpression = GetDbParameterNpgsqlDbTypeAssignmentExpression(parameterVariableExpression,
                classProperty);
            parameterAssignmentExpressions.AddIfNotNull(npgsqlDbTypeAssignmentExpression);

            // DbParameter.Direction
            if (dbSetting.IsDirectionSupported)
            {
                var directionAssignmentExpression = GetDbParameterDirectionAssignmentExpression(parameterVariableExpression, direction);
                parameterAssignmentExpressions.AddIfNotNull(directionAssignmentExpression);
            }

            // DbParameter.Size
            if (dbField.Size != null)
            {
                var sizeAssignmentExpression = GetDbParameterSizeAssignmentExpression(parameterVariableExpression, dbField.Size.Value);
                parameterAssignmentExpressions.AddIfNotNull(sizeAssignmentExpression);
            }

            // DbParameter.Precision
            if (dbField.Precision != null)
            {
                var precisionAssignmentExpression = GetDbParameterPrecisionAssignmentExpression(parameterVariableExpression, dbField.Precision.Value);
                parameterAssignmentExpressions.AddIfNotNull(precisionAssignmentExpression);
            }

            // DbParameter.Scale
            if (dbField.Scale != null)
            {
                var scaleAssignmentExpression = GetDbParameterScaleAssignmentExpression(parameterVariableExpression, dbField.Scale.Value);
                parameterAssignmentExpressions.AddIfNotNull(scaleAssignmentExpression);
            }

            // DbCommand.Parameters.Add
            var dbParametersAddExpression = GetDbCommandParametersAddExpression(commandParameterExpression, parameterVariableExpression);
            parameterAssignmentExpressions.AddIfNotNull(dbParametersAddExpression);

            // Return the value
            return Expression.Block(new[] { parameterVariableExpression }, parameterAssignmentExpressions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandParameterExpression"></param>
        /// <param name="entityExpression"></param>
        /// <param name="fieldDirection"></param>
        /// <param name="entityIndex"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static Expression GetPropertyFieldExpression(ParameterExpression commandParameterExpression,
            ParameterExpression entityExpression,
            FieldDirection fieldDirection,
            int entityIndex,
            IDbSetting dbSetting)
        {
            var propertyListExpression = new List<Expression>();
            var propertyVariableListExpression = new List<ParameterExpression>();
            var propertyVariableExpression = (ParameterExpression)null;
            var propertyInstanceExpression = (Expression)null;
            var classProperty = (ClassProperty)null;
            var propertyName = fieldDirection.DbField.Name.AsUnquoted(true, dbSetting);

            // Set the proper assignments (property)
            if (entityExpression.Type.IsClassType() == false || entityExpression.Type.IsGenericType)
            {
                var typeGetPropertyMethod = StaticType.Type.GetMethod("GetProperty", new[]
                {
                    StaticType.String,
                    StaticType.BindingFlags
                });
                var objectGetTypeMethod = StaticType.Object.GetMethod("GetType");
                propertyVariableExpression = Expression.Variable(StaticType.PropertyInfo, string.Concat("propertyVariable", propertyName));
                propertyInstanceExpression = Expression.Call(Expression.Call(entityExpression, objectGetTypeMethod),
                    typeGetPropertyMethod, new[]
                    {
                        Expression.Constant(propertyName),
                        Expression.Constant(BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase)
                    });
            }
            else
            {
                var entityProperties = PropertyCache.Get(entityExpression.Type);
                classProperty = entityProperties.First(property =>
                    string.Equals(property.GetMappedName().AsUnquoted(true, dbSetting),
                        propertyName.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase));

                if (classProperty != null)
                {
                    propertyVariableExpression = Expression.Variable(classProperty.PropertyInfo.PropertyType, string.Concat("propertyVariable", propertyName));
                    propertyInstanceExpression = Expression.Property(entityExpression, classProperty.PropertyInfo);
                }
            }

            // Add the variables
            if (propertyVariableExpression != null && propertyInstanceExpression != null)
            {
                propertyVariableListExpression.Add(propertyVariableExpression);
                propertyListExpression.Add(Expression.Assign(propertyVariableExpression, propertyInstanceExpression));

                // Execute the function
                var parameterAssignment = GetParameterAssignmentExpression(commandParameterExpression,
                    entityIndex,
                    entityExpression,
                    propertyVariableExpression,
                    fieldDirection.DbField,
                    classProperty,
                    fieldDirection.Direction,
                    dbSetting);
                propertyListExpression.Add(parameterAssignment);
            }

            // Add the property block
            return Expression.Block(propertyVariableListExpression, propertyListExpression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandParameterExpression"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbCommandParametersClearExpression(ParameterExpression commandParameterExpression)
        {
            var dbParameterCollection = Expression.Property(commandParameterExpression,
                StaticType.DbCommand.GetProperty("Parameters"));
            return Expression.Call(dbParameterCollection, StaticType.DbParameterCollection.GetMethod("Clear"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entitiesParameterExpression"></param>
        /// <param name="typeOfListEntity"></param>
        /// <param name="entityIndex"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetListEntityIndexerExpression(Expression entitiesParameterExpression,
            Type typeOfListEntity,
            int entityIndex)
        {
            var listIndexerMethod = typeOfListEntity.GetMethod("get_Item", new[] { StaticType.Int32 });
            return Expression.Call(entitiesParameterExpression, listIndexerMethod,
                Expression.Constant(entityIndex));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static Expression ThrowIfNullAfterClassHandlerExpression<TEntity>(Expression expression)
        {
            var typeOfEntity = typeof(TEntity);
            var isNullExpression = Expression.Equal(Expression.Constant(null), expression);
            var exception = new NullReferenceException($"Entity of type '{typeOfEntity}' must not be null. If you have defined a class handler, please check the 'Set' method.");
            return Expression.IfThen(isNullExpression, Expression.Throw(Expression.Constant(exception)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="commandParameterExpression"></param>
        /// <param name="entitiesParameterExpression"></param>
        /// <param name="fieldDirections"></param>
        /// <param name="entityIndex"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static Expression GetIndexDbParameterSetterExpression<TEntity>(ParameterExpression commandParameterExpression,
            Expression entitiesParameterExpression,
            IEnumerable<FieldDirection> fieldDirections,
            int entityIndex,
            IDbSetting dbSetting)
            where TEntity : class
        {
            // Get the current instance
            var typeOfEntity = typeof(TEntity);
            var entityVariableExpression = Expression.Variable(typeOfEntity, "instance");
            var typeOfListEntity = typeof(IList<TEntity>);
            var entityParameter = (Expression)GetListEntityIndexerExpression(entitiesParameterExpression, typeOfListEntity, entityIndex);
            var entityExpressions = new List<Expression>();
            var entityVariables = new List<ParameterExpression>();

            // Class handler
            entityParameter = ConvertValueExpressionToClassHandlerSetExpression<TEntity>(entityParameter);

            // Entity instance
            entityVariables.Add(entityVariableExpression);
            entityExpressions.Add(Expression.Assign(entityVariableExpression, entityParameter));

            // Throw if null
            entityExpressions.Add(ThrowIfNullAfterClassHandlerExpression<TEntity>(entityVariableExpression));

            // Iterate the input fields
            foreach (var fieldDirection in fieldDirections)
            {
                // Add the property block
                var propertyBlock = GetPropertyFieldExpression(commandParameterExpression,
                    entityVariableExpression, fieldDirection, entityIndex, dbSetting);

                // Add to instance expression
                entityExpressions.Add(propertyBlock);
            }

            // Add to the instance block
            return Expression.Block(entityVariables, entityExpressions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entityExpression"></param>
        /// <param name="readerParameterExpression"></param>
        /// <returns></returns>
        internal static Expression ConvertValueExpressionToClassHandlerGetExpression<TEntity>(Expression entityExpression,
            ParameterExpression readerParameterExpression)
            where TEntity : class
        {
            var typeOfEntity = typeof(TEntity);

            // Check the handler
            var handlerInstance = GetClassHandler(typeOfEntity);
            if (handlerInstance == null)
            {
                return entityExpression;
            }

            // Validate
            var handlerType = handlerInstance.GetType();
            if (handlerType.IsClassHandlerValidForModel(typeOfEntity) == false)
            {
                throw new InvalidTypeException($"The class handler '{handlerType.FullName}' cannot be used for the type '{typeOfEntity.FullName}'.");
            }

            // Call the ClassHandler.Get method
            var getMethod = GetClassHandlerGetMethod(handlerInstance);
            return Expression.Call(Expression.Constant(handlerInstance),
                getMethod,
                entityExpression,
                readerParameterExpression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entityOrEntitiesExpression"></param>
        /// <returns></returns>
        internal static Expression ConvertValueExpressionToClassHandlerSetExpression<TEntity>(Expression entityOrEntitiesExpression)
            where TEntity : class
        {
            var typeOfEntity = typeof(TEntity);

            // Check the handler
            var handlerInstance = GetClassHandler(typeOfEntity);
            if (handlerInstance == null)
            {
                return entityOrEntitiesExpression;
            }

            // Validate
            var handlerType = handlerInstance.GetType();
            if (handlerType.IsClassHandlerValidForModel(typeOfEntity) == false)
            {
                throw new InvalidTypeException($"The class handler '{handlerType.FullName}' cannot be used for type '{typeOfEntity.FullName}'.");
            }

            // Call the IClassHandler.Set method
            var typeOfListEntity = typeof(IList<TEntity>);
            if (typeOfListEntity.IsAssignableFrom(entityOrEntitiesExpression.Type))
            {
                var setMethod = GetClassHandlerSetMethod(handlerInstance, typeOfListEntity);
                entityOrEntitiesExpression = Expression.Call(Expression.Constant(handlerInstance),
                    setMethod,
                    entityOrEntitiesExpression);
            }
            else
            {
                var setMethod = GetClassHandlerSetMethod(handlerInstance, typeOfEntity);
                entityOrEntitiesExpression = Expression.Call(Expression.Constant(handlerInstance),
                    setMethod,
                    entityOrEntitiesExpression);
            }

            // Return the block
            return entityOrEntitiesExpression;
        }

        #endregion
    }
}
