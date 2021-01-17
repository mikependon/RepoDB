using System;
using System.Data.Common;
using System.Linq.Expressions;
using System.Linq;
using System.Dynamic;
using RepoDb.Extensions;
using System.Collections.Generic;
using RepoDb.Interfaces;

namespace RepoDb.Reflection
{
    internal partial class Compiler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="dbFields"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static Func<DbDataReader, ExpandoObject> CompileDataReaderToExpandoObject(DbDataReader reader,
            IEnumerable<DbField> dbFields,
            IDbSetting dbSetting)
        {
            var readerParameterExpression = Expression.Parameter(StaticType.DbDataReader, "reader");
            var readerFields = GetDataReaderFields(reader, dbFields, dbSetting);
            var memberBindings = GetMemberBindingsForDictionary(readerParameterExpression,
                readerFields?.AsList());

            // Throw an error if there are no matching at least one
            if (memberBindings.Any() != true)
            {
                throw new InvalidOperationException($"There are no member bindings found from the ResultSet of the data reader.");
            }

            // Initialize the members
            var body = Expression.ListInit(Expression.New(StaticType.ExpandoObject), memberBindings);

            // Set the function value
            return Expression
                .Lambda<Func<DbDataReader, ExpandoObject>>(body, readerParameterExpression)
                .Compile();
        }
    }
}
