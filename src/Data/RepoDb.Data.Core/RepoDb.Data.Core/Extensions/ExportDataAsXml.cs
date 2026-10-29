#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Data.Helpers;
using RepoDb.Data.Interfaces;
using RepoDb.Data.Models;
using RepoDb.Interfaces;

namespace RepoDb.Data
{
    /// <summary>
    /// Contains the extension methods of <see cref="IDbConnection"/> object for exporting the rows of a table into a XML file.
    /// </summary>
    public static class ExportDataAsXmlExtension
    {
        #region Public Methods

        /// <summary>
        /// Exports the rows of a table into a XML file without holding them in memory: the rows are counted first (with the filter), then they are read forward only and every row
        /// is written into the file as soon as it is read. The file is an XML document: a <c>rows</c> root element that has a <c>row</c> element for each row, and each one has an element for each column (named by the column, encoded if it is not a valid XML name), so <c>ImportDataFromXml</c> can load it back. A <c>null</c> is a self-closing element, and an empty text is an element with its start and end tags.
        /// The rows are written into a temporary file that replaces <paramref name="filename"/> only when the export has succeeded, so a failed export leaves no partial file behind.
        /// </summary>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="tableName">The name of the table that the rows are exported from.</param>
        /// <param name="filename">The path of the XML file. An existing file is replaced.</param>
        /// <param name="mappings">The mappings of the columns of the table (the source) to the names of the elements of the file (the destination), as in <c>CopyDataTo</c> (see <see cref="ColumnMapping"/>). Only the mapped columns are selected and written. The default is <c>null</c>, which selects all the columns of the table.</param>
        /// <param name="where">The filter of the rows to be exported, which also filters the rows that are counted. The default is <c>null</c>, which exports all the rows.</param>
        /// <param name="dataInterceptors">The interceptors that every row goes through before it is written, in the order that they are given: each one receives the <see cref="Row"/> that the previous one returned, and the row that the last one returns is the one that is written, with the columns that it has. The columns of the row are the ones that are selected (named by the <paramref name="mappings"/>, if there are any). The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="ExportProgress"/> after the rows are counted, after every row is written, and when the export has ended (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.ExportDataAsXml"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <returns>The <see cref="ExportProgress"/> of the export, which is the same instance that the callback receives.</returns>
        /// <exception cref="ArgumentNullException">The connection, the table name or the file name is <c>null</c> (or white space).</exception>
        /// <exception cref="ArgumentException">A data interceptor is <c>null</c>, or a mapping is not valid (it is <c>null</c>, it has an empty name, its source column is not a column of the table, or its destination is used more than once).</exception>
        /// <exception cref="InvalidOperationException">A data interceptor returned <c>null</c>.</exception>
        public static ExportProgress ExportDataAsXml(this IDbConnection connection,
            string tableName,
            string filename,
            IList<ColumnMapping> mappings = null,
            QueryGroup where = null,
            IList<IDataInterceptor<Row>> dataInterceptors = null,
            Action<ExportProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.ExportDataAsXml,
            ITrace trace = null)
        {
            return ExportPipeline.Run(connection,
                tableName,
                filename,
                mappings,
                where,
                dataInterceptors,
                progressCallback,
                commandTimeout,
                traceKey,
                trace,
                stream => new XmlExportWriter(stream));
        }

        /// <summary>
        /// Exports the rows of a table into a XML file in an asynchronous way. See <see cref="ExportDataAsXml"/> for how the rows are counted, read and written.
        /// </summary>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="tableName">The name of the table that the rows are exported from.</param>
        /// <param name="filename">The path of the XML file. An existing file is replaced.</param>
        /// <param name="mappings">The mappings of the columns of the table (the source) to the names of the elements of the file (the destination), as in <c>CopyDataTo</c> (see <see cref="ColumnMapping"/>). Only the mapped columns are selected and written. The default is <c>null</c>, which selects all the columns of the table.</param>
        /// <param name="where">The filter of the rows to be exported, which also filters the rows that are counted. The default is <c>null</c>, which exports all the rows.</param>
        /// <param name="dataInterceptors">The interceptors that every row goes through before it is written, in the order that they are given: each one receives the <see cref="Row"/> that the previous one returned, and the row that the last one returns is the one that is written, with the columns that it has. The columns of the row are the ones that are selected (named by the <paramref name="mappings"/>, if there are any). The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="ExportProgress"/> after the rows are counted, after every row is written, and when the export has ended (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.ExportDataAsXml"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="ExportProgress"/> of the export, which is the same instance that the callback receives.</returns>
        /// <exception cref="ArgumentNullException">The connection, the table name or the file name is <c>null</c> (or white space).</exception>
        /// <exception cref="ArgumentException">A data interceptor is <c>null</c>, or a mapping is not valid (it is <c>null</c>, it has an empty name, its source column is not a column of the table, or its destination is used more than once).</exception>
        /// <exception cref="InvalidOperationException">A data interceptor returned <c>null</c>.</exception>
        public static Task<ExportProgress> ExportDataAsXmlAsync(this IDbConnection connection,
            string tableName,
            string filename,
            IList<ColumnMapping> mappings = null,
            QueryGroup where = null,
            IList<IDataInterceptor<Row>> dataInterceptors = null,
            Action<ExportProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.ExportDataAsXml,
            ITrace trace = null,
            CancellationToken cancellationToken = default)
        {
            return ExportPipeline.RunAsync(connection,
                tableName,
                filename,
                mappings,
                where,
                dataInterceptors,
                progressCallback,
                commandTimeout,
                traceKey,
                trace,
                stream => new XmlExportWriter(stream),
                cancellationToken);
        }

        /// <summary>
        /// Exports the rows of the table mapped to <typeparamref name="TEntity"/> into a XML file. See <see cref="ExportDataAsXml(IDbConnection, string, string, IList{ColumnMapping}, QueryGroup, IList{IDataInterceptor{Row}}, Action{ExportProgress}, int? , string, ITrace)"/> for how the rows are counted, read and written.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="filename">The path of the XML file. An existing file is replaced.</param>
        /// <param name="mappings">The mappings of the columns of the table (the source) to the names of the elements of the file (the destination), as in <c>CopyDataTo</c> (see <see cref="ColumnMapping"/>). Only the mapped columns are selected and written. The default is <c>null</c>, which selects all the columns of the table.</param>
        /// <param name="where">The expression used to filter the rows of the table to be exported, which also filters the rows that are counted. The default is <c>null</c>, which exports all the rows.</param>
        /// <param name="dataInterceptors">The interceptors that every row goes through before it is written, in the order that they are given: each one receives the <see cref="Row"/> that the previous one returned, and the row that the last one returns is the one that is written, with the columns that it has. The columns of the row are the ones that are selected (named by the <paramref name="mappings"/>, if there are any). The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="ExportProgress"/> after the rows are counted, after every row is written, and when the export has ended (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.ExportDataAsXml"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <returns>The <see cref="ExportProgress"/> of the export, which is the same instance that the callback receives.</returns>
        /// <exception cref="ArgumentNullException">The connection, the table name or the file name is <c>null</c> (or white space).</exception>
        /// <exception cref="ArgumentException">A data interceptor is <c>null</c>, or a mapping is not valid (it is <c>null</c>, it has an empty name, its source column is not a column of the table, or its destination is used more than once).</exception>
        /// <exception cref="InvalidOperationException">A data interceptor returned <c>null</c>.</exception>
        public static ExportProgress ExportDataAsXml<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            string filename,
            IList<ColumnMapping> mappings = null,
            Expression<Func<TEntity, bool>> where = null,
            IList<IDataInterceptor<Row>> dataInterceptors = null,
            Action<ExportProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.ExportDataAsXml,
            ITrace trace = null)
            where TEntity : class =>
            connection.ExportDataAsXml(ClassMappedNameCache.Get<TEntity>(),
                filename,
                mappings,
                where == null ? null : QueryGroup.Parse(where),
                dataInterceptors,
                progressCallback,
                commandTimeout,
                traceKey,
                trace);

        /// <summary>
        /// Exports the rows of a table into a XML file, filtered by an expression of <typeparamref name="TEntity"/>. See <see cref="ExportDataAsXml(IDbConnection, string, string, IList{ColumnMapping}, QueryGroup, IList{IDataInterceptor{Row}}, Action{ExportProgress}, int? , string, ITrace)"/> for how the rows are counted, read and written.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that the expression is of.</typeparam>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="tableName">The name of the table that the rows are exported from.</param>
        /// <param name="filename">The path of the XML file. An existing file is replaced.</param>
        /// <param name="mappings">The mappings of the columns of the table (the source) to the names of the elements of the file (the destination), as in <c>CopyDataTo</c> (see <see cref="ColumnMapping"/>). Only the mapped columns are selected and written. The default is <c>null</c>, which selects all the columns of the table.</param>
        /// <param name="where">The expression used to filter the rows of the table to be exported, which also filters the rows that are counted. The default is <c>null</c>, which exports all the rows.</param>
        /// <param name="dataInterceptors">The interceptors that every row goes through before it is written, in the order that they are given: each one receives the <see cref="Row"/> that the previous one returned, and the row that the last one returns is the one that is written, with the columns that it has. The columns of the row are the ones that are selected (named by the <paramref name="mappings"/>, if there are any). The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="ExportProgress"/> after the rows are counted, after every row is written, and when the export has ended (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.ExportDataAsXml"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <returns>The <see cref="ExportProgress"/> of the export, which is the same instance that the callback receives.</returns>
        /// <exception cref="ArgumentNullException">The connection, the table name or the file name is <c>null</c> (or white space).</exception>
        /// <exception cref="ArgumentException">A data interceptor is <c>null</c>, or a mapping is not valid (it is <c>null</c>, it has an empty name, its source column is not a column of the table, or its destination is used more than once).</exception>
        /// <exception cref="InvalidOperationException">A data interceptor returned <c>null</c>.</exception>
        public static ExportProgress ExportDataAsXml<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            string tableName,
            string filename,
            IList<ColumnMapping> mappings = null,
            Expression<Func<TEntity, bool>> where = null,
            IList<IDataInterceptor<Row>> dataInterceptors = null,
            Action<ExportProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.ExportDataAsXml,
            ITrace trace = null)
            where TEntity : class =>
            connection.ExportDataAsXml(tableName,
                filename,
                mappings,
                where == null ? null : QueryGroup.Parse(where),
                dataInterceptors,
                progressCallback,
                commandTimeout,
                traceKey,
                trace);

        /// <summary>
        /// Exports the rows of the table mapped to <typeparamref name="TEntity"/> into a XML file in an asynchronous way. See <see cref="ExportDataAsXml(IDbConnection, string, string, IList{ColumnMapping}, QueryGroup, IList{IDataInterceptor{Row}}, Action{ExportProgress}, int? , string, ITrace)"/> for how the rows are counted, read and written.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="filename">The path of the XML file. An existing file is replaced.</param>
        /// <param name="mappings">The mappings of the columns of the table (the source) to the names of the elements of the file (the destination), as in <c>CopyDataTo</c> (see <see cref="ColumnMapping"/>). Only the mapped columns are selected and written. The default is <c>null</c>, which selects all the columns of the table.</param>
        /// <param name="where">The expression used to filter the rows of the table to be exported, which also filters the rows that are counted. The default is <c>null</c>, which exports all the rows.</param>
        /// <param name="dataInterceptors">The interceptors that every row goes through before it is written, in the order that they are given: each one receives the <see cref="Row"/> that the previous one returned, and the row that the last one returns is the one that is written, with the columns that it has. The columns of the row are the ones that are selected (named by the <paramref name="mappings"/>, if there are any). The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="ExportProgress"/> after the rows are counted, after every row is written, and when the export has ended (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.ExportDataAsXml"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="ExportProgress"/> of the export, which is the same instance that the callback receives.</returns>
        /// <exception cref="ArgumentNullException">The connection, the table name or the file name is <c>null</c> (or white space).</exception>
        /// <exception cref="ArgumentException">A data interceptor is <c>null</c>, or a mapping is not valid (it is <c>null</c>, it has an empty name, its source column is not a column of the table, or its destination is used more than once).</exception>
        /// <exception cref="InvalidOperationException">A data interceptor returned <c>null</c>.</exception>
        public static Task<ExportProgress> ExportDataAsXmlAsync<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            string filename,
            IList<ColumnMapping> mappings = null,
            Expression<Func<TEntity, bool>> where = null,
            IList<IDataInterceptor<Row>> dataInterceptors = null,
            Action<ExportProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.ExportDataAsXml,
            ITrace trace = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            connection.ExportDataAsXmlAsync(ClassMappedNameCache.Get<TEntity>(),
                filename,
                mappings,
                where == null ? null : QueryGroup.Parse(where),
                dataInterceptors,
                progressCallback,
                commandTimeout,
                traceKey,
                trace,
                cancellationToken);

        /// <summary>
        /// Exports the rows of a table into a XML file in an asynchronous way, filtered by an expression of <typeparamref name="TEntity"/>. See <see cref="ExportDataAsXml(IDbConnection, string, string, IList{ColumnMapping}, QueryGroup, IList{IDataInterceptor{Row}}, Action{ExportProgress}, int? , string, ITrace)"/> for how the rows are counted, read and written.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that the expression is of.</typeparam>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="tableName">The name of the table that the rows are exported from.</param>
        /// <param name="filename">The path of the XML file. An existing file is replaced.</param>
        /// <param name="mappings">The mappings of the columns of the table (the source) to the names of the elements of the file (the destination), as in <c>CopyDataTo</c> (see <see cref="ColumnMapping"/>). Only the mapped columns are selected and written. The default is <c>null</c>, which selects all the columns of the table.</param>
        /// <param name="where">The expression used to filter the rows of the table to be exported, which also filters the rows that are counted. The default is <c>null</c>, which exports all the rows.</param>
        /// <param name="dataInterceptors">The interceptors that every row goes through before it is written, in the order that they are given: each one receives the <see cref="Row"/> that the previous one returned, and the row that the last one returns is the one that is written, with the columns that it has. The columns of the row are the ones that are selected (named by the <paramref name="mappings"/>, if there are any). The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="ExportProgress"/> after the rows are counted, after every row is written, and when the export has ended (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.ExportDataAsXml"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="ExportProgress"/> of the export, which is the same instance that the callback receives.</returns>
        /// <exception cref="ArgumentNullException">The connection, the table name or the file name is <c>null</c> (or white space).</exception>
        /// <exception cref="ArgumentException">A data interceptor is <c>null</c>, or a mapping is not valid (it is <c>null</c>, it has an empty name, its source column is not a column of the table, or its destination is used more than once).</exception>
        /// <exception cref="InvalidOperationException">A data interceptor returned <c>null</c>.</exception>
        public static Task<ExportProgress> ExportDataAsXmlAsync<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            string tableName,
            string filename,
            IList<ColumnMapping> mappings = null,
            Expression<Func<TEntity, bool>> where = null,
            IList<IDataInterceptor<Row>> dataInterceptors = null,
            Action<ExportProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.ExportDataAsXml,
            ITrace trace = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            connection.ExportDataAsXmlAsync(tableName,
                filename,
                mappings,
                where == null ? null : QueryGroup.Parse(where),
                dataInterceptors,
                progressCallback,
                commandTimeout,
                traceKey,
                trace,
                cancellationToken);

        #endregion
    }
}
