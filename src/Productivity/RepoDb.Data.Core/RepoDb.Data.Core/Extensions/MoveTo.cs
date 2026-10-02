#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using RepoDb.Schema;

namespace RepoDb.Data
{
    /// <summary>
    /// Contains the extension methods of <see cref="IDbConnection"/> object for moving data between databases.
    /// </summary>
    public static class MoveToExtension
    {
        #region MoveTo (Registered Connection, Sync)

        /// <summary>
        /// Moves all the rows of the table mapped to the <typeparamref name="TEntity"/> type of the current connection
        /// into the same-named table of the destination connection that is registered in the <see cref="ConnectionManager"/>.
        /// The destination table must already exist. The rows of the source table are left untouched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="schemaProvider">The schema provider to be used. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the destination table.</returns>
        public static int MoveTo<TEntity>(this IDbConnection connection,
            string destinationConnection,
            int batchSize = 1000,
            ISchemaProvider schemaProvider = null)
            where TEntity : class =>
            connection.MoveTo<TEntity>(ConnectionManager.Get(destinationConnection), batchSize, schemaProvider);

        /// <summary>
        /// Moves all the rows of the table mapped to the <typeparamref name="TEntity"/> type of the current connection
        /// into the target table of the destination connection that is registered in the <see cref="ConnectionManager"/>.
        /// The target table must already exist and have the same column names as the source table.
        /// The rows of the source table are left untouched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the source table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="targetTable">The name of the table in the destination database that receives the rows.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="schemaProvider">The schema provider to be used. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the target table.</returns>
        public static int MoveTo<TEntity>(this IDbConnection connection,
            string targetTable,
            string destinationConnection,
            int batchSize = 1000,
            ISchemaProvider schemaProvider = null)
            where TEntity : class =>
            connection.MoveTo<TEntity>(targetTable, ConnectionManager.Get(destinationConnection), batchSize, schemaProvider);

        /// <summary>
        /// Moves all the rows of the source table of the current connection into the same-named table of the destination connection
        /// that is registered in the <see cref="ConnectionManager"/>.
        /// The destination table must already exist. The rows of the source table are left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the table to be moved.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="schemaProvider">The schema provider to be used. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the destination table.</returns>
        public static int MoveTo(this IDbConnection connection,
            string tableName,
            string destinationConnection,
            int batchSize = 1000,
            ISchemaProvider schemaProvider = null) =>
            connection.MoveTo(tableName, ConnectionManager.Get(destinationConnection), batchSize, schemaProvider);

        /// <summary>
        /// Moves all the rows of the source table of the current connection into the target table of the destination connection
        /// that is registered in the <see cref="ConnectionManager"/>.
        /// The target table must already exist and have the same column names as the source table.
        /// The rows of the source table are left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="sourceTable">The name of the table to be moved.</param>
        /// <param name="targetTable">The name of the table in the destination database that receives the rows.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="schemaProvider">The schema provider to be used. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the target table.</returns>
        public static int MoveTo(this IDbConnection connection,
            string sourceTable,
            string targetTable,
            string destinationConnection,
            int batchSize = 1000,
            ISchemaProvider schemaProvider = null) =>
            connection.MoveTo(sourceTable, targetTable, ConnectionManager.Get(destinationConnection), batchSize, schemaProvider);

        #endregion

        #region MoveTo (IDbConnection, Sync)

        /// <summary>
        /// Moves all the rows of the table mapped to the <typeparamref name="TEntity"/> type of the current connection
        /// into the same-named table of the destination connection.
        /// The destination table must already exist. The rows of the source table are left untouched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="schemaProvider">The schema provider to be used. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the destination table.</returns>
        public static int MoveTo<TEntity>(this IDbConnection connection,
            IDbConnection destinationConnection,
            int batchSize = 1000,
            ISchemaProvider schemaProvider = null)
            where TEntity : class =>
            connection.MoveTo(ClassMappedNameCache.Get<TEntity>(), destinationConnection, batchSize, schemaProvider);

        /// <summary>
        /// Moves all the rows of the table mapped to the <typeparamref name="TEntity"/> type of the current connection
        /// into the target table of the destination connection.
        /// The target table must already exist and have the same column names as the source table.
        /// The rows of the source table are left untouched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the source table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="targetTable">The name of the table in the destination database that receives the rows.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="schemaProvider">The schema provider to be used. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the target table.</returns>
        public static int MoveTo<TEntity>(this IDbConnection connection,
            string targetTable,
            IDbConnection destinationConnection,
            int batchSize = 1000,
            ISchemaProvider schemaProvider = null)
            where TEntity : class =>
            connection.MoveTo(ClassMappedNameCache.Get<TEntity>(), targetTable, destinationConnection, batchSize, schemaProvider);

        /// <summary>
        /// Moves all the rows of the source table of the current connection into the same-named table of the destination connection.
        /// The destination table must already exist. The rows of the source table are left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the table to be moved.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="schemaProvider">The schema provider to be used. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the destination table.</returns>
        public static int MoveTo(this IDbConnection connection,
            string tableName,
            IDbConnection destinationConnection,
            int batchSize = 1000,
            ISchemaProvider schemaProvider = null) =>
            connection.MoveTo(tableName, tableName, destinationConnection, batchSize, schemaProvider);

        /// <summary>
        /// Moves all the rows of the source table of the current connection into the target table of the destination connection.
        /// The target table must already exist and have the same column names as the source table.
        /// The rows of the source table are left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="sourceTable">The name of the table to be moved.</param>
        /// <param name="targetTable">The name of the table in the destination database that receives the rows.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="schemaProvider">The schema provider to be used. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the target table.</returns>
        public static int MoveTo(this IDbConnection connection,
            string sourceTable,
            string targetTable,
            IDbConnection destinationConnection,
            int batchSize = 1000,
            ISchemaProvider schemaProvider = null)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (string.IsNullOrWhiteSpace(sourceTable))
            {
                throw new ArgumentNullException(nameof(sourceTable));
            }
            if (string.IsNullOrWhiteSpace(targetTable))
            {
                throw new ArgumentNullException(nameof(targetTable));
            }
            if (destinationConnection == null)
            {
                throw new ArgumentNullException(nameof(destinationConnection));
            }
            if (batchSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize), "The batch size must be greater than zero.");
            }

            var inserted = 0;
            var batch = new List<object>(batchSize);

            foreach (object row in connection.QueryAll(sourceTable))
            {
                batch.Add(row);
                if (batch.Count == batchSize)
                {
                    inserted += destinationConnection.InsertAll(targetTable, batch, batchSize);
                    batch.Clear();
                }
            }

            if (batch.Count > 0)
            {
                inserted += destinationConnection.InsertAll(targetTable, batch, batchSize);
            }
            return inserted;
        }

        #endregion

    }
}
