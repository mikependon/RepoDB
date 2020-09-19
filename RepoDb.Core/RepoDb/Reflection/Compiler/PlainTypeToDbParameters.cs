using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;

namespace RepoDb.Reflection
{
    internal partial class Compiler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static Action<DbCommand, object> GetPlainTypeToDbParametersCompiledFunction(Type type)
        {
            var commandParameterExpression = Expression.Parameter(StaticType.DbCommand, "command");
            var entityParameterExpression = Expression.Parameter(StaticType.Object, "entity");
            var entityExpression = ConvertExpressionToTypeExpression(entityParameterExpression, type);
            var methodInfo = GetDbCommandCreateParameterMethod();
            var callExpressions = new List<Expression>();

            // Iterate
            foreach (var classProperty in PropertyCache.Get(type))
            {
                // Value
                var valueExpression = (Expression)Expression.Property(entityExpression, classProperty.PropertyInfo);

                // PropertyHandler
                valueExpression = ConvertExpressionToPropertyHandlerSetExpression(valueExpression,
                    classProperty, classProperty.PropertyInfo.PropertyType);

                // DbType
                var dbType = classProperty.GetDbType();
                var dbTypeExpression = dbType == null ? GetNullableTypeExpression(StaticType.DbType) :
                    ConvertExpressionToNullableExpression(Expression.Constant(dbType), StaticType.DbType);

                // DbCommandExtension.CreateParameter
                var expression = Expression.Call(methodInfo, new Expression[]
                {
                    commandParameterExpression,
                    Expression.Constant(classProperty.GetMappedName()),
                    ConvertExpressionToTypeExpression( valueExpression, StaticType.Object),
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
