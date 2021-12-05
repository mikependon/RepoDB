using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using RepoDb.Interfaces;

namespace RepoDb.Reflection
{
    internal partial class Compiler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="inputFields"></param>
        /// <param name="outputFields"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static Action<DbCommand, object> CompileDataEntityDbParameterSetter(Type entityType,
            IEnumerable<DbField> inputFields,
            IEnumerable<DbField> outputFields,
            IDbSetting dbSetting)
        {
            var commandParameterExpression = Expression.Parameter(StaticType.DbCommand, "command");
            var entityParameterExpression = Expression.Parameter(StaticType.Object, "entityParameter");
            var dbParameterCollectionExpression = Expression.Property(commandParameterExpression,
                StaticType.DbCommand.GetProperty("Parameters"));
            var entityVariableExpression = Expression.Variable(StaticType.Object, "entityVariable");
            var entityExpressions = new List<Expression>();
            var entityVariableExpressions = new List<ParameterExpression>();
            var fieldDirections = new List<FieldDirection>();
            var bodyExpressions = new List<Expression>();

            // Class handler
            var handledEntityParameterExpression = ConvertExpressionToClassHandlerSetExpression(entityType, entityParameterExpression);

            // Field directions
            fieldDirections.AddRange(GetInputFieldDirections(inputFields));
            fieldDirections.AddRange(GetOutputFieldDirections(outputFields));

            // Clear the parameter collection first
            bodyExpressions.Add(GetDbParameterCollectionClearMethodExpression(dbParameterCollectionExpression));

            // Entity instance
            entityVariableExpressions.Add(entityVariableExpression);
            entityExpressions.Add(Expression.Assign(entityVariableExpression, handledEntityParameterExpression));

            // Throw if null
            entityExpressions.Add(ThrowIfNullAfterClassHandlerExpression(entityType, entityVariableExpression));

            // Iterate the input fields
            foreach (var fieldDirection in fieldDirections)
            {
                // Add the property block
                var propertyBlock = GetPropertyFieldExpression(commandParameterExpression,
                    ConvertExpressionToTypeExpression(entityVariableExpression, entityType),
                    fieldDirection,
                    0,
                    dbSetting);

                // Add to instance expression
                entityExpressions.Add(propertyBlock);
            }

            // Add to the instance block
            var instanceBlock = Expression.Block(entityVariableExpressions, entityExpressions);

            // Add to the body
            bodyExpressions.Add(instanceBlock);

            // Set the function value
            return Expression
                .Lambda<Action<DbCommand, object>>(Expression.Block(bodyExpressions), commandParameterExpression, entityParameterExpression)
                .Compile();
        }
    }
}
