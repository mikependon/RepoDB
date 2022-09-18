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
                var parameterVariableExpression = Expression.Parameter(StaticType.DbParameter);

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
                    #region Original

                    //var valueType = targetProperty.PropertyInfo.PropertyType.GetUnderlyingType();

                    //// Property Handler
                    //InvokePropertyHandlerViaExpression(paramProperty, ref valueType, ref valueExpression);

                    //// Create
                    //if (valueType.IsEnum)
                    //{
                    //    parameterExpression = GetPlainTypeToDbParametersForEnumCompiledFunction(commandParameterExpression,
                    //        paramProperty,
                    //        dbField,
                    //        valueType,
                    //        valueExpression);
                    //}
                    //else
                    //{
                    //    parameterExpression = GetPlainTypeToDbParametersForNonEnumCompiledFunction(commandParameterExpression,
                    //        paramProperty,
                    //        entityProperty,
                    //        dbField,
                    //        valueType,
                    //        valueExpression);
                    //}

                    //// Size
                    //var size = GetSize(null, dbField);
                    //if (size > 0)
                    //{
                    //    var setSizeExpression = GetDbParameterSizeAssignmentExpression(parameterExpression, size);
                    //    callExpressions.AddIfNotNull(setSizeExpression);
                    //}

                    //// Type map attributes
                    //var parameterPropertyValueSetterAttributesExpressions = GetParameterPropertyValueSetterAttributesAssignmentExpressions(
                    //    parameterExpression,
                    //    (entityProperty ?? paramProperty));
                    //callExpressions.AddRangeIfNotNullOrNotEmpty(parameterPropertyValueSetterAttributesExpressions);

                    #endregion

                    var valueType = targetProperty.PropertyInfo.PropertyType.GetUnderlyingType();

                    // Create parameter
                    parameterExpression = CreateDbParameterExpression(
                        commandParameterExpression, paramProperty.GetMappedName());

                    // Variable
                    callExpressions.Add(Expression.Assign(parameterVariableExpression,
                        ConvertExpressionToTypeExpression(parameterExpression, StaticType.DbParameter)));

                    // Convert
                    if (Converter.ConversionType == ConversionType.Automatic && dbField?.Type != null)
                    {
                        valueType = dbField.Type.GetUnderlyingType();
                        valueExpression = ConvertExpressionWithAutomaticConversion(valueExpression, valueType);
                    }

                    // Property Handler
                    InvokePropertyHandlerViaExpression(parameterVariableExpression, paramProperty, ref valueType, ref valueExpression);

                    // Value
                    var setValueExpression = GetDbParameterValueAssignmentExpression(parameterVariableExpression,
                        valueExpression);
                    callExpressions.AddIfNotNull(setValueExpression);

                    // DbType
                    var setDbTypeExpression = GetDbParameterDbTypeAssignmentExpression(parameterVariableExpression,
                        paramProperty, entityProperty, dbField, valueType);
                    callExpressions.AddIfNotNull(setDbTypeExpression);

                    // Size
                    var size = GetSize(null, dbField);
                    if (size > 0)
                    {
                        var setSizeExpression = GetDbParameterSizeAssignmentExpression(parameterVariableExpression, size);
                        callExpressions.AddIfNotNull(setSizeExpression);
                    }

                    // Type map attributes
                    var parameterPropertyValueSetterAttributesExpressions = GetParameterPropertyValueSetterAttributesAssignmentExpressions(
                        parameterVariableExpression,
                        (entityProperty ?? paramProperty));
                    callExpressions.AddRangeIfNotNullOrNotEmpty(parameterPropertyValueSetterAttributesExpressions);
                }

                // DbCommand.Parameters.Add
                var parametersExpression = Expression.Property(commandParameterExpression, "Parameters");
                var addExpression = Expression.Call(parametersExpression, GetDbParameterCollectionAddMethod(), parameterVariableExpression);

                // Add
                callExpressions.Add(addExpression);
            }

            // Return
            return Expression
                .Lambda<Action<DbCommand, object>>(Expression.Block(callExpressions), commandParameterExpression, entityParameterExpression)
                .Compile();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandParameterExpression"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static Expression CreateDbParameterExpression(Expression commandParameterExpression,
            string parameterName)
        {
            var methodInfo = GetDbCommandCreateParameterMethod();

            return Expression.Call(methodInfo, new Expression[]
            {
                commandParameterExpression,
                Expression.Constant(parameterName),
                ConvertExpressionToTypeExpression(Expression.Constant(DBNull.Value), StaticType.Object),
                Expression.Default(StaticType.DbTypeNullable),
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandParameterExpression"></param>
        /// <param name="paramProperty"></param>
        /// <param name="entityProperty"></param>
        /// <param name="dbField"></param>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public static Expression GetDbParameterDbTypeAssignmentExpression(Expression commandParameterExpression,
            ClassProperty paramProperty,
            ClassProperty entityProperty,
            DbField dbField,
            Type valueType)
        {
            // Automatic
            if (Converter.ConversionType == ConversionType.Automatic && dbField?.Type != null)
            {
                valueType = dbField.Type.GetUnderlyingType();
            }

            // DbType
            // TODO: Optimize with below.
            // var dbType = (entityProperty ?? paramProperty).GetDbType() ?? (dbField?.Type ?? valueType ?? valueType)?.GetDbType();
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
            // End TODO

            // Return
            return dbType == null ? null :
                GetDbParameterDbTypeAssignmentExpression(commandParameterExpression, dbType.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandParameterExpression"></param>
        /// <param name="paramProperty"></param>
        /// <param name="entityProperty"></param>
        /// <param name="dbField"></param>
        /// <param name="valueType"></param>
        /// <param name="valueExpression"></param>
        /// <returns></returns>
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
            // TODO: Optimize with below.
            // var dbType = (entityProperty ?? paramProperty).GetDbType() ?? (dbField?.Type ?? valueType ?? valueType)?.GetDbType();
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
            // End TODO

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
        /// <param name="paramterExpression"></param>
        /// <param name="classProperty"></param>
        /// <param name="valueType"></param>
        /// <param name="valueExpression"></param>
        public static void InvokePropertyHandlerViaExpression(Expression paramterExpression,
            ClassProperty classProperty,
            ref Type valueType,
            ref Expression valueExpression)
        {
            var (expression, type) = ConvertExpressionToPropertyHandlerSetExpressionTuple(valueExpression, paramterExpression, classProperty, valueType);
            if (type != null)
            {
                valueType = type;
                valueExpression = expression;
            }
        }
    }
}
