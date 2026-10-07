#region Copyright Attributions

// Copyright (c) 2018 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Diagnostics.CodeAnalysis;
using RepoDb.Interfaces;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A static classed used to manipulate the <see cref="DbDataReader"/> object.
    /// </summary>
    public static class DataReader
    {
        #region ToEnumerable<TResult>

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of data entity objects.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="dbFields">The list of the <see cref="DbField"/> objects to be used.</param>
        /// <param name="dbSetting">The instance of <see cref="IDbSetting"/> object to be used.</param>
        /// <returns>A list of the target result type.</returns>
        public static IEnumerable<TResult> ToEnumerable<[DynamicallyAccessedMembers(Trimming.Entity)] TResult>(DbDataReader reader,
            DbFieldCollection dbFields = null,
            IDbSetting dbSetting = null)
        {
            if (reader?.IsClosed == false && reader.HasRows && reader.Read())
            {
                // Some providers resolve column types only after stepping onto the first row.
                var func = FunctionCache.GetDataReaderToTypeCompiledFunction<TResult>(reader,
                    dbFields,
                    dbSetting);
                do
                {
                    yield return func(reader);
                }
                while (reader.Read());
            }
        }

        #endregion

        #region ToEnumerableAsync<TResult>

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of data entity objects in asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="dbFields">The list of the <see cref="DbField"/> objects to be used.</param>
        /// <param name="dbSetting">The instance of <see cref="IDbSetting"/> object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A list of the target result type.</returns>
        public static async IAsyncEnumerable<TResult> ToEnumerableAsync<[DynamicallyAccessedMembers(Trimming.Entity)] TResult>(DbDataReader reader,
            DbFieldCollection dbFields = null,
            IDbSetting dbSetting = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (reader?.IsClosed != false || !reader.HasRows ||
                !await reader.ReadAsync(cancellationToken).ConfigureAwait(false)) yield break;
            
            var func = FunctionCache.GetDataReaderToTypeCompiledFunction<TResult>(reader,
                dbFields,
                dbSetting);
            
            do
            {
                yield return func(reader);
            }
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false));
        }

        #endregion

        #region ToEnumerable<dynamic>

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of dynamic objects.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="dbFields">The list of the <see cref="DbField"/> objects to be used.</param>
        /// <param name="dbSetting">The instance of <see cref="IDbSetting"/> object to be used.</param>
        /// <returns>An array of dynamic objects.</returns>
        public static IEnumerable<dynamic> ToEnumerable(DbDataReader reader,
            DbFieldCollection dbFields = null,
            IDbSetting dbSetting = null)
        {
            if (reader?.IsClosed == false && reader.HasRows && reader.Read())
            {
                var func = FunctionCache.GetDataReaderToExpandoObjectCompileFunction(reader,
                    dbFields,
                    dbSetting);
                do
                {
                    yield return func(reader);
                }
                while (reader.Read());
            }
        }

        #endregion

        #region ToEnumerableAsync<dynamic>

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of dynamic objects.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="dbFields">The list of the <see cref="DbField"/> objects to be used.</param>
        /// <param name="dbSetting">The instance of <see cref="IDbSetting"/> object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An array of dynamic objects.</returns>
        public static async IAsyncEnumerable<dynamic> ToEnumerableAsync(DbDataReader reader,
            DbFieldCollection dbFields = null,
            IDbSetting dbSetting = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (reader?.IsClosed != false || !reader.HasRows ||
                !await reader.ReadAsync(cancellationToken).ConfigureAwait(false)) yield break;
            
            var func = FunctionCache.GetDataReaderToExpandoObjectCompileFunction(reader,
                dbFields,
                dbSetting);
            
            do
            {
                yield return func(reader);
            }
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false));
        }

        #endregion
    }
}
