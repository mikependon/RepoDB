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
    /// <summary>
    /// The compiler class of the library.
    /// </summary>
    internal partial class Compiler
    {
        #region SubClasses/SubStructs


        /// <summary>
        /// A class that contains both the instance of <see cref="RepoDb.ClassProperty"/> and <see cref="System.Reflection.ParameterInfo"/> objects.
        /// </summary>
        internal class ClassPropertyParameterInfo
        {
            private string descriptiveContextString = null;

            /// <summary>
            /// Gets the instance of <see cref="RepoDb.ClassProperty"/> object in used.
            /// </summary>
            public ClassProperty ClassProperty { get; set; }

            /// <summary>
            /// Gets the instance of <see cref="System.Reflection.ParameterInfo"/> object in used.
            /// </summary>
            public ParameterInfo ParameterInfo { get; set; }

            /// <summary>
            /// Gets the instance of <see cref="RepoDb.ClassProperty"/> object that is mapped to the current <see cref="ParameterInfo"/>.
            /// </summary>
            public ClassProperty ParameterInfoMappedClassProperty { get; set; }

            /// <summary>
            /// Gets the target type.
            /// </summary>
            public Type TargetType { get; set; }

            /// <summary>
            /// Gets the target type based on the combinations.
            /// </summary>
            /// <returns></returns>
            public Type GetTargetType() =>
                TargetType ?? ParameterInfo?.ParameterType ?? ClassProperty?.PropertyInfo?.PropertyType;

            /// <summary>
            /// Gets the descriptive context string for error messaging.
            /// </summary>
            /// <returns></returns>
            internal string GetDescriptiveContextString()
            {
                if (descriptiveContextString != null)
                {
                    return descriptiveContextString;
                }

                // Variable
                var message = $"Context :: TargetType: {GetTargetType()} ";

                // ParameterInfo
                if (ParameterInfo != null)
                {
                    message = string.Concat(descriptiveContextString, $"Parameter: {ParameterInfo.Name} ({ParameterInfo.ParameterType}) ");
                }

                // ClassProperty
                if (ClassProperty?.PropertyInfo != null)
                {
                    message = string.Concat(descriptiveContextString, $"PropertyInfo: {ClassProperty.PropertyInfo.Name} ({ClassProperty.PropertyInfo.PropertyType}), DeclaringType: {ClassProperty.GetDeclaringType()} ");
                }

                // Return
                return (descriptiveContextString = message.Trim());
            }

            /// <summary>
            /// Returns the string that represents this object.
            /// </summary>
            /// <returns>The presented string.</returns>
            public override string ToString() =>
                string.Concat("TargetType = ", GetTargetType()?.FullName, ", ClassProperty = ", ClassProperty?.ToString(), ", ",
                    "ParameterInfo = ", ParameterInfo?.ToString(), ")", ", TargetType = ", TargetType?.ToString(), ", ");
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
        internal class MemberBinding
        {
            /// <summary>
            /// Gets the instance of <see cref="ClassProperty"/> object in used.
            /// </summary>
            public ClassProperty ClassProperty { get; set; }

            /// <summary>
            /// Gets the instance of <see cref="ParameterInfo"/> object in used.
            /// </summary>
            public ParameterInfo ParameterInfo { get; set; }

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
                ClassProperty?.ToString() ?? ParameterInfo?.ToString();
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
            return fields.Select((value, index) => new FieldDirection
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
            return fields.Select((value, index) => new FieldDirection
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
        internal static MethodInfo GetSystemConvertToTypeMethod(Type fromType,
            Type toType) =>
            StaticType.Convert.GetMethod(string.Concat("To", toType.GetUnderlyingType().Name),
                new[] { fromType.GetUnderlyingType() });

        /// <summary>
        ///
        /// </summary>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        internal static MethodInfo GetSystemConvertChangeTypeMethod(Type conversionType) =>
            StaticType.Convert.GetMethod("ChangeType",
                new[] { StaticType.Object, conversionType.GetUnderlyingType() });

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
        /// <param name="handlerInstance"></param>
        /// <returns></returns>
        internal static MethodInfo GetClassHandlerGetMethod(object handlerInstance) =>
            handlerInstance?.GetType().GetMethod("Get");

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
        /// <param name="handlerInstance"></param>
        /// <returns></returns>
        internal static MethodInfo GetPropertyHandlerGetMethod(object handlerInstance) =>
            handlerInstance?.GetType().GetMethod("Get");

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static MethodInfo GetDbCommandCreateParameterMethod() =>
            StaticType.DbCommandExtension.GetMethod("CreateParameter", new[]
            {
                StaticType.IDbCommand,
                StaticType.String,
                StaticType.Object,
                StaticType.DbTypeNullable
            });

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static MethodInfo GetDbParameterCollectionAddMethod() =>
            StaticType.DbParameterCollection.GetMethod("Add");

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
            GetPropertyHandlerGetParameter(classPropertyParameterInfo?.ClassProperty);

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
            getMethod?.GetParameters().First();

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
            setMethod?.GetParameters().First();

        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static IEnumerable<DataReaderField> GetDataReaderFields(DbDataReader reader,
            IDbSetting dbSetting) =>
            GetDataReaderFields(reader, null, dbSetting);

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
        /// <returns></returns>
        internal static MethodInfo GetDbParameterValueSetMethod() =>
            StaticType.DbParameter.GetProperty("Value").SetMethod;

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static PropertyInfo GetTimeSpanTicksProperty() =>
            StaticType.TimeSpan.GetProperty("Ticks");

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static MethodInfo GetTimeSpanTicksPropertyGetMethod() =>
            GetTimeSpanTicksProperty().GetMethod;

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static PropertyInfo GetDateTimeTimeOfDayProperty() =>
            StaticType.DateTime.GetProperty("TimeOfDay");

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static MethodInfo GetDateTimeTimeOfDayPropertyGetMethod() =>
            GetDateTimeTimeOfDayProperty().GetMethod;

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToNullableGetValueOrDefaultExpression(Expression expression)
        {
            if (Nullable.GetUnderlyingType(expression.Type) != null)
            {
                var underlyingType = expression.Type.GetUnderlyingType();
                var method = expression.Type.GetMethod("GetValueOrDefault", new Type[] { underlyingType });
                return Expression.Call(expression, method, Expression.Default(underlyingType));
            }
            return expression;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToNullableValueExpression(Expression expression)
        {
            if (Nullable.GetUnderlyingType(expression.Type) != null)
            {
                Expression.Call(expression, expression.Type.GetProperty("Value").GetMethod, expression);
            }
            return expression;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToGuidToStringExpression(Expression expression) =>
            Expression.Call(ConvertExpressionToNullableGetValueOrDefaultExpression(expression), StaticType.Guid.GetMethod("ToString", new Type[0]));

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToStringToGuidExpression(Expression expression) =>
            Expression.New(StaticType.Guid.GetConstructor(new[] { StaticType.String }), ConvertExpressionToNullableGetValueOrDefaultExpression(expression));

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToTimeSpanToDateTimeExpression(Expression expression) =>
            Expression.New(StaticType.DateTime.GetConstructor(new[] { StaticType.Int64 }),
                ConvertExpressionToNullableGetValueOrDefaultExpression(ConvertExpressionToTimeSpanTicksExpression(expression)));

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToDateTimeToTimeSpanExpression(Expression expression) =>
            ConvertExpressionToNullableGetValueOrDefaultExpression(ConvertExpressionToDateTimeTimeOfDayExpression(expression));

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToTimeSpanTicksExpression(Expression expression) =>
            Expression.Call(expression, GetTimeSpanTicksPropertyGetMethod());

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToDateTimeTimeOfDayExpression(Expression expression) =>
            Expression.Call(expression, GetDateTimeTimeOfDayPropertyGetMethod());

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="toType"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToSystemConvertExpression(Expression expression,
            Type toType)
        {
            if (expression.Type.GetUnderlyingType() == toType)
            {
                return expression;
            }

            // Identify
            if (toType.IsAssignableFrom(expression.Type))
            {
                return ConvertExpressionToTypeExpression(expression, toType);
            }

            // Convert.To<Type>()
            var methodInfo = GetSystemConvertToTypeMethod(expression.Type, toType);
            if (methodInfo != null)
            {
                return Expression.Call(methodInfo, ConvertExpressionToNullableGetValueOrDefaultExpression(expression));
            }

            // Convert.ChangeType
            methodInfo = GetSystemConvertChangeTypeMethod(toType);
            if (methodInfo != null)
            {
                expression = ConvertExpressionToNullableGetValueOrDefaultExpression(expression);
                return Expression.Call(methodInfo, new Expression[]
                {
                    ConvertExpressionToTypeExpression(expression, StaticType.Object),
                    Expression.Constant(toType.GetUnderlyingType())
                });
            }

            // Return
            return expression;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="toType"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToTypeExpression(Expression expression,
            Type toType) =>
            (expression.Type != toType) ? Expression.Convert(expression, toType) : expression;

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="fromType"></param>
        /// <param name="toEnumType"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToEnumExpression(Expression expression,
            Type fromType,
            Type toEnumType) =>
            (fromType == StaticType.String) ?
                ConvertExpressionToEnumExpressionForString(expression, toEnumType) :
                    ConvertExpressionToEnumExpressionForNonString(expression, toEnumType);

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="toEnumType"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToEnumExpressionForString(Expression expression,
            Type toEnumType)
        {
            var method = StaticType.Enum.GetMethod("Parse", new[] { StaticType.Type, StaticType.String, StaticType.Boolean });
            if (method == null)
            {
                throw new InvalidOperationException($"There is no enum 'Parse' method found between '{toEnumType.FullName}' and '{StaticType.String.FullName}'.");
            }
            var parameters = new Expression[]
            {
                Expression.Constant(toEnumType),
                expression,
                Expression.Constant(true)
            };
            return Expression.Call(method, parameters);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="toEnumType"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToEnumExpressionForNonString(Expression expression,
            Type toEnumType)
        {
            var method = StaticType.Enum.GetMethod("GetName", new[] { StaticType.Type, StaticType.Object });
            if (method == null)
            {
                throw new InvalidOperationException($"There is no enum 'GetName' method found between '{toEnumType.FullName}' and '{StaticType.String.FullName}'.");
            }
            var parameters = new Expression[]
            {
                Expression.Constant(toEnumType),
                ConvertExpressionToTypeExpression(expression, StaticType.Object)
            };
            return ConvertExpressionToEnumExpressionForString(Expression.Call(method, parameters), toEnumType);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="toType"></param>
        /// <returns></returns>
        internal static Expression ConvertEnumExpressionToTypeExpression(Expression expression,
            Type toType)
        {
            if (toType.GetUnderlyingType() == StaticType.String || toType.GetUnderlyingType() == StaticType.Boolean)
            {
                return ConvertEnumExpressionToTypeExpressionForString(expression);
            }
            else
            {
                return ConvertEnumExpressionToTypeExpressionForNonString(expression, toType);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static Expression ConvertEnumExpressionToTypeExpressionForString(Expression expression)
        {
            var method = StaticType.Convert.GetMethod("ToString", new[] { StaticType.Object });

            // Variables
            var isNullExpression = (Expression)null;
            var trueExpression = (Expression)null;
            var falseExpression = (Expression)null;

            // Ensure (Ref/Nullable)
            if (expression.Type.IsNullable())
            {
                // Check
                isNullExpression = Expression.Equal(Expression.Constant(null), expression);

                // True
                trueExpression = Expression.Convert(Expression.Constant(null), StaticType.String);
            }

            // False
            var methodCallExpression = Expression.Call(method, ConvertExpressionToTypeExpression(expression, StaticType.Object));
            falseExpression = ConvertExpressionToTypeExpression(methodCallExpression, StaticType.String);

            // Call and return
            return isNullExpression == null ? falseExpression :
                Expression.Condition(isNullExpression, trueExpression, falseExpression);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="toType"></param>
        /// <returns></returns>
        internal static Expression ConvertEnumExpressionToTypeExpressionForNonString(Expression expression,
            Type toType)
        {
            var isNullExpression = (Expression)null;
            var trueExpression = (Expression)null;
            var falseExpression = expression;

            // Ensure (Ref/Nullable)
            if (expression.Type.IsNullable())
            {
                isNullExpression = Expression.Equal(Expression.Constant(null), expression);
                trueExpression = GetNullableTypeExpression(toType);
            }

            // Casting
            if (expression.Type.GetUnderlyingType() != toType.GetUnderlyingType())
            {
                falseExpression = ConvertExpressionToTypeExpression(expression, toType);
            }

            // Nullable
            if (expression.Type.IsNullable())
            {
                falseExpression = ConvertExpressionToNullableExpression(falseExpression, toType);
            }

            // Return
            return isNullExpression == null ? falseExpression :
                Expression.Condition(isNullExpression, trueExpression, falseExpression);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToDbNullExpression(Expression expression,
            string propertyName)
        {
            var valueVariable = Expression.Variable(StaticType.Object, string.Concat("valueOf", propertyName));
            var valueIsNull = Expression.Equal(valueVariable, Expression.Constant(null));
            var dbNullValue = ConvertExpressionToTypeExpression(Expression.Constant(DBNull.Value), StaticType.Object);

            // Set the property value
            return Expression.Block(new[] { valueVariable }, Expression.Assign(valueVariable, expression),
                Expression.Condition(valueIsNull, dbNullValue, valueVariable));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="targetNullableType"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToNullableExpression(Expression expression,
            Type targetNullableType)
        {
            var underlyingType = Nullable.GetUnderlyingType(expression.Type);
            targetNullableType = targetNullableType.GetUnderlyingType();

            if (targetNullableType.IsValueType && (underlyingType == null || underlyingType != targetNullableType))
            {
                var nullableType = StaticType.Nullable.MakeGenericType(targetNullableType);
                var constructor = nullableType.GetConstructor(new[] { targetNullableType });
                expression = expression.Type.IsNullable() ? expression :
                    Expression.New(constructor, ConvertExpressionToTypeExpression(expression, targetNullableType));
            }

            return expression;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="handlerInstance"></param>
        /// <param name="classPropertyParameterInfo"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToPropertyHandlerGetExpression(Expression expression,
            object handlerInstance,
            ClassPropertyParameterInfo classPropertyParameterInfo)
        {
            // Return if null
            if (handlerInstance == null)
            {
                return expression;
            }

            // Variables Needed
            var getMethod = GetPropertyHandlerGetMethod(handlerInstance);
            var getParameter = GetPropertyHandlerGetParameter(getMethod);

            // Call the PropertyHandler.Get
            expression = Expression.Call(Expression.Constant(handlerInstance), getMethod, new[]
            {
                ConvertExpressionToTypeExpression(expression, getParameter.ParameterType),
                Expression.Convert(Expression.Constant(classPropertyParameterInfo?.ClassProperty), StaticType.ClassProperty)
            });

            // Convert to the return type
            return ConvertExpressionToTypeExpression(expression, getMethod.ReturnType);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="toType"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionWithAutomaticConversion(Expression expression,
            Type toType)
        {
            var fromType = expression.Type.GetUnderlyingType();

            // Guid to String
            if (fromType == StaticType.Guid && toType == StaticType.String)
            {
                expression = ConvertExpressionToGuidToStringExpression(expression);
            }

            // String to Guid
            else if (fromType == StaticType.String && toType == StaticType.Guid)
            {
                expression = ConvertExpressionToStringToGuidExpression(expression);
            }

            // TimeSpan to DateTime
            else if (fromType == StaticType.TimeSpan && toType == StaticType.DateTime)
            {
                expression = ConvertExpressionToTimeSpanToDateTimeExpression(expression);
            }

            // DateTime to TimeSpan
            else if (fromType == StaticType.DateTime && toType == StaticType.TimeSpan)
            {
                expression = ConvertExpressionToDateTimeToTimeSpanExpression(expression);
            }

            // Others
            else
            {
                expression = ConvertExpressionToSystemConvertExpression(expression, toType);
            }

            // Return
            return expression;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="classProperty"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToPropertyHandlerSetExpression(Expression expression,
            ClassProperty classProperty,
            Type targetType)
            => ConvertExpressionToPropertyHandlerSetExpressionTuple(expression, classProperty, targetType).convertedExpression;

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="classProperty"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        internal static (Expression convertedExpression, Type handlerSetReturnType) ConvertExpressionToPropertyHandlerSetExpressionTuple(Expression expression,
            ClassProperty classProperty,
            Type targetType)
        {
            var handlerInstance = classProperty?.GetPropertyHandler() ??
                PropertyHandlerCache.Get<object>(targetType);

            // Check
            if (handlerInstance == null)
            {
                return (expression, null);
            }

            // Variables
            var setMethod = GetPropertyHandlerSetMethod(handlerInstance);
            var setParameter = GetPropertyHandlerSetParameter(setMethod);

            // Nullable
            if (Nullable.GetUnderlyingType(setParameter.ParameterType) != null)
            {
                expression = ConvertExpressionToNullableExpression(expression, targetType);
            }

            // Call
            var valueExpression = ConvertExpressionToTypeExpression(expression, setParameter.ParameterType);
            var classPropertyExpression = classProperty != null ? (Expression)Expression.Constant(classProperty) : Expression.Default(StaticType.ClassProperty);
            expression = Expression.Call(Expression.Constant(handlerInstance),
                setMethod,
                new[] { valueExpression, classPropertyExpression });

            // Align
            return (ConvertExpressionToTypeExpression(expression, setMethod.ReturnType), setMethod.ReturnType);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="entityExpression"></param>
        /// <param name="readerParameterExpression"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToClassHandlerGetExpression<TResult>(Expression entityExpression,
            ParameterExpression readerParameterExpression)
        {
            var typeOfResult = typeof(TResult);

            // Check the handler
            var handlerInstance = GetClassHandler(typeOfResult);
            if (handlerInstance == null)
            {
                return entityExpression;
            }

            // Validate
            var handlerType = handlerInstance.GetType();
            if (handlerType.IsClassHandlerValidForModel(typeOfResult) == false)
            {
                throw new InvalidTypeException($"The class handler '{handlerType.FullName}' cannot be used for the type '{typeOfResult.FullName}'.");
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
        /// <typeparam name="TResult"></typeparam>
        /// <param name="entityOrEntitiesExpression"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToClassHandlerSetExpression<TResult>(Expression entityOrEntitiesExpression)
        {
            var typeOfResult = typeof(TResult);

            // Check the handler
            var handlerInstance = GetClassHandler(typeOfResult);
            if (handlerInstance == null)

            {
                return entityOrEntitiesExpression;
            }

            // Validate
            var handlerType = handlerInstance.GetType();
            if (handlerType.IsClassHandlerValidForModel(typeOfResult) == false)
            {
                throw new InvalidTypeException($"The class handler '{handlerType.FullName}' cannot be used for type '{typeOfResult.FullName}'.");
            }

            // Call the IClassHandler.Set method
            var typeOfListEntity = typeof(IList<TResult>);
            if (typeOfListEntity.IsAssignableFrom(entityOrEntitiesExpression.Type))
            {
                var setMethod = GetClassHandlerSetMethod(handlerInstance, typeOfListEntity);
                entityOrEntitiesExpression = Expression.Call(Expression.Constant(handlerInstance),
                    setMethod,
                    entityOrEntitiesExpression);
            }
            else
            {
                var setMethod = GetClassHandlerSetMethod(handlerInstance, typeOfResult);
                entityOrEntitiesExpression = Expression.Call(Expression.Constant(handlerInstance),
                    setMethod,
                    entityOrEntitiesExpression);
            }

            // Return the block
            return entityOrEntitiesExpression;
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
            // False expression
            var falseExpression = GetClassPropertyParameterInfoIsDbNullFalseValueExpression(readerParameterExpression,
                classPropertyParameterInfo, readerField);

            // Skip if possible
            if (readerField?.DbField?.IsNullable == false)
            {
                return falseExpression;
            }

            // IsDbNull Check
            var isDbNullExpression = Expression.Call(readerParameterExpression,
                StaticType.DbDataReader.GetMethod("IsDBNull"), Expression.Constant(readerField.Ordinal));

            // True Expression
            var trueExpression = GetClassPropertyParameterInfoIsDbNullTrueValueExpression(classPropertyParameterInfo,
                readerField);

            // Set the value
            return Expression.Condition(isDbNullExpression, trueExpression, falseExpression);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="classPropertyParameterInfo"></param>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static Expression GetClassPropertyParameterInfoIsDbNullTrueValueExpression(ClassPropertyParameterInfo classPropertyParameterInfo,
            DataReaderField readerField)
        {
            var parameterType = GetPropertyHandlerGetParameter(classPropertyParameterInfo)?.ParameterType;
            var classPropertyParameterInfoType = classPropertyParameterInfo.GetTargetType();

            // get handler on class property or type level. for detect default value type and convert
            var handlerInstance = GetHandlerInstance(classPropertyParameterInfo, readerField) ?? PropertyHandlerCache.Get<object>(classPropertyParameterInfo.GetTargetType());

            // default value expression
            var valueType = handlerInstance == null ?
                parameterType ?? classPropertyParameterInfoType :
                GetPropertyHandlerGetParameter(GetPropertyHandlerGetMethod(handlerInstance)).ParameterType;
            Expression valueExpression = Expression.Default(valueType);

            // Property Handler
            try
            {
                valueExpression = ConvertExpressionToPropertyHandlerGetExpression(valueExpression, handlerInstance, classPropertyParameterInfo);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Compiler.DataReader.IsDbNull.TrueExpression: Failed to convert the value expression for property handler '{handlerInstance?.GetType()}'. " +
                    $"{classPropertyParameterInfo.GetDescriptiveContextString()}", ex);
            }

            // Align the type
            try
            {
                valueExpression = ConvertExpressionToTypeExpression(valueExpression, classPropertyParameterInfoType);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Compiler.DataReader.IsDbNull.TrueExpression: Failed to convert the value expression into its destination .NET CLR Type '{classPropertyParameterInfoType.FullName}'. " +
                    $"{classPropertyParameterInfo.GetDescriptiveContextString()}", ex);
            }

            // Return
            return valueExpression;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="readerParameterExpression"></param>
        /// <param name="classPropertyParameterInfo"></param>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static Expression GetClassPropertyParameterInfoIsDbNullFalseValueExpression(ParameterExpression readerParameterExpression,
            ClassPropertyParameterInfo classPropertyParameterInfo,
            DataReaderField readerField)
        {
            var parameterType = GetPropertyHandlerGetParameter(classPropertyParameterInfo)?.ParameterType;
            var classPropertyParameterInfoType = classPropertyParameterInfo.GetTargetType();
            var targetType = parameterType ?? classPropertyParameterInfoType;
            var readerGetValueMethod = GetDbReaderGetValueOrDefaultMethod(readerField);
            var valueExpression = (Expression)GetDbReaderGetValueExpression(readerParameterExpression,
                readerGetValueMethod, readerField.Ordinal);
            var targetTypeUnderlyingType = targetType.GetUnderlyingType();
            var isAutomaticConversion = Converter.ConversionType == ConversionType.Automatic ||
                targetTypeUnderlyingType == StaticType.TimeSpan ||
                /* SQLite: Guid/String (Vice-Versa) : Enforce automatic conversion for the Primary/Identity fields */
                readerField.DbField?.IsPrimary == true || readerField.DbField?.IsIdentity == true;

            // get handler on class property or type level
            var handlerInstance = GetHandlerInstance(classPropertyParameterInfo, readerField) ?? PropertyHandlerCache.Get<object>(classPropertyParameterInfo.GetTargetType());

            // Auto-conversion
            if (isAutomaticConversion == true)
            {
                try
                {
                    valueExpression = ConvertExpressionWithAutomaticConversion(valueExpression, targetTypeUnderlyingType);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Compiler.DataReader.IsDbNull.FalseExpression: Failed to automatically convert the value expression. " +
                        $"{classPropertyParameterInfo.GetDescriptiveContextString()}", ex);
                }
            }
            else
            {
                // Enumerations
                if (targetTypeUnderlyingType.IsEnum)
                {
                    // if has PropertyHandler and parameter's type match, skip auto conversion.
                    var autoConvertEnum = true;
                    if (handlerInstance != null)
                    {
                        var getMethod = GetPropertyHandlerGetMethod(handlerInstance);
                        var getParameter = GetPropertyHandlerGetParameter(getMethod);
                        autoConvertEnum = !(getParameter.ParameterType.GetUnderlyingType() == readerField.Type);
                    }
                    if (autoConvertEnum)
                    {
                        try
                        {
                            valueExpression = ConvertExpressionToEnumExpression(valueExpression, readerField.Type, targetTypeUnderlyingType);
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidOperationException($"Compiler.DataReader.IsDbNull.FalseExpression: Failed to convert the value expression into enum type '{targetType.GetUnderlyingType()}'. " +
                                $"{classPropertyParameterInfo.GetDescriptiveContextString()}", ex);
                        }

                    }
                }
            }

            // Property Handler
            try
            {
                valueExpression = ConvertExpressionToPropertyHandlerGetExpression(valueExpression, handlerInstance, classPropertyParameterInfo);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Compiler.DataReader.IsDbNull.FalseExpression: Failed to convert the value expression for property handler '{handlerInstance?.GetType()}'. " +
                    $"{classPropertyParameterInfo.GetDescriptiveContextString()}", ex);
            }

            // Align the type
            try
            {
                valueExpression = ConvertExpressionToTypeExpression(valueExpression, classPropertyParameterInfoType);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Compiler.DataReader.IsDbNull.FalseExpression: Failed to convert the value expression into its destination .NET CLR Type '{classPropertyParameterInfoType.FullName}'. " +
                    $"{classPropertyParameterInfo.GetDescriptiveContextString()}", ex);
            }

            // Return
            return valueExpression;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        internal static Expression GetNullableTypeExpression(Type targetType) =>
            Expression.New(StaticType.Nullable.MakeGenericType(targetType.GetUnderlyingType()));

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="readerFieldsName"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static IEnumerable<ClassPropertyParameterInfo> GetClassPropertyParameterInfos<TResult>(IEnumerable<string> readerFieldsName,
            IDbSetting dbSetting)
        {
            var typeOfResult = typeof(TResult);
            var list = new List<ClassPropertyParameterInfo>();

            // Parameter information
            var constructorInfo = typeOfResult.GetConstructorWithMostArguments();
            var parameterInfos = constructorInfo?.GetParameters().AsList();

            // Class properties
            var classProperties = PropertyCache
                .Get(typeOfResult)?
                //.Where(property => property.PropertyInfo.CanWrite)
                .Where(property =>
                    readerFieldsName?.FirstOrDefault(field =>
                        string.Equals(field.AsUnquoted(true, dbSetting), property.GetMappedName().AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) != null)
                .AsList();

            // ParameterInfos
            parameterInfos?
                .ForEach(parameterInfo =>
                {
                    var classProperty = classProperties?.
                        FirstOrDefault(property =>
                            string.Equals(property.PropertyInfo.Name, parameterInfo.Name, StringComparison.OrdinalIgnoreCase));
                    if (classProperty != null)
                    {
                        list.Add(new ClassPropertyParameterInfo
                        {
                            ClassProperty = classProperty.PropertyInfo.CanWrite ? classProperty : null,
                            ParameterInfo = parameterInfo,
                            ParameterInfoMappedClassProperty = classProperty
                        });
                    }
                });

            // ClassProperties
            classProperties
                .Where(property => property.PropertyInfo.CanWrite)
                .AsList()
                .ForEach(property =>
                {
                    var listItem = list.FirstOrDefault(item => item.ClassProperty == property);
                    if (listItem != null)
                    {
                        return;
                    }
                    list.Add(new ClassPropertyParameterInfo { ClassProperty = property });
                });

            // Return the list
            return list;
        }

        /// <summary>
        /// Returns the list of the bindings for the entity.
        /// </summary>
        /// <typeparam name="TResult">The target entity type.</typeparam>
        /// <param name="readerParameterExpression">The data reader parameter.</param>
        /// <param name="readerFields">The list of fields to be bound from the data reader.</param>
        /// <param name="dbSetting">The database setting that is being used.</param>
        /// <returns>The enumerable list of <see cref="MemberBinding"/> objects.</returns>
        internal static IEnumerable<MemberBinding> GetMemberBindingsForDataEntity<TResult>(ParameterExpression readerParameterExpression,
            IEnumerable<DataReaderField> readerFields,
            IDbSetting dbSetting)
        {
            // Variables needed
            var readerFieldsName = readerFields.Select(f => f.Name.ToLowerInvariant()).AsList();
            var classPropertyParameterInfos = GetClassPropertyParameterInfos<TResult>(readerFieldsName, dbSetting);

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
                var mappedName = classPropertyParameterInfo.ParameterInfoMappedClassProperty?.GetMappedName().AsUnquoted(true, dbSetting) ??
                    classPropertyParameterInfo.ParameterInfo?.Name.AsUnquoted(true, dbSetting) ??
                    classPropertyParameterInfo.ClassProperty?.GetMappedName().AsUnquoted(true, dbSetting);

                // Skip if not found
                var ordinal = readerFieldsName.IndexOf(mappedName?.ToLowerInvariant());
                if (ordinal < 0)
                {
                    continue;
                }

                // Get the value expression
                var readerField = readerFields.First(f => string.Equals(f.Name.AsUnquoted(true, dbSetting), mappedName.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase));
                var expression = GetClassPropertyParameterInfoValueExpression(readerParameterExpression,
                    classPropertyParameterInfo, readerField);

                try
                {
                    // Member values
                    var memberAssignment = classPropertyParameterInfo.ClassProperty != null ?
                        Expression.Bind(classPropertyParameterInfo.ClassProperty.PropertyInfo, expression) : null;
                    var argument = classPropertyParameterInfo.ParameterInfo != null ? expression : null;

                    // Add the bindings
                    memberBindings.Add(new MemberBinding
                    {
                        ClassProperty = classPropertyParameterInfo.ClassProperty,
                        ParameterInfo = classPropertyParameterInfo?.ParameterInfo,
                        MemberAssignment = memberAssignment,
                        Argument = argument
                    });
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Compiler.MemberBinding: Failed to bind the value expression into a property/ctor-argument. " +
                        $"{classPropertyParameterInfo.GetDescriptiveContextString()}", ex);
                }
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
            GetDbReaderGetValueExpression(readerParameterExpression, readerGetValueMethod, Expression.Constant(ordinal));

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
                var expression = (Expression)GetDbReaderGetValueExpression(readerParameterExpression, readerGetValueMethod, ordinal);

                // Check for nullables
                if (readerField.DbField == null || readerField.DbField?.IsNullable == true)
                {
                    var isDbNullExpression = GetDbNullExpression(readerParameterExpression, ordinal);
                    var toType = (readerField.Type?.IsValueType != true) ? (readerField.Type ?? StaticType.Object) : StaticType.Object;
                    expression = Expression.Condition(isDbNullExpression, Expression.Default(toType),
                        ConvertExpressionToTypeExpression(expression, toType));
                }

                // Add to the bindings
                var values = new Expression[]
                {
                    Expression.Constant(readerField.Name),
                    ConvertExpressionToTypeExpression(expression, StaticType.Object)
                };
                elementInits.Add(Expression.ElementInit(addMethod, values));
            }

            // Return the result
            return elementInits;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="entityInstanceExpression"></param>
        /// <param name="classProperty"></param>
        /// <param name="dbField"></param>
        /// <returns></returns>
        internal static Expression GetEntityInstancePropertyValueExpression(Expression entityInstanceExpression,
            ClassProperty classProperty,
            DbField dbField)
        {
            var expression = (Expression)Expression.Property(entityInstanceExpression, classProperty.PropertyInfo);

            // Target type
            var handlerInstance = classProperty.GetPropertyHandler() ?? PropertyHandlerCache.Get<object>(dbField.Type.GetUnderlyingType());
            var targetType = GetPropertyHandlerSetParameter(handlerInstance)?.ParameterType ?? dbField.Type;

            // Auto-conversion Handling
            if (Converter.ConversionType == ConversionType.Automatic || dbField?.IsPrimary == true || dbField?.IsIdentity == true)
            {
                try
                {
                    expression = ConvertExpressionWithAutomaticConversion(expression, targetType?.GetUnderlyingType());
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Compiler.Entity/Object.Property: Failed to automatically convert the value expression for " +
                        $"property '{classProperty.GetMappedName()} ({classProperty.PropertyInfo.PropertyType.FullName})'. {classProperty}", ex);
                }
            }

            /*
             * Note: The other data provider can coerce the Enum into its destination data type in the DB by default,
             *       except for PostgreSQL. The code written below is only to address the issue for this specific provider.
             */

            // Enum Handling
            if (classProperty.PropertyInfo.PropertyType.GetUnderlyingType().IsEnum == true)
            {
                try
                {
                    var dbType = classProperty.GetDbType();
                    var toType = dbType.HasValue ? new DbTypeToClientTypeResolver().Resolve(dbType.Value) : targetType?.GetUnderlyingType();
                    expression = ConvertEnumExpressionToTypeExpression(expression, toType);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Compiler.Entity/Object.Property: Failed to convert the value expression from " +
                        $"enumeration '{classProperty.PropertyInfo.PropertyType.FullName}' to type '{targetType?.GetUnderlyingType()}'. {classProperty}", ex);
                }
            }

            // Property Handler
            try
            {
                expression = ConvertExpressionToPropertyHandlerSetExpression(expression, classProperty,
                    dbField?.Type.GetUnderlyingType());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Compiler.Entity/Object.Property: Failed to convert the value expression for property handler '{handlerInstance?.GetType()}'. " +
                    $"{classProperty}", ex);
            }

            // Convert to object
            return ConvertExpressionToTypeExpression(expression, StaticType.Object);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="propertyExpression"></param>
        /// <param name="objectInstanceExpression"></param>
        /// <param name="dbField"></param>
        /// <returns></returns>
        internal static Expression GetObjectInstancePropertyValueExpression(ParameterExpression propertyExpression,
            Expression objectInstanceExpression,
            DbField dbField)
        {
            var methodInfo = StaticType.PropertyInfo.GetMethod("GetValue", new[] { StaticType.Object });
            var expression = (Expression)Expression.Call(propertyExpression, methodInfo, objectInstanceExpression);

            // Property Handler
            expression = ConvertExpressionToPropertyHandlerSetExpression(expression, null,
                dbField?.Type.GetUnderlyingType());

            // Convert to object
            return ConvertExpressionToTypeExpression(expression, StaticType.Object);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dictionaryInstanceExpression"></param>
        /// <param name="dbField"></param>
        /// <returns></returns>
        internal static Expression GetDictionaryStringObjectPropertyValueExpression(Expression dictionaryInstanceExpression,
            DbField dbField)
        {
            var methodInfo = StaticType.IDictionaryStringObject.GetMethod("get_Item", new[] { StaticType.String });
            var expression = (Expression)Expression.Call(dictionaryInstanceExpression, methodInfo, Expression.Constant(dbField.Name));

            // Property Handler
            expression = ConvertExpressionToPropertyHandlerSetExpression(expression, null,
                dbField.Type.GetUnderlyingType());

            // Convert to object
            return ConvertExpressionToTypeExpression(expression, StaticType.Object);
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
        internal static Expression GetDataEntityDbParameterValueAssignmentExpression(ParameterExpression parameterVariableExpression,
            Expression entityExpression,
            ParameterExpression propertyExpression,
            ClassProperty classProperty,
            DbField dbField,
            IDbSetting dbSetting)
        {
            var expression = (Expression)null;

            // Get the property value
            if (propertyExpression.Type == StaticType.PropertyInfo)
            {
                expression = GetObjectInstancePropertyValueExpression(propertyExpression, entityExpression, dbField);
            }
            else
            {
                expression = GetEntityInstancePropertyValueExpression(entityExpression, classProperty, dbField);
            }

            // Nullable
            if (dbField?.IsNullable == true)
            {
                expression = ConvertExpressionToDbNullExpression(expression, dbField.Name.AsUnquoted(true, dbSetting).AsAlphaNumeric());
            }

            // Set the value
            return Expression.Call(parameterVariableExpression, GetDbParameterValueSetMethod(), expression);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameterVariableExpression"></param>
        /// <param name="dictionaryInstanceExpression"></param>
        /// <param name="dbField"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static Expression GetDictionaryStringObjectDbParameterValueAssignmentExpression(ParameterExpression parameterVariableExpression,
            Expression dictionaryInstanceExpression,
            DbField dbField,
            IDbSetting dbSetting)
        {
            var expression = GetDictionaryStringObjectPropertyValueExpression(dictionaryInstanceExpression, dbField);

            // Nullable
            if (dbField?.IsNullable == true)
            {
                expression = ConvertExpressionToDbNullExpression(expression, dbField.Name.AsUnquoted(true, dbSetting).AsAlphaNumeric());
            }

            // Set the value
            return Expression.Call(parameterVariableExpression, GetDbParameterValueSetMethod(), expression);
        }

        private static DbType? GetDbType(ClassProperty classProperty,
            Type dbFieldType)
        {
            var dbType = classProperty?.GetDbType();
            if (dbType == null)
            {
                var underlyingType = dbFieldType?.GetUnderlyingType();
                dbType = TypeMapper.Get(underlyingType) ?? new ClientTypeToDbTypeResolver().Resolve(underlyingType);
            }
            return dbType;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameterVariableExpression"></param>
        /// <param name="classProperty"></param>
        /// <param name="dbField"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbParameterDbTypeAssignmentExpression(ParameterExpression parameterVariableExpression,
            ClassProperty classProperty,
            DbField dbField)
            => GetDbParameterDbTypeAssignmentExpression(parameterVariableExpression, GetDbType(classProperty, dbField.Type));

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameterVariableExpression"></param>
        /// <param name="dbField"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbParameterDbTypeAssignmentExpression(ParameterExpression parameterVariableExpression,
            DbField dbField)
            => GetDbParameterDbTypeAssignmentExpression(parameterVariableExpression, GetDbType(null, dbField.Type));

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameterVariableExpression"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbParameterDbTypeAssignmentExpression(ParameterExpression parameterVariableExpression,
            DbType? dbType)
        {
            var expression = (MethodCallExpression)null;
            var dbParameterDbTypeSetMethod = StaticType.DbParameter.GetProperty("DbType").SetMethod;

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
        /// <param name="parameterVariableExpression"></param>
        /// <param name="dbField"></param>
        /// <param name="entityIndex"></param>
        /// <param name="dbSetting"></param>
        internal static MethodCallExpression GetDbParameterNameAssignmentExpression(ParameterExpression parameterVariableExpression,
            DbField dbField,
            int entityIndex,
            IDbSetting dbSetting)
        {
            var dbParameterParameterNameSetMethod = StaticType.DbParameter.GetProperty("ParameterName").SetMethod;
            var parameterName = dbField.Name.AsUnquoted(true, dbSetting).AsAlphaNumeric();
            parameterName = entityIndex > 0 ? string.Concat(dbSetting.ParameterPrefix, parameterName, "_", entityIndex) :
                string.Concat(dbSetting.ParameterPrefix, parameterName);
            return Expression.Call(parameterVariableExpression, dbParameterParameterNameSetMethod,
                Expression.Constant(parameterName));
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
        internal static Expression GetDataEntityParameterAssignmentExpression(ParameterExpression commandParameterExpression,
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
                var valueAssignmentExpression = GetDataEntityDbParameterValueAssignmentExpression(parameterVariableExpression,
                    entityExpression,
                    propertyExpression,
                    classProperty,
                    dbField,
                    dbSetting);
                parameterAssignmentExpressions.AddIfNotNull(valueAssignmentExpression);
            }

            // DbParameter.DbType
            var dbTypeAssignmentExpression = GetDbParameterDbTypeAssignmentExpression(parameterVariableExpression,
                classProperty, dbField);
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
        /// <param name="entityIndex"></param>
        /// <param name="dictionaryInstanceExpression"></param>
        /// <param name="dbField"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static Expression GetDictionaryStringObjectParameterAssignmentExpression(ParameterExpression commandParameterExpression,
            int entityIndex,
            Expression dictionaryInstanceExpression,
            DbField dbField,
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
            var valueAssignmentExpression = GetDictionaryStringObjectDbParameterValueAssignmentExpression(parameterVariableExpression,
                dictionaryInstanceExpression,
                dbField,
                dbSetting);
            parameterAssignmentExpressions.AddIfNotNull(valueAssignmentExpression);

            // DbParameter.DbType
            var dbTypeAssignmentExpression = GetDbParameterDbTypeAssignmentExpression(parameterVariableExpression,
                dbField);
            parameterAssignmentExpressions.AddIfNotNull(dbTypeAssignmentExpression);

            // DbParameter.Direction
            if (dbSetting.IsDirectionSupported)
            {
                var directionAssignmentExpression = GetDbParameterDirectionAssignmentExpression(parameterVariableExpression, ParameterDirection.Input);
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

            // Add to body
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
            if (entityExpression.Type.IsClassType() == false)
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
                classProperty = entityProperties.FirstOrDefault(property =>
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
                var parameterAssignment = GetDataEntityParameterAssignmentExpression(commandParameterExpression,
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
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static Expression ThrowIfNullAfterClassHandlerExpression<TResult>(Expression expression)
        {
            var typeOfResult = typeof(TResult);
            var isNullExpression = Expression.Equal(Expression.Constant(null), expression);
            var exception = new NullReferenceException($"Entity of type '{typeOfResult}' must not be null. If you have defined a class handler, please check the 'Set' method.");
            return Expression.IfThen(isNullExpression, Expression.Throw(Expression.Constant(exception)));
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="commandParameterExpression"></param>
        /// <param name="entitiesParameterExpression"></param>
        /// <param name="fieldDirections"></param>
        /// <param name="entityIndex"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static Expression GetIndexDbParameterSetterExpression<TResult>(ParameterExpression commandParameterExpression,
            Expression entitiesParameterExpression,
            IEnumerable<FieldDirection> fieldDirections,
            int entityIndex,
            IDbSetting dbSetting)
        {
            // Get the current instance
            var typeOfResult = typeof(TResult);
            var entityVariableExpression = Expression.Variable(typeOfResult, "instance");
            var typeOfListEntity = typeof(IList<TResult>);
            var entityParameter = (Expression)GetListEntityIndexerExpression(entitiesParameterExpression, typeOfListEntity, entityIndex);
            var entityExpressions = new List<Expression>();
            var entityVariables = new List<ParameterExpression>();

            // Class handler
            entityParameter = ConvertExpressionToClassHandlerSetExpression<TResult>(entityParameter);

            // Entity instance
            entityVariables.Add(entityVariableExpression);
            entityExpressions.Add(Expression.Assign(entityVariableExpression, entityParameter));

            // Throw if null
            entityExpressions.Add(ThrowIfNullAfterClassHandlerExpression<TResult>(entityVariableExpression));

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

        #endregion
    }
}
