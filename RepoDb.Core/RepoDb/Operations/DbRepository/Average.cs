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
        where TDbConnection : DbConnection, new()
    {

        #region Average<TEntity>

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public double Average<TEntity>(Field field,
            object where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average<TEntity>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public double Average<TEntity>(Field field,
            Expression<Func<TEntity, bool>> where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average<TEntity>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public double Average<TEntity>(Field field,
            QueryField where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average<TEntity>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public double Average<TEntity>(Field field,
            IEnumerable<QueryField> where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average<TEntity>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public double Average<TEntity>(Field field,
            QueryGroup where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average<TEntity>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public double Average<TEntity>(Expression<Func<TEntity, object>> field,
            object where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average<TEntity>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public double Average<TEntity>(Expression<Func<TEntity, object>> field,
            Expression<Func<TEntity, bool>> where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average<TEntity>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public double Average<TEntity>(Expression<Func<TEntity, object>> field,
            QueryField where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average<TEntity>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public double Average<TEntity>(Expression<Func<TEntity, object>> field,
            IEnumerable<QueryField> where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average<TEntity>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public double Average<TEntity>(Expression<Func<TEntity, object>> field,
            QueryGroup where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average<TEntity>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<double> AverageAsync<TEntity>(Field field,
            object where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync<TEntity>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<double> AverageAsync<TEntity>(Field field,
            Expression<Func<TEntity, bool>> where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync<TEntity>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<double> AverageAsync<TEntity>(Field field,
            QueryField where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync<TEntity>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<double> AverageAsync<TEntity>(Field field,
            IEnumerable<QueryField> where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync<TEntity>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<double> AverageAsync<TEntity>(Field field,
            QueryGroup where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync<TEntity>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<double> AverageAsync<TEntity>(Expression<Func<TEntity, object>> field,
            object where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync<TEntity>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<double> AverageAsync<TEntity>(Expression<Func<TEntity, object>> field,
            Expression<Func<TEntity, bool>> where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync<TEntity>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<double> AverageAsync<TEntity>(Expression<Func<TEntity, object>> field,
            QueryField where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync<TEntity>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<double> AverageAsync<TEntity>(Expression<Func<TEntity, object>> field,
            IEnumerable<QueryField> where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync<TEntity>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<double> AverageAsync<TEntity>(Expression<Func<TEntity, object>> field,
            QueryGroup where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync<TEntity>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        #region Average<TEntity, TResult>

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TEntity, TResult>(Field field,
            object where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average<TEntity, TResult>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TEntity, TResult>(Field field,
            Expression<Func<TEntity, bool>> where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average<TEntity, TResult>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TEntity, TResult>(Field field,
            QueryField where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average<TEntity, TResult>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TEntity, TResult>(Field field,
            IEnumerable<QueryField> where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average<TEntity, TResult>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TEntity, TResult>(Field field,
            QueryGroup where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average<TEntity, TResult>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TEntity, TResult>(Expression<Func<TEntity, TResult>> field,
            object where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average<TEntity, TResult>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TEntity, TResult>(Expression<Func<TEntity, TResult>> field,
            Expression<Func<TEntity, bool>> where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average<TEntity, TResult>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TEntity, TResult>(Expression<Func<TEntity, TResult>> field,
            QueryField where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average<TEntity, TResult>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TEntity, TResult>(Expression<Func<TEntity, TResult>> field,
            IEnumerable<QueryField> where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average<TEntity, TResult>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TEntity, TResult>(Expression<Func<TEntity, TResult>> field,
            QueryGroup where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average<TEntity, TResult>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<TResult> AverageAsync<TEntity, TResult>(Field field,
            object where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync<TEntity, TResult>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<TResult> AverageAsync<TEntity, TResult>(Field field,
            Expression<Func<TEntity, bool>> where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync<TEntity, TResult>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<TResult> AverageAsync<TEntity, TResult>(Field field,
            QueryField where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync<TEntity, TResult>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<TResult> AverageAsync<TEntity, TResult>(Field field,
            IEnumerable<QueryField> where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync<TEntity, TResult>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<TResult> AverageAsync<TEntity, TResult>(Field field,
            QueryGroup where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync<TEntity, TResult>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<TResult> AverageAsync<TEntity, TResult>(Expression<Func<TEntity, TResult>> field,
            object where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync<TEntity, TResult>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<TResult> AverageAsync<TEntity, TResult>(Expression<Func<TEntity, TResult>> field,
            Expression<Func<TEntity, bool>> where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync<TEntity, TResult>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<TResult> AverageAsync<TEntity, TResult>(Expression<Func<TEntity, TResult>> field,
            QueryField where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync<TEntity, TResult>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<TResult> AverageAsync<TEntity, TResult>(Expression<Func<TEntity, TResult>> field,
            IEnumerable<QueryField> where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync<TEntity, TResult>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<TResult> AverageAsync<TEntity, TResult>(Expression<Func<TEntity, TResult>> field,
            QueryGroup where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync<TEntity, TResult>(field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        #region Average(TableName)

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public double Average(string tableName,
            Field field,
            object where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average(tableName: tableName,
                    field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public double Average(string tableName,
            Field field,
            QueryField where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average(tableName: tableName,
                    field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public double Average(string tableName,
            Field field,
            IEnumerable<QueryField> where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average(tableName: tableName,
                    field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public double Average(string tableName,
            Field field,
            QueryGroup where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average(tableName: tableName,
                    field: field,
                    hints: hints,
                    where: where,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<double> AverageAsync(string tableName,
            Field field,
            object where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync(tableName: tableName,
                    field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<double> AverageAsync(string tableName,
            Field field,
            QueryField where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync(tableName: tableName,
                    field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<double> AverageAsync(string tableName,
            Field field,
            IEnumerable<QueryField> where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync(tableName: tableName,
                    field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<double> AverageAsync(string tableName,
            Field field,
            QueryGroup where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync(tableName: tableName,
                    field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        #region Average<TResult>(TableName)

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TResult>(string tableName,
            Field field,
            object where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average<TResult>(tableName: tableName,
                    field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TResult>(string tableName,
            Field field,
            QueryField where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average<TResult>(tableName: tableName,
                    field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TResult>(string tableName,
            Field field,
            IEnumerable<QueryField> where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average<TResult>(tableName: tableName,
                    field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TResult>(string tableName,
            Field field,
            QueryGroup where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Average<TResult>(tableName: tableName,
                    field: field,
                    hints: hints,
                    where: where,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<TResult> AverageAsync<TResult>(string tableName,
            Field field,
            object where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync<TResult>(tableName: tableName,
                    field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<TResult> AverageAsync<TResult>(string tableName,
            Field field,
            QueryField where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync<TResult>(tableName: tableName,
                    field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<TResult> AverageAsync<TResult>(string tableName,
            Field field,
            IEnumerable<QueryField> where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync<TResult>(tableName: tableName,
                    field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<TResult> AverageAsync<TResult>(string tableName,
            Field field,
            QueryGroup where,
            string hints = null,
			string traceKey = TraceKeys.Average,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.AverageAsync<TResult>(tableName: tableName,
                    field: field,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
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

    }
}
