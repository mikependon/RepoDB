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
                var parameterName = paramProperty.GetMappedName(); // There is a purpose of why it is not 'targetProperty'
                var valueExpression = (Expression)Expression.Property(entityExpression, paramProperty.PropertyInfo);

                // Add the value itself
                if (StaticType.IDbDataParameter.IsAssignableFrom(targetProperty.PropertyInfo.PropertyType))
                {
                    // The 'valueExpression' is of type 'IDbDataParameter' itself

                    #region DbParameter

                    // Set the name
                    var setNameExpression = GetDbParameterNameAssignmentExpression(valueExpression, parameterName);
                    callExpressions.AddIfNotNull(setNameExpression);

                    // DbCommand.Parameters.Add
                    var addExpression = GetDbCommandParametersAddExpression(commandParameterExpression, valueExpression);
                    callExpressions.Add(addExpression);

                    #endregion
                }
                else
                {
                    #region NewParameter

                    var underlyingType = targetProperty.PropertyInfo.PropertyType.GetUnderlyingType();
                    var valueType = GetPropertyHandlerSetMethodReturnType(paramProperty, underlyingType) ?? underlyingType;
                    var parameterVariableExpression = Expression.Variable(StaticType.DbParameter, $"var{parameterName}");
                    var parameterCallExpressions = new List<Expression>();

                    // Create
                    var createParameterExpression =
                        CreateDbParameterExpression(commandParameterExpression, parameterName, valueExpression);
                    parameterCallExpressions.Add(
                        Expression.Assign(parameterVariableExpression,
                            ConvertExpressionToTypeExpression(createParameterExpression, StaticType.DbParameter)));

                    // Convert
                    if (Converter.ConversionType == ConversionType.Automatic && dbField?.Type != null)
                    {
                        valueType = dbField.Type.GetUnderlyingType();
                        valueExpression = ConvertExpressionWithAutomaticConversion(valueExpression, valueType);
                    }

                    // DbType
                    var dbType = (DbType?)null;
                    if (valueType.IsEnum)
                    {
                        dbType = IsPostgreSqlUserDefined(dbField) ? default :
                            paramProperty.GetDbType() ??
                            valueType.GetDbType() ??
                            (dbField != null ? new ClientTypeToDbTypeResolver().Resolve(dbField.Type) : null) ??
                            (DbType?)Converter.EnumDefaultDatabaseType;
                    }
                    else
                    {
                        var targetType = dbField?.Type.GetUnderlyingType() ?? valueType;
                        dbType = targetProperty.GetDbType() ??
                            targetType?.GetDbType() ??
                            new ClientTypeToDbTypeResolver().Resolve(targetType);
                    }
                    var setDbTypeExpression = GetDbParameterDbTypeAssignmentExpression(parameterVariableExpression, dbType);
                    parameterCallExpressions.AddIfNotNull(setDbTypeExpression);

                    // PropertyHandler
                    InvokePropertyHandlerViaExpression(
                        parameterVariableExpression, paramProperty, ref valueType, ref valueExpression);

                    // Value
                    var setValueExpression = GetDbParameterValueAssignmentExpression(parameterVariableExpression,
                        valueExpression);
                    parameterCallExpressions.AddIfNotNull(setValueExpression);

                    // Size
                    var size = GetSize(null, dbField);
                    if (size > 0)
                    {
                        var setSizeExpression = GetDbParameterSizeAssignmentExpression(parameterVariableExpression, size);
                        parameterCallExpressions.AddIfNotNull(setSizeExpression);
                    }

                    // Table-Valued Parameters
                    if (valueType == StaticType.DataTable)
                    {
                        parameterCallExpressions.AddIfNotNull(EnsureTableValueParameterExpression(parameterVariableExpression));
                    }

                    // Type map attributes
                    var parameterPropertyValueSetterAttributesExpressions = GetParameterPropertyValueSetterAttributesAssignmentExpressions(
                        parameterVariableExpression, targetProperty);
                    parameterCallExpressions.AddRangeIfNotNullOrNotEmpty(parameterPropertyValueSetterAttributesExpressions);

                    // DbCommand.Parameters.Add
                    var addExpression = GetDbCommandParametersAddExpression(commandParameterExpression, parameterVariableExpression);
                    parameterCallExpressions.Add(addExpression);

                    // Add the parameter block
                    callExpressions.Add(Expression.Block(new[] { parameterVariableExpression }, parameterCallExpressions));

                    #endregion
                }
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
        /// <param name="valueExpression"></param>
        /// <returns></returns>
        public static Expression CreateDbParameterExpression(Expression commandParameterExpression,
            string parameterName,
            Expression valueExpression)
        {
            var methodInfo = GetDbCommandCreateParameterMethod();

            return Expression.Call(methodInfo, new Expression[]
            {
                commandParameterExpression,
                Expression.Constant(parameterName),
                ConvertExpressionToTypeExpression(valueExpression, StaticType.Object),
                Expression.Default(StaticType.DbTypeNullable),
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
