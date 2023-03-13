using System;
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
        /// <param name="entityExpression"></param>
        /// <param name="propertyExpression"></param>
        /// <param name="dbField"></param>
        /// <param name="classProperty"></param>
        /// <param name="direction"></param>
        /// <param name="dbSetting"></param>
        /// <param name="dbHelper"></param>
        /// <returns></returns>
        internal static Expression GetDataEntityParameterAssignmentExpression(ParameterExpression dbCommandExpression,
            int entityIndex,
            Expression entityExpression,
            ParameterExpression propertyExpression,
            DbField dbField,
            ClassProperty classProperty,
            ParameterDirection direction,
            IDbSetting dbSetting,
            IDbHelper dbHelper)
        {
            var parameterAssignmentExpressions = new List<Expression>();
            var dbParameterExpression = Expression.Variable(StaticType.DbParameter,
                string.Concat("parameter", dbField.Name.AsUnquoted(true, dbSetting).AsAlphaNumeric()));

            // Variable
            var createParameterExpression = GetDbCommandCreateParameterExpression(dbCommandExpression, dbField);
            parameterAssignmentExpressions.AddIfNotNull(Expression.Assign(dbParameterExpression, createParameterExpression));

            // DbParameter.ParameterName
            var nameAssignmentExpression = GetDbParameterNameAssignmentExpression(dbParameterExpression,
                dbField,
                entityIndex,
                dbSetting);
            parameterAssignmentExpressions.AddIfNotNull(nameAssignmentExpression);

            // DbParameter.Value
            if (direction != ParameterDirection.Output)
            {
                var valueAssignmentExpression = GetDataEntityDbParameterValueAssignmentExpression(dbParameterExpression,
                    entityExpression,
                    propertyExpression,
                    classProperty,
                    dbField,
                    dbSetting);
                parameterAssignmentExpressions.AddIfNotNull(valueAssignmentExpression);
            }

            // DbParameter.DbType
            var dbTypeAssignmentExpression = GetDbParameterDbTypeAssignmentExpression(dbParameterExpression,
                classProperty, dbField);
            parameterAssignmentExpressions.AddIfNotNull(dbTypeAssignmentExpression);

            // DbParameter.Direction
            if (dbSetting.IsDirectionSupported)
            {
                var directionAssignmentExpression = GetDbParameterDirectionAssignmentExpression(dbParameterExpression, direction);
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

            // PropertyValueAttributes / DbField must precide
            var propertyValueAttributeAssignmentExpressions = GetPropertyValueAttributeAssignmentExpressions(dbParameterExpression, classProperty);
            parameterAssignmentExpressions.AddRangeIfNotNullOrNotEmpty(propertyValueAttributeAssignmentExpressions);

            // DbCommand.Parameters.Add
            var dbParametersAddExpression = GetDbCommandParametersAddExpression(dbCommandExpression, dbParameterExpression);
            parameterAssignmentExpressions.AddIfNotNull(dbParametersAddExpression);

            // Return the value
            return Expression.Block(new[] { dbParameterExpression }, parameterAssignmentExpressions);
        }
    }
}
