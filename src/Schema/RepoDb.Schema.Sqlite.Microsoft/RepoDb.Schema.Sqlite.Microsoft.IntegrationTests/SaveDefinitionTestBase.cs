#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Interfaces;

namespace RepoDb.Schema.Sqlite.Microsoft.IntegrationTests
{
    /// <summary>
    /// Contains the save definition test base behavior.
    /// </summary>
    public abstract class SaveDefinitionTestBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        protected SqliteConnection Connection { get; private set; }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        protected string Filename { get; private set; }

        #endregion

        #region Initialization and Cleanup

        /// <summary>
        /// Performs initialize.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration.Setup().UseSqliteSchema();
            Filename=Path.Combine(Path.GetTempPath(),
                "repodb-live-export-" + Guid.NewGuid().ToString("N"));
            Connection = new SqliteConnection("Data Source=:memory:");
            Connection.Open();
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = @"CREATE TABLE Customer (Id integer PRIMARY KEY);
                    CREATE TABLE Invoice (Id integer PRIMARY KEY, CustomerId integer REFERENCES Customer(Id),
                        Amount decimal(18,4) NOT NULL DEFAULT 0, Name nvarchar(100) COLLATE NOCASE,
                        CONSTRAINT CK_Amount CHECK (Amount >= 0));
                    CREATE TABLE InvoiceLine (Id integer PRIMARY KEY, InvoiceId integer REFERENCES Invoice(Id));
                    CREATE INDEX IX_Invoice_Customer ON Invoice(CustomerId);";
                command.ExecuteNonQuery();
            }

            SchemaReaderMapper.Add<SqliteConnection>(new SqliteSchemaReader(),
                true);
        }

        /// <summary>
        /// Performs cleanup.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            SchemaReaderMapper.Remove<SqliteConnection>();
            Connection?.Dispose();
            if (File.Exists(Filename))
                File.Delete(Filename);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Contains the catalog trace behavior.
        /// </summary>
        protected sealed class CatalogTrace : ITrace
        {
        #region Properties

            /// <summary>
            /// Gets or sets the after keys.
            /// </summary>
            internal List<string> AfterKeys { get; } = new List<string>();
            /// <summary>
            /// Gets or sets the before keys.
            /// </summary>
            internal List<string> BeforeKeys { get; } = new List<string>();
            /// <summary>
            /// Gets or sets the timeouts.
            /// </summary>
            internal List<int?> Timeouts { get; } = new List<int?>();

        #endregion

        #region Public Methods

            /// <summary>
            /// Performs before execution.
            /// </summary>
            /// <param name="log">The log.</param>
            public void BeforeExecution(CancellableTraceLog log)
            {
                BeforeKeys.Add(log.Key);
                Timeouts.Add(SchemaReadContext.Current?.CommandTimeout);
            }

            /// <summary>
            /// Performs after execution.
            /// </summary>
            /// <param name="log">The log.</param>
            /// <typeparam name="TResult">The tresult type.</typeparam>
            public void AfterExecution<TResult>(ResultTraceLog<TResult> log) => AfterKeys.Add(log.Key);
            /// <summary>
            /// Performs before execution async.
            /// </summary>
            /// <param name="log">The log.</param>
            /// <param name="cancellationToken">The cancellation token.</param>
            public Task BeforeExecutionAsync(CancellableTraceLog log,
                CancellationToken cancellationToken = default) => throw new NotSupportedException();
            /// <summary>
            /// Performs after execution async.
            /// </summary>
            /// <param name="log">The log.</param>
            /// <param name="cancellationToken">The cancellation token.</param>
            /// <typeparam name="TResult">The tresult type.</typeparam>
            public Task AfterExecutionAsync<TResult>(ResultTraceLog<TResult> log,
                CancellationToken cancellationToken = default) => throw new NotSupportedException();
        #endregion
        }
        #endregion
    }
}
