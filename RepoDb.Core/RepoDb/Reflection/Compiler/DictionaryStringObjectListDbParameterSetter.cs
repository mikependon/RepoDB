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
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="inputFields"></param>
        /// <param name="batchSize"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static Action<DbCommand, IList<TEntity>> CompileDictionaryStringObjectListDbParameterSetter<TEntity>(IEnumerable<DbField> inputFields,
            int batchSize,
            IDbSetting dbSetting)
            where TEntity : class
        {
            var typeOfListEntity = typeof(IList<TEntity>);
            var getItemMethod = typeOfListEntity.GetMethod("get_Item", new[] { StaticType.Int32 });
            var commandParameterExpression = Expression.Parameter(StaticType.DbCommand, "command");
            var entitiesParameterExpression = Expression.Parameter(typeOfListEntity, "entities");
            var dbParameterCollectionExpression = Expression.Property(commandParameterExpression,
                StaticType.DbCommand.GetProperty("Parameters"));
            var bodyExpressions = new List<Expression>();

            // Clear the parameter collection first
            bodyExpressions.Add(GetDbParameterCollectionClearMethodExpression(dbParameterCollectionExpression));

            // Iterate by batch size
            for (var entityIndex = 0; entityIndex < batchSize; entityIndex++)
            {
                var currentInstanceExpression = Expression.Call(entitiesParameterExpression, getItemMethod, Expression.Constant(entityIndex));
                var dictionaryInstanceExpression = ConvertExpressionToTypeExpression(currentInstanceExpression, StaticType.IDictionaryStringObject);

                // Iterate the fields
                foreach (var dbField in inputFields)
                {
                    var dictionaryParameterExpression = GetDictionaryStringObjectParameterAssignmentExpression(commandParameterExpression,
                        entityIndex,
                        dictionaryInstanceExpression,
                        dbField,
                        dbSetting);

                    // Add to body
                    bodyExpressions.Add(dictionaryParameterExpression);
                }
            }

            // Compile
            return Expression
                .Lambda<Action<DbCommand, IList<TEntity>>>(Expression.Block(bodyExpressions), commandParameterExpression, entitiesParameterExpression)
                .Compile();
        }
    }
}
