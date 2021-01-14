using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public partial class DbRepository<TDbConnection> : IDisposable
        where TDbConnection : DbConnection
    {
        #region QueryMultiple<TEntity>

        #region T1, T2

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 2 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <param name="where1">The query expression to be used (at T1).</param>
        /// <param name="where2">The query expression to be used (at T2).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (at T1).</param>
        /// <param name="top1">The number of rows to be returned (at T1).</param>
        /// <param name="hints1">The table hints to be used (at T1).</param>
        /// <param name="orderBy2">The order definition of the fields to be used (at T2).</param>
        /// <param name="top2">The number of rows to be returned (at T2).</param>
        /// <param name="hints2">The table hints to be used (at T2).</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>A tuple of 2 enumerable target data entity types.</returns>
        public Tuple<IEnumerable<T1>, IEnumerable<T2>> QueryMultiple<T1, T2>(Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            int? top2 = 0,
            IEnumerable<OrderField> orderBy2 = null,
            string hints2 = null,
            IDbTransaction transaction = null)
            where T1 : class
            where T2 : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.QueryMultiple<T1, T2>(where1: where1,
                    where2: where2,
                    orderBy1: orderBy1,
                    top1: top1,
                    hints1: hints1,
                    top2: top2,
                    orderBy2: orderBy2,
                    hints2: hints2,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        #endregion

        #region T1, T2, T3

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 3 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <param name="where1">The query expression to be used (at T1).</param>
        /// <param name="where2">The query expression to be used (at T2).</param>
        /// <param name="where3">The query expression to be used (at T3).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (at T1).</param>
        /// <param name="top1">The number of rows to be returned (at T1).</param>
        /// <param name="hints1">The table hints to be used (at T1).</param>
        /// <param name="orderBy2">The order definition of the fields to be used (at T2).</param>
        /// <param name="top2">The number of rows to be returned (at T2).</param>
        /// <param name="hints2">The table hints to be used (at T2).</param>
        /// <param name="orderBy3">The order definition of the fields to be used (at T3).</param>
        /// <param name="top3">The number of rows to be returned (at T3).</param>
        /// <param name="hints3">The table hints to be used (at T3).</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>A tuple of 3 enumerable target data entity types.</returns>
        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> QueryMultiple<T1, T2, T3>(Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IDbTransaction transaction = null)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.QueryMultiple<T1, T2, T3>(where1: where1,
                    where2: where2,
                    where3: where3,
                    orderBy1: orderBy1,
                    top1: top1,
                    hints1: hints1,
                    orderBy2: orderBy2,
                    top2: top2,
                    hints2: hints2,
                    orderBy3: orderBy3,
                    top3: top3,
                    hints3: hints3,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        #endregion

        #region T1, T2, T3, T4

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 4 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <param name="where1">The query expression to be used (at T1).</param>
        /// <param name="where2">The query expression to be used (at T2).</param>
        /// <param name="where3">The query expression to be used (at T3).</param>
        /// <param name="where4">The query expression to be used (at T4).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (at T1).</param>
        /// <param name="top1">The number of rows to be returned (at T1).</param>
        /// <param name="hints1">The table hints to be used (at T1).</param>
        /// <param name="orderBy2">The order definition of the fields to be used (at T2).</param>
        /// <param name="top2">The number of rows to be returned (at T2).</param>
        /// <param name="hints2">The table hints to be used (at T2).</param>
        /// <param name="orderBy3">The order definition of the fields to be used (at T3).</param>
        /// <param name="top3">The number of rows to be returned (at T3).</param>
        /// <param name="hints3">The table hints to be used (at T3).</param>
        /// <param name="orderBy4">The order definition of the fields to be used (at T4).</param>
        /// <param name="top4">The number of rows to be returned (at T4).</param>
        /// <param name="hints4">The table hints to be used (at T4).</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>A tuple of 4 enumerable target data entity types.</returns>
        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>
            QueryMultiple<T1, T2, T3, T4>(Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            IDbTransaction transaction = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.QueryMultiple<T1, T2, T3, T4>(where1: where1,
                    where2: where2,
                    where3: where3,
                    where4: where4,
                    orderBy1: orderBy1,
                    top1: top1,
                    hints1: hints1,
                    orderBy2: orderBy2,
                    top2: top2,
                    hints2: hints2,
                    orderBy3: orderBy3,
                    top3: top3,
                    hints3: hints3,
                    orderBy4: orderBy4,
                    top4: top4,
                    hints4: hints4,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        #endregion

        #region T1, T2, T3, T4, T5

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 5 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <param name="where1">The query expression to be used (at T1).</param>
        /// <param name="where2">The query expression to be used (at T2).</param>
        /// <param name="where3">The query expression to be used (at T3).</param>
        /// <param name="where4">The query expression to be used (at T4).</param>
        /// <param name="where5">The query expression to be used (at T5).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (at T1).</param>
        /// <param name="top1">The number of rows to be returned (at T1).</param>
        /// <param name="hints1">The table hints to be used (at T1).</param>
        /// <param name="orderBy2">The order definition of the fields to be used (at T2).</param>
        /// <param name="top2">The number of rows to be returned (at T2).</param>
        /// <param name="hints2">The table hints to be used (at T2).</param>
        /// <param name="orderBy3">The order definition of the fields to be used (at T3).</param>
        /// <param name="top3">The number of rows to be returned (at T3).</param>
        /// <param name="hints3">The table hints to be used (at T3).</param>
        /// <param name="orderBy4">The order definition of the fields to be used (at T4).</param>
        /// <param name="top4">The number of rows to be returned (at T4).</param>
        /// <param name="hints4">The table hints to be used (at T4).</param>
        /// <param name="orderBy5">The order definition of the fields to be used (at T5).</param>
        /// <param name="top5">The number of rows to be returned (at T5).</param>
        /// <param name="hints5">The table hints to be used (at T5).</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>A tuple of 5 enumerable target data entity types.</returns>
        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>
            QueryMultiple<T1, T2, T3, T4, T5>(Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            Expression<Func<T5, bool>> where5,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            IDbTransaction transaction = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.QueryMultiple<T1, T2, T3, T4, T5>(where1: where1,
                    where2: where2,
                    where3: where3,
                    where4: where4,
                    where5: where5,
                    orderBy1: orderBy1,
                    top1: top1,
                    hints1: hints1,
                    orderBy2: orderBy2,
                    top2: top2,
                    hints2: hints2,
                    orderBy3: orderBy3,
                    top3: top3,
                    hints3: hints3,
                    orderBy4: orderBy4,
                    top4: top4,
                    hints4: hints4,
                    orderBy5: orderBy5,
                    top5: top5,
                    hints5: hints5,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        #endregion

        #region T1, T2, T3, T4, T5, T6

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 6 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <param name="where1">The query expression to be used (at T1).</param>
        /// <param name="where2">The query expression to be used (at T2).</param>
        /// <param name="where3">The query expression to be used (at T3).</param>
        /// <param name="where4">The query expression to be used (at T4).</param>
        /// <param name="where5">The query expression to be used (at T5).</param>
        /// <param name="where6">The query expression to be used (at T6).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (at T1).</param>
        /// <param name="top1">The number of rows to be returned (at T1).</param>
        /// <param name="hints1">The table hints to be used (at T1).</param>
        /// <param name="orderBy2">The order definition of the fields to be used (at T2).</param>
        /// <param name="top2">The number of rows to be returned (at T2).</param>
        /// <param name="hints2">The table hints to be used (at T2).</param>
        /// <param name="orderBy3">The order definition of the fields to be used (at T3).</param>
        /// <param name="top3">The number of rows to be returned (at T3).</param>
        /// <param name="hints3">The table hints to be used (at T3).</param>
        /// <param name="orderBy4">The order definition of the fields to be used (at T4).</param>
        /// <param name="top4">The number of rows to be returned (at T4).</param>
        /// <param name="hints4">The table hints to be used (at T4).</param>
        /// <param name="orderBy5">The order definition of the fields to be used (at T5).</param>
        /// <param name="top5">The number of rows to be returned (at T5).</param>
        /// <param name="hints5">The table hints to be used (at T5).</param>
        /// <param name="orderBy6">The order definition of the fields to be used (at T6).</param>
        /// <param name="top6">The number of rows to be returned (at T6).</param>
        /// <param name="hints6">The table hints to be used (at T6).</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>A tuple of 6 enumerable target data entity types.</returns>
        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>
            QueryMultiple<T1, T2, T3, T4, T5, T6>(Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            Expression<Func<T5, bool>> where5,
            Expression<Func<T6, bool>> where6,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            IDbTransaction transaction = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.QueryMultiple<T1, T2, T3, T4, T5, T6>(where1: where1,
                    where2: where2,
                    where3: where3,
                    where4: where4,
                    where5: where5,
                    where6: where6,
                    orderBy1: orderBy1,
                    top1: top1,
                    hints1: hints1,
                    orderBy2: orderBy2,
                    top2: top2,
                    hints2: hints2,
                    orderBy3: orderBy3,
                    top3: top3,
                    hints3: hints3,
                    orderBy4: orderBy4,
                    top4: top4,
                    hints4: hints4,
                    orderBy5: orderBy5,
                    top5: top5,
                    hints5: hints5,
                    orderBy6: orderBy6,
                    top6: top6,
                    hints6: hints6,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        #endregion

        #region T1, T2, T3, T4, T5, T6, T7

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 7 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <typeparam name="T7">The seventh target type.</typeparam>
        /// <param name="where1">The query expression to be used (at T1).</param>
        /// <param name="where2">The query expression to be used (at T2).</param>
        /// <param name="where3">The query expression to be used (at T3).</param>
        /// <param name="where4">The query expression to be used (at T4).</param>
        /// <param name="where5">The query expression to be used (at T5).</param>
        /// <param name="where6">The query expression to be used (at T6).</param>
        /// <param name="where7">The query expression to be used (at T7).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (at T1).</param>
        /// <param name="top1">The number of rows to be returned (at T1).</param>
        /// <param name="hints1">The table hints to be used (at T1).</param>
        /// <param name="orderBy2">The order definition of the fields to be used (at T2).</param>
        /// <param name="top2">The number of rows to be returned (at T2).</param>
        /// <param name="hints2">The table hints to be used (at T2).</param>
        /// <param name="orderBy3">The order definition of the fields to be used (at T3).</param>
        /// <param name="top3">The number of rows to be returned (at T3).</param>
        /// <param name="hints3">The table hints to be used (at T3).</param>
        /// <param name="orderBy4">The order definition of the fields to be used (at T4).</param>
        /// <param name="top4">The number of rows to be returned (at T4).</param>
        /// <param name="hints4">The table hints to be used (at T4).</param>
        /// <param name="orderBy5">The order definition of the fields to be used (at T5).</param>
        /// <param name="top5">The number of rows to be returned (at T5).</param>
        /// <param name="hints5">The table hints to be used (at T5).</param>
        /// <param name="orderBy6">The order definition of the fields to be used (at T6).</param>
        /// <param name="top6">The number of rows to be returned (at T6).</param>
        /// <param name="hints6">The table hints to be used (at T6).</param>
        /// <param name="orderBy7">The order definition of the fields to be used (at T7).</param>
        /// <param name="top7">The number of rows to be returned (at T7).</param>
        /// <param name="hints7">The table hints to be used (at T7).</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>A tuple of 7 enumerable target data entity types.</returns>
        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>
            QueryMultiple<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            Expression<Func<T5, bool>> where5,
            Expression<Func<T6, bool>> where6,
            Expression<Func<T7, bool>> where7,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            IEnumerable<OrderField> orderBy7 = null,
            int? top7 = 0,
            string hints7 = null,
            IDbTransaction transaction = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.QueryMultiple<T1, T2, T3, T4, T5, T6, T7>(where1: where1,
                    where2: where2,
                    where3: where3,
                    where4: where4,
                    where5: where5,
                    where6: where6,
                    where7: where7,
                    orderBy1: orderBy1,
                    top1: top1,
                    hints1: hints1,
                    orderBy2: orderBy2,
                    top2: top2,
                    hints2: hints2,
                    orderBy3: orderBy3,
                    top3: top3,
                    hints3: hints3,
                    orderBy4: orderBy4,
                    top4: top4,
                    hints4: hints4,
                    orderBy5: orderBy5,
                    top5: top5,
                    hints5: hints5,
                    orderBy6: orderBy6,
                    top6: top6,
                    hints6: hints6,
                    orderBy7: orderBy7,
                    top7: top7,
                    hints7: hints7,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        #endregion

        #endregion

        #region QueryMultipleAsync<TEntity>

        #region T1, T2

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 2 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <param name="where1">The query expression to be used (at T1).</param>
        /// <param name="where2">The query expression to be used (at T2).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (at T1).</param>
        /// <param name="top1">The number of rows to be returned (at T1).</param>
        /// <param name="hints1">The table hints to be used (at T1).</param>
        /// <param name="orderBy2">The order definition of the fields to be used (at T2).</param>
        /// <param name="top2">The number of rows to be returned (at T2).</param>
        /// <param name="hints2">The table hints to be used (at T2).</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 2 enumerable target data entity types.</returns>
        public async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>>> QueryMultipleAsync<T1, T2>(Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            int? top2 = 0,
            IEnumerable<OrderField> orderBy2 = null,
            string hints2 = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.QueryMultipleAsync<T1, T2>(where1: where1,
                    where2: where2,
                    orderBy1: orderBy1,
                    top1: top1,
                    hints1: hints1,
                    top2: top2,
                    orderBy2: orderBy2,
                    hints2: hints2,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        #endregion

        #region T1, T2, T3

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 3 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <param name="where1">The query expression to be used (at T1).</param>
        /// <param name="where2">The query expression to be used (at T2).</param>
        /// <param name="where3">The query expression to be used (at T3).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (at T1).</param>
        /// <param name="top1">The number of rows to be returned (at T1).</param>
        /// <param name="hints1">The table hints to be used (at T1).</param>
        /// <param name="orderBy2">The order definition of the fields to be used (at T2).</param>
        /// <param name="top2">The number of rows to be returned (at T2).</param>
        /// <param name="hints2">The table hints to be used (at T2).</param>
        /// <param name="orderBy3">The order definition of the fields to be used (at T3).</param>
        /// <param name="top3">The number of rows to be returned (at T3).</param>
        /// <param name="hints3">The table hints to be used (at T3).</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 3 enumerable target data entity types.</returns>
        public async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>> QueryMultipleAsync<T1, T2, T3>(Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.QueryMultipleAsync<T1, T2, T3>(where1: where1,
                    where2: where2,
                    where3: where3,
                    orderBy1: orderBy1,
                    top1: top1,
                    hints1: hints1,
                    orderBy2: orderBy2,
                    top2: top2,
                    hints2: hints2,
                    orderBy3: orderBy3,
                    top3: top3,
                    hints3: hints3,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        #endregion

        #region T1, T2, T3, T4

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 4 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <param name="where1">The query expression to be used (at T1).</param>
        /// <param name="where2">The query expression to be used (at T2).</param>
        /// <param name="where3">The query expression to be used (at T3).</param>
        /// <param name="where4">The query expression to be used (at T4).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (at T1).</param>
        /// <param name="top1">The number of rows to be returned (at T1).</param>
        /// <param name="hints1">The table hints to be used (at T1).</param>
        /// <param name="orderBy2">The order definition of the fields to be used (at T2).</param>
        /// <param name="top2">The number of rows to be returned (at T2).</param>
        /// <param name="hints2">The table hints to be used (at T2).</param>
        /// <param name="orderBy3">The order definition of the fields to be used (at T3).</param>
        /// <param name="top3">The number of rows to be returned (at T3).</param>
        /// <param name="hints3">The table hints to be used (at T3).</param>
        /// <param name="orderBy4">The order definition of the fields to be used (at T4).</param>
        /// <param name="top4">The number of rows to be returned (at T4).</param>
        /// <param name="hints4">The table hints to be used (at T4).</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 4 enumerable target data entity types.</returns>
        public async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>>
            QueryMultipleAsync<T1, T2, T3, T4>(Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.QueryMultipleAsync<T1, T2, T3, T4>(where1: where1,
                    where2: where2,
                    where3: where3,
                    where4: where4,
                    orderBy1: orderBy1,
                    top1: top1,
                    hints1: hints1,
                    orderBy2: orderBy2,
                    top2: top2,
                    hints2: hints2,
                    orderBy3: orderBy3,
                    top3: top3,
                    hints3: hints3,
                    orderBy4: orderBy4,
                    top4: top4,
                    hints4: hints4,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        #endregion

        #region T1, T2, T3, T4, T5

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 5 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <param name="where1">The query expression to be used (at T1).</param>
        /// <param name="where2">The query expression to be used (at T2).</param>
        /// <param name="where3">The query expression to be used (at T3).</param>
        /// <param name="where4">The query expression to be used (at T4).</param>
        /// <param name="where5">The query expression to be used (at T5).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (at T1).</param>
        /// <param name="top1">The number of rows to be returned (at T1).</param>
        /// <param name="hints1">The table hints to be used (at T1).</param>
        /// <param name="orderBy2">The order definition of the fields to be used (at T2).</param>
        /// <param name="top2">The number of rows to be returned (at T2).</param>
        /// <param name="hints2">The table hints to be used (at T2).</param>
        /// <param name="orderBy3">The order definition of the fields to be used (at T3).</param>
        /// <param name="top3">The number of rows to be returned (at T3).</param>
        /// <param name="hints3">The table hints to be used (at T3).</param>
        /// <param name="orderBy4">The order definition of the fields to be used (at T4).</param>
        /// <param name="top4">The number of rows to be returned (at T4).</param>
        /// <param name="hints4">The table hints to be used (at T4).</param>
        /// <param name="orderBy5">The order definition of the fields to be used (at T5).</param>
        /// <param name="top5">The number of rows to be returned (at T5).</param>
        /// <param name="hints5">The table hints to be used (at T5).</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 5 enumerable target data entity types.</returns>
        public async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>>
            QueryMultipleAsync<T1, T2, T3, T4, T5>(Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            Expression<Func<T5, bool>> where5,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.QueryMultipleAsync<T1, T2, T3, T4, T5>(where1: where1,
                    where2: where2,
                    where3: where3,
                    where4: where4,
                    where5: where5,
                    orderBy1: orderBy1,
                    top1: top1,
                    hints1: hints1,
                    orderBy2: orderBy2,
                    top2: top2,
                    hints2: hints2,
                    orderBy3: orderBy3,
                    top3: top3,
                    hints3: hints3,
                    orderBy4: orderBy4,
                    top4: top4,
                    hints4: hints4,
                    orderBy5: orderBy5,
                    top5: top5,
                    hints5: hints5,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        #endregion

        #region T1, T2, T3, T4, T5, T6

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 6 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <param name="where1">The query expression to be used (at T1).</param>
        /// <param name="where2">The query expression to be used (at T2).</param>
        /// <param name="where3">The query expression to be used (at T3).</param>
        /// <param name="where4">The query expression to be used (at T4).</param>
        /// <param name="where5">The query expression to be used (at T5).</param>
        /// <param name="where6">The query expression to be used (at T6).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (at T1).</param>
        /// <param name="top1">The number of rows to be returned (at T1).</param>
        /// <param name="hints1">The table hints to be used (at T1).</param>
        /// <param name="orderBy2">The order definition of the fields to be used (at T2).</param>
        /// <param name="top2">The number of rows to be returned (at T2).</param>
        /// <param name="hints2">The table hints to be used (at T2).</param>
        /// <param name="orderBy3">The order definition of the fields to be used (at T3).</param>
        /// <param name="top3">The number of rows to be returned (at T3).</param>
        /// <param name="hints3">The table hints to be used (at T3).</param>
        /// <param name="orderBy4">The order definition of the fields to be used (at T4).</param>
        /// <param name="top4">The number of rows to be returned (at T4).</param>
        /// <param name="hints4">The table hints to be used (at T4).</param>
        /// <param name="orderBy5">The order definition of the fields to be used (at T5).</param>
        /// <param name="top5">The number of rows to be returned (at T5).</param>
        /// <param name="hints5">The table hints to be used (at T5).</param>
        /// <param name="orderBy6">The order definition of the fields to be used (at T6).</param>
        /// <param name="top6">The number of rows to be returned (at T6).</param>
        /// <param name="hints6">The table hints to be used (at T6).</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 6 enumerable target data entity types.</returns>
        public async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>>
            QueryMultipleAsync<T1, T2, T3, T4, T5, T6>(Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            Expression<Func<T5, bool>> where5,
            Expression<Func<T6, bool>> where6,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.QueryMultipleAsync<T1, T2, T3, T4, T5, T6>(where1: where1,
                    where2: where2,
                    where3: where3,
                    where4: where4,
                    where5: where5,
                    where6: where6,
                    orderBy1: orderBy1,
                    top1: top1,
                    hints1: hints1,
                    orderBy2: orderBy2,
                    top2: top2,
                    hints2: hints2,
                    orderBy3: orderBy3,
                    top3: top3,
                    hints3: hints3,
                    orderBy4: orderBy4,
                    top4: top4,
                    hints4: hints4,
                    orderBy5: orderBy5,
                    top5: top5,
                    hints5: hints5,
                    orderBy6: orderBy6,
                    top6: top6,
                    hints6: hints6,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        #endregion

        #region T1, T2, T3, T4, T5, T6, T7

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 7 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <typeparam name="T7">The seventh target type.</typeparam>
        /// <param name="where1">The query expression to be used (at T1).</param>
        /// <param name="where2">The query expression to be used (at T2).</param>
        /// <param name="where3">The query expression to be used (at T3).</param>
        /// <param name="where4">The query expression to be used (at T4).</param>
        /// <param name="where5">The query expression to be used (at T5).</param>
        /// <param name="where6">The query expression to be used (at T6).</param>
        /// <param name="where7">The query expression to be used (at T7).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (at T1).</param>
        /// <param name="top1">The number of rows to be returned (at T1).</param>
        /// <param name="hints1">The table hints to be used (at T1).</param>
        /// <param name="orderBy2">The order definition of the fields to be used (at T2).</param>
        /// <param name="top2">The number of rows to be returned (at T2).</param>
        /// <param name="hints2">The table hints to be used (at T2).</param>
        /// <param name="orderBy3">The order definition of the fields to be used (at T3).</param>
        /// <param name="top3">The number of rows to be returned (at T3).</param>
        /// <param name="hints3">The table hints to be used (at T3).</param>
        /// <param name="orderBy4">The order definition of the fields to be used (at T4).</param>
        /// <param name="top4">The number of rows to be returned (at T4).</param>
        /// <param name="hints4">The table hints to be used (at T4).</param>
        /// <param name="orderBy5">The order definition of the fields to be used (at T5).</param>
        /// <param name="top5">The number of rows to be returned (at T5).</param>
        /// <param name="hints5">The table hints to be used (at T5).</param>
        /// <param name="orderBy6">The order definition of the fields to be used (at T6).</param>
        /// <param name="top6">The number of rows to be returned (at T6).</param>
        /// <param name="hints6">The table hints to be used (at T6).</param>
        /// <param name="orderBy7">The order definition of the fields to be used (at T7).</param>
        /// <param name="top7">The number of rows to be returned (at T7).</param>
        /// <param name="hints7">The table hints to be used (at T7).</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 7 enumerable target data entity types.</returns>
        public async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>>
            QueryMultipleAsync<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            Expression<Func<T5, bool>> where5,
            Expression<Func<T6, bool>> where6,
            Expression<Func<T7, bool>> where7,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            IEnumerable<OrderField> orderBy7 = null,
            int? top7 = 0,
            string hints7 = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.QueryMultipleAsync<T1, T2, T3, T4, T5, T6, T7>(where1: where1,
                    where2: where2,
                    where3: where3,
                    where4: where4,
                    where5: where5,
                    where6: where6,
                    where7: where7,
                    orderBy1: orderBy1,
                    top1: top1,
                    hints1: hints1,
                    orderBy2: orderBy2,
                    top2: top2,
                    hints2: hints2,
                    orderBy3: orderBy3,
                    top3: top3,
                    hints3: hints3,
                    orderBy4: orderBy4,
                    top4: top4,
                    hints4: hints4,
                    orderBy5: orderBy5,
                    top5: top5,
                    hints5: hints5,
                    orderBy6: orderBy6,
                    top6: top6,
                    hints6: hints6,
                    orderBy7: orderBy7,
                    top7: top7,
                    hints7: hints7,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        #endregion

        #endregion
    }
}
