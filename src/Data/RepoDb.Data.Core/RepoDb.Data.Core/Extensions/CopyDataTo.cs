#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Diagnostics.CodeAnalysis;
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
        /// Copies the rows of the table mapped to <typeparamref name="TEntity"/> to the same-named table of the destination connection that is registered in the <see cref="ConnectionManager"/>. The destination table must already exist, unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="columnMappingsBehavior">Defines how the source columns are aligned with the target columns by the bulk insert of the provider (ignored if it has none). The default is <see cref="LoadColumnMappingBehavior.Automatic"/>.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopyProgress"/> of the operation: its totals and the tables that were copied (see <see cref="CopyProgress.Items"/>).</returns>
        public static CopyProgress CopyDataTo<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            string destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadColumnMappingBehavior columnMappingsBehavior = LoadColumnMappingBehavior.Automatic,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            connection.CopyDataTo(ConnectionManager.Get(destinationConnection), where, bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, columnMappingsBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction);

        /// <summary>
        /// Copies the rows of the table mapped to <typeparamref name="TEntity"/> to the target table of the destination connection that is registered in the <see cref="ConnectionManager"/>. The target table must already exist (with the same column names), unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the source table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="targetTable">The name of the target table. Its schema (i.e.: <c>Sales.Person</c>) is used when the schema is copied.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="columnMappingsBehavior">Defines how the source columns are aligned with the target columns by the bulk insert of the provider (ignored if it has none). The default is <see cref="LoadColumnMappingBehavior.Automatic"/>.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopyProgress"/> of the operation: its totals and the tables that were copied (see <see cref="CopyProgress.Items"/>).</returns>
        public static CopyProgress CopyDataTo<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            string targetTable,
            string destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadColumnMappingBehavior columnMappingsBehavior = LoadColumnMappingBehavior.Automatic,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            connection.CopyDataTo(targetTable, ConnectionManager.Get(destinationConnection), where, bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, columnMappingsBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction);

        /// <summary>
        /// Copies the rows of the source table to the same-named table of the destination connection that is registered in the <see cref="ConnectionManager"/>. The destination table must already exist, unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the source table.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="columnMappingsBehavior">Defines how the source columns are aligned with the target columns by the bulk insert of the provider (ignored if it has none). The default is <see cref="LoadColumnMappingBehavior.Automatic"/>.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopyProgress"/> of the operation: its totals and the tables that were copied (see <see cref="CopyProgress.Items"/>).</returns>
        public static CopyProgress CopyDataTo(this IDbConnection connection,
            string tableName,
            string destinationConnection,
            QueryGroup where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadColumnMappingBehavior columnMappingsBehavior = LoadColumnMappingBehavior.Automatic,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null) =>
            connection.CopyDataTo(tableName, ConnectionManager.Get(destinationConnection), where, bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, columnMappingsBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction);

        /// <summary>
        /// Copies the rows of the source table to the target table of the destination connection that is registered in the <see cref="ConnectionManager"/>. The target table must already exist (with the same column names), unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="sourceTable">The name of the source table.</param>
        /// <param name="targetTable">The name of the target table. Its schema (i.e.: <c>Sales.Person</c>) is used when the schema is copied.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="columnMappingsBehavior">Defines how the source columns are aligned with the target columns by the bulk insert of the provider (ignored if it has none). The default is <see cref="LoadColumnMappingBehavior.Automatic"/>.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopyProgress"/> of the operation: its totals and the tables that were copied (see <see cref="CopyProgress.Items"/>).</returns>
        public static CopyProgress CopyDataTo(this IDbConnection connection,
            string sourceTable,
            string targetTable,
            string destinationConnection,
            QueryGroup where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadColumnMappingBehavior columnMappingsBehavior = LoadColumnMappingBehavior.Automatic,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null) =>
            connection.CopyDataTo(sourceTable, targetTable, ConnectionManager.Get(destinationConnection), where, bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, columnMappingsBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction);

        #endregion

        #region CopyDataTo (IDbConnection, Sync)

        /// <summary>
        /// Copies the rows of the table mapped to <typeparamref name="TEntity"/> to the same-named table of the destination connection. The destination table must already exist, unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The destination connection.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="columnMappingsBehavior">Defines how the source columns are aligned with the target columns by the bulk insert of the provider (ignored if it has none). The default is <see cref="LoadColumnMappingBehavior.Automatic"/>.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopyProgress"/> of the operation: its totals and the tables that were copied (see <see cref="CopyProgress.Items"/>).</returns>
        public static CopyProgress CopyDataTo<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            IDbConnection destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadColumnMappingBehavior columnMappingsBehavior = LoadColumnMappingBehavior.Automatic,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            connection.CopyDataTo(ClassMappedNameCache.Get<TEntity>(), destinationConnection, where == null ? null : QueryGroup.Parse(where), bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, columnMappingsBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction);

        /// <summary>
        /// Copies the rows of the table mapped to <typeparamref name="TEntity"/> to the target table of the destination connection. The target table must already exist (with the same column names), unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the source table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="targetTable">The name of the target table. Its schema (i.e.: <c>Sales.Person</c>) is used when the schema is copied.</param>
        /// <param name="destinationConnection">The destination connection.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="columnMappingsBehavior">Defines how the source columns are aligned with the target columns by the bulk insert of the provider (ignored if it has none). The default is <see cref="LoadColumnMappingBehavior.Automatic"/>.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopyProgress"/> of the operation: its totals and the tables that were copied (see <see cref="CopyProgress.Items"/>).</returns>
        public static CopyProgress CopyDataTo<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            string targetTable,
            IDbConnection destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadColumnMappingBehavior columnMappingsBehavior = LoadColumnMappingBehavior.Automatic,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            connection.CopyDataTo(ClassMappedNameCache.Get<TEntity>(), targetTable, destinationConnection, where == null ? null : QueryGroup.Parse(where), bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, columnMappingsBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction);

        /// <summary>
        /// Copies the rows of the source table to the same-named table of the destination connection. The destination table must already exist, unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the source table.</param>
        /// <param name="destinationConnection">The destination connection.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="columnMappingsBehavior">Defines how the source columns are aligned with the target columns by the bulk insert of the provider (ignored if it has none). The default is <see cref="LoadColumnMappingBehavior.Automatic"/>.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopyProgress"/> of the operation: its totals and the tables that were copied (see <see cref="CopyProgress.Items"/>).</returns>
        public static CopyProgress CopyDataTo(this IDbConnection connection,
            string tableName,
            IDbConnection destinationConnection,
            QueryGroup where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadColumnMappingBehavior columnMappingsBehavior = LoadColumnMappingBehavior.Automatic,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null) =>
            connection.CopyDataTo(tableName, tableName, destinationConnection, where, bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, columnMappingsBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction);

        /// <summary>
        /// Copies the rows of the source table to the target table of the destination connection. The target table must already exist (with the same column names), unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="sourceTable">The name of the source table.</param>
        /// <param name="targetTable">The name of the target table. Its schema (i.e.: <c>Sales.Person</c>) is used when the schema is copied.</param>
        /// <param name="destinationConnection">The destination connection.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="columnMappingsBehavior">Defines how the source columns are aligned with the target columns by the bulk insert of the provider (ignored if it has none). The default is <see cref="LoadColumnMappingBehavior.Automatic"/>.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopyProgress"/> of the operation: its totals and the tables that were copied (see <see cref="CopyProgress.Items"/>).</returns>
        public static CopyProgress CopyDataTo(this IDbConnection connection,
            string sourceTable,
            string targetTable,
            IDbConnection destinationConnection,
            QueryGroup where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadColumnMappingBehavior columnMappingsBehavior = LoadColumnMappingBehavior.Automatic,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
        {
            var targetSchema = GetTargetSchema(destinationConnection, targetTable);
            Validate(connection, sourceTable, targetTable, destinationConnection, targetSchema, bufferSize, relationshipBehavior, tableExistenceBehavior, columnMappingsBehavior, identityBehavior, null);
            var options = new CopyProgress { StartTime = DateTime.UtcNow };
            var tables = GetTables(connection, destinationConnection, sourceTable, targetTable, targetSchema, where, relationshipBehavior, tableExistenceBehavior, commandTimeout, trace, transaction);
            options.TableCount = tables.Count;
            foreach (var (source, target, filter) in tables)
            {
                CopyRows(connection, destinationConnection, source, target, filter, bufferSize, bypassExistingRows, qualifiers, columnMappingsBehavior, identityBehavior, null, options, progressCallback, commandTimeout, traceKey, trace, transaction);
            }
            options.EndTime = DateTime.UtcNow;
            return options;
        }

        #endregion

        #region CopyDataTo (Registered Connection, Async)

        /// <summary>
        /// Copies the rows of the table mapped to <typeparamref name="TEntity"/> to the same-named table of the destination connection that is registered in the <see cref="ConnectionManager"/>. The destination table must already exist, unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="columnMappingsBehavior">Defines how the source columns are aligned with the target columns by the bulk insert of the provider (ignored if it has none). The default is <see cref="LoadColumnMappingBehavior.Automatic"/>.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopyProgress"/> of the operation: its totals, and the tables that were copied (see <see cref="CopyProgress.Items"/>), each one with its source and its target table, its batches and its logs.</returns>
        public static Task<CopyProgress> CopyDataToAsync<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            string destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadColumnMappingBehavior columnMappingsBehavior = LoadColumnMappingBehavior.Automatic,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            connection.CopyDataToAsync(ConnectionManager.Get(destinationConnection), where, bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, columnMappingsBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Copies the rows of the table mapped to <typeparamref name="TEntity"/> to the target table of the destination connection that is registered in the <see cref="ConnectionManager"/>. The target table must already exist (with the same column names), unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the source table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="targetTable">The name of the target table. Its schema (i.e.: <c>Sales.Person</c>) is used when the schema is copied.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="columnMappingsBehavior">Defines how the source columns are aligned with the target columns by the bulk insert of the provider (ignored if it has none). The default is <see cref="LoadColumnMappingBehavior.Automatic"/>.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopyProgress"/> of the operation: its totals, and the tables that were copied (see <see cref="CopyProgress.Items"/>), each one with its source and its target table, its batches and its logs.</returns>
        public static Task<CopyProgress> CopyDataToAsync<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            string targetTable,
            string destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadColumnMappingBehavior columnMappingsBehavior = LoadColumnMappingBehavior.Automatic,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            connection.CopyDataToAsync(targetTable, ConnectionManager.Get(destinationConnection), where, bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, columnMappingsBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Copies the rows of the source table to the same-named table of the destination connection that is registered in the <see cref="ConnectionManager"/>. The destination table must already exist, unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the source table.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="columnMappingsBehavior">Defines how the source columns are aligned with the target columns by the bulk insert of the provider (ignored if it has none). The default is <see cref="LoadColumnMappingBehavior.Automatic"/>.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopyProgress"/> of the operation: its totals, and the tables that were copied (see <see cref="CopyProgress.Items"/>), each one with its source and its target table, its batches and its logs.</returns>
        public static Task<CopyProgress> CopyDataToAsync(this IDbConnection connection,
            string tableName,
            string destinationConnection,
            QueryGroup where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadColumnMappingBehavior columnMappingsBehavior = LoadColumnMappingBehavior.Automatic,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            connection.CopyDataToAsync(tableName, ConnectionManager.Get(destinationConnection), where, bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, columnMappingsBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Copies the rows of the source table to the target table of the destination connection that is registered in the <see cref="ConnectionManager"/>. The target table must already exist (with the same column names), unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="sourceTable">The name of the source table.</param>
        /// <param name="targetTable">The name of the target table. Its schema (i.e.: <c>Sales.Person</c>) is used when the schema is copied.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="columnMappingsBehavior">Defines how the source columns are aligned with the target columns by the bulk insert of the provider (ignored if it has none). The default is <see cref="LoadColumnMappingBehavior.Automatic"/>.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopyProgress"/> of the operation: its totals, and the tables that were copied (see <see cref="CopyProgress.Items"/>), each one with its source and its target table, its batches and its logs.</returns>
        public static Task<CopyProgress> CopyDataToAsync(this IDbConnection connection,
            string sourceTable,
            string targetTable,
            string destinationConnection,
            QueryGroup where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadColumnMappingBehavior columnMappingsBehavior = LoadColumnMappingBehavior.Automatic,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            connection.CopyDataToAsync(sourceTable, targetTable, ConnectionManager.Get(destinationConnection), where, bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, columnMappingsBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction, cancellationToken);

        #endregion

        #region CopyDataTo (IDbConnection, Async)

        /// <summary>
        /// Copies the rows of the table mapped to <typeparamref name="TEntity"/> to the same-named table of the destination connection. The destination table must already exist, unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The destination connection.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="columnMappingsBehavior">Defines how the source columns are aligned with the target columns by the bulk insert of the provider (ignored if it has none). The default is <see cref="LoadColumnMappingBehavior.Automatic"/>.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopyProgress"/> of the operation: its totals, and the tables that were copied (see <see cref="CopyProgress.Items"/>), each one with its source and its target table, its batches and its logs.</returns>
        public static Task<CopyProgress> CopyDataToAsync<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            IDbConnection destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadColumnMappingBehavior columnMappingsBehavior = LoadColumnMappingBehavior.Automatic,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            connection.CopyDataToAsync(ClassMappedNameCache.Get<TEntity>(), destinationConnection, where == null ? null : QueryGroup.Parse(where), bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, columnMappingsBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Copies the rows of the table mapped to <typeparamref name="TEntity"/> to the target table of the destination connection. The target table must already exist (with the same column names), unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the source table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="targetTable">The name of the target table. Its schema (i.e.: <c>Sales.Person</c>) is used when the schema is copied.</param>
        /// <param name="destinationConnection">The destination connection.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="columnMappingsBehavior">Defines how the source columns are aligned with the target columns by the bulk insert of the provider (ignored if it has none). The default is <see cref="LoadColumnMappingBehavior.Automatic"/>.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopyProgress"/> of the operation: its totals, and the tables that were copied (see <see cref="CopyProgress.Items"/>), each one with its source and its target table, its batches and its logs.</returns>
        public static Task<CopyProgress> CopyDataToAsync<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            string targetTable,
            IDbConnection destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadColumnMappingBehavior columnMappingsBehavior = LoadColumnMappingBehavior.Automatic,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            connection.CopyDataToAsync(ClassMappedNameCache.Get<TEntity>(), targetTable, destinationConnection, where == null ? null : QueryGroup.Parse(where), bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, columnMappingsBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Copies the rows of the source table to the same-named table of the destination connection. The destination table must already exist, unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the source table.</param>
        /// <param name="destinationConnection">The destination connection.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="columnMappingsBehavior">Defines how the source columns are aligned with the target columns by the bulk insert of the provider (ignored if it has none). The default is <see cref="LoadColumnMappingBehavior.Automatic"/>.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopyProgress"/> of the operation: its totals, and the tables that were copied (see <see cref="CopyProgress.Items"/>), each one with its source and its target table, its batches and its logs.</returns>
        public static Task<CopyProgress> CopyDataToAsync(this IDbConnection connection,
            string tableName,
            IDbConnection destinationConnection,
            QueryGroup where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadColumnMappingBehavior columnMappingsBehavior = LoadColumnMappingBehavior.Automatic,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            connection.CopyDataToAsync(tableName, tableName, destinationConnection, where, bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, columnMappingsBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Copies the rows of the source table to the target table of the destination connection. The target table must already exist (with the same column names), unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="sourceTable">The name of the source table.</param>
        /// <param name="targetTable">The name of the target table. Its schema (i.e.: <c>Sales.Person</c>) is used when the schema is copied.</param>
        /// <param name="destinationConnection">The destination connection.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="columnMappingsBehavior">Defines how the source columns are aligned with the target columns by the bulk insert of the provider (ignored if it has none). The default is <see cref="LoadColumnMappingBehavior.Automatic"/>.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopyProgress"/> of the operation: its totals, and the tables that were copied (see <see cref="CopyProgress.Items"/>), each one with its source and its target table, its batches and its logs.</returns>
        public static async Task<CopyProgress> CopyDataToAsync(this IDbConnection connection,
            string sourceTable,
            string targetTable,
            IDbConnection destinationConnection,
            QueryGroup where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadColumnMappingBehavior columnMappingsBehavior = LoadColumnMappingBehavior.Automatic,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var targetSchema = GetTargetSchema(destinationConnection, targetTable);
            Validate(connection, sourceTable, targetTable, destinationConnection, targetSchema, bufferSize, relationshipBehavior, tableExistenceBehavior, columnMappingsBehavior, identityBehavior, null);
            var options = new CopyProgress { StartTime = DateTime.UtcNow };
            var tables = await GetTablesAsync(connection, destinationConnection, sourceTable, targetTable, targetSchema, where, relationshipBehavior, tableExistenceBehavior, commandTimeout, trace, transaction, cancellationToken).ConfigureAwait(false);
            options.TableCount = tables.Count;
            foreach (var (source, target, filter) in tables)
            {
                await CopyRowsAsync(connection, destinationConnection, source, target, filter, bufferSize, bypassExistingRows, qualifiers, columnMappingsBehavior, identityBehavior, null, options, progressCallback, commandTimeout, traceKey, trace, transaction, cancellationToken).ConfigureAwait(false);
            }
            options.EndTime = DateTime.UtcNow;
            return options;
        }

        #endregion

        #region CopyDataTo With Mappings (Registered Connection, Sync)

        /// <summary>
        /// Copies the rows of the table mapped to <typeparamref name="TEntity"/> to the same-named table of the destination connection that is registered in the <see cref="ConnectionManager"/>. The destination table must already exist, unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="mappings">The mappings of the source columns to the target columns (see <see cref="CopyColumnMapping"/>). Only the mapped columns are copied, and only for the requested table; the related tables are copied by their matching column names.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopyProgress"/> of the operation: its totals and the tables that were copied (see <see cref="CopyProgress.Items"/>).</returns>
        public static CopyProgress CopyDataTo<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            string destinationConnection,
            IList<CopyColumnMapping> mappings,
            Expression<Func<TEntity, bool>> where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            connection.CopyDataTo(ConnectionManager.Get(destinationConnection), mappings, where, bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction);

        /// <summary>
        /// Copies the rows of the table mapped to <typeparamref name="TEntity"/> to the target table of the destination connection that is registered in the <see cref="ConnectionManager"/>. The target table must already exist (with the same column names), unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the source table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="targetTable">The name of the target table. Its schema (i.e.: <c>Sales.Person</c>) is used when the schema is copied.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="mappings">The mappings of the source columns to the target columns (see <see cref="CopyColumnMapping"/>). Only the mapped columns are copied, and only for the requested table; the related tables are copied by their matching column names.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopyProgress"/> of the operation: its totals and the tables that were copied (see <see cref="CopyProgress.Items"/>).</returns>
        public static CopyProgress CopyDataTo<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            string targetTable,
            string destinationConnection,
            IList<CopyColumnMapping> mappings,
            Expression<Func<TEntity, bool>> where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            connection.CopyDataTo(targetTable, ConnectionManager.Get(destinationConnection), mappings, where, bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction);

        /// <summary>
        /// Copies the rows of the source table to the same-named table of the destination connection that is registered in the <see cref="ConnectionManager"/>. The destination table must already exist, unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the source table.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="mappings">The mappings of the source columns to the target columns (see <see cref="CopyColumnMapping"/>). Only the mapped columns are copied, and only for the requested table; the related tables are copied by their matching column names.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopyProgress"/> of the operation: its totals and the tables that were copied (see <see cref="CopyProgress.Items"/>).</returns>
        public static CopyProgress CopyDataTo(this IDbConnection connection,
            string tableName,
            string destinationConnection,
            IList<CopyColumnMapping> mappings,
            QueryGroup where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null) =>
            connection.CopyDataTo(tableName, ConnectionManager.Get(destinationConnection), mappings, where, bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction);

        /// <summary>
        /// Copies the rows of the source table to the target table of the destination connection that is registered in the <see cref="ConnectionManager"/>. The target table must already exist (with the same column names), unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="sourceTable">The name of the source table.</param>
        /// <param name="targetTable">The name of the target table. Its schema (i.e.: <c>Sales.Person</c>) is used when the schema is copied.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="mappings">The mappings of the source columns to the target columns (see <see cref="CopyColumnMapping"/>). Only the mapped columns are copied, and only for the requested table; the related tables are copied by their matching column names.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopyProgress"/> of the operation: its totals and the tables that were copied (see <see cref="CopyProgress.Items"/>).</returns>
        public static CopyProgress CopyDataTo(this IDbConnection connection,
            string sourceTable,
            string targetTable,
            string destinationConnection,
            IList<CopyColumnMapping> mappings,
            QueryGroup where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null) =>
            connection.CopyDataTo(sourceTable, targetTable, ConnectionManager.Get(destinationConnection), mappings, where, bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction);

        #endregion

        #region CopyDataTo With Mappings (IDbConnection, Sync)

        /// <summary>
        /// Copies the rows of the table mapped to <typeparamref name="TEntity"/> to the same-named table of the destination connection. The destination table must already exist, unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The destination connection.</param>
        /// <param name="mappings">The mappings of the source columns to the target columns (see <see cref="CopyColumnMapping"/>). Only the mapped columns are copied, and only for the requested table; the related tables are copied by their matching column names.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopyProgress"/> of the operation: its totals and the tables that were copied (see <see cref="CopyProgress.Items"/>).</returns>
        public static CopyProgress CopyDataTo<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            IDbConnection destinationConnection,
            IList<CopyColumnMapping> mappings,
            Expression<Func<TEntity, bool>> where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            connection.CopyDataTo(ClassMappedNameCache.Get<TEntity>(), destinationConnection, mappings, where == null ? null : QueryGroup.Parse(where), bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction);

        /// <summary>
        /// Copies the rows of the table mapped to <typeparamref name="TEntity"/> to the target table of the destination connection. The target table must already exist (with the same column names), unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the source table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="targetTable">The name of the target table. Its schema (i.e.: <c>Sales.Person</c>) is used when the schema is copied.</param>
        /// <param name="destinationConnection">The destination connection.</param>
        /// <param name="mappings">The mappings of the source columns to the target columns (see <see cref="CopyColumnMapping"/>). Only the mapped columns are copied, and only for the requested table; the related tables are copied by their matching column names.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopyProgress"/> of the operation: its totals and the tables that were copied (see <see cref="CopyProgress.Items"/>).</returns>
        public static CopyProgress CopyDataTo<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            string targetTable,
            IDbConnection destinationConnection,
            IList<CopyColumnMapping> mappings,
            Expression<Func<TEntity, bool>> where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            connection.CopyDataTo(ClassMappedNameCache.Get<TEntity>(), targetTable, destinationConnection, mappings, where == null ? null : QueryGroup.Parse(where), bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction);

        /// <summary>
        /// Copies the rows of the source table to the same-named table of the destination connection. The destination table must already exist, unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the source table.</param>
        /// <param name="destinationConnection">The destination connection.</param>
        /// <param name="mappings">The mappings of the source columns to the target columns (see <see cref="CopyColumnMapping"/>). Only the mapped columns are copied, and only for the requested table; the related tables are copied by their matching column names.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopyProgress"/> of the operation: its totals and the tables that were copied (see <see cref="CopyProgress.Items"/>).</returns>
        public static CopyProgress CopyDataTo(this IDbConnection connection,
            string tableName,
            IDbConnection destinationConnection,
            IList<CopyColumnMapping> mappings,
            QueryGroup where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null) =>
            connection.CopyDataTo(tableName, tableName, destinationConnection, mappings, where, bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction);

        /// <summary>
        /// Copies the rows of the source table to the target table of the destination connection. The target table must already exist (with the same column names), unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="sourceTable">The name of the source table.</param>
        /// <param name="targetTable">The name of the target table. Its schema (i.e.: <c>Sales.Person</c>) is used when the schema is copied.</param>
        /// <param name="destinationConnection">The destination connection.</param>
        /// <param name="mappings">The mappings of the source columns to the target columns (see <see cref="CopyColumnMapping"/>). Only the mapped columns are copied, and only for the requested table; the related tables are copied by their matching column names.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopyProgress"/> of the operation: its totals and the tables that were copied (see <see cref="CopyProgress.Items"/>).</returns>
        public static CopyProgress CopyDataTo(this IDbConnection connection,
            string sourceTable,
            string targetTable,
            IDbConnection destinationConnection,
            IList<CopyColumnMapping> mappings,
            QueryGroup where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
        {
            var targetSchema = GetTargetSchema(destinationConnection, targetTable);
            Validate(connection, sourceTable, targetTable, destinationConnection, targetSchema, bufferSize, relationshipBehavior, tableExistenceBehavior, LoadColumnMappingBehavior.Automatic, identityBehavior, mappings);
            var options = new CopyProgress { StartTime = DateTime.UtcNow };
            var tables = GetTables(connection, destinationConnection, sourceTable, targetTable, targetSchema, where, relationshipBehavior, tableExistenceBehavior, commandTimeout, trace, transaction, mappings);
            options.TableCount = tables.Count;
            foreach (var (source, target, filter) in tables)
            {
                CopyRows(connection, destinationConnection, source, target, filter, bufferSize, bypassExistingRows, qualifiers, LoadColumnMappingBehavior.Automatic, identityBehavior, string.Equals(source, sourceTable, StringComparison.Ordinal) ? mappings : null, options, progressCallback, commandTimeout, traceKey, trace, transaction);
            }
            options.EndTime = DateTime.UtcNow;
            return options;
        }

        #endregion

        #region CopyDataTo With Mappings (Registered Connection, Async)

        /// <summary>
        /// Copies the rows of the table mapped to <typeparamref name="TEntity"/> to the same-named table of the destination connection that is registered in the <see cref="ConnectionManager"/>. The destination table must already exist, unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="mappings">The mappings of the source columns to the target columns (see <see cref="CopyColumnMapping"/>). Only the mapped columns are copied, and only for the requested table; the related tables are copied by their matching column names.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopyProgress"/> of the operation: its totals, and the tables that were copied (see <see cref="CopyProgress.Items"/>), each one with its source and its target table, its batches and its logs.</returns>
        public static Task<CopyProgress> CopyDataToAsync<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            string destinationConnection,
            IList<CopyColumnMapping> mappings,
            Expression<Func<TEntity, bool>> where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            connection.CopyDataToAsync(ConnectionManager.Get(destinationConnection), mappings, where, bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Copies the rows of the table mapped to <typeparamref name="TEntity"/> to the target table of the destination connection that is registered in the <see cref="ConnectionManager"/>. The target table must already exist (with the same column names), unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the source table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="targetTable">The name of the target table. Its schema (i.e.: <c>Sales.Person</c>) is used when the schema is copied.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="mappings">The mappings of the source columns to the target columns (see <see cref="CopyColumnMapping"/>). Only the mapped columns are copied, and only for the requested table; the related tables are copied by their matching column names.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopyProgress"/> of the operation: its totals, and the tables that were copied (see <see cref="CopyProgress.Items"/>), each one with its source and its target table, its batches and its logs.</returns>
        public static Task<CopyProgress> CopyDataToAsync<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            string targetTable,
            string destinationConnection,
            IList<CopyColumnMapping> mappings,
            Expression<Func<TEntity, bool>> where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            connection.CopyDataToAsync(targetTable, ConnectionManager.Get(destinationConnection), mappings, where, bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Copies the rows of the source table to the same-named table of the destination connection that is registered in the <see cref="ConnectionManager"/>. The destination table must already exist, unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the source table.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="mappings">The mappings of the source columns to the target columns (see <see cref="CopyColumnMapping"/>). Only the mapped columns are copied, and only for the requested table; the related tables are copied by their matching column names.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopyProgress"/> of the operation: its totals, and the tables that were copied (see <see cref="CopyProgress.Items"/>), each one with its source and its target table, its batches and its logs.</returns>
        public static Task<CopyProgress> CopyDataToAsync(this IDbConnection connection,
            string tableName,
            string destinationConnection,
            IList<CopyColumnMapping> mappings,
            QueryGroup where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            connection.CopyDataToAsync(tableName, ConnectionManager.Get(destinationConnection), mappings, where, bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Copies the rows of the source table to the target table of the destination connection that is registered in the <see cref="ConnectionManager"/>. The target table must already exist (with the same column names), unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="sourceTable">The name of the source table.</param>
        /// <param name="targetTable">The name of the target table. Its schema (i.e.: <c>Sales.Person</c>) is used when the schema is copied.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="mappings">The mappings of the source columns to the target columns (see <see cref="CopyColumnMapping"/>). Only the mapped columns are copied, and only for the requested table; the related tables are copied by their matching column names.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopyProgress"/> of the operation: its totals, and the tables that were copied (see <see cref="CopyProgress.Items"/>), each one with its source and its target table, its batches and its logs.</returns>
        public static Task<CopyProgress> CopyDataToAsync(this IDbConnection connection,
            string sourceTable,
            string targetTable,
            string destinationConnection,
            IList<CopyColumnMapping> mappings,
            QueryGroup where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            connection.CopyDataToAsync(sourceTable, targetTable, ConnectionManager.Get(destinationConnection), mappings, where, bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction, cancellationToken);

        #endregion

        #region CopyDataTo With Mappings (IDbConnection, Async)

        /// <summary>
        /// Copies the rows of the table mapped to <typeparamref name="TEntity"/> to the same-named table of the destination connection. The destination table must already exist, unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The destination connection.</param>
        /// <param name="mappings">The mappings of the source columns to the target columns (see <see cref="CopyColumnMapping"/>). Only the mapped columns are copied, and only for the requested table; the related tables are copied by their matching column names.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopyProgress"/> of the operation: its totals, and the tables that were copied (see <see cref="CopyProgress.Items"/>), each one with its source and its target table, its batches and its logs.</returns>
        public static Task<CopyProgress> CopyDataToAsync<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            IDbConnection destinationConnection,
            IList<CopyColumnMapping> mappings,
            Expression<Func<TEntity, bool>> where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            connection.CopyDataToAsync(ClassMappedNameCache.Get<TEntity>(), destinationConnection, mappings, where == null ? null : QueryGroup.Parse(where), bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Copies the rows of the table mapped to <typeparamref name="TEntity"/> to the target table of the destination connection. The target table must already exist (with the same column names), unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the source table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="targetTable">The name of the target table. Its schema (i.e.: <c>Sales.Person</c>) is used when the schema is copied.</param>
        /// <param name="destinationConnection">The destination connection.</param>
        /// <param name="mappings">The mappings of the source columns to the target columns (see <see cref="CopyColumnMapping"/>). Only the mapped columns are copied, and only for the requested table; the related tables are copied by their matching column names.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopyProgress"/> of the operation: its totals, and the tables that were copied (see <see cref="CopyProgress.Items"/>), each one with its source and its target table, its batches and its logs.</returns>
        public static Task<CopyProgress> CopyDataToAsync<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            string targetTable,
            IDbConnection destinationConnection,
            IList<CopyColumnMapping> mappings,
            Expression<Func<TEntity, bool>> where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            connection.CopyDataToAsync(ClassMappedNameCache.Get<TEntity>(), targetTable, destinationConnection, mappings, where == null ? null : QueryGroup.Parse(where), bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Copies the rows of the source table to the same-named table of the destination connection. The destination table must already exist, unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the source table.</param>
        /// <param name="destinationConnection">The destination connection.</param>
        /// <param name="mappings">The mappings of the source columns to the target columns (see <see cref="CopyColumnMapping"/>). Only the mapped columns are copied, and only for the requested table; the related tables are copied by their matching column names.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopyProgress"/> of the operation: its totals, and the tables that were copied (see <see cref="CopyProgress.Items"/>), each one with its source and its target table, its batches and its logs.</returns>
        public static Task<CopyProgress> CopyDataToAsync(this IDbConnection connection,
            string tableName,
            IDbConnection destinationConnection,
            IList<CopyColumnMapping> mappings,
            QueryGroup where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            connection.CopyDataToAsync(tableName, tableName, destinationConnection, mappings, where, bufferSize, bypassExistingRows, qualifiers, relationshipBehavior, tableExistenceBehavior, identityBehavior, dataInterceptors, progressCallback, commandTimeout, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Copies the rows of the source table to the target table of the destination connection. The target table must already exist (with the same column names), unless the schema is copied first (see <paramref name="relationshipBehavior"/> and <paramref name="tableExistenceBehavior"/>).
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="sourceTable">The name of the source table.</param>
        /// <param name="targetTable">The name of the target table. Its schema (i.e.: <c>Sales.Person</c>) is used when the schema is copied.</param>
        /// <param name="destinationConnection">The destination connection.</param>
        /// <param name="mappings">The mappings of the source columns to the target columns (see <see cref="CopyColumnMapping"/>). Only the mapped columns are copied, and only for the requested table; the related tables are copied by their matching column names.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be copied. The default is <c>null</c>, which copies all the rows.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted per batch. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the target table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the target table as the qualifiers (its identity column if it has no primary key), unless <paramref name="qualifiers"/> are given. An <see cref="InvalidOperationException"/> is thrown if there are no qualifiers. If the provider has no bulk merge, the rows are merged one by one (an update by the qualifiers, then an insert if no row was updated). The default is <c>false</c>.</param>
        /// <param name="qualifiers">The names of the columns that identify the existing rows, which are the qualifiers of the merge. They are used only if <paramref name="bypassExistingRows"/> is enabled, and they must be columns of the target table that are copied. The default is <c>null</c>, which uses the primary key columns of the target table (its identity column if it has no primary key).</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table, parents first. The <paramref name="where"/> filter applies to the requested table only. Anything other than <see cref="CopyRelationshipBehavior.TableOnly"/> requires the source and the target tables to have the same name. The default is <see cref="CopyRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="tableExistenceBehavior">Defines what happens to the existing tables of the destination database before the rows are copied, as in <see cref="CopyTableSchemaTo"/>. The default is <see cref="TableExistsBehavior.Skip"/>. <b>WARNING:</b> <see cref="TableExistsBehavior.Drop"/> permanently deletes the existing tables and their data.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the source are kept, or ignored so the target database generates them (which requires <see cref="CopyRelationshipBehavior.TableOnly"/>). The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="CopyProgress"/> after every batch is inserted (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.CopyDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopyProgress"/> of the operation: its totals, and the tables that were copied (see <see cref="CopyProgress.Items"/>), each one with its source and its target table, its batches and its logs.</returns>
        public static async Task<CopyProgress> CopyDataToAsync(this IDbConnection connection,
            string sourceTable,
            string targetTable,
            IDbConnection destinationConnection,
            IList<CopyColumnMapping> mappings,
            QueryGroup where = null,
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            IList<string> qualifiers = null,
            CopyRelationshipBehavior relationshipBehavior = CopyRelationshipBehavior.TableOnly,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.CopyDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var targetSchema = GetTargetSchema(destinationConnection, targetTable);
            Validate(connection, sourceTable, targetTable, destinationConnection, targetSchema, bufferSize, relationshipBehavior, tableExistenceBehavior, LoadColumnMappingBehavior.Automatic, identityBehavior, mappings);
            var options = new CopyProgress { StartTime = DateTime.UtcNow };
            var tables = await GetTablesAsync(connection, destinationConnection, sourceTable, targetTable, targetSchema, where, relationshipBehavior, tableExistenceBehavior, commandTimeout, trace, transaction, cancellationToken, mappings).ConfigureAwait(false);
            options.TableCount = tables.Count;
            foreach (var (source, target, filter) in tables)
            {
                await CopyRowsAsync(connection, destinationConnection, source, target, filter, bufferSize, bypassExistingRows, qualifiers, LoadColumnMappingBehavior.Automatic, identityBehavior, string.Equals(source, sourceTable, StringComparison.Ordinal) ? mappings : null, options, progressCallback, commandTimeout, traceKey, trace, transaction, cancellationToken).ConfigureAwait(false);
            }
            options.EndTime = DateTime.UtcNow;
            return options;
        }

        #endregion

        #endregion
    }
}
