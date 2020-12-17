using RepoDb.Enumerations;
using RepoDb.Extensions;
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
        /// <param name="type"></param>
        /// <param name="dbFields"></param>
        /// <returns></returns>
        internal static Action<DbCommand, object> GetPlainTypeToDbParametersCompiledFunction(Type type,
            IEnumerable<DbField> dbFields = null)
        {
            var commandParameterExpression = Expression.Parameter(StaticType.DbCommand, "command");
            var entityParameterExpression = Expression.Parameter(StaticType.Object, "entity");
            var entityExpression = ConvertExpressionToTypeExpression(entityParameterExpression, type);
            var methodInfo = GetDbCommandCreateParameterMethod();
            var callExpressions = new List<Expression>();

            // Iterate
            foreach (var classProperty in PropertyCache.Get(type))
            {
                var dbField = dbFields?.FirstOrDefault(df =>
                    string.Equals(df.Name, classProperty.GetMappedName(), StringComparison.OrdinalIgnoreCase));
                var valueExpression = (Expression)Expression.Property(entityExpression, classProperty.PropertyInfo);

                // PropertyHandler
                valueExpression = ConvertExpressionToPropertyHandlerSetExpression(valueExpression,
                    classProperty, classProperty.PropertyInfo.PropertyType);

                // Automatic
                if (Converter.ConversionType == ConversionType.Automatic && dbField?.Type != null)
                {
                    valueExpression = ConvertExpressionWithAutomaticConversion(valueExpression,
                        dbField.Type.GetUnderlyingType());
                }

                //// DbType
                //var dbType = (returnType != null ? clientTypeToDbTypeResolver.Resolve(returnType) : null) ??
                //    classProperty.GetDbType() ??
                //    value?.GetType()?.GetDbType();

                // DbType
                var dbType = classProperty.GetDbType();
                if (dbType == null && classProperty.PropertyInfo.PropertyType.IsEnum)
                {
                    dbType = Converter.EnumDefaultDatabaseType;
                }
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
