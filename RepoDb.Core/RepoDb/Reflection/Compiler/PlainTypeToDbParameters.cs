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
            var paramProperties = PropertyCache.Get(paramType);
            var entityProperties = PropertyCache.Get(entityType);

            // Iterate
            foreach (var paramProperty in paramProperties)
            {
                // Ensure it matching any params
                var entityProperty = entityProperties?.FirstOrDefault(e =>
                    string.Equals(e.GetMappedName(), paramProperty.GetMappedName(), StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(e.PropertyInfo.Name, paramProperty.PropertyInfo.Name, StringComparison.OrdinalIgnoreCase));

                // Variables
                var dbField = dbFields?.FirstOrDefault(df =>
                    string.Equals(df.Name, paramProperty.GetMappedName(), StringComparison.OrdinalIgnoreCase));
                var valueExpression = (Expression)Expression.Property(entityExpression, paramProperty.PropertyInfo);
                var targetProperty = (entityProperty ?? paramProperty);
                var valueType = targetProperty.PropertyInfo.PropertyType.GetUnderlyingType();

                // Enum
                if (targetProperty.PropertyInfo.PropertyType.IsEnum && dbField != null)
                {
                    valueExpression = ConvertEnumExpressionToTypeExpression(valueExpression, dbField.Type);
                }

                // PropertyHandler
                var (convertedExpression, handlerSetReturnType) = ConvertExpressionToPropertyHandlerSetExpressionTuple(valueExpression, paramProperty, valueType);
                if (handlerSetReturnType != null)
                {
                    valueExpression = convertedExpression;
                    valueType = handlerSetReturnType;
                }

                // Automatic
                if (Converter.ConversionType == ConversionType.Automatic && dbField?.Type != null)
                {
                    valueType = dbField.Type.GetUnderlyingType();
                    valueExpression = ConvertExpressionWithAutomaticConversion(valueExpression, valueType);
                }

                // DbType
                var dbType =
                    (paramProperty == null ? null : TypeMapCache.Get(paramProperty.GetDeclaringType(), paramProperty.PropertyInfo)) ??
                    (entityProperty == null ? null : TypeMapCache.Get(entityProperty.GetDeclaringType(), entityProperty.PropertyInfo));
                if (dbType == null && (valueType ??= dbField?.Type.GetUnderlyingType()) != null)
                {
                    var resolver = new ClientTypeToDbTypeResolver();
                    dbType =
                        valueType.GetDbType() ??                // type level, use TypeMapCache
                        resolver.Resolve(valueType) ??          // type level, primitive mapping
                        (valueType.IsEnum ?
                            (dbField?.Type != null ? resolver.Resolve(dbField.Type) : null) ?? // use the DBField.Type
                                Converter.EnumDefaultDatabaseType :  null);  // use Converter.EnumDefaultDatabaseType
                }

                var dbTypeExpression = dbType == null ? GetNullableTypeExpression(StaticType.DbType) :
                    ConvertExpressionToNullableExpression(Expression.Constant(dbType), StaticType.DbType);

                // DbCommandExtension.CreateParameter
                var expression = Expression.Call(methodInfo, new Expression[]
                {
                    commandParameterExpression,
                    Expression.Constant(paramProperty.GetMappedName()),
                    ConvertExpressionToTypeExpression(valueExpression, StaticType.Object),
                    dbTypeExpression
                });

                // DbCommand.Parameters.Add
                var parametersExpression = Expression.Property(commandParameterExpression, "Parameters");
                var addExpression = Expression.Call(parametersExpression, GetDbParameterCollectionAddMethod(), expression);

                // Add
                callExpressions.Add(addExpression);
            }

            // Return
            return Expression
                .Lambda<Action<DbCommand, object>>(Expression.Block(callExpressions), commandParameterExpression, entityParameterExpression)
                .Compile();
        }
    }
}
