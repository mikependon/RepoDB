using System;
using System.Data.Common;
using System.Data;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;
using RepoDb.Interfaces;
using System.Threading.Tasks;

namespace RepoDb.Reflection
{
    internal partial class Compiler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="reader"></param>
        /// <param name="connection"></param>
        /// <param name="connectionString"></param>
        /// <param name="transaction"></param>
        /// <param name="enableValidation"></param>
        /// <returns></returns>
        public static Func<DbDataReader, TResult> CompileDataReaderToDataEntity<TResult>(DbDataReader reader,
            IDbConnection connection,
            string connectionString,
            IDbTransaction transaction,
            bool enableValidation)
        {
            // Expression variables
            var dbFields = GetDbFields(connection,
                ClassMappedNameCache.Get<TResult>(),
                connectionString,
                transaction,
                enableValidation);

            // return the value
            return CompileDataReaderToDataEntity<TResult>(reader, dbFields, connection?.GetDbSetting());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="reader"></param>
        /// <param name="connection"></param>
        /// <param name="connectionString"></param>
        /// <param name="transaction"></param>
        /// <param name="enableValidation"></param>
        /// <returns></returns>
        public static async Task<Func<DbDataReader, TResult
            >> CompileDataReaderToDataEntityAsync<TResult>(DbDataReader reader,
            IDbConnection connection,
            string connectionString,
            IDbTransaction transaction,
            bool enableValidation)
        {
            // Expression variables
            var dbFields = await GetDbFieldsAsync(connection,
                ClassMappedNameCache.Get<TResult>(),
                connectionString,
                transaction,
                enableValidation);

            // return the value
            return CompileDataReaderToDataEntity<TResult>(reader, dbFields, connection?.GetDbSetting());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="reader"></param>
        /// <param name="dbFields"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static Func<DbDataReader, TResult> CompileDataReaderToDataEntity<TResult>(DbDataReader reader,
            IEnumerable<DbField> dbFields,
            IDbSetting dbSetting)
        {
            var readerParameterExpression = Expression.Parameter(StaticType.DbDataReader, "reader");
            var readerFields = GetDataReaderFields(reader, dbFields, dbSetting);
            var memberBindings = GetMemberBindingsForDataEntity<TResult>(readerParameterExpression,
                readerFields, dbSetting);
            var memberAssignments = memberBindings?
                .Where(item => item.MemberAssignment != null)
                .Select(item => item.MemberAssignment);
            var arguments = memberBindings?
                .Where(item => item.Argument != null)
                .Select(item => item.Argument);
            var typeOfResult = typeof(TResult);

            // Throw an error if there are no bindings
            if (arguments?.Any() != true && memberAssignments?.Any() != true)
            {
                throw new InvalidOperationException($"There are no 'contructor parameter' and/or 'property member' bindings found between the resultset of the data reader and the type '{typeOfResult.FullName}'.");
            }

            // Initialize the members
            var constructorInfo = typeOfResult
                .GetConstructors()?
                .Where(item => item.GetParameters().Length > 0)?
                .OrderByDescending(item => item.GetParameters().Length)?
                .FirstOrDefault();
            var entityExpression = (Expression)null;

            // Check the arguments
            entityExpression = arguments?.Any() == true ?
                Expression.New(constructorInfo, arguments) : Expression.New(typeOfResult);

            // Bind the members
            entityExpression = memberAssignments?.Any() == true ?
                (Expression)Expression.MemberInit((NewExpression)entityExpression, memberAssignments) : entityExpression;

            // Class handler
            entityExpression = ConvertValueExpressionToClassHandlerGetExpression<TResult>(entityExpression,
                readerParameterExpression);

            // Set the function value
            return Expression
                .Lambda<Func<DbDataReader, TResult>>(entityExpression, readerParameterExpression)
                .Compile();
        }
    }
}
