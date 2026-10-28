#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Interfaces;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Extensions
{
    [TestClass]
    public class LoadDefinitionTest
    {
        private string filename;

        [TestInitialize]
        public void Initialize()
        {
            filename = Path.Combine(Path.GetTempPath(), "repodb-load-core-" + Guid.NewGuid().ToString("N"));
            SchemaReaderMapper.Clear();
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeName(It.IsAny<TableInfo>())).Returns<TableInfo>(table => table.Name);
            composer.Setup(c => c.ComposeTableExists(It.IsAny<string>())).Returns<string>(table => "EXISTS " + table);
            composer.Setup(c => c.ComposeSchema(It.IsAny<TableSchema>())).Returns<TableSchema>(table => new[] { "CREATE " + table.Table.Name });
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns<IEnumerable<TableSchema>>(tables => tables.Select(table => "CREATE " + table.Table.Name));
            SchemaComposerMapper.Add<CustomDbConnection>(composer.Object, true);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(filename)) File.Delete(filename);
            SchemaReaderMapper.Clear();
            SchemaComposerMapper.Clear();
        }

        [TestMethod]
        [DataRow("json", false)]
        [DataRow("json", true)]
        [DataRow("xml", false)]
        [DataRow("xml", true)]
        [DataRow("yaml", false)]
        [DataRow("yaml", true)]
        public async Task ForwardsTransactionTimeoutAndTraceToProbesAndCommands(string format, bool asynchronous)
        {
            Write(format);
            using var connection = new CustomDbConnection();
            using var transaction = connection.BeginTransaction();
            var trace = new RecordingTrace(asynchronous);
            var callbacks = new List<CopyTableSchemaResultItem>();
            var result = await Load(connection, format, asynchronous, callbacks.Add, transaction, trace);
            Assert.AreEqual(2, connection.Details.Count);
            Assert.IsTrue(connection.Details.All(d => d.CommandTimeout == 17 && ReferenceEquals(d.Transaction, transaction)));
            Assert.IsTrue(trace.Keys.Count >= 3); // File read, destination probe, and DDL.
            Assert.IsTrue(trace.Keys.All(key => key == "load-test"));
            Assert.AreEqual(1, callbacks.Count);
            Assert.AreSame(callbacks.Single(), result.CopyResult.Items.Single());
            Assert.AreSame(result.Definition.Tables.Single(), callbacks.Single().Source.Table);
            Assert.AreEqual("FileDatabase", callbacks.Single().Source.Database);
            Assert.AreEqual("FileProvider", callbacks.Single().Source.DatabaseType);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TraceCancellationDoesNotAccessDatabase(bool asynchronous)
        {
            using var connection = new CustomDbConnection();
            var trace = new RecordingTrace(asynchronous) { Cancel = true };
            Assert.IsNull(await Load(connection, "json", asynchronous, trace: trace));
            Assert.AreEqual(0, connection.Details.Count);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task CallbackRunsOnlyAfterSuccessfulExecution(bool asynchronous)
        {
            Write("json");
            using var connection = new CustomDbConnection { FailWhen = _ => true };
            var callbacks = new List<CopyTableSchemaResultItem>();
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => Load(connection, "json", asynchronous, callbacks.Add));
            Assert.AreEqual(0, callbacks.Count);
        }

        [TestMethod]
        public async Task InvalidArgumentsDoNotReadFileOrExecuteCommands()
        {
            using var connection = new CustomDbConnection();
            using var other = new CustomDbConnection();
            using var transaction = other.BeginTransaction();
            await Assert.ThrowsExactlyAsync<ArgumentException>(() => connection.LoadDefinitionFromJsonAsync(filename, transaction: transaction));
            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() => connection.LoadDefinitionFromYamlAsync(filename, commandTimeout: -1));
            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() => connection.LoadDefinitionFromXmlAsync(filename, (TableExistsBehavior)99));
            await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => ((IDbConnection)null).LoadDefinitionFromJsonAsync(filename));
            await Assert.ThrowsExactlyAsync<ArgumentException>(() => connection.LoadDefinitionFromJsonAsync(" "));
            Assert.AreEqual(0, connection.Details.Count);
        }

        [TestMethod]
        [DataRow("json", false)]
        [DataRow("json", true)]
        [DataRow("xml", false)]
        [DataRow("xml", true)]
        [DataRow("yaml", false)]
        [DataRow("yaml", true)]
        public async Task LoadsCyclesAndSelfReferencesOncePerTable(string format, bool asynchronous)
        {
            var first = new TableSchema(new TableInfo("CycleA", null, "FileDatabase", "FileProvider"));
            var second = new TableSchema(new TableInfo("CycleB", null, "FileDatabase", "FileProvider"));
            foreach (var table in new[] { first, second })
            {
                table.Columns.Add(new ColumnInfo { Ordinal = 1, Field = new DbField("Id", true, false, false, typeof(int), null, null, null, null) });
                table.PrimaryKey = new PrimaryKeyInfo("PK_" + table.Table.Name) { Columns = new[] { "Id" } };
            }
            first.ForeignKeys.Add(new ForeignKeyInfo("FK_A_B") { Columns = new[] { "Id" }, ReferencedTable = second.Table, ReferencedColumns = new[] { "Id" } });
            first.ForeignKeys.Add(new ForeignKeyInfo("FK_A_A") { Columns = new[] { "Id" }, ReferencedTable = first.Table, ReferencedColumns = new[] { "Id" } });
            second.ForeignKeys.Add(new ForeignKeyInfo("FK_B_A") { Columns = new[] { "Id" }, ReferencedTable = first.Table, ReferencedColumns = new[] { "Id" } });
            var definition = SchemaDefinition.Create(first.Table, new[] { second, first }, RelatedTableBehavior.ParentsAndChildren);
            File.WriteAllText(filename, SchemaDefinitionSerializer.Serialize(definition, format));
            using var connection = new CustomDbConnection();
            var callbacks = new List<CopyTableSchemaResultItem>();
            var result = await Load(connection, format, asynchronous, callbacks.Add);
            Assert.AreEqual(2, result.CopyResult.Items.Count);
            Assert.AreEqual(2, callbacks.Count);
            Assert.AreEqual(2, connection.ExecutedCommands.Count);
            Assert.AreEqual(3, result.Definition.Tables.Sum(table => table.ForeignKeys.Count));
        }

        private void Write(string format)
        {
            var schema = new TableSchema(new TableInfo("FileTable", "FileSchema", "FileDatabase", "FileProvider"));
            schema.Columns.Add(new ColumnInfo { Ordinal = 1, Field = new DbField("Id", true, false, false, typeof(int), null, null, null, null) });
            schema.PrimaryKey = new PrimaryKeyInfo("PK_FileTable") { Columns = new[] { "Id" } };
            var definition = SchemaDefinition.Create(schema.Table, new[] { schema }, RelatedTableBehavior.TableOnly);
            File.WriteAllText(filename, SchemaDefinitionSerializer.Serialize(definition, format));
        }

        private Task<LoadDefinitionResult> Load(IDbConnection connection, string format, bool asynchronous,
            Action<CopyTableSchemaResultItem> callback = null, IDbTransaction transaction = null, ITrace trace = null)
        {
            if (format == "json") return asynchronous ? connection.LoadDefinitionFromJsonAsync(filename, createdCallback: callback, commandTimeout: 17, traceKey: "load-test", trace: trace, transaction: transaction)
                : Task.FromResult(connection.LoadDefinitionFromJson(filename, createdCallback: callback, commandTimeout: 17, traceKey: "load-test", trace: trace, transaction: transaction));
            if (format == "xml") return asynchronous ? connection.LoadDefinitionFromXmlAsync(filename, createdCallback: callback, commandTimeout: 17, traceKey: "load-test", trace: trace, transaction: transaction)
                : Task.FromResult(connection.LoadDefinitionFromXml(filename, createdCallback: callback, commandTimeout: 17, traceKey: "load-test", trace: trace, transaction: transaction));
            return asynchronous ? connection.LoadDefinitionFromYamlAsync(filename, createdCallback: callback, commandTimeout: 17, traceKey: "load-test", trace: trace, transaction: transaction)
                : Task.FromResult(connection.LoadDefinitionFromYaml(filename, createdCallback: callback, commandTimeout: 17, traceKey: "load-test", trace: trace, transaction: transaction));
        }

        private sealed class RecordingTrace : ITrace
        {
            private readonly bool asynchronous;
            internal RecordingTrace(bool asynchronous) { this.asynchronous = asynchronous; }
            internal bool Cancel { get; set; }
            internal List<string> Keys { get; } = new List<string>();
            public void BeforeExecution(CancellableTraceLog log)
            {
                Assert.IsFalse(asynchronous);
                Keys.Add(log.Key);
                if (Cancel) log.Cancel(false);
            }
            public void AfterExecution<TResult>(ResultTraceLog<TResult> log) => Assert.IsFalse(asynchronous);
            public Task BeforeExecutionAsync(CancellableTraceLog log, CancellationToken cancellationToken = default)
            {
                Assert.IsTrue(asynchronous);
                Keys.Add(log.Key);
                if (Cancel) log.Cancel(false);
                return Task.CompletedTask;
            }
            public Task AfterExecutionAsync<TResult>(ResultTraceLog<TResult> log, CancellationToken cancellationToken = default)
            {
                Assert.IsTrue(asynchronous);
                return Task.CompletedTask;
            }
        }
    }
}
