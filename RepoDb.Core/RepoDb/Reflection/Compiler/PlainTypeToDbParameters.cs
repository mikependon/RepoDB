using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Resolvers;

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
            DbFieldCollection dbFields = null)
        {
            var dbCommandExpression = Expression.Parameter(StaticType.DbCommand, "command");
            var entityParameterExpression = Expression.Parameter(StaticType.Object, "entity");
            var entityExpression = ConvertExpressionToTypeExpression(entityParameterExpression, paramType);
            var methodInfo = GetDbCommandCreateParameterMethod();
            var callExpressions = new List<Expression>();

            // Iterate
            foreach (var paramProperty in PropertyCache.Get(paramType))
            {
                var mappedParamPropertyName = paramProperty.GetMappedName();

                // Ensure it matches to atleast one param
                var entityProperty = PropertyCache.Get(entityType)?.FirstOrDefault(e =>
                    string.Equals(e.GetMappedName(), mappedParamPropertyName, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(e.PropertyInfo.Name, paramProperty.PropertyInfo.Name, StringComparison.OrdinalIgnoreCase));

                // Variables
                var dbField = dbFields?.GetByName(mappedParamPropertyName);
                var targetProperty = (entityProperty ?? paramProperty);
                var parameterName = mappedParamPropertyName; // There is a purpose of why it is not 'targetProperty'
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
                    var addExpression = GetDbCommandParametersAddExpression(dbCommandExpression, valueExpression);
                    callExpressions.Add(addExpression);

                    #endregion
                }
                else
                {
                    #region NewParameter

                    var propertyType = targetProperty.PropertyInfo.PropertyType;
                    var underlyingType = TypeCache.Get(propertyType).GetUnderlyingType();
                    var valueType = GetPropertyHandlerSetMethodReturnType(paramProperty, underlyingType) ?? underlyingType;
                    var dbParameterExpression = Expression.Variable(StaticType.DbParameter, $"var{parameterName}");
                    var parameterCallExpressions = new List<Expression>();

                    // Create
                    var createParameterExpression =
                        CreateDbParameterExpression(dbCommandExpression, parameterName, valueExpression);
                    parameterCallExpressions.Add(
                        Expression.Assign(dbParameterExpression,
                            ConvertExpressionToTypeExpression(createParameterExpression, StaticType.DbParameter)));

                    // Convert

                    // DbType
                    var dbType = (DbType?)null;
                    if (valueType.IsEnum)
                    {
                        /*
                         * Note: The other data provider can coerce the Enum into its destination data type in the DB by default,
                         *       except for PostgreSQL. The code written below is only to address the issue for this specific provider.
                         */

                        if (!IsPostgreSqlUserDefined(dbField))
                        {
                            dbType = paramProperty.GetDbType() ??
                                valueType.GetDbType() ??
                                (dbField != null ? new ClientTypeToDbTypeResolver().Resolve(dbField.Type) : null) ??
                                (DbType?)GlobalConfiguration.Options.EnumDefaultDatabaseType;
                        }
                        else
                        {
                            dbType = default;
                        }
                    }
                    else if (GlobalConfiguration.Options.ConversionType == ConversionType.Automatic && dbField?.Type != null)
                    {
                        valueExpression = ConvertExpressionWithAutomaticConversion(valueExpression, dbField.TypeNullable());
                        dbType = default;
                    }
                    else
                    {
                        var targetType = TypeCache.Get(dbField?.Type).GetUnderlyingType() ?? valueType;
                        dbType = targetProperty.GetDbType() ??
                            targetType?.GetDbType() ??
                            new ClientTypeToDbTypeResolver().Resolve(targetType);
                    }
                    var setDbTypeExpression = GetDbParameterDbTypeAssignmentExpression(dbParameterExpression, dbType);
                    parameterCallExpressions.AddIfNotNull(setDbTypeExpression);

                    // PropertyHandler
                    InvokePropertyHandlerViaExpression(
                        dbParameterExpression, paramProperty, ref valueType, ref valueExpression);

                    // Value
                    var setValueExpression = GetDbParameterValueAssignmentExpression(dbParameterExpression,
                        valueExpression);
                    parameterCallExpressions.AddIfNotNull(setValueExpression);

                    // Size
                    var size = GetSize(null, dbField);
                    if (size > 0)
                    {
                        var setSizeExpression = GetDbParameterSizeAssignmentExpression(dbParameterExpression, size);
                        parameterCallExpressions.AddIfNotNull(setSizeExpression);
                    }

                    // Table-Valued Parameters
                    if (valueType == StaticType.DataTable)
                    {
                        parameterCallExpressions.AddIfNotNull(EnsureTableValueParameterExpression(dbParameterExpression));
                    }

                    // Type map attributes
                    var parameterPropertyValueSetterAttributesExpressions = GetParameterPropertyValueSetterAttributesAssignmentExpressions(
                        dbParameterExpression, targetProperty);
                    parameterCallExpressions.AddRangeIfNotNullOrNotEmpty(parameterPropertyValueSetterAttributesExpressions);

                    // DbCommand.Parameters.Add
                    var addExpression = GetDbCommandParametersAddExpression(dbCommandExpression, dbParameterExpression);
                    parameterCallExpressions.Add(addExpression);

                    // Add the parameter block
                    callExpressions.Add(Expression.Block(new[] { dbParameterExpression }, parameterCallExpressions));

                    #endregion
                }
            }

            // Return
            return Expression
                .Lambda<Action<DbCommand, object>>(Expression.Block(callExpressions), dbCommandExpression, entityParameterExpression)
                .Compile();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbCommandExpression"></param>
        /// <param name="parameterName"></param>
        /// <param name="valueExpression"></param>
        /// <returns></returns>
        public static Expression CreateDbParameterExpression(Expression dbCommandExpression,
            string parameterName,
            Expression valueExpression)
        {
            var methodInfo = GetDbCommandCreateParameterMethod();

            return Expression.Call(methodInfo, new Expression[]
            {
                dbCommandExpression,
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
