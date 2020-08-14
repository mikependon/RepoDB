using System;
using System.Data.Common;
using System.Data;
using System.Linq.Expressions;
using System.Linq;
using System.Dynamic;
using RepoDb.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public static Func<DbDataReader, ExpandoObject> CompileDataReaderToExpandoObject(DbDataReader reader,
            string tableName,
            IDbConnection connection,
            IDbTransaction transaction)
        {
            // Get the fields
            var dbFields = !string.IsNullOrWhiteSpace(tableName) ?
                GetDbFields(connection, tableName, null, transaction, true) : null;

            // Return the value
            return CompileDataReaderToExpandoObject(reader, dbFields, tableName, connection);
        }


        /// <summary>
        /// Gets a compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of dynamic objects in an asynchronous way.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>A compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of dynamic objects.</returns>
        public static async Task<Func<DbDataReader, ExpandoObject>> CompileDataReaderToExpandoObjectAsync(DbDataReader reader,
            string tableName,
            IDbConnection connection,
            IDbTransaction transaction)
        {
            // Get the fields
            var dbFields = !string.IsNullOrWhiteSpace(tableName) ?
                await GetDbFieldsAsync(connection, tableName, null, transaction, true) : null;

            // Return
            return CompileDataReaderToExpandoObject(reader, dbFields, tableName, connection);
        }


        /// <summary>
        /// Gets a compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of dynamic objects.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="dbFields">The list of the <see cref="DbField"/> objects.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <returns>A compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of dynamic objects.</returns>
        public static Func<DbDataReader, ExpandoObject> CompileDataReaderToExpandoObject(DbDataReader reader,
            IEnumerable<DbField> dbFields,
            string tableName,
            IDbConnection connection)
        {
            var readerParameterExpression = Expression.Parameter(StaticType.DbDataReader, "reader");
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
