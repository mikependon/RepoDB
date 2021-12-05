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
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static Action<DbCommand, object> CompileDictionaryStringObjectDbParameterSetter(Type entityType,
            IEnumerable<DbField> inputFields,
            IDbSetting dbSetting)
        {
            var commandParameterExpression = Expression.Parameter(StaticType.DbCommand, "command");
            var entityParameterExpression = Expression.Parameter(StaticType.Object, "entityParameter");
            var dbParameterCollectionExpression = Expression.Property(commandParameterExpression,
                StaticType.DbCommand.GetProperty("Parameters"));
            var dictionaryInstanceExpression = ConvertExpressionToTypeExpression(entityParameterExpression, StaticType.IDictionaryStringObject);
            var bodyExpressions = new List<Expression>();

            // Clear the parameter collection first
            bodyExpressions.Add(GetDbParameterCollectionClearMethodExpression(dbParameterCollectionExpression));

            // Iterate the fields
            foreach (var dbField in inputFields)
            {
                var dictionaryParameterExpression = GetDictionaryStringObjectParameterAssignmentExpression(commandParameterExpression,
                    0,
                    dictionaryInstanceExpression,
                    dbField,
                    dbSetting);

                // Add to body
                bodyExpressions.Add(dictionaryParameterExpression);
            }

            // Compile
            return Expression
                .Lambda<Action<DbCommand, object>>(Expression.Block(bodyExpressions),
                    commandParameterExpression,
                    entityParameterExpression)
                .Compile();
        }
    }
}
