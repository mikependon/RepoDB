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
using RepoDb.Interfaces;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

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
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
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
        /// <param name="targetTable">The name of the table in the destination database that receives the rows.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
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
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
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
        /// <param name="targetTable">The name of the table in the destination database that receives the rows.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
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
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
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
        /// <param name="targetTable">The name of the table in the destination database that receives the rows.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
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
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
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
        /// <param name="targetTable">The name of the table in the destination database that receives the rows.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
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
            Validate(connection, sourceTable, targetTable, destinationConnection, batchSize, relationshipBehavior, tableExistenceBehavior);

            var options = new CopyDataProgress { StartTime = DateTime.UtcNow };
            foreach (var (source, target, filter) in GetTables(connection, destinationConnection, sourceTable, targetTable, where, relationshipBehavior, tableExistenceBehavior, commandTimeout, trace, transaction))
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
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
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
        /// <param name="targetTable">The name of the table in the destination database that receives the rows.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
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
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
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
        /// <param name="targetTable">The name of the table in the destination database that receives the rows.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
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
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
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
        /// <param name="targetTable">The name of the table in the destination database that receives the rows.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
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
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
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
        /// <param name="targetTable">The name of the table in the destination database that receives the rows.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) have their data copied together with it. The schema of the related tables is copied first (see <see cref="CopySchemaTo"/>), then their rows are copied in the order that a table comes after the tables that it references. The <paramref name="where"/> filter applies to the requested table only, all the rows of the related tables are copied. The default is <see cref="CopyDataRelationshipBehavior.TableOnly"/>. Anything other than <see cref="CopyDataRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the table (and to its related tables) in the destination database before the rows are copied, as in <see cref="CopySchemaTo"/>: <see cref="CopySchemaExistsBehavior.Skip"/> creates the missing tables and leaves the existing ones as they are, <see cref="CopySchemaExistsBehavior.Align"/> also adds the missing columns and indexes of the existing tables, <see cref="CopySchemaExistsBehavior.Throw"/> throws if a table exists and <see cref="CopySchemaExistsBehavior.Drop"/> drops the existing tables and creates them again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>, which together with <see cref="CopyDataRelationshipBehavior.TableOnly"/> does not copy any schema, so the table must already exist. Anything else requires the source and the target tables to have the same name. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
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
            Validate(connection, sourceTable, targetTable, destinationConnection, batchSize, relationshipBehavior, tableExistenceBehavior);

            var options = new CopyDataProgress { StartTime = DateTime.UtcNow };
            var tables = await GetTablesAsync(connection, destinationConnection, sourceTable, targetTable, where, relationshipBehavior, tableExistenceBehavior, commandTimeout, trace, transaction, cancellationToken).ConfigureAwait(false);
            foreach (var (source, target, filter) in tables)
            {
                await CopyRowsAsync(connection, destinationConnection, source, target, filter, batchSize, options, progressCallback, commandTimeout, traceKey, trace, transaction, cancellationToken).ConfigureAwait(false);
            }
            return options.TotalCopiedRowCount;
        }

        #endregion

        #endregion

        #region Helpers

        /// <summary>
        /// Gets the tables whose rows are to be copied, in the order that they must be copied (a table comes after the tables that it references).
        /// If the schema is requested (see <see cref="IsSchemaRequested"/>), it is copied first, with <see cref="CopySchemaToExtension.CopySchemaTo(IDbConnection, IEnumerable{string}, IDbConnection, CopySchemaExistsBehavior, CopySchemaRelationshipBehavior, Action{CopySchemaResult}, Action{CopySchemaError}, int?, string, ITrace, IDbTransaction)"/>.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="destinationConnection"></param>
        /// <param name="sourceTable"></param>
        /// <param name="targetTable"></param>
        /// <param name="where"></param>
        /// <param name="relationshipBehavior"></param>
        /// <param name="tableExistenceBehavior"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="trace"></param>
        /// <param name="transaction"></param>
        /// <returns>The source table, the target table and the filter of each table.</returns>
        private static IList<(string Source, string Target, QueryGroup Where)> GetTables(IDbConnection connection,
            IDbConnection destinationConnection,
            string sourceTable,
            string targetTable,
            QueryGroup where,
            CopyDataRelationshipBehavior relationshipBehavior,
            CopySchemaExistsBehavior tableExistenceBehavior,
            int? commandTimeout,
            ITrace trace,
            IDbTransaction transaction)
        {
            var requested = new List<(string, string, QueryGroup)> { (sourceTable, targetTable, where) };
            if (!IsSchemaRequested(relationshipBehavior, tableExistenceBehavior))
            {
                return requested;
            }

            var schemaBehavior = ToSchemaBehavior(relationshipBehavior);
            connection.CopySchemaTo(new[] { sourceTable }, destinationConnection, tableExistenceBehavior, schemaBehavior,
                commandTimeout: commandTimeout, trace: trace, transaction: transaction);
            if (relationshipBehavior == CopyDataRelationshipBehavior.TableOnly)
            {
                return requested;
            }

            var reader = SchemaReaderMapper.Get(connection);
            var relationships = reader.GetDependencyOrder(reader.GetRelatedTables(new[] { sourceTable }, schemaBehavior));
            return ToTables(relationships, reader.GetTableSchema(sourceTable).Table, requested[0], connection, destinationConnection);
        }

        /// <summary>
        /// Gets the tables whose rows are to be copied (see <see cref="GetTables"/>).
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="destinationConnection"></param>
        /// <param name="sourceTable"></param>
        /// <param name="targetTable"></param>
        /// <param name="where"></param>
        /// <param name="relationshipBehavior"></param>
        /// <param name="tableExistenceBehavior"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="trace"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The source table, the target table and the filter of each table.</returns>
        private static async Task<IList<(string Source, string Target, QueryGroup Where)>> GetTablesAsync(IDbConnection connection,
            IDbConnection destinationConnection,
            string sourceTable,
            string targetTable,
            QueryGroup where,
            CopyDataRelationshipBehavior relationshipBehavior,
            CopySchemaExistsBehavior tableExistenceBehavior,
            int? commandTimeout,
            ITrace trace,
            IDbTransaction transaction,
            CancellationToken cancellationToken)
        {
            var requested = new List<(string, string, QueryGroup)> { (sourceTable, targetTable, where) };
            if (!IsSchemaRequested(relationshipBehavior, tableExistenceBehavior))
            {
                return requested;
            }

            var schemaBehavior = ToSchemaBehavior(relationshipBehavior);
            await connection.CopySchemaToAsync(new[] { sourceTable }, destinationConnection, tableExistenceBehavior, schemaBehavior,
                commandTimeout: commandTimeout, trace: trace, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (relationshipBehavior == CopyDataRelationshipBehavior.TableOnly)
            {
                return requested;
            }

            var reader = SchemaReaderMapper.Get(connection);
            var names = await reader.GetRelatedTablesAsync(new[] { sourceTable }, schemaBehavior, cancellationToken).ConfigureAwait(false);
            var relationships = await reader.GetDependencyOrderAsync(names, cancellationToken).ConfigureAwait(false);
            var table = (await reader.GetTableSchemaAsync(sourceTable, cancellationToken).ConfigureAwait(false)).Table;
            return ToTables(relationships, table, requested[0], connection, destinationConnection);
        }

        /// <summary>
        /// Gets the tables to be copied from the relationships: the requested table keeps its names and its filter, and the related tables are copied as a whole.
        /// </summary>
        /// <param name="relationships"></param>
        /// <param name="requested"></param>
        /// <param name="requestedTable"></param>
        /// <param name="connection"></param>
        /// <param name="destinationConnection"></param>
        /// <returns></returns>
        private static IList<(string Source, string Target, QueryGroup Where)> ToTables(IEnumerable<RelationshipInfo> relationships,
            TableInfo requested,
            (string Source, string Target, QueryGroup Where) requestedTable,
            IDbConnection connection,
            IDbConnection destinationConnection) =>
            relationships
                .Select(relationship => relationship.Schema.Table)
                .Select(table => table.Equals(requested)
                    ? requestedTable
                    : (GetName(connection, table), GetName(destinationConnection, table), (QueryGroup)null))
                .ToList();

        /// <summary>
        /// Gets the name of the table, quoted for the database of the connection.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        private static string GetName(IDbConnection connection,
            TableInfo table)
        {
            var setting = connection.GetDbSetting();
            string Quote(string part) => $"{setting.OpeningQuote}{part}{setting.ClosingQuote}";
            return string.IsNullOrWhiteSpace(table.Schema)
                ? Quote(table.Name)
                : $"{Quote(table.Schema)}.{Quote(table.Name)}";
        }

        /// <summary>
        /// Checks whether the schema of the tables is to be copied before their rows: the default behaviors
        /// (<see cref="CopyDataRelationshipBehavior.TableOnly"/> and <see cref="CopySchemaExistsBehavior.Skip"/>) do not copy it.
        /// </summary>
        /// <param name="relationshipBehavior"></param>
        /// <param name="tableExistenceBehavior"></param>
        /// <returns></returns>
        private static bool IsSchemaRequested(CopyDataRelationshipBehavior relationshipBehavior,
            CopySchemaExistsBehavior tableExistenceBehavior) =>
            relationshipBehavior != CopyDataRelationshipBehavior.TableOnly ||
            tableExistenceBehavior != CopySchemaExistsBehavior.Skip;

        /// <summary>
        /// Gets the relationship behavior of the schema copy that is equivalent to the one of the data copy.
        /// </summary>
        /// <param name="relationshipBehavior"></param>
        /// <returns></returns>
        private static CopySchemaRelationshipBehavior ToSchemaBehavior(CopyDataRelationshipBehavior relationshipBehavior)
        {
            switch (relationshipBehavior)
            {
                case CopyDataRelationshipBehavior.TableOnly:
                    return CopySchemaRelationshipBehavior.TableOnly;
                case CopyDataRelationshipBehavior.Parents:
                    return CopySchemaRelationshipBehavior.Parents;
                case CopyDataRelationshipBehavior.Children:
                    return CopySchemaRelationshipBehavior.Children;
                case CopyDataRelationshipBehavior.ParentsAndChildren:
                    return CopySchemaRelationshipBehavior.ParentsAndChildren;
                default:
                    throw new ArgumentOutOfRangeException(nameof(relationshipBehavior));
            }
        }

        /// <summary>
        /// Copies the rows of the source table into the target table, in batches.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="destinationConnection"></param>
        /// <param name="sourceTable"></param>
        /// <param name="targetTable"></param>
        /// <param name="where"></param>
        /// <param name="batchSize"></param>
        /// <param name="options"></param>
        /// <param name="progressCallback"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="traceKey"></param>
        /// <param name="trace"></param>
        /// <param name="transaction"></param>
        private static void CopyRows(IDbConnection connection,
            IDbConnection destinationConnection,
            string sourceTable,
            string targetTable,
            QueryGroup where,
            int batchSize,
            CopyDataProgress options,
            Action<CopyDataProgress> progressCallback,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction)
        {
            var batch = new List<object>(batchSize);

            void Flush()
            {
                var inserted = destinationConnection.InsertAll(targetTable, batch, batchSize, commandTimeout: commandTimeout, traceKey: traceKey, transaction: transaction, trace: trace);
                options.BatchNumber++;
                options.RowCount = inserted;
                options.TotalCopiedRowCount += inserted;
                options.EndTime = DateTime.UtcNow;
                batch.Clear();
                progressCallback?.Invoke(options);
            }

            foreach (object row in where == null ? connection.QueryAll(sourceTable, commandTimeout: commandTimeout, traceKey: traceKey, trace: trace) : connection.Query(sourceTable, where, commandTimeout: commandTimeout, traceKey: traceKey, trace: trace))
            {
                batch.Add(row);
                if (batch.Count == batchSize)
                {
                    Flush();
                }
            }

            if (batch.Count > 0)
            {
                Flush();
            }
        }

        /// <summary>
        /// Copies the rows of the source table into the target table, in batches.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="destinationConnection"></param>
        /// <param name="sourceTable"></param>
        /// <param name="targetTable"></param>
        /// <param name="where"></param>
        /// <param name="batchSize"></param>
        /// <param name="options"></param>
        /// <param name="progressCallback"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="traceKey"></param>
        /// <param name="trace"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        private static async Task CopyRowsAsync(IDbConnection connection,
            IDbConnection destinationConnection,
            string sourceTable,
            string targetTable,
            QueryGroup where,
            int batchSize,
            CopyDataProgress options,
            Action<CopyDataProgress> progressCallback,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction,
            CancellationToken cancellationToken)
        {
            var batch = new List<object>(batchSize);

            async Task Flush()
            {
                var inserted = await destinationConnection.InsertAllAsync(targetTable, batch, batchSize, commandTimeout: commandTimeout, traceKey: traceKey, transaction: transaction, trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false);
                options.BatchNumber++;
                options.RowCount = inserted;
                options.TotalCopiedRowCount += inserted;
                options.EndTime = DateTime.UtcNow;
                batch.Clear();
                progressCallback?.Invoke(options);
            }

            var rows = where == null
                ? await connection.QueryAllAsync(sourceTable, commandTimeout: commandTimeout, traceKey: traceKey, trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false)
                : await connection.QueryAsync(sourceTable, where, commandTimeout: commandTimeout, traceKey: traceKey, trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false);

            foreach (object row in rows)
            {
                cancellationToken.ThrowIfCancellationRequested();
                batch.Add(row);
                if (batch.Count == batchSize)
                {
                    await Flush().ConfigureAwait(false);
                }
            }

            if (batch.Count > 0)
            {
                await Flush().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sourceTable"></param>
        /// <param name="targetTable"></param>
        /// <param name="destinationConnection"></param>
        /// <param name="batchSize"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static void Validate(IDbConnection connection,
            string sourceTable,
            string targetTable,
            IDbConnection destinationConnection,
            int batchSize,
            CopyDataRelationshipBehavior relationshipBehavior,
            CopySchemaExistsBehavior tableExistenceBehavior)
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
            if (IsSchemaRequested(relationshipBehavior, tableExistenceBehavior) && !string.Equals(sourceTable, targetTable, StringComparison.Ordinal))
            {
                throw new ArgumentException("The schema is copied with the same name as the source table, so the source and the target tables must have the same name when the relationship behavior is not 'TableOnly' or the table existence behavior is not 'Skip'.", nameof(targetTable));
            }
        }

        #endregion
    }
}
