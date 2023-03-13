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
        /// <param name="dbHelper"></param>
        /// <returns></returns>
        internal static Action<DbCommand, object> CompileDictionaryStringObjectDbParameterSetter(Type entityType,
            IEnumerable<DbField> inputFields,
            IDbSetting dbSetting,
            IDbHelper dbHelper)
        {
            var dbCommandExpression = Expression.Parameter(StaticType.DbCommand, "command");
            var entityParameterExpression = Expression.Parameter(StaticType.Object, "entityParameter");
            var dbParameterCollectionExpression = Expression.Property(dbCommandExpression,
                StaticType.DbCommand.GetProperty("Parameters"));
            var dictionaryInstanceExpression = ConvertExpressionToTypeExpression(entityParameterExpression, StaticType.IDictionaryStringObject);
            var bodyExpressions = new List<Expression>();

            // Clear the parameter collection first
            bodyExpressions.Add(GetDbParameterCollectionClearMethodExpression(dbParameterCollectionExpression));

            // Iterate the fields
            foreach (var dbField in inputFields)
            {
                var dictionaryParameterExpression = GetDictionaryStringObjectParameterAssignmentExpression(dbCommandExpression,
                    0,
                    dictionaryInstanceExpression,
                    dbField,
                    dbSetting,
                    dbHelper);

                // Add to body
                bodyExpressions.Add(dictionaryParameterExpression);
            }

            // Compile
            return Expression
                .Lambda<Action<DbCommand, object>>(Expression.Block(bodyExpressions),
                    dbCommandExpression,
                    entityParameterExpression)
                .Compile();
        }
    }
}
