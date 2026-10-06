#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Data.Enumerations;
using RepoDb.Data.Interfaces;
using RepoDb.Data.Models;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using static RepoDb.Data.DbConnection;

namespace RepoDb.Data
{
    /// <summary>
    /// Contains the extension methods of <see cref="IDbConnection"/> object for copying data between databases.
    /// </summary>
    public static class CopyDataToExtension
    {
        #region Public Methods

        #region CopyDataTo (Registered Connection, Sync)

        /// <summary>
        /// Copies all the rows of the table mapped to the <typeparamref name="TEntity"/> type of the current connection
        /// into the same-named table of the destination connection that is registered in the <see cref="ConnectionManager"/>.
        /// The destination table must already exist, unless the <paramref name="relationshipBehavior"/> or the <paramref name="tableExistenceBehavior"/> asks for the schema to be copied first. The rows of the source table are left untouched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name (the schema of the target table can be different).</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name (the schema of the target table can be different). <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the destination table.</returns>
        public static int CopyDataTo<TEntity>(this IDbConnection connection,
            string destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int batchSize = 1000,
            CopyDataRelationshipBehavior relationshipBehavior = CopyDataRelationshipBehavior.TableOnly,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.Skip,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            connection.CopyDataTo(ConnectionManager.Get(destinationConnection), where, batchSize, relationshipBehavior, tableExistenceBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction);

        /// <summary>
        /// Copies all the rows of the table mapped to the <typeparamref name="TEntity"/> type of the current connection
        /// into the target table of the destination connection that is registered in the <see cref="ConnectionManager"/>.
        /// The target table must already exist and have the same column names as the source table, unless the <paramref name="relationshipBehavior"/> or the <paramref name="tableExistenceBehavior"/> asks for the schema to be copied first.
        /// The rows of the source table are left untouched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the source table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="targetTable">The name of the table in the destination database that receives the rows. When the schema is copied, the schema of the name (i.e.: <c>Sales.Person</c>) is the schema that the tables are created in; without a schema, the default schema of the destination database is used.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name (the schema of the target table can be different).</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name (the schema of the target table can be different). <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the target table.</returns>
        public static int CopyDataTo<TEntity>(this IDbConnection connection,
            string targetTable,
            string destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int batchSize = 1000,
            CopyDataRelationshipBehavior relationshipBehavior = CopyDataRelationshipBehavior.TableOnly,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.Skip,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            connection.CopyDataTo(targetTable, ConnectionManager.Get(destinationConnection), where, batchSize, relationshipBehavior, tableExistenceBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction);

        /// <summary>
        /// Copies all the rows of the source table of the current connection into the same-named table of the destination connection
        /// that is registered in the <see cref="ConnectionManager"/>.
        /// The destination table must already exist, unless the <paramref name="relationshipBehavior"/> or the <paramref name="tableExistenceBehavior"/> asks for the schema to be copied first. The rows of the source table are left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the table to be copied.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name (the schema of the target table can be different).</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name (the schema of the target table can be different). <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the destination table.</returns>
        public static int CopyDataTo(this IDbConnection connection,
            string tableName,
            string destinationConnection,
            QueryGroup where = null,
            int batchSize = 1000,
            CopyDataRelationshipBehavior relationshipBehavior = CopyDataRelationshipBehavior.TableOnly,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.Skip,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null) =>
            connection.CopyDataTo(tableName, ConnectionManager.Get(destinationConnection), where, batchSize, relationshipBehavior, tableExistenceBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction);

        /// <summary>
        /// Copies all the rows of the source table of the current connection into the target table of the destination connection
        /// that is registered in the <see cref="ConnectionManager"/>.
        /// The target table must already exist and have the same column names as the source table, unless the <paramref name="relationshipBehavior"/> or the <paramref name="tableExistenceBehavior"/> asks for the schema to be copied first.
        /// The rows of the source table are left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="sourceTable">The name of the table to be copied.</param>
        /// <param name="targetTable">The name of the table in the destination database that receives the rows. When the schema is copied, the schema of the name (i.e.: <c>Sales.Person</c>) is the schema that the tables are created in; without a schema, the default schema of the destination database is used.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name (the schema of the target table can be different).</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name (the schema of the target table can be different). <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the target table.</returns>
        public static int CopyDataTo(this IDbConnection connection,
            string sourceTable,
            string targetTable,
            string destinationConnection,
            QueryGroup where = null,
            int batchSize = 1000,
            CopyDataRelationshipBehavior relationshipBehavior = CopyDataRelationshipBehavior.TableOnly,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.Skip,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null) =>
            connection.CopyDataTo(sourceTable, targetTable, ConnectionManager.Get(destinationConnection), where, batchSize, relationshipBehavior, tableExistenceBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction);

        #endregion

        #region CopyDataTo (IDbConnection, Sync)

        /// <summary>
        /// Copies all the rows of the table mapped to the <typeparamref name="TEntity"/> type of the current connection
        /// into the same-named table of the destination connection.
        /// The destination table must already exist, unless the <paramref name="relationshipBehavior"/> or the <paramref name="tableExistenceBehavior"/> asks for the schema to be copied first. The rows of the source table are left untouched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name (the schema of the target table can be different).</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name (the schema of the target table can be different). <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the destination table.</returns>
        public static int CopyDataTo<TEntity>(this IDbConnection connection,
            IDbConnection destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int batchSize = 1000,
            CopyDataRelationshipBehavior relationshipBehavior = CopyDataRelationshipBehavior.TableOnly,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.Skip,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            connection.CopyDataTo(ClassMappedNameCache.Get<TEntity>(), destinationConnection, where == null ? null : QueryGroup.Parse(where), batchSize, relationshipBehavior, tableExistenceBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction);

        /// <summary>
        /// Copies all the rows of the table mapped to the <typeparamref name="TEntity"/> type of the current connection
        /// into the target table of the destination connection.
        /// The target table must already exist and have the same column names as the source table, unless the <paramref name="relationshipBehavior"/> or the <paramref name="tableExistenceBehavior"/> asks for the schema to be copied first.
        /// The rows of the source table are left untouched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the source table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="targetTable">The name of the table in the destination database that receives the rows. When the schema is copied, the schema of the name (i.e.: <c>Sales.Person</c>) is the schema that the tables are created in; without a schema, the default schema of the destination database is used.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name (the schema of the target table can be different).</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name (the schema of the target table can be different). <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the target table.</returns>
        public static int CopyDataTo<TEntity>(this IDbConnection connection,
            string targetTable,
            IDbConnection destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int batchSize = 1000,
            CopyDataRelationshipBehavior relationshipBehavior = CopyDataRelationshipBehavior.TableOnly,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.Skip,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            connection.CopyDataTo(ClassMappedNameCache.Get<TEntity>(), targetTable, destinationConnection, where == null ? null : QueryGroup.Parse(where), batchSize, relationshipBehavior, tableExistenceBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction);

        /// <summary>
        /// Copies all the rows of the source table of the current connection into the same-named table of the destination connection.
        /// The destination table must already exist, unless the <paramref name="relationshipBehavior"/> or the <paramref name="tableExistenceBehavior"/> asks for the schema to be copied first. The rows of the source table are left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the table to be copied.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name (the schema of the target table can be different).</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name (the schema of the target table can be different). <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the destination table.</returns>
        public static int CopyDataTo(this IDbConnection connection,
            string tableName,
            IDbConnection destinationConnection,
            QueryGroup where = null,
            int batchSize = 1000,
            CopyDataRelationshipBehavior relationshipBehavior = CopyDataRelationshipBehavior.TableOnly,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.Skip,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null) =>
            connection.CopyDataTo(tableName, tableName, destinationConnection, where, batchSize, relationshipBehavior, tableExistenceBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction);

        /// <summary>
        /// Copies all the rows of the source table of the current connection into the target table of the destination connection.
        /// The target table must already exist and have the same column names as the source table, unless the <paramref name="relationshipBehavior"/> or the <paramref name="tableExistenceBehavior"/> asks for the schema to be copied first.
        /// The rows of the source table are left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="sourceTable">The name of the table to be copied.</param>
        /// <param name="targetTable">The name of the table in the destination database that receives the rows. When the schema is copied, the schema of the name (i.e.: <c>Sales.Person</c>) is the schema that the tables are created in; without a schema, the default schema of the destination database is used.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name (the schema of the target table can be different).</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name (the schema of the target table can be different). <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the target table.</returns>
        public static int CopyDataTo(this IDbConnection connection,
            string sourceTable,
            string targetTable,
            IDbConnection destinationConnection,
            QueryGroup where = null,
            int batchSize = 1000,
            CopyDataRelationshipBehavior relationshipBehavior = CopyDataRelationshipBehavior.TableOnly,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.Skip,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
        {
            var targetSchema = GetTargetSchema(destinationConnection, targetTable);
            Validate(connection, sourceTable, targetTable, destinationConnection, targetSchema, batchSize, relationshipBehavior, tableExistenceBehavior);
            var options = new CopyDataProgress { StartTime = DateTime.UtcNow };
            foreach (var (source, target, filter) in GetTables(connection, destinationConnection, targetSchema, sourceTable, targetTable, where, relationshipBehavior, tableExistenceBehavior, commandTimeout, trace, transaction))
            {
                CopyRows(connection, destinationConnection, source, target, filter, batchSize, options, progressCallback, commandTimeout, traceKey, trace, transaction);
            }
            return options.TotalCopiedRowCount;
        }

        #endregion

        #region CopyDataTo (Registered Connection, Async)

        /// <summary>
        /// Copies all the rows of the table mapped to the <typeparamref name="TEntity"/> type of the current connection
        /// into the same-named table of the destination connection that is registered in the <see cref="ConnectionManager"/>.
        /// The destination table must already exist, unless the <paramref name="relationshipBehavior"/> or the <paramref name="tableExistenceBehavior"/> asks for the schema to be copied first. The rows of the source table are left untouched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name (the schema of the target table can be different).</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name (the schema of the target table can be different). <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows inserted into the destination table.</returns>
        public static Task<int> CopyDataToAsync<TEntity>(this IDbConnection connection,
            string destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int batchSize = 1000,
            CopyDataRelationshipBehavior relationshipBehavior = CopyDataRelationshipBehavior.TableOnly,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.Skip,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            connection.CopyDataToAsync(ConnectionManager.Get(destinationConnection), where, batchSize, relationshipBehavior, tableExistenceBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Copies all the rows of the table mapped to the <typeparamref name="TEntity"/> type of the current connection
        /// into the target table of the destination connection that is registered in the <see cref="ConnectionManager"/>.
        /// The target table must already exist and have the same column names as the source table, unless the <paramref name="relationshipBehavior"/> or the <paramref name="tableExistenceBehavior"/> asks for the schema to be copied first.
        /// The rows of the source table are left untouched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the source table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="targetTable">The name of the table in the destination database that receives the rows. When the schema is copied, the schema of the name (i.e.: <c>Sales.Person</c>) is the schema that the tables are created in; without a schema, the default schema of the destination database is used.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name (the schema of the target table can be different).</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name (the schema of the target table can be different). <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows inserted into the target table.</returns>
        public static Task<int> CopyDataToAsync<TEntity>(this IDbConnection connection,
            string targetTable,
            string destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int batchSize = 1000,
            CopyDataRelationshipBehavior relationshipBehavior = CopyDataRelationshipBehavior.TableOnly,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.Skip,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            connection.CopyDataToAsync(targetTable, ConnectionManager.Get(destinationConnection), where, batchSize, relationshipBehavior, tableExistenceBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Copies all the rows of the source table of the current connection into the same-named table of the destination connection
        /// that is registered in the <see cref="ConnectionManager"/>.
        /// The destination table must already exist, unless the <paramref name="relationshipBehavior"/> or the <paramref name="tableExistenceBehavior"/> asks for the schema to be copied first. The rows of the source table are left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the table to be copied.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name (the schema of the target table can be different).</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name (the schema of the target table can be different). <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows inserted into the destination table.</returns>
        public static Task<int> CopyDataToAsync(this IDbConnection connection,
            string tableName,
            string destinationConnection,
            QueryGroup where = null,
            int batchSize = 1000,
            CopyDataRelationshipBehavior relationshipBehavior = CopyDataRelationshipBehavior.TableOnly,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.Skip,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            connection.CopyDataToAsync(tableName, ConnectionManager.Get(destinationConnection), where, batchSize, relationshipBehavior, tableExistenceBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Copies all the rows of the source table of the current connection into the target table of the destination connection
        /// that is registered in the <see cref="ConnectionManager"/>.
        /// The target table must already exist and have the same column names as the source table, unless the <paramref name="relationshipBehavior"/> or the <paramref name="tableExistenceBehavior"/> asks for the schema to be copied first.
        /// The rows of the source table are left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="sourceTable">The name of the table to be copied.</param>
        /// <param name="targetTable">The name of the table in the destination database that receives the rows. When the schema is copied, the schema of the name (i.e.: <c>Sales.Person</c>) is the schema that the tables are created in; without a schema, the default schema of the destination database is used.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name (the schema of the target table can be different).</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name (the schema of the target table can be different). <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows inserted into the target table.</returns>
        public static Task<int> CopyDataToAsync(this IDbConnection connection,
            string sourceTable,
            string targetTable,
            string destinationConnection,
            QueryGroup where = null,
            int batchSize = 1000,
            CopyDataRelationshipBehavior relationshipBehavior = CopyDataRelationshipBehavior.TableOnly,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.Skip,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            connection.CopyDataToAsync(sourceTable, targetTable, ConnectionManager.Get(destinationConnection), where, batchSize, relationshipBehavior, tableExistenceBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction, cancellationToken);

        #endregion

        #region CopyDataTo (IDbConnection, Async)

        /// <summary>
        /// Copies all the rows of the table mapped to the <typeparamref name="TEntity"/> type of the current connection
        /// into the same-named table of the destination connection.
        /// The destination table must already exist, unless the <paramref name="relationshipBehavior"/> or the <paramref name="tableExistenceBehavior"/> asks for the schema to be copied first. The rows of the source table are left untouched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name (the schema of the target table can be different).</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name (the schema of the target table can be different). <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows inserted into the destination table.</returns>
        public static Task<int> CopyDataToAsync<TEntity>(this IDbConnection connection,
            IDbConnection destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int batchSize = 1000,
            CopyDataRelationshipBehavior relationshipBehavior = CopyDataRelationshipBehavior.TableOnly,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.Skip,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            connection.CopyDataToAsync(ClassMappedNameCache.Get<TEntity>(), destinationConnection, where == null ? null : QueryGroup.Parse(where), batchSize, relationshipBehavior, tableExistenceBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Copies all the rows of the table mapped to the <typeparamref name="TEntity"/> type of the current connection
        /// into the target table of the destination connection.
        /// The target table must already exist and have the same column names as the source table, unless the <paramref name="relationshipBehavior"/> or the <paramref name="tableExistenceBehavior"/> asks for the schema to be copied first.
        /// The rows of the source table are left untouched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the source table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="targetTable">The name of the table in the destination database that receives the rows. When the schema is copied, the schema of the name (i.e.: <c>Sales.Person</c>) is the schema that the tables are created in; without a schema, the default schema of the destination database is used.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name (the schema of the target table can be different).</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name (the schema of the target table can be different). <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows inserted into the target table.</returns>
        public static Task<int> CopyDataToAsync<TEntity>(this IDbConnection connection,
            string targetTable,
            IDbConnection destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int batchSize = 1000,
            CopyDataRelationshipBehavior relationshipBehavior = CopyDataRelationshipBehavior.TableOnly,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.Skip,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            connection.CopyDataToAsync(ClassMappedNameCache.Get<TEntity>(), targetTable, destinationConnection, where == null ? null : QueryGroup.Parse(where), batchSize, relationshipBehavior, tableExistenceBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Copies all the rows of the source table of the current connection into the same-named table of the destination connection.
        /// The destination table must already exist, unless the <paramref name="relationshipBehavior"/> or the <paramref name="tableExistenceBehavior"/> asks for the schema to be copied first. The rows of the source table are left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the table to be copied.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name (the schema of the target table can be different).</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name (the schema of the target table can be different). <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows inserted into the destination table.</returns>
        public static Task<int> CopyDataToAsync(this IDbConnection connection,
            string tableName,
            IDbConnection destinationConnection,
            QueryGroup where = null,
            int batchSize = 1000,
            CopyDataRelationshipBehavior relationshipBehavior = CopyDataRelationshipBehavior.TableOnly,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.Skip,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            connection.CopyDataToAsync(tableName, tableName, destinationConnection, where, batchSize, relationshipBehavior, tableExistenceBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Copies all the rows of the source table of the current connection into the target table of the destination connection.
        /// The target table must already exist and have the same column names as the source table, unless the <paramref name="relationshipBehavior"/> or the <paramref name="tableExistenceBehavior"/> asks for the schema to be copied first.
        /// The rows of the source table are left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="sourceTable">The name of the table to be copied.</param>
        /// <param name="targetTable">The name of the table in the destination database that receives the rows. When the schema is copied, the schema of the name (i.e.: <c>Sales.Person</c>) is the schema that the tables are created in; without a schema, the default schema of the destination database is used.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name (the schema of the target table can be different).</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name (the schema of the target table can be different). <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows inserted into the target table.</returns>
        public static async Task<int> CopyDataToAsync(this IDbConnection connection,
            string sourceTable,
            string targetTable,
            IDbConnection destinationConnection,
            QueryGroup where = null,
            int batchSize = 1000,
            CopyDataRelationshipBehavior relationshipBehavior = CopyDataRelationshipBehavior.TableOnly,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.Skip,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var targetSchema = GetTargetSchema(destinationConnection, targetTable);
            Validate(connection, sourceTable, targetTable, destinationConnection, targetSchema, batchSize, relationshipBehavior, tableExistenceBehavior);
            var options = new CopyDataProgress { StartTime = DateTime.UtcNow };
            var tables = await GetTablesAsync(connection, destinationConnection, targetSchema, sourceTable, targetTable, where, relationshipBehavior, tableExistenceBehavior, commandTimeout, trace, transaction, cancellationToken).ConfigureAwait(false);
            foreach (var (source, target, filter) in tables)
            {
                await CopyRowsAsync(connection, destinationConnection, source, target, filter, batchSize, options, progressCallback, commandTimeout, traceKey, trace, transaction, cancellationToken).ConfigureAwait(false);
            }
            return options.TotalCopiedRowCount;
        }

        #endregion

        #endregion
    }
}
