using System;
using System.Data.Common;
using System.Data;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;
using RepoDb.Interfaces;
using System.Threading.Tasks;
using System.Reflection;
using RepoDb.Extensions;
using RepoDb.Enumerations;
using RepoDb.Exceptions;

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
        internal static Func<DbDataReader, TResult> CompileDataReaderToType<TResult>(DbDataReader reader,
            IDbConnection connection,
            string connectionString,
            IDbTransaction transaction,
            bool enableValidation)
        {
            // Expression variables
            var typeOfResult = typeof(TResult);
            var dbFields = (IEnumerable<DbField>)null;

            // Check the type
            if (typeOfResult.IsClassType())
            {
                dbFields = GetDbFields(connection,
                    ClassMappedNameCache.Get<TResult>(),
                    connectionString,
                    transaction,
                    enableValidation);
            }

            // return the value
            return CompileDataReaderToType<TResult>(reader, dbFields, connection?.GetDbSetting());
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
        internal static async Task<Func<DbDataReader, TResult>> CompileDataReaderToTypeAsync<TResult>(DbDataReader reader,
            IDbConnection connection,
            string connectionString,
            IDbTransaction transaction,
            bool enableValidation)
        {
            // Expression variables
            var typeOfResult = typeof(TResult);
            var dbFields = (IEnumerable<DbField>)null;

            // Check the type
            if (typeOfResult.IsClassType())
            {
                dbFields = await GetDbFieldsAsync(connection,
                    ClassMappedNameCache.Get<TResult>(),
                    connectionString,
                    transaction,
                    enableValidation);
            }

            // return the value
            return CompileDataReaderToType<TResult>(reader, dbFields, connection?.GetDbSetting());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="reader"></param>
        /// <param name="dbFields"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static Func<DbDataReader, TResult> CompileDataReaderToType<TResult>(DbDataReader reader,
            IEnumerable<DbField> dbFields,
            IDbSetting dbSetting)
        {
            var typeOfResult = typeof(TResult);

            // EntityModel/Class
            if (typeOfResult.IsClassType())
            {
                return CompileDataReaderToDataEntity<TResult>(reader, dbFields, dbSetting);
            }

            // .NET CLR Type
            else
            {
                return CompileDataReaderToTargetType<TResult>(reader, dbSetting);
            }

            // Throw an exception
            throw new InvalidOperationException($"No compiled expression found for '{typeOfResult.FullName}' type.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="reader"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static Func<DbDataReader, TResult> CompileDataReaderToTargetType<TResult>(DbDataReader reader,
            IDbSetting dbSetting)
        {
            var typeOfResult = typeof(TResult);

            // Check the field count
            if (reader.FieldCount != 1)
            {
                throw new InvalidOperationException($"The number of fields from the instance of '{StaticType.DbDataReader.FullName}' object must only be 1 when extracting to '{typeOfResult.FullName}' type.");
            }

            // Variables
            var readerParameterExpression = Expression.Parameter(StaticType.DbDataReader, "reader");
            var readerField = GetDataReaderFields(reader, dbSetting).First();
            var classPropertyParameterInfo = new ClassPropertyParameterInfo { TargetType = typeOfResult };
            var expression = GetClassPropertyParameterInfoValueExpression(readerParameterExpression,
                classPropertyParameterInfo, readerField);

            // Return
            return Expression
                .Lambda<Func<DbDataReader, TResult>>(expression, readerParameterExpression)
                .Compile();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="reader"></param>
        /// <param name="dbFields"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static Func<DbDataReader, TResult> CompileDataReaderToDataEntity<TResult>(DbDataReader reader,
            IEnumerable<DbField> dbFields,
            IDbSetting dbSetting)
        {
            var readerParameterExpression = Expression.Parameter(StaticType.DbDataReader, "reader");
            var readerFields = GetDataReaderFields(reader, dbFields, dbSetting);
            var memberBindings = GetMemberBindingsForDataEntity<TResult>(readerParameterExpression,
                readerFields, dbSetting);
            var memberAssignments = memberBindings?.Where(item => item.MemberAssignment != null).Select(item => item.MemberAssignment);
            var arguments = memberBindings?.Where(item => item.Argument != null).Select(item => item.Argument);
            var typeOfResult = typeof(TResult);

            // Throw an error if there are no bindings
            if (arguments?.Any() != true && memberAssignments?.Any() != true)
            {
                throw new InvalidOperationException($"There are no 'contructor parameter' and/or 'property member' bindings found between the resultset of the data reader and the type '{typeOfResult.FullName}'.");
            }

            // Initialize the members
            var constructorInfo = typeOfResult.GetConstructorWithMostArguments();
            var entityExpression = (Expression)null;

            try
            {
                // Constructor arguments
                entityExpression = arguments?.Any() == true ?
                    Expression.New(constructorInfo, arguments) : Expression.New(typeOfResult);

                // Bind the members
                entityExpression = memberAssignments?.Any() == true ?
                    Expression.MemberInit((NewExpression)entityExpression, memberAssignments) : entityExpression;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Compiler: Failed to initialize the member properties or the constructor arguments.", ex);
            }

            // Class handler
            entityExpression = ConvertExpressionToClassHandlerGetExpression<TResult>(entityExpression,
                    readerParameterExpression);

            // Set the function value
            return Expression
                .Lambda<Func<DbDataReader, TResult>>(entityExpression, readerParameterExpression)
                .Compile();
        }
    }
}
