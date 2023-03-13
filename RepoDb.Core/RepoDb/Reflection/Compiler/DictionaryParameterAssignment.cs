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
        /// <param name="dbCommandExpression"></param>
        /// <param name="entityIndex"></param>
        /// <param name="dictionaryInstanceExpression"></param>
        /// <param name="dbField"></param>
        /// <param name="dbSetting"></param>
        /// <param name="dbHelper"></param>
        /// <returns></returns>
        internal static Expression GetDictionaryStringObjectParameterAssignmentExpression(ParameterExpression dbCommandExpression,
            int entityIndex,
            Expression dictionaryInstanceExpression,
            DbField dbField,
            IDbSetting dbSetting,
            IDbHelper dbHelper)
        {
            var parameterAssignmentExpressions = new List<Expression>();
            var dbParameterExpression = Expression.Variable(StaticType.DbParameter,
                string.Concat("parameter", dbField.Name.AsUnquoted(true, dbSetting).AsAlphaNumeric()));

            // Variable
            var createParameterExpression = GetDbCommandCreateParameterExpression(dbCommandExpression, dbField);
            parameterAssignmentExpressions.AddIfNotNull(Expression.Assign(dbParameterExpression, createParameterExpression));

            // DbParameter.Name
            var nameAssignmentExpression = GetDbParameterNameAssignmentExpression(dbParameterExpression,
                dbField,
                entityIndex,
                dbSetting);
            parameterAssignmentExpressions.AddIfNotNull(nameAssignmentExpression);

            // DbParameter.Value
            var valueAssignmentExpression = GetDictionaryStringObjectDbParameterValueAssignmentExpression(dbParameterExpression,
                dictionaryInstanceExpression,
                dbField,
                dbSetting);
            parameterAssignmentExpressions.AddIfNotNull(valueAssignmentExpression);

            // DbParameter.DbType
            var dbTypeAssignmentExpression = GetDbParameterDbTypeAssignmentExpression(dbParameterExpression,
                dbField);
            parameterAssignmentExpressions.AddIfNotNull(dbTypeAssignmentExpression);

            // DbParameter.Direction
            if (dbSetting.IsDirectionSupported)
            {
                var directionAssignmentExpression = GetDbParameterDirectionAssignmentExpression(dbParameterExpression, ParameterDirection.Input);
                parameterAssignmentExpressions.AddIfNotNull(directionAssignmentExpression);
            }

            // DbParameter.Size
            if (dbField.Size != null)
            {
                var sizeAssignmentExpression = GetDbParameterSizeAssignmentExpression(dbParameterExpression, dbField.Size.Value);
                parameterAssignmentExpressions.AddIfNotNull(sizeAssignmentExpression);
            }

            // DbParameter.Precision
            if (dbField.Precision != null)
            {
                var precisionAssignmentExpression = GetDbParameterPrecisionAssignmentExpression(dbParameterExpression, dbField.Precision.Value);
                parameterAssignmentExpressions.AddIfNotNull(precisionAssignmentExpression);
            }

            // DbParameter.Scale
            if (dbField.Scale != null)
            {
                var scaleAssignmentExpression = GetDbParameterScaleAssignmentExpression(dbParameterExpression, dbField.Scale.Value);
                parameterAssignmentExpressions.AddIfNotNull(scaleAssignmentExpression);
            }

            // Compiler.DbParameterPostCreation
            var dbParameterPostCreationExpression = GetCompilerDbParameterPostCreationExpression(dbParameterExpression, dbHelper);
            parameterAssignmentExpressions.AddIfNotNull(dbParameterPostCreationExpression);

            // DbCommand.Parameters.Add
            var dbParametersAddExpression = GetDbCommandParametersAddExpression(dbCommandExpression, dbParameterExpression);
            parameterAssignmentExpressions.AddIfNotNull(dbParametersAddExpression);

            // Add to body
            return Expression.Block(new[] { dbParameterExpression }, parameterAssignmentExpressions);
        }
    }
}
