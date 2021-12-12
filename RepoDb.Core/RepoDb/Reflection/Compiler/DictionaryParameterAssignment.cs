using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.Reflection
{
    internal partial class Compiler
    {
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
            var createParameterExpression = GetDbCommandCreateParameterExpression(commandParameterExpression, dbField);
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

            // Npgsql (Unknown)
            if (IsPostgreSqlUserDefined(dbField))
            {
                var setToUnknownExpression = GetSetToUnknownNpgsqlParameterExpression(parameterVariableExpression, dbField);
                parameterAssignmentExpressions.AddIfNotNull(setToUnknownExpression);
            }

            // Add to body
            return Expression.Block(new[] { parameterVariableExpression }, parameterAssignmentExpressions);
        }
    }
}
