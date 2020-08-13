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
        /// Gets a compiled function that is used to set the <see cref="DbParameter"/> objects of the <see cref="DbCommand"/> object based from the values of the data entity/dynamic object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity objects.</typeparam>
        /// <param name="inputFields">The list of the input <see cref="DbField"/> objects.</param>
        /// <param name="outputFields">The list of the output <see cref="DbField"/> objects.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The compiled function.</returns>
        public static Action<DbCommand, TEntity> CompileDataEntityDbParameterSetter<TEntity>(IEnumerable<DbField> inputFields,
            IEnumerable<DbField> outputFields,
            IDbSetting dbSetting)
            where TEntity : class
        {
            var typeOfEntity = typeof(TEntity);
            var commandParameterExpression = Expression.Parameter(StaticType.DbCommand, "command");
            var dbParameterCollection = Expression.Property(commandParameterExpression,
                StaticType.DbCommand.GetProperty("Parameters"));
            var fieldDirections = new List<FieldDirection>();
            var entityParameter = Expression.Parameter(typeOfEntity, "entity");
            var entityVariable = Expression.Variable(typeOfEntity, "entity");
            var entityExpressions = new List<Expression>();
            var entityVariables = new List<ParameterExpression>();
            var bodyExpressions = new List<Expression>();

            // Field directions
            fieldDirections.AddRange(GetInputFieldDirections(inputFields));
            fieldDirections.AddRange(GetOutputFieldDirections(outputFields));

            // Clear the parameter collection first
            bodyExpressions.Add(GetDbParameterCollectionClearMethodExpression(dbParameterCollection));

            // Entity instance
            entityVariables.Add(entityVariable);
            entityExpressions.Add(Expression.Assign(entityVariable, entityParameter));

            // Iterate the input fields
            foreach (var fieldDirection in fieldDirections)
            {
                // Add the property block
                var propertyBlock = GetPropertyFieldExpression(commandParameterExpression,
                    entityVariable, fieldDirection, 0, dbSetting);

                // Add to instance expression
                entityExpressions.Add(propertyBlock);
            }

            // Add to the instance block
            var instanceBlock = Expression.Block(entityVariables, entityExpressions);

            // Add to the body
            bodyExpressions.Add(instanceBlock);

            // Set the function value
            return Expression
                .Lambda<Action<DbCommand, TEntity>>(Expression.Block(bodyExpressions), commandParameterExpression, entityParameter)
                .Compile();
        }
    }
}
