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
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Tests
{
    /// <summary>
    /// Exercises file loading with each real provider composer and its recording destination connection.
    /// </summary>
    public abstract class LoadDefinitionTestBase
    {
        private string filename;

        protected abstract IDbConnection Connection(bool exists);
        protected abstract IList<string> Commands(IDbConnection connection);

        [TestInitialize]
        public void Initialize()
        {
            filename = Path.Combine(Path.GetTempPath(), "repodb-load-" + Guid.NewGuid().ToString("N"));
            SchemaReaderMapper.Clear();
            SchemaComposerMapper.Clear();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(filename)) File.Delete(filename);
            SchemaReaderMapper.Clear();
            SchemaComposerMapper.Clear();
        }

        public static IEnumerable<object[]> Cases()
        {
            foreach (var format in new[] { "json", "xml", "yaml" })
                foreach (RelatedTableBehavior behavior in Enum.GetValues(typeof(RelatedTableBehavior)))
                    foreach (var asynchronous in new[] { false, true })
                        yield return new object[] { format, behavior, asynchronous };
        }

        [TestMethod]
        [DynamicData(nameof(Cases))]
        public async Task LoadsFileThroughProviderComposerAndReportsCompletedTables(string format, RelatedTableBehavior behavior, bool asynchronous)
        {
            var document = Write(format, behavior);
            using var connection = Connection(false);
            var callbacks = new List<CopyTableSchemaResultItem>();
            var result = await Load(connection, format, asynchronous, callback: callbacks.Add);

            Assert.IsTrue(JsonNode.DeepEquals(document, result.Definition.Definition));
            Assert.AreEqual(behavior, result.Definition.RelationshipBehavior);
            Assert.AreEqual(document["tables"].AsArray().Count, result.CopyResult.Items.Count);
            Assert.AreEqual(result.CopyResult.Items.Count, callbacks.Count);
            Assert.IsTrue(Commands(connection).Count > 0);
            Assert.AreEqual(0, result.CopyResult.Errors.Count);
            for (var position = 0; position < callbacks.Count; position++)
            {
                Assert.AreSame(callbacks[position], result.CopyResult.Items[position]);
                Assert.AreEqual(TableOutcome.Created, callbacks[position].Outcome);
                Assert.AreEqual("SavedDatabase", callbacks[position].Source.Database);
                Assert.AreEqual("SavedProvider", callbacks[position].Source.DatabaseType);
                Assert.IsNull(callbacks[position].Source.Server);
                Assert.IsTrue(result.Definition.Tables.Contains(callbacks[position].Source.Table));
            }
            var names = result.CopyResult.Items.Select(i => i.Source.Table.Table.Name).ToList();
            if (behavior == RelatedTableBehavior.Parents || behavior == RelatedTableBehavior.ParentsAndChildren)
                Assert.IsTrue(names.IndexOf("DefinitionParent") < names.IndexOf("DefinitionRoot"));
            if (behavior == RelatedTableBehavior.Children || behavior == RelatedTableBehavior.ParentsAndChildren)
                Assert.IsTrue(names.IndexOf("DefinitionRoot") < names.IndexOf("DefinitionChild"));
            var root = result.Definition.Tables.Single(t => t.Table.Name == "DefinitionRoot");
            Assert.AreEqual(3, root.Columns.Count);
            Assert.AreEqual(1, root.Indexes.Count);
            Assert.AreEqual(1, root.UniqueConstraints.Count);
            Assert.AreEqual(1, root.CheckConstraints.Count);
            Assert.IsNotNull(root.PrimaryKey);
            Assert.AreEqual(behavior == RelatedTableBehavior.TableOnly ? 0 : 1, root.ForeignKeys.Count);
        }

        [TestMethod]
        [DataRow("json", false)]
        [DataRow("json", true)]
        [DataRow("xml", false)]
        [DataRow("xml", true)]
        [DataRow("yaml", false)]
        [DataRow("yaml", true)]
        public async Task ExistingTablePoliciesMatchCopyBehavior(string format, bool asynchronous)
        {
            Write(format, RelatedTableBehavior.TableOnly);
            foreach (var behavior in new[] { TableExistsBehavior.Skip, TableExistsBehavior.Align, TableExistsBehavior.Throw, TableExistsBehavior.Drop })
            {
                using var connection = Connection(true);
                var callbacks = new List<CopyTableSchemaResultItem>();
                if (behavior == TableExistsBehavior.Throw)
                {
                    await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => Load(connection, format, asynchronous, behavior, callbacks.Add));
                    Assert.AreEqual(0, callbacks.Count);
                    Assert.AreEqual(0, Commands(connection).Count);
                    continue;
                }
                var result = await Load(connection, format, asynchronous, behavior, callbacks.Add);
                Assert.AreEqual(1, callbacks.Count);
                Assert.AreSame(result.CopyResult.Items.Single(), callbacks.Single());
                Assert.AreEqual(behavior == TableExistsBehavior.Skip ? TableOutcome.Skipped
                    : behavior == TableExistsBehavior.Align ? TableOutcome.Aligned : TableOutcome.Dropped, callbacks.Single().Outcome);
                if (behavior != TableExistsBehavior.Drop) Assert.AreEqual(0, Commands(connection).Count);
                else Assert.IsTrue(Commands(connection).Count > 0);
            }
        }

        [TestMethod]
        [DataRow("json")]
        [DataRow("xml")]
        [DataRow("yaml")]
        public async Task InvalidFileAndCancellationDoNotExecuteCommands(string format)
        {
            using var connection = Connection(false);
            File.WriteAllText(filename, "malformed definition");
            await Assert.ThrowsExactlyAsync<InvalidDataException>(() => Load(connection, format, true));
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();
            Write(format, RelatedTableBehavior.ParentsAndChildren);
            await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => Load(connection, format, true, token: cancellation.Token));
            Assert.AreEqual(0, Commands(connection).Count);
        }

        private JsonObject Write(string format, RelatedTableBehavior behavior)
        {
            var parent = Table("DefinitionParent");
            var root = Table("DefinitionRoot", parent.Table);
            var child = Table("DefinitionChild", root.Table);
            root.Indexes.Add(new IndexInfo("IX_DefinitionRoot_Value") { Columns = new[] { "Value" } });
            root.UniqueConstraints.Add(new UniqueConstraintInfo("UQ_DefinitionRoot_Value") { Columns = new[] { "Value" } });
            root.CheckConstraints.Add(new CheckConstraintInfo("CK_DefinitionRoot_Value") { Expression = "Value >= 0" });
            var tables = new List<TableSchema> { root };
            if (behavior == RelatedTableBehavior.Parents || behavior == RelatedTableBehavior.ParentsAndChildren) tables.Add(parent);
            if (behavior == RelatedTableBehavior.Children || behavior == RelatedTableBehavior.ParentsAndChildren) tables.Add(child);
            var document = SchemaDefinition.Create(root.Table, tables, behavior);
            File.WriteAllText(filename, SchemaDefinitionSerializer.Serialize(document, format));
            return document;
        }

        private static TableSchema Table(string name, TableInfo parent = null)
        {
            var table = new TableSchema(new TableInfo(name, null, "SavedDatabase", "SavedProvider"));
            table.Columns.Add(new ColumnInfo { Ordinal = 1, Field = Field("Id", true) });
            table.Columns.Add(new ColumnInfo { Ordinal = 2, Field = Field("Value") });
            if (parent != null)
            {
                table.Columns.Add(new ColumnInfo { Ordinal = 3, Field = Field("ParentId") });
                table.ForeignKeys.Add(new ForeignKeyInfo("FK_" + name)
                { Columns = new[] { "ParentId" }, ReferencedTable = parent, ReferencedColumns = new[] { "Id" } });
            }
            table.PrimaryKey = new PrimaryKeyInfo("PK_" + name) { Columns = new[] { "Id" } };
            return table;
        }

        private static DbField Field(string name, bool primary = false) =>
            new DbField(name, primary, false, !primary, typeof(int), null, null, null, null);

        private Task<LoadDefinitionResult> Load(IDbConnection connection, string format, bool asynchronous,
            TableExistsBehavior behavior = TableExistsBehavior.Skip, Action<CopyTableSchemaResultItem> callback = null, CancellationToken token = default)
        {
            if (format == "json") return asynchronous ? connection.LoadDefinitionFromJsonAsync(filename, behavior, callback, cancellationToken: token)
                : Task.FromResult(connection.LoadDefinitionFromJson(filename, behavior, callback));
            if (format == "xml") return asynchronous ? connection.LoadDefinitionFromXmlAsync(filename, behavior, createdCallback: callback, cancellationToken: token)
                : Task.FromResult(connection.LoadDefinitionFromXml(filename, behavior, createdCallback: callback));
            return asynchronous ? connection.LoadDefinitionFromYamlAsync(filename, behavior, callback, cancellationToken: token)
                : Task.FromResult(connection.LoadDefinitionFromYaml(filename, behavior, callback));
        }
    }
}
