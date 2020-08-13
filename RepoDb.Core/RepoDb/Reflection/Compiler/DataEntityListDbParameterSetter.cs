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
        /// Gets a compiled function that is used to set the <see cref="DbParameter"/> objects of the <see cref="DbCommand"/> object based from the values of the data entity/dynamic objects.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity objects.</typeparam>
        /// <param name="inputFields">The list of the input <see cref="DbField"/> objects.</param>
        /// <param name="outputFields">The list of the input <see cref="DbField"/> objects.</param>
        /// <param name="batchSize">The batch size of the entity to be passed.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The compiled function.</returns>
        public static Action<DbCommand, IList<TEntity>> CompileDataEntityListDbParameterSetter<TEntity>(IEnumerable<DbField> inputFields,
            IEnumerable<DbField> outputFields,
            int batchSize,
            IDbSetting dbSetting)
            where TEntity : class
        {
            var typeOfListEntity = typeof(IList<TEntity>);
            var typeOfEntity = typeof(TEntity);
            var commandParameterExpression = Expression.Parameter(StaticType.DbCommand, "command");
            var entitiesParameterExpression = Expression.Parameter(typeOfListEntity, "entities");
            var fieldDirections = new List<FieldDirection>();
            var instanceVariable = Expression.Variable(typeOfEntity, "instance");
            var bodyExpressions = new List<Expression>();

            // Field directions
            fieldDirections.AddRange(GetInputFieldDirections(inputFields));
            fieldDirections.AddRange(GetOutputFieldDirections(outputFields));

            // Clear the parameter collection first
            bodyExpressions.Add(GetDbCommandParametersClearExpression(commandParameterExpression));

            // Iterate by batch size
            for (var entityIndex = 0; entityIndex < batchSize; entityIndex++)
            {
                // Get the current instance
                var entityParameter = GetListEntityIndexerExpression(entitiesParameterExpression, typeOfListEntity, entityIndex);
                var entityExpressions = new List<Expression>();
                var entityVariables = new List<ParameterExpression>();

                // Entity instance
                entityVariables.Add(instanceVariable);
                entityExpressions.Add(Expression.Assign(instanceVariable, entityParameter));

                // Iterate the input fields
                foreach (var fieldDirection in fieldDirections)
                {
                    // Add the property block
                    var propertyBlock = GetPropertyFieldExpression(commandParameterExpression,
                        instanceVariable, fieldDirection, entityIndex, dbSetting);

                    // Add to instance expression
                    entityExpressions.Add(propertyBlock);
                }

                // Add to the instance block
                var instanceBlock = Expression.Block(entityVariables, entityExpressions);

                // Add to the body
                bodyExpressions.Add(instanceBlock);
            }

            // Set the function value
            return Expression
                .Lambda<Action<DbCommand, IList<TEntity>>>(Expression.Block(bodyExpressions), commandParameterExpression, entitiesParameterExpression)
                .Compile();
        }
    }
}
