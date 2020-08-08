using System;
using System.Data.Common;
using System.Data;
using System.Linq.Expressions;
using System.Linq;
using System.Dynamic;
using RepoDb.Extensions;

namespace RepoDb.Reflection
{
    internal partial class Compiler
    {
        /// <summary>
        /// Gets a compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of dynamic objects.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>A compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of dynamic objects.</returns>
        internal static Func<DbDataReader, ExpandoObject> GetDataReaderToExpandoObjectConverterFunction(DbDataReader reader,
            string tableName,
            IDbConnection connection,
            IDbTransaction transaction)
        {
            // Expression variables
            var readerParameterExpression = Expression.Parameter(StaticType.DbDataReader, "reader");
            var dbFields = !string.IsNullOrWhiteSpace(tableName) ?
                    GetDbFields(connection, tableName, null, transaction, true) : null;
            var readerFields = GetDataReaderFields(reader, dbFields, connection?.GetDbSetting());
            var memberBindings = GetMemberBindingsForDictionary(readerParameterExpression,
                readerFields?.AsList());

            // Throw an error if there are no matching atleast one
            if (memberBindings.Any() != true)
            {
                throw new InvalidOperationException($"There are no member bindings found between the resultset of the data reader and the table '{tableName}'.");
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
