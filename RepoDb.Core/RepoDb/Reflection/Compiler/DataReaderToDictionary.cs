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
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="tableName"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        internal static Func<DbDataReader, ExpandoObject> CompileDataReaderToExpandoObject(DbDataReader reader,
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
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="tableName"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        internal static async Task<Func<DbDataReader, ExpandoObject>> CompileDataReaderToExpandoObjectAsync(DbDataReader reader,
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
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="dbFields"></param>
        /// <param name="tableName"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        internal static Func<DbDataReader, ExpandoObject> CompileDataReaderToExpandoObject(DbDataReader reader,
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
