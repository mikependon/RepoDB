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
        /// <param name="batchSize"></param>
        /// <param name="dbSetting"></param>
        /// <param name="dbHelper"></param>
        /// <returns></returns>
        internal static Action<DbCommand, IList<object>> CompileDataEntityListDbParameterSetter(Type entityType,
            IEnumerable<DbField> inputFields,
            IEnumerable<DbField> outputFields,
            int batchSize,
            IDbSetting dbSetting,
            IDbHelper dbHelper)
        {
            var typeOfListEntity = typeof(IList<>).MakeGenericType(StaticType.Object);
            var dbCommandExpression = Expression.Parameter(StaticType.DbCommand, "command");
            var entitiesParameterExpression = Expression.Parameter(typeOfListEntity, "entities");
            var fieldDirections = new List<FieldDirection>();
            var bodyExpressions = new List<Expression>();

            // Field directions
            fieldDirections.AddRange(GetInputFieldDirections(inputFields));
            fieldDirections.AddRange(GetOutputFieldDirections(outputFields));

            // Clear the parameter collection first
            bodyExpressions.Add(GetDbCommandParametersClearExpression(dbCommandExpression));

            // Iterate by batch size
            for (var entityIndex = 0; entityIndex < batchSize; entityIndex++)
            {
                // Add to the instance block
                var indexDbParameterSetterExpression = GetIndexDbParameterSetterExpression(entityType,
                    dbCommandExpression,
                    entitiesParameterExpression,
                    fieldDirections,
                    entityIndex,
                    dbSetting,
                    dbHelper);

                // Add to the body
                bodyExpressions.Add(indexDbParameterSetterExpression);
            }

            // Set the function value
            return Expression
                .Lambda<Action<DbCommand, IList<object>>>(Expression.Block(bodyExpressions), dbCommandExpression, entitiesParameterExpression)
                .Compile();
        }
    }
}
