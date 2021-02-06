using System;
using System.Data.Common;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;
using RepoDb.Interfaces;
using RepoDb.Extensions;

namespace RepoDb.Reflection
{
    internal partial class Compiler
    {
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
                throw new InvalidOperationException($"There are no 'constructor parameter' and/or 'property member' bindings found between the resultset of the data reader and the type '{typeOfResult.FullName}'. " +
                    $"Make sure the 'constructor arguments' and/or 'model properties' are matching the list of the fields returned by the data reader object.");
            }

            // Initialize the members
            var constructorInfo = typeOfResult.GetConstructorWithMostArguments();
            var entityExpression = (Expression)null;

            // Validate arguments equality
            if (arguments?.Any() == true)
            {
                var parameters = constructorInfo.GetParameters();
                var unmatches = parameters
                    .Where(e => memberBindings.FirstOrDefault(binding => binding.Argument != null &&
                        string.Equals(binding.ParameterInfo?.Name, e.Name, StringComparison.OrdinalIgnoreCase)) == null);

                // Throw the detailed message
                if (unmatches?.Any() == true)
                {
                    var unmatchesNames = unmatches.Select(e => e.Name).Join(",");
                    throw new MissingMemberException($"The following ctor arguments ('{unmatchesNames}') for type '{typeOfResult.FullName}' are not matching from any of the resultset fields returned by the data reader object.");
                }
            }

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
