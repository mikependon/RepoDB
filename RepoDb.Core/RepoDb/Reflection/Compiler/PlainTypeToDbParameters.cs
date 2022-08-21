using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Resolvers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace RepoDb.Reflection
{
    internal partial class Compiler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramType"></param>
        /// <param name="entityType"></param>
        /// <param name="dbFields"></param>
        /// <returns></returns>
        internal static Action<DbCommand, object> GetPlainTypeToDbParametersCompiledFunction(Type paramType,
            Type entityType,
            IEnumerable<DbField> dbFields = null)
        {
            var commandParameterExpression = Expression.Parameter(StaticType.DbCommand, "command");
            var entityParameterExpression = Expression.Parameter(StaticType.Object, "entity");
            var entityExpression = ConvertExpressionToTypeExpression(entityParameterExpression, paramType);
            var methodInfo = GetDbCommandCreateParameterMethod();
            var callExpressions = new List<Expression>();

            // Iterate
            foreach (var paramProperty in PropertyCache.Get(paramType))
            {
                // Ensure it matches to atleast one param
                var entityProperty = PropertyCache.Get(entityType)?.FirstOrDefault(e =>
                    string.Equals(e.GetMappedName(), paramProperty.GetMappedName(), StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(e.PropertyInfo.Name, paramProperty.PropertyInfo.Name, StringComparison.OrdinalIgnoreCase));

                // Variables
                var dbField = dbFields?.FirstOrDefault(df =>
                    string.Equals(df.Name, paramProperty.GetMappedName(), StringComparison.OrdinalIgnoreCase));
                var targetProperty = (entityProperty ?? paramProperty);
                var valueExpression = (Expression)Expression.Property(entityExpression, paramProperty.PropertyInfo);
                var parameterExpression = (Expression)null;

                // Add the value itself
                if (StaticType.IDbDataParameter.IsAssignableFrom(targetProperty.PropertyInfo.PropertyType))
                {
                    parameterExpression = valueExpression;

                    // Set the name
                    var dbParameterParameterNameSetMethod = StaticType.DbParameter.GetProperty("ParameterName").SetMethod;
                    var parameterName = targetProperty.PropertyInfo.Name;
                    var setParameterNameExpression = Expression.Call(parameterExpression, dbParameterParameterNameSetMethod,
                        Expression.Constant(parameterName));
                    callExpressions.AddIfNotNull(setParameterNameExpression);
                }
                else
                {
                    var valueType = targetProperty.PropertyInfo.PropertyType.GetUnderlyingType();

                    // Property Handler
                    InvokePropertyHandlerViaExpression(paramProperty, ref valueType, ref valueExpression);

                    // Create
                    if (valueType.IsEnum)
                    {
                        parameterExpression = GetPlainTypeToDbParametersForEnumCompiledFunction(commandParameterExpression,
                            paramProperty,
                            dbField,
                            valueType,
                            valueExpression);
                    }
                    else
                    {
                        parameterExpression = GetPlainTypeToDbParametersForNonEnumCompiledFunction(commandParameterExpression,
                            paramProperty,
                            entityProperty,
                            dbField,
                            valueType,
                            valueExpression);
                    }

                    // Size
                    var size = GetSize(null, dbField);
                    if (size > 0)
                    {
                        var sizeExpression = GetDbParameterSizeAssignmentExpression(parameterExpression, GetSize(null, dbField));
                        callExpressions.AddIfNotNull(sizeExpression);
                    }

                    // Type map attributes
                    var parameterPropertyValueSetterAttributesExpressions = GetParameterPropertyValueSetterAttributesAssignmentExpressions(
                        parameterExpression,
                        (entityProperty ?? paramProperty));
                    callExpressions.AddRangeIfNotNullOrNotEmpty(parameterPropertyValueSetterAttributesExpressions);
                }

                // DbCommand.Parameters.Add
                var parametersExpression = Expression.Property(commandParameterExpression, "Parameters");
                var addExpression = Expression.Call(parametersExpression, GetDbParameterCollectionAddMethod(), parameterExpression);

                // Add
                callExpressions.Add(addExpression);
            }

            // Return
            return Expression
                .Lambda<Action<DbCommand, object>>(Expression.Block(callExpressions), commandParameterExpression, entityParameterExpression)
                .Compile();
        }

        public static Expression GetPlainTypeToDbParametersForNonEnumCompiledFunction(Expression commandParameterExpression,
            ClassProperty paramProperty,
            ClassProperty entityProperty,
            DbField dbField,
            Type valueType,
            Expression valueExpression)
        {
            // Automatic
            if (Converter.ConversionType == ConversionType.Automatic && dbField?.Type != null)
            {
                valueType = dbField.Type.GetUnderlyingType();
                valueExpression = ConvertExpressionWithAutomaticConversion(valueExpression, valueType);
            }

            // DbType
            var dbType = (paramProperty == null ? null : TypeMapCache.Get(paramProperty.GetDeclaringType(), paramProperty.PropertyInfo)) ??
                (entityProperty == null ? null : TypeMapCache.Get(entityProperty.GetDeclaringType(), entityProperty.PropertyInfo));

            valueType ??= dbField?.Type.GetUnderlyingType();
            if (dbType == null && valueType != null)
            {
                var resolver = new ClientTypeToDbTypeResolver();
                dbType =
                    valueType.GetDbType() ??                        // type level, use TypeMapCache
                    resolver.Resolve(valueType) ??                  // type level, primitive mapping
                    (dbField?.Type != null ?
                        resolver.Resolve(dbField?.Type) : null);    // Fallback to the database type
            }
            var dbTypeExpression = dbType == null ? GetNullableTypeExpression(StaticType.DbType) :
                ConvertExpressionToNullableExpression(Expression.Constant(dbType), StaticType.DbType);

            // DbCommandExtension.CreateParameter
            var methodInfo = GetDbCommandCreateParameterMethod();
            return Expression.Call(methodInfo, new Expression[]
            {
                commandParameterExpression,
                Expression.Constant(paramProperty.GetMappedName()),
                ConvertExpressionToTypeExpression(valueExpression, StaticType.Object),
                dbTypeExpression
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandParameterExpression"></param>
        /// <param name="paramProperty"></param>
        /// <param name="dbField"></param>
        /// <param name="valueType"></param>
        /// <param name="valueExpression"></param>
        /// <returns></returns>
        public static Expression GetPlainTypeToDbParametersForEnumCompiledFunction(Expression commandParameterExpression,
            ClassProperty paramProperty,
            DbField dbField,
            Type valueType,
            Expression valueExpression)
        {
            // DbType
            var dbType = IsPostgreSqlUserDefined(dbField) ? default :
                paramProperty.GetDbType() ??
                valueType.GetDbType() ??
                (dbField != null ? new ClientTypeToDbTypeResolver().Resolve(dbField.Type) : null) ??
                (DbType?)Converter.EnumDefaultDatabaseType;

            // DbCommandExtension.CreateParameter
            var methodInfo = GetDbCommandCreateParameterMethod();
            return Expression.Call(methodInfo, new Expression[]
            {
                commandParameterExpression,
                Expression.Constant(paramProperty.GetMappedName()),
                ConvertExpressionToTypeExpression(valueExpression, StaticType.Object),
                ConvertExpressionToNullableExpression(Expression.Constant(dbType), StaticType.DbType)
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classProperty"></param>
        /// <param name="valueType"></param>
        /// <param name="valueExpression"></param>
        public static void InvokePropertyHandlerViaExpression(ClassProperty classProperty,
            ref Type valueType,
            ref Expression valueExpression)
        {
            var (expression, type) = ConvertExpressionToPropertyHandlerSetExpressionTuple(valueExpression, classProperty, valueType);
            if (type != null)
            {
                valueType = type;
                valueExpression = expression;
            }
        }
    }
}
