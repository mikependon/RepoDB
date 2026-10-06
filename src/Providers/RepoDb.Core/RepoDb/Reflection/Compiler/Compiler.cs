#region Copyright Attributions

// Copyright (c) 2020 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Diagnostics.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Globalization;
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
            public Type GetTargetType()
            {
                return TargetType ?? ParameterInfo?.ParameterType ?? ClassProperty?.PropertyInfo?.PropertyType;
            }

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
            public override string ToString()
            {
                return string.Concat("TargetType = ", GetTargetType()?.FullName, ", ClassProperty = ", ClassProperty?.ToString(), ", ",
                    "ParameterInfo = ", ParameterInfo?.ToString(), ")", ", TargetType = ", TargetType?.ToString(), ", ");
            }
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
            public override string ToString()
            {
                return ClassProperty?.ToString() ?? ParameterInfo?.ToString();
            }
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
            Type toType)
        {
            return typeof(System.Convert).GetMethod(string.Concat("To", TypeCache.Get(toType).GetUnderlyingType().Name),
                new[] { TypeCache.Get(fromType).GetUnderlyingType() });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        internal static MethodInfo GetSystemConvertChangeTypeMethod(Type conversionType)
        {
            return typeof(System.Convert).GetMethod("ChangeType",
                new[] { StaticType.Object, TypeCache.Get(conversionType).GetUnderlyingType() });
        }

        /// <summary>
        /// Gets the <see cref="Convert.ChangeType(object, Type, IFormatProvider)"/> method.
        /// </summary>
        /// <returns></returns>
        internal static MethodInfo GetConvertChangeTypeWithProviderMethod()
        {
            return typeof(System.Convert).GetMethod("ChangeType",
                new[] { StaticType.Object, StaticType.Type, typeof(IFormatProvider) });
        }

        /// <summary>
        /// Checks whether the type is a numeric primitive or <see cref="decimal"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsNumericType(Type type) =>
            type == StaticType.Decimal || type == StaticType.Double || type == StaticType.Single ||
            type == StaticType.Int64 || type == StaticType.Int32 || type == StaticType.Int16 ||
            type == StaticType.Byte || type == StaticType.SByte || type == StaticType.UInt16 ||
            type == StaticType.UInt32 || type == StaticType.UInt64;

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static object GetClassHandler(Type type)
        {
            return ClassHandlerCache.Get<object>(type);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="handlerInstance"></param>
        /// <returns></returns>
        internal static MethodInfo GetClassHandlerGetMethod(object handlerInstance,
            Type modelType)
        {
            return GetInterfaceMethod(GetClassHandlerInterface(handlerInstance, modelType), classHandlerGetMethodDefinition);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="handlerInstance"></param>
        /// <returns></returns>
        internal static MethodInfo GetClassHandlerSetMethod(object handlerInstance,
            Type modelType)
        {
            return GetInterfaceMethod(GetClassHandlerInterface(handlerInstance, modelType), classHandlerSetMethodDefinition);
        }

        private static readonly MethodInfo classHandlerGetMethodDefinition =
            typeof(IClassHandler<>).GetMethod(nameof(IClassHandler<object>.Get));

        private static readonly MethodInfo classHandlerSetMethodDefinition =
            typeof(IClassHandler<>).GetMethod(nameof(IClassHandler<object>.Set));

        private static readonly MethodInfo propertyHandlerGetMethodDefinition =
            typeof(IPropertyHandler<,>).GetMethod(nameof(IPropertyHandler<object, object>.Get));

        private static readonly MethodInfo propertyHandlerSetMethodDefinition =
            typeof(IPropertyHandler<,>).GetMethod(nameof(IPropertyHandler<object, object>.Set));

        /// <summary>
        /// Returns the implemented <see cref="IClassHandler{TEntity}"/> interface of the class handler for the model type.
        /// </summary>
        /// <param name="handlerInstance">The class handler instance.</param>
        /// <param name="modelType">The target model type.</param>
        /// <returns>The implemented interface, or null if the handler cannot be used for the model type.</returns>
        internal static Type GetClassHandlerInterface(object handlerInstance,
            Type modelType)
        {
            return GetHandlerInterfaces(handlerInstance)?
                .FirstOrDefault(interfaceType => interfaceType.IsGenericTypeOf(StaticType.IClassHandler) &&
                    interfaceType.GetGenericArguments()[0] == modelType);
        }

        /// <summary>
        /// Returns the method of a constructed generic interface based on the method of its generic type definition.
        /// </summary>
        /// <param name="interfaceType">The constructed generic interface.</param>
        /// <param name="methodDefinition">The method of the generic type definition.</param>
        /// <returns>The method of the constructed generic interface.</returns>
        private static MethodInfo GetInterfaceMethod(Type interfaceType,
            MethodInfo methodDefinition)
        {
            return interfaceType == null ? null :
                (MethodInfo)MethodBase.GetMethodFromHandle(methodDefinition.MethodHandle, interfaceType.TypeHandle);
        }

        /// <summary>
        /// Returns the interfaces implemented by the type of a property or class handler instance.
        /// </summary>
        /// <param name="handlerInstance">The handler instance.</param>
        /// <returns>The list of implemented interfaces.</returns>
        [UnconditionalSuppressMessage("Trimming", "IL2075:'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method.",
            Justification = "The handler types are registered via the 'ClassHandler'/'PropertyHandler' attributes, or via the generic mapper " +
                "methods, all of which are annotated to preserve the implemented interfaces (Trimming.Handler). The registration methods " +
                "accepting an untyped handler object are annotated with RequiresUnreferencedCode.")]
        private static Type[] GetHandlerInterfaces(object handlerInstance) =>
            handlerInstance?.GetType().GetInterfaces();

        /// <summary>
        ///
        /// </summary>
        /// <param name="handlerInstance"></param>
        /// <returns></returns>
        internal static Type GetPropertyHandlerInterface(object handlerInstance)
        {
            // In F#, the instance is not a concrete class, therefore, we need to extract it by interface
            return GetHandlerInterfaces(handlerInstance)?
                .FirstOrDefault(interfaceType => interfaceType.IsGenericTypeOf(StaticType.IPropertyHandler));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="handlerInstance"></param>
        /// <returns></returns>
        internal static MethodInfo GetPropertyHandlerGetMethod(object handlerInstance)
        {
            return GetInterfaceMethod(GetPropertyHandlerInterface(handlerInstance), propertyHandlerGetMethodDefinition);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="handlerInstance"></param>
        /// <returns></returns>
        internal static MethodInfo GetPropertyHandlerGetMethodFromInterface(object handlerInstance)
        {
            return GetPropertyHandlerGetMethod(handlerInstance);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static MethodInfo GetDbCommandCreateParameterMethod()
        {
            return typeof(RepoDb.Extensions.DbCommandExtension).GetMethod("CreateParameter", new[]
            {
                StaticType.IDbCommand,
                StaticType.String,
                StaticType.Object,
                StaticType.DbTypeNullable
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static MethodInfo GetDbParameterCollectionAddMethod()
        {
            return typeof(System.Data.Common.DbParameterCollection).GetMethod("Add");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="handlerInstance"></param>
        /// <returns></returns>
        internal static MethodInfo GetPropertyHandlerSetMethod(object handlerInstance)
        {
            return GetInterfaceMethod(GetPropertyHandlerInterface(handlerInstance), propertyHandlerSetMethodDefinition);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        internal static Type GetPropertyHandlerSetMethodReturnType(ClassProperty property)
        {
            return GetPropertyHandlerSetMethod(property?.GetPropertyHandler())?.ReturnType;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="handlerInstance"></param>
        /// <returns></returns>
        internal static Type GetPropertyHandlerSetMethodReturnType(object handlerInstance)
        {
            return GetPropertyHandlerSetMethod(handlerInstance)?.ReturnType;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="classPropertyParameterInfo"></param>
        /// <returns></returns>
        internal static ParameterInfo GetPropertyHandlerGetParameter(ClassPropertyParameterInfo classPropertyParameterInfo)
        {
            return GetPropertyHandlerGetParameter(classPropertyParameterInfo?.ClassProperty);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="property"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        internal static Type GetPropertyHandlerSetMethodReturnType(ClassProperty property,
            Type targetType)
        {
            return GetPropertyHandlerSetMethod(property?.GetPropertyHandler() ??
                PropertyHandlerCache.Get<object>(targetType))?.ReturnType;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="classProperty"></param>
        /// <returns></returns>
        internal static ParameterInfo GetPropertyHandlerGetParameter(ClassProperty classProperty)
        {
            return GetPropertyHandlerGetParameter(classProperty?.GetPropertyHandler());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="handlerInstance"></param>
        /// <returns></returns>
        internal static ParameterInfo GetPropertyHandlerGetParameter(object handlerInstance)
        {
            return GetPropertyHandlerGetParameter(GetPropertyHandlerGetMethod(handlerInstance));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="getMethod"></param>
        /// <returns></returns>
        internal static ParameterInfo GetPropertyHandlerGetParameter(MethodInfo getMethod)
        {
            return getMethod?.GetParameters()[0];
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="handlerInstance"></param>
        /// <returns></returns>
        internal static ParameterInfo GetPropertyHandlerSetParameter(object handlerInstance)
        {
            return GetPropertyHandlerSetParameter(GetPropertyHandlerSetMethod(handlerInstance));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="setMethod"></param>
        /// <returns></returns>
        internal static ParameterInfo GetPropertyHandlerSetParameter(MethodInfo setMethod)
        {
            return setMethod?.GetParameters()[0];
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static IEnumerable<DataReaderField> GetDataReaderFields(DbDataReader reader,
            IDbSetting dbSetting)
        {
            return GetDataReaderFields(reader, dbFields: null, dbSetting);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="dbFields"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static IEnumerable<DataReaderField> GetDataReaderFields(DbDataReader reader,
            DbFieldCollection dbFields,
            IDbSetting dbSetting)
        {
            return Enumerable.Range(0, reader.FieldCount)
                .Select(reader.GetName)
                .Select((name, ordinal) => new DataReaderField
                {
                    Name = name,
                    Ordinal = ordinal,
                    Type = reader.GetFieldType(ordinal) ?? StaticType.Object,
                    DbField = dbFields?.GetByUnquotedName(name.AsUnquoted(trim: true, dbSetting))
                });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="classPropertyParameterInfo"></param>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static object GetHandlerInstance(ClassPropertyParameterInfo classPropertyParameterInfo,
            DataReaderField readerField)
        {
            return GetHandlerInstance(classPropertyParameterInfo.ClassProperty, readerField);
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
                    .Get<object>(TypeCache.Get(readerField.Type).GetUnderlyingType());
            }
            return value;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static MethodInfo GetDbReaderGetValueMethod(DataReaderField readerField)
        {
            return GetDbReaderGetValueMethod(readerField.Type);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        internal static MethodInfo GetDbReaderGetValueMethod(Type targetType)
        {
            // The typed getters of the DbDataReader (i.e.: 'Get' + the name of the type), statically defined for trimming.
            return targetType == null ? null : Type.GetTypeCode(targetType) switch
            {
                TypeCode.Boolean => typeof(DbDataReader).GetMethod(nameof(DbDataReader.GetBoolean)),
                TypeCode.Byte => typeof(DbDataReader).GetMethod(nameof(DbDataReader.GetByte)),
                TypeCode.Char => typeof(DbDataReader).GetMethod(nameof(DbDataReader.GetChar)),
                TypeCode.DateTime => typeof(DbDataReader).GetMethod(nameof(DbDataReader.GetDateTime)),
                TypeCode.Decimal => typeof(DbDataReader).GetMethod(nameof(DbDataReader.GetDecimal)),
                TypeCode.Double => typeof(DbDataReader).GetMethod(nameof(DbDataReader.GetDouble)),
                TypeCode.Int16 => typeof(DbDataReader).GetMethod(nameof(DbDataReader.GetInt16)),
                TypeCode.Int32 => typeof(DbDataReader).GetMethod(nameof(DbDataReader.GetInt32)),
                TypeCode.Int64 => typeof(DbDataReader).GetMethod(nameof(DbDataReader.GetInt64)),
                TypeCode.String => typeof(DbDataReader).GetMethod(nameof(DbDataReader.GetString)),
                _ => targetType == StaticType.Guid ? typeof(DbDataReader).GetMethod(nameof(DbDataReader.GetGuid)) :
                    targetType == typeof(System.IO.Stream) ? typeof(DbDataReader).GetMethod(nameof(DbDataReader.GetStream)) :
                    targetType == typeof(System.IO.TextReader) ? typeof(DbDataReader).GetMethod(nameof(DbDataReader.GetTextReader)) :
                    null
            };
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static MethodInfo GetDbReaderGetValueMethod()
        {
            return typeof(System.Data.Common.DbDataReader).GetMethod("GetValue");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static MethodInfo GetDbReaderGetValueOrDefaultMethod(DataReaderField readerField)
        {
            return GetDbReaderGetValueOrDefaultMethod(readerField.Type);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        internal static MethodInfo GetDbReaderGetValueOrDefaultMethod(Type targetType)
        {
            return GetDbReaderGetValueMethod(targetType) ?? GetDbReaderGetValueMethod();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static MethodInfo GetDbParameterValueSetMethod()
        {
            return typeof(System.Data.Common.DbParameter).GetProperty("Value").SetMethod;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static PropertyInfo GetTimeSpanTicksProperty()
        {
            return typeof(System.TimeSpan).GetProperty("Ticks");
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static MethodInfo GetTimeSpanTicksPropertyGetMethod()
        {
            return GetTimeSpanTicksProperty().GetMethod;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static PropertyInfo GetDateTimeTimeOfDayProperty()
        {
            return typeof(System.DateTime).GetProperty("TimeOfDay");
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static MethodInfo GetDateTimeTimeOfDayPropertyGetMethod()
        {
            return GetDateTimeTimeOfDayProperty().GetMethod;
        }
#if NET6_0_OR_GREATER
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static MethodInfo GetDateOnlyFromDateTimeStaticMethod()
        {
            return typeof(System.DateOnly).GetMethod("FromDateTime");
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static MethodInfo GetDateTimeFromDateOnlyMethod()
        {
            return typeof(System.DateOnly).GetMethod("ToDateTime", new Type[] { StaticType.TimeOnly });
        }
#endif

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static MethodInfo GetEnumGetNameMethod()
        {
            return typeof(System.Enum).GetMethod("GetName", new[] { StaticType.Type, StaticType.Object });
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static MethodInfo GetEnumIsDefinedMethod()
        {
            return typeof(System.Enum).GetMethod("IsDefined", new[] { StaticType.Type, StaticType.Object });
        }

        internal static MethodInfo GetEnumParseNullMethod()
        {
            return typeof(Compiler).GetMethod(nameof(EnumParseNull), BindingFlags.Static | BindingFlags.NonPublic);
        }

        private static object EnumParseNull(Type enumType,
            string value)
        {
            return EnumTryParse(enumType, value, out var r) ? r : null;
        }

        internal static MethodInfo GetEnumParseNullDefinedMethod()
        {
            return typeof(Compiler).GetMethod(nameof(EnumParseNullDefined), BindingFlags.Static | BindingFlags.NonPublic);
        }

        private static object EnumParseNullDefined(Type enumType,
            string value)
        {
            return EnumTryParse(enumType, value, out var r) && Enum.IsDefined(enumType, r) ? r : null;
        }

        /// <summary>
        /// A non-generic equivalent of the case insensitive 'Enum.TryParse&lt;TEnum&gt;()' method, which does not require
        /// the generic method to be instantiated at runtime (not possible with NativeAOT).
        /// </summary>
        private static bool EnumTryParse(Type enumType,
            string value,
            out object result)
        {
#if NET
            return Enum.TryParse(enumType, value, ignoreCase: true, out result);
#else
            result = null;
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            try
            {
                result = Enum.Parse(enumType, value, ignoreCase: true);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (OverflowException)
            {
                return false;
            }
#endif
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// 
        /// <returns></returns>
        internal static Expression GetNullableHasValueExpression(Expression expression)
        {
            // Equivalent to 'Nullable<T>.HasValue'
            return Expression.NotEqual(expression, Expression.Constant(null, expression.Type));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToNullableValue(Expression expression)
        {
            if (Nullable.GetUnderlyingType(expression.Type) is { } underlyingType)
            {
                // Equivalent to 'Nullable<T>.Value' (throws if the value is null)
                return Expression.Convert(expression, underlyingType);
            }
            return expression;
        }

#if NET6_0_OR_GREATER
        internal static Expression ConvertExpressionToNullableGetValueOrDefaultExpression(Func<Expression, Expression> converter, Expression expression)
        {
            if (Nullable.GetUnderlyingType(expression.Type) != null)
            {
                var converted = converter(ConvertExpressionToNullableValue(expression));
                var nullableType = converted.Type.MakeNullableType();
                return Expression.Condition(
                    GetNullableHasValueExpression(expression),
                    Expression.Convert(converted, nullableType),
                    Expression.Constant(null, nullableType)
                );
            }

            return converter(expression);
        }
#endif

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToNullableValueExpression(Expression expression)
        {
            return expression;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToGuidToStringExpression(Expression expression)
        {
            return Expression.Call(ConvertExpressionToNullableValue(expression), typeof(System.Guid).GetMethod("ToString", Array.Empty<Type>()));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToStringToGuidExpression(Expression expression)
        {
            return Expression.New(typeof(System.Guid).GetConstructor(new[] { StaticType.String }), ConvertExpressionToNullableValue(expression));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToTimeSpanToDateTimeExpression(Expression expression)
        {
            return Expression.New(typeof(System.DateTime).GetConstructor(new[] { StaticType.Int64 }),
                ConvertExpressionToNullableValue(ConvertExpressionToTimeSpanTicksExpression(expression)));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToDateTimeToTimeSpanExpression(Expression expression)
        {
            return ConvertExpressionToNullableValue(ConvertExpressionToDateTimeTimeOfDayExpression(expression));
        }
#if NET6_0_OR_GREATER
        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToDateTimeToDateOnlyExpression(Expression expression)
        {
            return ConvertExpressionToNullableGetValueOrDefaultExpression(ConvertExpressionToDateTimeFromDateOnlyExpression, expression);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToDateOnlyToDateTimeExpression(Expression expression)
        {
            return ConvertExpressionToNullableGetValueOrDefaultExpression(ConvertExpressionToDateOnlyFromDateTimeExpression, expression);
        }
#endif
        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToTimeSpanTicksExpression(Expression expression)
        {
            return Expression.Call(expression, GetTimeSpanTicksPropertyGetMethod());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToDateTimeTimeOfDayExpression(Expression expression)
        {
            return Expression.Call(expression, GetDateTimeTimeOfDayPropertyGetMethod());
        }
#if NET6_0_OR_GREATER
        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToDateOnlyFromDateTimeExpression(Expression expression)
        {
            return Expression.Call(expression, GetDateTimeFromDateOnlyMethod(), Expression.Constant(default(TimeOnly)));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToDateTimeFromDateOnlyExpression(Expression expression)
        {
            return Expression.Call(instance: null, GetDateOnlyFromDateTimeStaticMethod(), expression);
        }
#endif
        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="toType"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToSystemConvertExpression(Expression expression,
            Type toType)
        {
            var fromType = expression.Type;
            var underlyingFromType = TypeCache.Get(fromType).GetUnderlyingType();
            var underlyingToType = TypeCache.Get(toType).GetUnderlyingType();

            if (fromType == toType)
            {
                return expression;
            }

            // Identify
            if (underlyingToType.IsAssignableFrom(fromType))
            {
                return ConvertExpressionToTypeExpression(expression, underlyingToType);
            }

            var result = ConvertExpressionToNullableValue(expression);

            // Convert.To<Type>()
            if (underlyingFromType.IsEnum)
            {
                if (underlyingToType == StaticType.String)
                {
                    result = Expression.Call(result, typeof(Enum).GetMethod(nameof(Enum.ToString), Type.EmptyTypes));
                }
                else if (underlyingToType.IsPrimitive &&
                    (underlyingToType) == StaticType.Int16
                    || underlyingToType == StaticType.Int32
                    || underlyingToType == StaticType.Int64
                    || underlyingToType == StaticType.Byte
                    || underlyingToType == StaticType.UInt16
                    || underlyingToType == StaticType.UInt32
                    || underlyingToType == StaticType.UInt64
                    || underlyingToType == StaticType.SByte)
                {
                    result = Expression.Convert(result, Enum.GetUnderlyingType(underlyingFromType));

                    if (result.Type != underlyingToType)
                        result = Expression.Convert(result, underlyingToType);
                }
                else
                    return result; // Will fail
            }
            else if (GetSystemConvertToTypeMethod(underlyingFromType, underlyingToType) is { } methodInfo)
            {
                result = Expression.Call(methodInfo, result);
            }
            else if (GetSystemConvertChangeTypeMethod(underlyingToType) is { } systemChangeType)
            {
                result = Expression.Call(systemChangeType, new Expression[]
                {
                    ConvertExpressionToTypeExpression(result, StaticType.Object),
                    Expression.Constant(TypeCache.Get(underlyingToType).GetUnderlyingType())
                });
            }
            else
            {
                return result; // Will fail!
            }

            // Do we need manual NULL handling?
            if ((!underlyingToType.IsValueType || underlyingToType != toType)
                && (!underlyingFromType.IsValueType || underlyingFromType != fromType))
            {
                Expression condition;
                if (underlyingFromType != fromType)
                {
                    // E.g. Nullable<System.Int32> -> string
                    condition = GetNullableHasValueExpression(expression);
                }
                else
                {
                    // E.g. String -> Nullable<System.Int32>
                    condition = Expression.NotEqual(expression, Expression.Constant(null, expression.Type));
                }

                return Expression.Condition(
                    condition,
                    (result.Type != toType) ? Expression.Convert(result, toType) : result,
                    Expression.Constant(null, toType));
            }

            // Return
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="toType"></param>
        /// <returns></returns>
        // The conversions from/into these types are defined as user-defined conversion operators (i.e.: 'op_Implicit' and
        // 'op_Explicit'), which are looked-up via reflection by the 'Expression.Convert()' method and must not be trimmed.
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods, typeof(decimal))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods, typeof(DateTimeOffset))]
        internal static Expression ConvertExpressionToTypeExpression(Expression expression,
            Type toType)
        {
            return (expression.Type != toType) ? Expression.Convert(expression, toType) : expression;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="fromType"></param>
        /// <param name="toEnumType"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToEnumExpression(Expression expression,
            Type fromType,
            Type toEnumType)
        {
            return (fromType == StaticType.String) ?
                ConvertExpressionToEnumExpressionForString(expression, toEnumType) :
                    ConvertExpressionToEnumExpressionForNonString(expression, toEnumType);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="toEnumType"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToEnumExpressionForString(Expression expression,
            Type toEnumType)
        {
            var checkMethod = (GlobalConfiguration.Options.ConversionType == ConversionType.Automatic || GlobalConfiguration.Options.EnumHandling == EnumHandling.Cast || toEnumType.GetCustomAttribute<FlagsAttribute>() != null)
                ? GetEnumParseNullMethod()
                : GetEnumParseNullDefinedMethod();

            return Expression.Convert(
                    Expression.Coalesce(
                        Expression.Call(checkMethod, Expression.Constant(toEnumType), expression),

                        (GlobalConfiguration.Options.EnumHandling == EnumHandling.UseDefault)
                        ? Expression.Convert(Expression.Default(toEnumType), StaticType.Object)
                        : Expression.Throw(Expression.New(
                            typeof(ArgumentOutOfRangeException).GetConstructor(new[] { StaticType.String, StaticType.Object, StaticType.String }),
                            Expression.Constant("value"),
                            expression,
                            Expression.Constant($"Invalid value for {toEnumType.Name}")),
                            StaticType.Object)),
                    toEnumType);
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
            if (GlobalConfiguration.Options.ConversionType == ConversionType.Automatic || GlobalConfiguration.Options.EnumHandling == EnumHandling.Cast)
            {
                return Expression.Convert(expression, toEnumType);
            }
            else
            {
                // Handle long/short to enum and/or non integer based enums
                if (expression.Type != Enum.GetUnderlyingType(toEnumType))
                    expression = Expression.Convert(expression, Enum.GetUnderlyingType(toEnumType));

                return Expression.Condition(
                    GetEnumIsDefinedExpression(expression, toEnumType), // Check if the value is defined
                    Expression.Convert(expression, toEnumType), // Cast to enum
                    GlobalConfiguration.Options.EnumHandling switch
                    {
                        EnumHandling.UseDefault => Expression.Default(toEnumType),
                        EnumHandling.ThrowError => Expression.Throw(Expression.New(typeof(InvalidEnumArgumentException).GetConstructor(new[] { StaticType.String, StaticType.Int32, StaticType.Type }),
                                                                    new Expression[] { Expression.Constant("value"), Expression.Convert(expression, StaticType.Int32), Expression.Constant(toEnumType) }),
                            toEnumType
                        ),
                        // MA0015: this validates the global GlobalConfiguration.Options.EnumHandling setting, not a parameter of this method, so the parameter-name overload does not apply here.
#pragma warning disable MA0015 // Specify the parameter name in ArgumentException
                        _ => throw new InvalidEnumArgumentException("EnumHandling set to invalid value")
#pragma warning restore MA0015

                    }); // Default value for undefined
            }
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
            var underlyingType = TypeCache.Get(toType).GetUnderlyingType();
            if (underlyingType == StaticType.String || underlyingType == StaticType.Boolean)
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
            var method = typeof(System.Convert).GetMethod("ToString", new[] { StaticType.Object });

            // Variables
            var isNullExpression = (Expression)null;
            var trueExpression = (Expression)null;

            // Ensure (Ref/Nullable)
            if (TypeCache.Get(expression.Type).IsNullable())
            {
                // Check
                isNullExpression = Expression.Equal(Expression.Constant(null), expression);

                // True
                trueExpression = Expression.Convert(Expression.Constant(null), StaticType.String);
            }

            // False
            var methodCallExpression = Expression.Call(method, ConvertExpressionToTypeExpression(expression, StaticType.Object));
            Expression falseExpression = ConvertExpressionToTypeExpression(methodCallExpression, StaticType.String);

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
            var cachedType = TypeCache.Get(expression.Type);
            if (cachedType.IsNullable())
            {
                isNullExpression = Expression.Equal(Expression.Constant(null), expression);
                trueExpression = GetNullableTypeExpression(toType);
            }

            // Casting
            var sourceUnderlyingType = cachedType.GetUnderlyingType();
            var targetUnderlyingType = TypeCache.Get(toType).GetUnderlyingType();
            if (sourceUnderlyingType != targetUnderlyingType)
            {
                if (sourceUnderlyingType?.IsEnum == true && Enum.GetUnderlyingType(sourceUnderlyingType) != targetUnderlyingType)
                {
                    // Expression.Convert() only recognizes a direct coercion between an enum and its
                    // own underlying integral type (e.g. Hands -> Int32); there is no built-in (nor
                    // user-defined) coercion from an enum straight to an unrelated numeric type like
                    // Decimal/Double/Single, even though a plain C# cast silently chains the two
                    // conversions for you. Chain them explicitly: enum -> its underlying integral type
                    // -> the actually requested toType. Without this, binding an enum-typed property
                    // to e.g. a NUMBER/DECIMAL column throws "No coercion operator is defined between
                    // types 'TEnum' and 'Decimal'".
                    var enumUnderlyingExpression = ConvertExpressionToTypeExpression(expression, Enum.GetUnderlyingType(sourceUnderlyingType));
                    falseExpression = ConvertExpressionToTypeExpression(enumUnderlyingExpression, toType);
                }
                else
                {
                    falseExpression = ConvertExpressionToTypeExpression(expression, toType);
                }
            }

            // Nullable
            if (cachedType.IsNullable())
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
        /// <returns></returns>
        internal static Expression ConvertExpressionToDbNullExpression(Expression expression)
        {
            var valueIsNullExpression = Expression.Equal(expression, Expression.Constant(null));
            var dbNullValueExpresion = ConvertExpressionToTypeExpression(Expression.Constant(DBNull.Value), StaticType.Object);
            return Expression.Condition(valueIsNullExpression, dbNullValueExpresion, expression);
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
            if (!expression.Type.IsValueType)
            {
                return expression;
            }

            var underlyingType = Nullable.GetUnderlyingType(expression.Type);
            targetNullableType = TypeCache.Get(targetNullableType).GetUnderlyingType();

            if (targetNullableType.IsValueType && (underlyingType == null || underlyingType != targetNullableType))
            {
                // Equivalent to 'new Nullable<T>(value)'
                var nullableType = targetNullableType.MakeNullableType();
                expression = TypeCache.Get(expression.Type).IsNullable() ? expression :
                    Expression.Convert(ConvertExpressionToTypeExpression(expression, targetNullableType), nullableType);
            }

            return expression;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="trueToType"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionWithAutomaticConversion(Expression expression,
            Type trueToType)
        {
            var fromType = TypeCache.Get(expression.Type).GetUnderlyingType();
            var toType = TypeCache.Get(trueToType)?.GetUnderlyingType();

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
#if NET6_0_OR_GREATER
            // DateTime to DateOnly
            else if (fromType == StaticType.DateTime && toType == StaticType.DateOnly)
            {
                expression = ConvertExpressionToDateTimeToDateOnlyExpression(expression);
            }

            // DateOnly to DateTime
            else if (fromType == StaticType.DateOnly && toType == StaticType.DateTime)
            {
                expression = ConvertExpressionToDateOnlyToDateTimeExpression(expression);
            }
#endif
            // Others
            else
            {
                expression = ConvertExpressionToSystemConvertExpression(expression, trueToType);
            }

            // Return
            return expression;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="readerExpression"></param>
        /// <param name="handlerInstance"></param>
        /// <param name="classPropertyParameterInfo"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToPropertyHandlerGetExpression(Expression expression,
            Expression readerExpression,
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
                CreatePropertyHandlerGetOptionsExpression(readerExpression, classPropertyParameterInfo?.ClassProperty)
            });

            // Convert to the return type
            return ConvertExpressionToTypeExpression(expression, getMethod.ReturnType);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="entityExpression"></param>
        /// <param name="readerParameterExpression"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToClassHandlerGetExpression<[DynamicallyAccessedMembers(Trimming.Entity)] TResult>(Expression entityExpression,
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
            var getMethod = GetClassHandlerGetMethod(handlerInstance, typeOfResult);
            if (getMethod == null)
            {
                throw new InvalidTypeException($"The class handler '{handlerInstance.GetType().FullName}' cannot be used for the type '{typeOfResult.FullName}'.");
            }

            // Call the ClassHandler.Get method
            return Expression.Call(Expression.Constant(handlerInstance),
                getMethod,
                entityExpression,
                CreateClassHandlerGetOptionsExpression(readerParameterExpression));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="parameterExpression"></param>
        /// <param name="classProperty"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToPropertyHandlerSetExpression(Expression expression,
            Expression parameterExpression,
            ClassProperty classProperty,
            Type targetType)
        {
            return ConvertExpressionToPropertyHandlerSetExpressionTuple(expression, parameterExpression, classProperty, targetType).convertedExpression;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="parameterExpression"></param>
        /// <param name="classProperty"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        internal static (Expression convertedExpression, Type handlerSetReturnType) ConvertExpressionToPropertyHandlerSetExpressionTuple(Expression expression,
            Expression parameterExpression,
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
            expression = ConvertExpressionToNullableExpression(expression,
                TypeCache.Get(setParameter.ParameterType).GetUnderlyingType() ?? targetType);

            // Call
            var valueExpression = ConvertExpressionToTypeExpression(expression, setParameter.ParameterType);
            expression = Expression.Call(Expression.Constant(handlerInstance),
                setMethod,
                new[]
                {
                    valueExpression,
                    CreatePropertyHandlerSetOptionsExpression(parameterExpression,classProperty)
                });

            // Align
            return (ConvertExpressionToTypeExpression(expression, setMethod.ReturnType), setMethod.ReturnType);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="commandExpression"></param>
        /// <param name="resultType"></param>
        /// <param name="entityOrEntitiesExpression"></param>
        /// <returns></returns>
        internal static Expression ConvertExpressionToClassHandlerSetExpression(Expression commandExpression,
            Type resultType,
            Expression entityOrEntitiesExpression)
        {
            // Check the handler
            var handlerInstance = GetClassHandler(resultType);
            if (handlerInstance == null)

            {
                return entityOrEntitiesExpression;
            }

            // Validate
            var setMethod = GetClassHandlerSetMethod(handlerInstance, resultType);
            if (setMethod == null)
            {
                throw new InvalidTypeException($"The class handler '{handlerInstance.GetType().FullName}' cannot be used for type '{resultType.FullName}'.");
            }

            // Call the IClassHandler.Set method
            entityOrEntitiesExpression = Expression.Call(Expression.Constant(handlerInstance),
                setMethod,
                ConvertExpressionToTypeExpression(entityOrEntitiesExpression, resultType),
                CreateClassHandlerSetOptionsExpression(commandExpression));

            // Return the block
            return entityOrEntitiesExpression;
        }

        #endregion

        #region Common

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="enumType"></param>
        /// <returns></returns>
        internal static Expression GetEnumIsDefinedExpression(Expression expression,
            Type enumType)
        {
            var parameters = new Expression[]
            {
                Expression.Constant(enumType),
                ConvertExpressionToTypeExpression(expression, StaticType.Object)
            };
            return Expression.Call(GetEnumIsDefinedMethod(), parameters);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="enumType"></param>
        /// <returns></returns>
        internal static Expression GetEnumGetNameExpression(Expression expression,
            Type enumType)
        {
            var parameters = new Expression[]
            {
                Expression.Constant(enumType),
                ConvertExpressionToTypeExpression(expression, StaticType.Object)
            };
            return Expression.Call(GetEnumGetNameMethod(), parameters);
        }

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
                typeof(System.Data.Common.DbDataReader).GetMethod("IsDBNull"), Expression.Constant(readerField.Ordinal));

            // True Expression
            var trueExpression = GetClassPropertyParameterInfoIsDbNullTrueValueExpression(readerParameterExpression,
                classPropertyParameterInfo, readerField);

            // Set the value
            return Expression.Condition(isDbNullExpression, trueExpression, falseExpression);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="readerExpression"></param>
        /// <param name="classPropertyParameterInfo"></param>
        /// <param name="readerField"></param>
        /// <returns></returns>
        internal static Expression GetClassPropertyParameterInfoIsDbNullTrueValueExpression(Expression readerExpression,
            ClassPropertyParameterInfo classPropertyParameterInfo,
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
                valueExpression = ConvertExpressionToPropertyHandlerGetExpression(valueExpression, readerExpression, handlerInstance, classPropertyParameterInfo);
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
            var targetTypeUnderlyingType = TypeCache.Get(targetType).GetUnderlyingType();
            var isAutomaticConversion = GlobalConfiguration.Options.ConversionType == ConversionType.Automatic ||
                targetTypeUnderlyingType == StaticType.TimeSpan ||
#if NET6_0_OR_GREATER
                targetTypeUnderlyingType == StaticType.DateOnly ||
#endif
                /* SQLite: Guid/String (Vice-Versa) : Enforce automatic conversion for the Primary/Identity fields */
                readerField.DbField?.IsPrimary == true || readerField.DbField?.IsIdentity == true;

            // get handler on class property or type level
            var handlerInstance = GetHandlerInstance(classPropertyParameterInfo, readerField) ?? PropertyHandlerCache.Get<object>(classPropertyParameterInfo.GetTargetType());

            // Enumerations
            if (targetTypeUnderlyingType.IsEnum)
            {
                // If it has a PropertyHandler and the parameter type is matching, then, skip the auto conversion.
                var autoConvertEnum = true;
                if (handlerInstance != null)
                {
                    var getParameter = GetPropertyHandlerGetParameter(GetPropertyHandlerGetMethod(handlerInstance));
                    autoConvertEnum = !(TypeCache.Get(getParameter.ParameterType).GetUnderlyingType() == readerField.Type);
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
            else
            {
                // Auto-conversion
                if (isAutomaticConversion)
                {
                    try
                    {
                        valueExpression = ConvertExpressionWithAutomaticConversion(valueExpression, targetType);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Compiler.DataReader.IsDbNull.FalseExpression: Failed to automatically convert the value expression. " +
                            $"{classPropertyParameterInfo.GetDescriptiveContextString()}", ex);
                    }
                }
                // String to numeric (no coercion exists): some providers (e.g. Ahtola) report NUMERIC/DECIMAL columns as String
                else if (readerField.Type == StaticType.String && IsNumericType(targetTypeUnderlyingType))
                {
                    valueExpression = Expression.Convert(
                        Expression.Call(GetConvertChangeTypeWithProviderMethod(),
                            Expression.Call(readerParameterExpression, GetDbReaderGetValueMethod(), Expression.Constant(readerField.Ordinal)),
                            Expression.Constant(targetTypeUnderlyingType, StaticType.Type),
                            Expression.Constant(CultureInfo.InvariantCulture, typeof(IFormatProvider))),
                        targetTypeUnderlyingType);
                }
            }

            // Property Handler
            try
            {
                valueExpression = ConvertExpressionToPropertyHandlerGetExpression(
                    valueExpression, readerParameterExpression, handlerInstance, classPropertyParameterInfo);
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
        internal static Expression GetNullableTypeExpression(Type targetType)
        {
            // Equivalent to 'new Nullable<T>()'
            return Expression.Default(TypeCache.Get(targetType).GetUnderlyingType().MakeNullableType());
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="readerFieldsName"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static IEnumerable<ClassPropertyParameterInfo> GetClassPropertyParameterInfos<[DynamicallyAccessedMembers(Trimming.Entity)] TResult>(IEnumerable<string> readerFieldsName,
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
                        string.Equals(field.AsUnquoted(trim: true, dbSetting), property.GetMappedName().AsUnquoted(trim: true, dbSetting), StringComparison.OrdinalIgnoreCase)) != null)
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
                            ClassProperty = classProperty, //classProperty.PropertyInfo.CanWrite ? classProperty : null,
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
        internal static IEnumerable<MemberBinding> GetMemberBindingsForDataEntity<[DynamicallyAccessedMembers(Trimming.Entity)] TResult>(ParameterExpression readerParameterExpression,
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
                var mappedName = classPropertyParameterInfo.ParameterInfoMappedClassProperty?.GetMappedName().AsUnquoted(trim: true, dbSetting) ??
                    classPropertyParameterInfo.ParameterInfo?.Name.AsUnquoted(trim: true, dbSetting) ??
                    classPropertyParameterInfo.ClassProperty?.GetMappedName().AsUnquoted(trim: true, dbSetting);

                // Skip if not found
                var ordinal = readerFieldsName.IndexOf(mappedName?.ToLowerInvariant());
                if (ordinal < 0)
                {
                    continue;
                }

                // Get the value expression
                var readerField = readerFields.First(f => string.Equals(f.Name.AsUnquoted(trim: true, dbSetting), mappedName.AsUnquoted(trim: true, dbSetting), StringComparison.OrdinalIgnoreCase));
                var expression = GetClassPropertyParameterInfoValueExpression(readerParameterExpression,
                    classPropertyParameterInfo, readerField);

                try
                {
                    // Member values
                    var memberAssignment = classPropertyParameterInfo.ClassProperty?.PropertyInfo?.CanWrite == true ?
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
            int ordinal)
        {
            return GetDbNullExpression(readerParameterExpression, Expression.Constant(ordinal));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="readerParameterExpression"></param>
        /// <param name="ordinalExpression"></param>
        /// <returns></returns>
        internal static Expression GetDbNullExpression(ParameterExpression readerParameterExpression,
            ConstantExpression ordinalExpression)
        {
            return Expression.Call(readerParameterExpression, typeof(System.Data.Common.DbDataReader).GetMethod("IsDBNull"), ordinalExpression);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="readerParameterExpression"></param>
        /// <param name="readerGetValueMethod"></param>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbReaderGetValueExpression(ParameterExpression readerParameterExpression,
            MethodInfo readerGetValueMethod,
            int ordinal)
        {
            return GetDbReaderGetValueExpression(readerParameterExpression, readerGetValueMethod, Expression.Constant(ordinal));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="readerParameterExpression"></param>
        /// <param name="readerGetValueMethod"></param>
        /// <param name="ordinalExpression"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbReaderGetValueExpression(ParameterExpression readerParameterExpression,
            MethodInfo readerGetValueMethod,
            ConstantExpression ordinalExpression)
        {
            return Expression.Call(readerParameterExpression, readerGetValueMethod, ordinalExpression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal static MethodInfo GetMaterializeDictionaryValueMethod()
        {
            return typeof(Compiler).GetMethod(nameof(MaterializeDictionaryValue), BindingFlags.Static | BindingFlags.NonPublic);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object MaterializeDictionaryValue(object value)
        {
            if (value is System.IO.Stream stream)
            {
                using var memoryStream = new System.IO.MemoryStream();
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
            return value;
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
            var addMethod = typeof(IDictionary<string, object>).GetMethod("Add", new[] { StaticType.String, StaticType.Object });

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
                    Expression.Call(GetMaterializeDictionaryValueMethod(),
                        ConvertExpressionToTypeExpression(expression, StaticType.Object))
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
            var handlerInstance = classProperty.GetPropertyHandler() ?? PropertyHandlerCache.Get<object>(TypeCache.Get(dbField.Type).GetUnderlyingType());
            var targetType = GetPropertyHandlerSetParameter(handlerInstance)?.ParameterType ?? dbField.TypeNullable();

            /*
             * Note: The other data provider can coerce the Enum into its destination data type in the DB by default,
             *       except for PostgreSQL. The code written below is only to address the issue for this specific provider.
             */

            // Enum Handling
            if (TypeCache.Get(classProperty.PropertyInfo.PropertyType).GetUnderlyingType().IsEnum)
            {
                try
                {
                    if (!IsPostgreSqlUserDefined(dbField))
                    {
                        var enumType = TypeCache.Get(classProperty.PropertyInfo.PropertyType).GetUnderlyingType();
                        var dbType = classProperty.GetDbType() ?? enumType.GetDbType();
                        var toType = dbType.HasValue ? new DbTypeToClientTypeResolver().Resolve(dbType.Value) : targetType;

                        expression = ConvertEnumExpressionToTypeExpression(expression, toType);
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Compiler.Entity/Object.Property: Failed to convert the value expression from " +
                        $"enumeration '{classProperty.PropertyInfo.PropertyType.FullName}' to type '{targetType?.GetUnderlyingType()}'. {classProperty}", ex);
                }
            }

            // Auto-conversion Handling
            if (GlobalConfiguration.Options.ConversionType == ConversionType.Automatic || dbField?.IsPrimary == true || dbField?.IsIdentity == true)
            {
                try
                {
                    var origExpression = expression;
                    expression = ConvertExpressionWithAutomaticConversion(expression, targetType);

                    if (dbField?.IsIdentity == true
                        && targetType.IsValueType && TypeCache.Get(targetType).GetUnderlyingType() == targetType
                        && TypeCache.Get(origExpression.Type).GetUnderlyingType() != origExpression.Type)
                    {
                        var nullableType = expression.Type.MakeNullableType();

                        // Don't set '0' in the identity output property
                        expression = Expression.Condition(
                            GetNullableHasValueExpression(origExpression),
                            Expression.Convert(expression, nullableType),
                            Expression.Constant(null, nullableType));
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Compiler.Entity/Object.Property: Failed to automatically convert the value expression for " +
                        $"property '{classProperty.GetMappedName()} ({classProperty.PropertyInfo.PropertyType.FullName})'. {classProperty}", ex);
                }
            }

            // Property Handler
            try
            {
                expression = ConvertExpressionToPropertyHandlerSetExpression(
                    expression, parameterExpression: null, classProperty, TypeCache.Get(dbField?.Type).GetUnderlyingType());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Compiler.Entity/Object.Property: Failed to convert the value expression for property handler '{handlerInstance?.GetType()}'. " +
                    $"{classProperty}", ex);
            }

            // Return the Value
            return ConvertExpressionToTypeExpression(expression, StaticType.Object);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbField"></param>
        /// <returns></returns>
        private static bool IsPostgreSqlUserDefined(DbField dbField)
        {
            return string.Equals(dbField?.DatabaseType, "USER-DEFINED", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(dbField?.Provider, "PGSQL", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="size"></param>
        /// <param name="dbField"></param>
        private static int GetSize(int? size,
            DbField dbField)
        {
            return size.HasValue ? size.Value :
                 dbField?.Size.HasValue == true ? dbField.Size.Value : default;
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
            var methodInfo = typeof(System.Reflection.PropertyInfo).GetMethod("GetValue", new[] { StaticType.Object });
            var expression = (Expression)Expression.Call(propertyExpression, methodInfo, objectInstanceExpression);

            // Property Handler
            expression = ConvertExpressionToPropertyHandlerSetExpression(expression,
parameterExpression: null, classProperty: null, TypeCache.Get(dbField?.Type).GetUnderlyingType());

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
            var methodInfo = typeof(IDictionary<string, object>).GetMethod("get_Item", new[] { StaticType.String });
            var expression = (Expression)Expression.Call(dictionaryInstanceExpression, methodInfo, Expression.Constant(dbField.Name));

            // Property Handler
            expression = ConvertExpressionToPropertyHandlerSetExpression(expression,
parameterExpression: null, classProperty: null, TypeCache.Get(dbField.Type).GetUnderlyingType());

            // Convert to object
            return ConvertExpressionToTypeExpression(expression, StaticType.Object);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbParameterExpression"></param>
        /// <param name="entityExpression"></param>
        /// <param name="propertyExpression"></param>
        /// <param name="classProperty"></param>
        /// <param name="dbField"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static Expression GetDataEntityDbParameterValueAssignmentExpression(ParameterExpression dbParameterExpression,
            Expression entityExpression,
            ParameterExpression propertyExpression,
            ClassProperty classProperty,
            DbField dbField,
            IDbSetting dbSetting)
        {
            Expression expression;

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
                expression = ConvertExpressionToDbNullExpression(expression);
            }

            // Set the value
            return Expression.Call(dbParameterExpression, GetDbParameterValueSetMethod(), expression);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbParameterExpression"></param>
        /// <param name="dictionaryInstanceExpression"></param>
        /// <param name="dbField"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static Expression GetDictionaryStringObjectDbParameterValueAssignmentExpression(ParameterExpression dbParameterExpression,
            Expression dictionaryInstanceExpression,
            DbField dbField,
            IDbSetting dbSetting)
        {
            var expression = GetDictionaryStringObjectPropertyValueExpression(dictionaryInstanceExpression, dbField);

            // Nullable
            if (dbField?.IsNullable == true)
            {
                expression = ConvertExpressionToDbNullExpression(expression);
            }

            // Set the value
            return Expression.Call(dbParameterExpression, GetDbParameterValueSetMethod(), expression);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="classProperty"></param>
        /// <param name="dbField"></param>
        /// <returns></returns>
        private static DbType? GetDbType(ClassProperty classProperty,
            DbField dbField)
        {
            var dbType = IsPostgreSqlUserDefined(dbField) ? DbType.Object : classProperty?.GetDbType();
            if (dbType == null)
            {
                var underlyingType = TypeCache.Get(dbField?.Type)?.GetUnderlyingType();
                dbType = TypeMapper.Get(underlyingType) ?? new ClientTypeToDbTypeResolver().Resolve(underlyingType);
            }
            return dbType;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbParameterExpression"></param>
        /// <param name="classProperty"></param>
        /// <param name="dbField"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbParameterDbTypeAssignmentExpression(ParameterExpression dbParameterExpression,
            ClassProperty classProperty,
            DbField dbField)
        {
            return GetDbParameterDbTypeAssignmentExpression(dbParameterExpression, GetDbType(classProperty, dbField));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbParameterExpression"></param>
        /// <param name="dbField"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbParameterDbTypeAssignmentExpression(ParameterExpression dbParameterExpression,
            DbField dbField)
        {
            return GetDbParameterDbTypeAssignmentExpression(dbParameterExpression, GetDbType(classProperty: null, dbField));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbParameterExpression"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbParameterDbTypeAssignmentExpression(ParameterExpression dbParameterExpression,
            DbType? dbType)
        {
            var expression = (MethodCallExpression)null;

            // Set the DB Type
            if (dbType != null)
            {
                var dbParameterDbTypeSetMethod = typeof(System.Data.Common.DbParameter).GetProperty("DbType").SetMethod;
                expression = Expression.Call(dbParameterExpression, dbParameterDbTypeSetMethod, Expression.Constant(dbType));
            }

            // Return the expression
            return expression;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbCommandExpression"></param>
        /// <param name="dbField"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbCommandCreateParameterExpression(ParameterExpression dbCommandExpression,
            DbField dbField)
        {
            var dbCommandCreateParameterMethod = typeof(System.Data.Common.DbCommand).GetMethod("CreateParameter");
            return Expression.Call(dbCommandExpression, dbCommandCreateParameterMethod);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbParameterExpression"></param>
        /// <param name="dbField"></param>
        /// <param name="entityIndex"></param>
        /// <param name="dbSetting"></param>
        internal static MethodCallExpression GetDbParameterNameAssignmentExpression(Expression dbParameterExpression,
            DbField dbField,
            int entityIndex,
            IDbSetting dbSetting)
        {
            var parameterName = dbField.Name.AsUnquoted(trim: true, dbSetting).AsAlphaNumeric();
            parameterName = entityIndex > 0 ? string.Concat(dbSetting.ParameterPrefix, parameterName, "_", entityIndex.ToString(System.Globalization.CultureInfo.InvariantCulture)) :
                string.Concat(dbSetting.ParameterPrefix, parameterName);
            return GetDbParameterNameAssignmentExpression(dbParameterExpression, parameterName);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbParameterExpression"></param>
        /// <param name="parameterName"></param>
        internal static MethodCallExpression GetDbParameterNameAssignmentExpression(Expression dbParameterExpression,
            string parameterName)
        {
            return GetDbParameterNameAssignmentExpression(dbParameterExpression, Expression.Constant(parameterName));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbParameterExpression"></param>
        /// <param name="paramaterNameExpression"></param>
        internal static MethodCallExpression GetDbParameterNameAssignmentExpression(Expression dbParameterExpression,
            Expression paramaterNameExpression)
        {
            var dbParameterValueNameMethod = typeof(System.Data.Common.DbParameter).GetProperty("ParameterName").SetMethod;
            return Expression.Call(dbParameterExpression, dbParameterValueNameMethod, paramaterNameExpression);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbParameterExpression"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbParameterValueAssignmentExpression(Expression dbParameterExpression,
            object value)
        {
            return GetDbParameterValueAssignmentExpression(dbParameterExpression, Expression.Constant(value));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbParameterExpression"></param>
        /// <param name="valueExpression"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbParameterValueAssignmentExpression(Expression dbParameterExpression,
            Expression valueExpression)
        {
            var parameterExpression = ConvertExpressionToTypeExpression(dbParameterExpression, StaticType.DbParameter);
            var dbParameterValueSetMethod = typeof(System.Data.Common.DbParameter).GetProperty("Value").SetMethod;
            var convertToDbNullMethod = typeof(RepoDb.Converter).GetMethod("NullToDbNull");
            return Expression.Call(parameterExpression, dbParameterValueSetMethod,
                Expression.Call(convertToDbNullMethod, ConvertExpressionToTypeExpression(valueExpression, StaticType.Object)));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbParameterExpression"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbParameterDbTypeAssignmentExpression(Expression dbParameterExpression,
            DbType dbType)
        {
            return GetDbParameterDbTypeAssignmentExpression(dbParameterExpression, Expression.Constant(dbType));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbParameterExpression"></param>
        /// <param name="dbTypeExpression"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbParameterDbTypeAssignmentExpression(Expression dbParameterExpression,
            Expression dbTypeExpression)
        {
            var parameterExpression = ConvertExpressionToTypeExpression(dbParameterExpression, StaticType.DbParameter);
            var dbParameterDbTypeSetMethod = typeof(System.Data.Common.DbParameter).GetProperty("DbType").SetMethod;
            return Expression.Call(parameterExpression, dbParameterDbTypeSetMethod, dbTypeExpression);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbParameterExpression"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbParameterDirectionAssignmentExpression(Expression dbParameterExpression,
            ParameterDirection direction)
        {
            return GetDbParameterDirectionAssignmentExpression(dbParameterExpression, Expression.Constant(direction));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbParameterExpression"></param>
        /// <param name="directionExpression"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbParameterDirectionAssignmentExpression(Expression dbParameterExpression,
            Expression directionExpression)
        {
            var parameterExpression = ConvertExpressionToTypeExpression(dbParameterExpression, StaticType.DbParameter);
            var dbParameterDirectionSetMethod = typeof(System.Data.Common.DbParameter).GetProperty("Direction").SetMethod;
            return Expression.Call(parameterExpression, dbParameterDirectionSetMethod, directionExpression);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbParameterExpression"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbParameterSizeAssignmentExpression(Expression dbParameterExpression,
            int size)
        {
            return GetDbParameterSizeAssignmentExpression(dbParameterExpression, Expression.Constant(size));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbParameterExpression"></param>
        /// <param name="sizeExpression"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbParameterSizeAssignmentExpression(Expression dbParameterExpression,
            Expression sizeExpression)
        {
            var parameterExpression = ConvertExpressionToTypeExpression(dbParameterExpression, StaticType.DbParameter);
            var dbParameterSizeSetMethod = typeof(System.Data.Common.DbParameter).GetProperty("Size").SetMethod;
            return Expression.Call(parameterExpression, dbParameterSizeSetMethod, sizeExpression);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbParameterExpression"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbParameterPrecisionAssignmentExpression(Expression dbParameterExpression,
            byte precision)
        {
            return GetDbParameterPrecisionAssignmentExpression(dbParameterExpression, Expression.Constant(precision));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbParameterExpression"></param>
        /// <param name="precisionExpression"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbParameterPrecisionAssignmentExpression(Expression dbParameterExpression,
            Expression precisionExpression)
        {
            var parameterExpression = ConvertExpressionToTypeExpression(dbParameterExpression, StaticType.DbParameter);
            var dbParameterPrecisionSetMethod = typeof(System.Data.Common.DbParameter).GetProperty("Precision").SetMethod;
            return Expression.Call(parameterExpression, dbParameterPrecisionSetMethod, precisionExpression);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbParameterExpression"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbParameterScaleAssignmentExpression(Expression dbParameterExpression,
            byte scale)
        {
            return GetDbParameterScaleAssignmentExpression(dbParameterExpression, Expression.Constant(scale));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbParameterExpression"></param>
        /// <param name="scaleExpression"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbParameterScaleAssignmentExpression(Expression dbParameterExpression,
            Expression scaleExpression)
        {
            var parameterExpression = ConvertExpressionToTypeExpression(dbParameterExpression, StaticType.DbParameter);
            var dbParameterScaleSetMethod = typeof(System.Data.Common.DbParameter).GetProperty("Scale").SetMethod;
            return Expression.Call(parameterExpression, dbParameterScaleSetMethod, scaleExpression);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbParameterExpression"></param>
        /// <returns></returns>
        internal static MethodCallExpression EnsureTableValueParameterExpression(Expression dbParameterExpression)
        {
            var method = typeof(RepoDb.Extensions.DbCommandExtension).GetMethod("EnsureTableValueParameter",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            return Expression.Call(method, dbParameterExpression);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbCommandExpression"></param>
        /// <param name="dbParameterExpression"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbCommandParametersAddExpression(Expression dbCommandExpression,
            Expression dbParameterExpression)
        {
            var dbCommandParametersProperty = typeof(System.Data.Common.DbCommand).GetProperty("Parameters");
            var dbParameterCollection = Expression.Property(dbCommandExpression, dbCommandParametersProperty);
            var dbParameterCollectionAddMethod = typeof(System.Data.Common.DbParameterCollection).GetMethod("Add", new[] { StaticType.Object });
            return Expression.Call(dbParameterCollection, dbParameterCollectionAddMethod, dbParameterExpression);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbParameterCollectionExpression"></param>
        /// <returns></returns>
        internal static Expression GetDbParameterCollectionClearMethodExpression(MemberExpression dbParameterCollectionExpression)
        {
            var dbParameterCollectionClearMethod = typeof(System.Data.Common.DbParameterCollection).GetMethod("Clear");
            return Expression.Call(dbParameterCollectionExpression, dbParameterCollectionClearMethod);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbCommandExpression"></param>
        /// <param name="entityExpression"></param>
        /// <param name="fieldDirection"></param>
        /// <param name="entityIndex"></param>
        /// <param name="dbSetting"></param>
        /// <param name="dbHelper"></param>
        /// <returns></returns>
        internal static Expression GetPropertyFieldExpression(ParameterExpression dbCommandExpression,
            [DynamicallyAccessedMembers(Trimming.Entity)] Type entityType,
            Expression entityExpression,
            FieldDirection fieldDirection,
            int entityIndex,
            IDbSetting dbSetting,
            IDbHelper dbHelper)
        {
            var propertyListExpression = new List<Expression>();
            var propertyVariableListExpression = new List<ParameterExpression>();
            var propertyVariableExpression = (ParameterExpression)null;
            var propertyInstanceExpression = (Expression)null;
            var classProperty = (ClassProperty)null;
            var propertyName = fieldDirection.DbField.Name.AsUnquoted(trim: true, dbSetting);

            // Set the proper assignments (property)
            if (!TypeCache.Get(entityType).IsClassType())
            {
                propertyVariableExpression = Expression.Variable(StaticType.PropertyInfo, string.Concat("propertyVariable", propertyName));
                propertyInstanceExpression = Expression.Call(GetRuntimePropertyMethod(),
                    ConvertExpressionToTypeExpression(entityExpression, StaticType.Object),
                    Expression.Constant(propertyName));
            }
            else
            {
                var entityProperties = PropertyCache.Get(entityType);
                classProperty = entityProperties.FirstOrDefault(property =>
                    string.Equals(property.GetMappedName().AsUnquoted(trim: true, dbSetting),
                        propertyName.AsUnquoted(trim: true, dbSetting), StringComparison.OrdinalIgnoreCase));

                if (classProperty != null)
                {
                    propertyVariableExpression = Expression.Variable(classProperty.PropertyInfo.PropertyType, string.Concat("propertyVariable", propertyName));
                    propertyInstanceExpression = Expression.Property(entityExpression, classProperty.PropertyInfo);
                }
                else
                {
                    throw new PropertyNotFoundException($"The property '{propertyName}' is not found from type '{entityExpression.Type}'. The current operation could not proceed.");
                }
            }

            // Add the variables
            if (propertyVariableExpression != null && propertyInstanceExpression != null)
            {
                propertyVariableListExpression.Add(propertyVariableExpression);
                propertyListExpression.Add(Expression.Assign(propertyVariableExpression, propertyInstanceExpression));

                // Execute the function
                var parameterAssignment = GetDataEntityParameterAssignmentExpression(dbCommandExpression,
                    entityIndex,
                    entityExpression,
                    propertyVariableExpression,
                    fieldDirection.DbField,
                    classProperty,
                    fieldDirection.Direction,
                    dbSetting,
                    dbHelper);
                propertyListExpression.Add(parameterAssignment);
            }

            // Add the property block
            return Expression.Block(propertyVariableListExpression, propertyListExpression);
        }

        /// <summary>
        /// Returns the <see cref="GetRuntimeProperty(object, string)"/> method.
        /// </summary>
        /// <returns>The method.</returns>
        private static MethodInfo GetRuntimePropertyMethod() =>
            typeof(Compiler).GetMethod(nameof(GetRuntimeProperty), BindingFlags.Static | BindingFlags.NonPublic);

        /// <summary>
        /// Returns the public instance property (case insensitive) of the runtime type of the (non-class typed) entity.
        /// </summary>
        /// <param name="entity">The entity object.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The property, or null if not found.</returns>
        [UnconditionalSuppressMessage("Trimming", "IL2075:'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method.",
            Justification = "The entities without a static class type are only passed to the object-based operations, which are annotated with RequiresUnreferencedCode.")]
        private static PropertyInfo GetRuntimeProperty(object entity,
            string propertyName) =>
            entity?.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

        /// <summary>
        /// Returns an expression that converts the (object) value into the target type via the non-generic
        /// 'Converter.ToType(object, Type)' method (equivalent to the generic 'Converter.ToType&lt;T&gt;(object)' method).
        /// </summary>
        /// <param name="valueExpression">The value expression (of type object).</param>
        /// <param name="type">The target (non-nullable) type.</param>
        /// <returns>The conversion expression.</returns>
        internal static Expression GetConverterToTypeExpression(Expression valueExpression,
            Type type)
        {
            var method = typeof(Converter).GetMethod(nameof(Converter.ToType), BindingFlags.Static | BindingFlags.NonPublic,
                binder: null, new[] { StaticType.Object, StaticType.Type }, modifiers: null);
            var convertedExpression = Expression.Call(method, valueExpression, Expression.Constant(type, StaticType.Type));
            if (!type.IsValueType)
            {
                return Expression.Convert(convertedExpression, type);
            }
            var variable = Expression.Variable(StaticType.Object, "converted");
            return Expression.Block(type, new[] { variable },
                Expression.Assign(variable, convertedExpression),
                Expression.Condition(Expression.Equal(variable, Expression.Constant(null)),
                    Expression.Default(type),
                    Expression.Convert(variable, type)));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbCommandExpression"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbCommandParametersClearExpression(ParameterExpression dbCommandExpression)
        {
            var dbParameterCollection = Expression.Property(dbCommandExpression,
                typeof(System.Data.Common.DbCommand).GetProperty("Parameters"));
            return Expression.Call(dbParameterCollection, typeof(System.Data.Common.DbParameterCollection).GetMethod("Clear"));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="entitiesParameterExpression"></param>
        /// <param name="typeOfListEntity"></param>
        /// <param name="entityIndex"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetListEntityIndexerExpression(Expression entitiesParameterExpression,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] Type typeOfListEntity,
            int entityIndex)
        {
            var listIndexerMethod = typeOfListEntity.GetMethod("get_Item", new[] { StaticType.Int32 });
            return Expression.Call(entitiesParameterExpression, listIndexerMethod,
                Expression.Constant(entityIndex));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="resultType"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static Expression ThrowIfNullAfterClassHandlerExpression(Type resultType,
            Expression expression)
        {
            var isNullExpression = Expression.Equal(Expression.Constant(null), expression);
            var exception = new ArgumentNullException($"Entity of type '{resultType}' must not be null. If you have defined a class handler, please check the 'Set' method.");
            return Expression.IfThen(isNullExpression, Expression.Throw(Expression.Constant(exception)));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="dbCommandExpression"></param>
        /// <param name="entitiesParameterExpression"></param>
        /// <param name="fieldDirections"></param>
        /// <param name="entityIndex"></param>
        /// <param name="dbSetting"></param>
        /// <param name="dbHelper"></param>
        /// <returns></returns>
        private static Expression GetIndexDbParameterSetterExpression([DynamicallyAccessedMembers(Trimming.Entity)] Type entityType,
            ParameterExpression dbCommandExpression,
            Expression entitiesParameterExpression,
            IEnumerable<FieldDirection> fieldDirections,
            int entityIndex,
            IDbSetting dbSetting,
            IDbHelper dbHelper)
        {
            // Get the current instance
            var entityVariableExpression = Expression.Variable(StaticType.Object, "instance");
            var typeOfListEntity = typeof(IList<object>);
            var entityParameter = (Expression)GetListEntityIndexerExpression(entitiesParameterExpression, typeOfListEntity, entityIndex);
            var entityExpressions = new List<Expression>();
            var entityVariables = new List<ParameterExpression>();

            // Class handler
            entityParameter = ConvertExpressionToClassHandlerSetExpression(dbCommandExpression, entityType, entityParameter);

            // Entity instance
            entityVariables.Add(entityVariableExpression);
            entityExpressions.Add(Expression.Assign(entityVariableExpression, entityParameter));

            // Throw if null
            entityExpressions.Add(ThrowIfNullAfterClassHandlerExpression(entityType, entityVariableExpression));

            // Iterate the input fields
            foreach (var fieldDirection in fieldDirections)
            {
                // Add the property block
                var propertyBlock = GetPropertyFieldExpression(dbCommandExpression,
                    entityType,
                    ConvertExpressionToTypeExpression(entityVariableExpression, entityType),
                    fieldDirection,
                    entityIndex,
                    dbSetting,
                    dbHelper);

                // Add to instance expression
                entityExpressions.Add(propertyBlock);
            }

            // Add to the instance block
            return Expression.Block(entityVariables, entityExpressions);
        }

        #endregion
    }
}
