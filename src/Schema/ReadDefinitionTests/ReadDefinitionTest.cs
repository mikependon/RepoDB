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
using Moq;
using RepoDb.Exceptions;
using RepoDb.Interfaces;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Tests
{
    /// <summary>
    /// Shared contract tests compiled in every schema provider's unit test project.
    /// </summary>
    [TestClass]
    public class ReadDefinitionTest
    {
        #region Fields

        private string filename;
        private IDbConnection connection;

        #endregion

        #region Initialization and Cleanup

        [TestInitialize]
        public void Initialize()
        {
            filename = Path.Combine(Path.GetTempPath(), "repodb-read-" + Guid.NewGuid().ToString("N"));
            connection = new Mock<IDbConnection>(MockBehavior.Strict).Object;
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(filename)) File.Delete(filename);
        }

        #endregion

        #region Methods

        public static IEnumerable<object[]> FormatsAndBehaviors()
        {
            foreach (var format in new[] { "json", "xml", "yaml" })
                foreach (RelatedTableBehavior behavior in Enum.GetValues(typeof(RelatedTableBehavior)))
                    foreach (var asynchronous in new[] { false, true })
                        yield return new object[] { format, behavior, asynchronous };
        }

        [TestMethod]
        [DynamicData(nameof(FormatsAndBehaviors))]
        public async Task RoundTripPreservesCompleteDefinition(string format, RelatedTableBehavior behavior, bool asynchronous)
        {
            // Setup
            var expected = Definition(behavior);
            File.WriteAllText(filename, SchemaDefinitionSerializer.Serialize(expected, format));

            // Act
            var actual = await Read(format, asynchronous);

            // Assert
            Assert.IsTrue(JsonNode.DeepEquals(expected, actual.Definition));
            Assert.AreEqual(1, actual.FormatVersion);
            Assert.AreEqual(behavior, actual.RelationshipBehavior);
            Assert.AreEqual("Source", actual.Database);
            Assert.AreEqual(Provider(), actual.Provider);
            Assert.AreEqual("Invoice", actual.RootTable.Name);
            Assert.AreEqual("sales", actual.RootTable.Schema);
            Assert.AreEqual(Provider(), actual.RootTable.Provider);
            var invoice = actual.Tables.Single(t => t.Table.Name == "Invoice");
            Assert.AreEqual("Source", invoice.Table.Database);
            Assert.AreEqual(Provider(), invoice.Table.Provider);
            var id = invoice.Columns.Single(c => c.Field.Name == "Id");
            Assert.AreEqual(typeof(long), id.Field.Type);
            Assert.IsTrue(id.Field.IsPrimary);
            Assert.IsTrue(id.Field.IsIdentity);
            Assert.IsFalse(id.Field.IsNullable);
            Assert.AreEqual(7L, id.IdentitySeed);
            Assert.AreEqual(3L, id.IdentityIncrement);
            var amount = invoice.Columns.Single(c => c.Field.Name == "Amount");
            Assert.AreEqual(300, amount.NumericPrecision);
            Assert.AreEqual(-2, amount.NumericScale);
            Assert.IsNull(amount.Field.Precision);
            Assert.IsNull(amount.Field.Scale);
            Assert.IsTrue(amount.Field.HasDefaultValue);
            Assert.AreEqual(ColumnDefaultValue.Literal, amount.DefaultValue);
            Assert.AreEqual("((0))", amount.DefaultExpression);
            Assert.AreEqual("decimal(300,-2)", amount.NativeType);
            Assert.AreEqual("Amount * 2", amount.ComputedExpression);
            Assert.AreEqual("A: 'quote'\n雪 & <xml>\t\"value\"", amount.Comment);
            var name = invoice.Columns.Single(c => c.Field.Name == "Name");
            Assert.AreEqual(-1, name.Field.Size);
            Assert.AreEqual(true, name.IsUnicode);
            Assert.AreEqual("UTF-16", name.CharacterSet);
            Assert.AreEqual("utf16CodeUnits", name.LengthUnit);
            Assert.AreEqual("Latin1_General_100_CI_AS", name.Collation);
            Assert.AreEqual(6, name.TemporalPrecision);
            CollectionAssert.AreEqual(new[] { "Id", "Amount" }, invoice.PrimaryKey.Columns.ToArray());
            Assert.IsTrue(invoice.PrimaryKey.IsClustered);
            CollectionAssert.AreEqual(new[] { "Name", "Amount" }, invoice.UniqueConstraints.Single().Columns.ToArray());
            var index = invoice.Indexes.Single();
            Assert.IsTrue(index.IsUnique);
            Assert.IsTrue(index.IsClustered);
            CollectionAssert.AreEqual(new[] { "Name", "Amount" }, index.Columns.ToArray());
            CollectionAssert.AreEqual(new[] { "Amount" }, index.DescendingColumns.ToArray());
            CollectionAssert.AreEqual(new[] { "Id" }, index.IncludedColumns.ToArray());
            Assert.AreEqual("Amount > 0", index.Filter);
            Assert.AreEqual("Amount >= 0", invoice.CheckConstraints.Single().Expression);
            if (behavior == RelatedTableBehavior.TableOnly)
            {
                Assert.AreEqual(1, actual.Tables.Count);
                Assert.AreEqual(0, invoice.ForeignKeys.Count);
            }
            else
            {
                var key = invoice.ForeignKeys.Single();
                CollectionAssert.AreEqual(new[] { "Id", "Amount" }, key.Columns.ToArray());
                CollectionAssert.AreEqual(new[] { "Id", "Amount" }, key.ReferencedColumns.ToArray());
                Assert.AreEqual("Customer", key.ReferencedTable.Name);
                Assert.AreEqual(Provider(), key.ReferencedTable.Provider);
                Assert.AreEqual(ForeignKeyRule.Cascade, key.DeleteRule);
                Assert.AreEqual(ForeignKeyRule.SetNull, key.UpdateRule);
                Assert.AreEqual(behavior == RelatedTableBehavior.Children ? 1 : 0, actual.ExternalDependencies.Count);
            }
            // Reserializing models also verifies every field used by the existing writer.
            Assert.IsTrue(JsonNode.DeepEquals(expected, SchemaDefinition.Create(actual.RootTable, actual.Tables, behavior)));
        }

        [TestMethod]
        [DataRow("json")]
        [DataRow("xml")]
        [DataRow("yaml")]
        public async Task RejectsMalformedFilesAndMissingMetadata(string format)
        {
            File.WriteAllText(filename, "not a definition");
            await Assert.ThrowsExactlyAsync<InvalidDataException>(() => Read(format, true));
            var document = Definition(RelatedTableBehavior.Parents);
            foreach (var property in new[] { "format", "formatVersion", "source", "extraction", "tables", "externalDependencies" })
            {
                var invalid = (JsonObject)document.DeepClone();
                invalid.Remove(property);
                File.WriteAllText(filename, SchemaDefinitionSerializer.Serialize(invalid, format));
                await Assert.ThrowsExactlyAsync<InvalidDataException>(() => Read(format, false));
                await Assert.ThrowsExactlyAsync<InvalidDataException>(() => Read(format, true));
            }
        }

        [TestMethod]
        [DataRow("json")]
        [DataRow("xml")]
        [DataRow("yaml")]
        public async Task RejectsUnsupportedVersionsAndInvalidReferences(string format)
        {
            foreach (var mutation in new Action<JsonObject>[]
            {
                d => d["formatVersion"] = 99,
                d => d["format"] = "Other.Schema",
                d => d["extraction"]["relationshipBehavior"] = "99",
                d => d["extraction"]["rootTable"]["name"] = "Missing",
                d => d["tables"][0]["primaryKey"]["columns"][0] = "Missing",
                d => d["tables"].AsArray().Add(d["tables"][0].DeepClone()),
                d => d["tables"][1]["columns"].AsArray().Add(d["tables"][1]["columns"][0].DeepClone()),
                d => d["tables"][1]["foreignKeys"][0]["references"]["columns"][0] = "Missing",
                d => d["tables"][1]["foreignKeys"][0]["onDelete"] = "Unknown",
                d => d["tables"][1]["foreignKeys"][0]["references"]["external"] = true,
                d => d["tables"][1]["columns"][0]["ordinal"] = 0,
                d => d["tables"][0] = null,
                d => d["tables"][1]["columns"][0] = null,
                d => d["tables"][1]["indexes"][0] = null
            })
            {
                var document = Definition(RelatedTableBehavior.Parents);
                mutation(document);
                File.WriteAllText(filename, SchemaDefinitionSerializer.Serialize(document, format));
                await Assert.ThrowsExactlyAsync<InvalidDataException>(() => Read(format, true));
            }
        }

        [TestMethod]
        [DataRow("json")]
        [DataRow("xml")]
        [DataRow("yaml")]
        public async Task ValidatesArgumentsAndMissingFiles(string format)
        {
            var path = filename;
            try
            {
                await Assert.ThrowsExactlyAsync<FileNotFoundException>(() => Read(format, true));
                filename = " ";
                await Assert.ThrowsExactlyAsync<ArgumentException>(() => Read(format, false));
                filename = path;
                connection = null;
                await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => Read(format, true));
            }
            finally { filename = path; }
        }

        [TestMethod]
        [DataRow("json")]
        [DataRow("xml")]
        [DataRow("yaml")]
        public async Task RejectsInvalidTextEncoding(string format)
        {
            File.WriteAllBytes(filename, new byte[] { 0xC3, 0x28 });
            await Assert.ThrowsExactlyAsync<InvalidDataException>(() => Read(format, false));
            await Assert.ThrowsExactlyAsync<InvalidDataException>(() => Read(format, true));
        }

        [TestMethod]
        [DataRow("json")]
        [DataRow("xml")]
        [DataRow("yaml")]
        public async Task HonorsCancellationBeforeFileAccess(string format)
        {
            using (var cancellation = new CancellationTokenSource())
            {
                cancellation.Cancel();
                await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => Read(format, true, token: cancellation.Token));
            }
        }

        [TestMethod]
        [DynamicData(nameof(FormatsAndBehaviors))]
        public async Task TracesFileReadAndResult(string format, RelatedTableBehavior behavior, bool asynchronous)
        {
            File.WriteAllText(filename, SchemaDefinitionSerializer.Serialize(Definition(behavior), format));
            var trace = new Mock<ITrace>(MockBehavior.Strict);
            CancellableTraceLog before = null;
            ResultTraceLog<SchemaDefinitionResult> after = null;
            if (asynchronous)
            {
                trace.Setup(t => t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(), It.IsAny<CancellationToken>()))
                    .Callback<CancellableTraceLog, CancellationToken>((log, _) => before = log).Returns(Task.CompletedTask);
                trace.Setup(t => t.AfterExecutionAsync(It.IsAny<ResultTraceLog<SchemaDefinitionResult>>(), It.IsAny<CancellationToken>()))
                    .Callback<ResultTraceLog<SchemaDefinitionResult>, CancellationToken>((log, _) => after = log).Returns(Task.CompletedTask);
            }
            else
            {
                trace.Setup(t => t.BeforeExecution(It.IsAny<CancellableTraceLog>())).Callback<CancellableTraceLog>(log => before = log);
                trace.Setup(t => t.AfterExecution(It.IsAny<ResultTraceLog<SchemaDefinitionResult>>()))
                    .Callback<ResultTraceLog<SchemaDefinitionResult>>(log => after = log);
            }
            var result = await Read(format, asynchronous, trace.Object);
            Assert.AreEqual(filename, before.Statement);
            Assert.AreEqual("custom-read", before.Key);
            Assert.AreEqual(before.SessionId, after.SessionId);
            Assert.AreSame(result, after.Result);
            Assert.AreSame(before, after.BeforeExecutionLog);
            trace.VerifyAll();
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(false, true)]
        [DataRow(true, false)]
        [DataRow(true, true)]
        public async Task TraceCanCancelWithoutReadingFile(bool asynchronous, bool throws)
        {
            var trace = new Mock<ITrace>(MockBehavior.Strict);
            if (asynchronous)
                trace.Setup(t => t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(), It.IsAny<CancellationToken>()))
                    .Callback<CancellableTraceLog, CancellationToken>((log, _) => log.Cancel(throws)).Returns(Task.CompletedTask);
            else
                trace.Setup(t => t.BeforeExecution(It.IsAny<CancellableTraceLog>())).Callback<CancellableTraceLog>(log => log.Cancel(throws));
            if (throws) await Assert.ThrowsExactlyAsync<CancelledExecutionException>(() => Read("json", asynchronous, trace.Object));
            else Assert.IsNull(await Read("json", asynchronous, trace.Object));
        }

        [TestMethod]
        public async Task RejectsXmlExternalEntities()
        {
            File.WriteAllText(filename, "<!DOCTYPE repoDbSchema [<!ENTITY x SYSTEM 'file:///missing'>]><repoDbSchema type='object'>&x;</repoDbSchema>");
            await Assert.ThrowsExactlyAsync<InvalidDataException>(() => Read("xml", true));
        }

        [TestMethod]
        public async Task ReadsIndependentJsonAndUnknownLogicalType()
        {
            File.WriteAllText(filename, @"{""format"":""RepoDB.Schema"",""formatVersion"":1,
                ""source"":{""provider"":""Example"",""database"":null},
                ""extraction"":{""rootTable"":{""name"":""T"",""schema"":null,""database"":null,""provider"":""Example""},""relationshipBehavior"":""TableOnly""},
                ""tables"":[{""table"":{""name"":""T"",""schema"":null,""database"":null,""provider"":""Example""},
                    ""columns"":[{""name"":""Value"",""ordinal"":1,""nullable"":true,
                        ""type"":{""logical"":""unknown"",""native"":""custom"",""provider"":""Example"",""length"":null,""precision"":null,""scale"":null,""unicode"":null,""characterSet"":null,""lengthUnit"":null},
                        ""identity"":null,""default"":null,""computed"":null,""collation"":null,""comment"":null}],
                    ""primaryKey"":null,""uniqueConstraints"":[],""indexes"":[],""checkConstraints"":[],""foreignKeys"":[]}],""externalDependencies"":[]}");
            var result = await Read("json", true);
            Assert.IsNull(result.Tables.Single().Columns.Single().Field.Type);
            Assert.AreEqual("custom", result.Tables.Single().Columns.Single().NativeType);
            Assert.IsNull(result.Database);
        }

        [TestMethod]
        [DataRow("json")]
        [DataRow("xml")]
        [DataRow("yaml")]
        public async Task PreservesCyclesAndSelfReferences(string format)
        {
            var document = Definition(RelatedTableBehavior.ParentsAndChildren);
            var customer = document["tables"][0];
            var invoice = document["tables"][1];
            var reverse = invoice["foreignKeys"][0].DeepClone();
            reverse["name"] = "FK_Reverse";
            reverse["references"]["table"] = invoice["table"].DeepClone();
            customer["foreignKeys"].AsArray().Add(reverse);
            var self = reverse.DeepClone();
            self["name"] = "FK_Self";
            self["references"]["table"] = customer["table"].DeepClone();
            customer["foreignKeys"].AsArray().Add(self);
            File.WriteAllText(filename, SchemaDefinitionSerializer.Serialize(document, format));
            var result = await Read(format, true);
            Assert.AreEqual(2, result.Tables.Count);
            Assert.AreEqual(2, result.Tables[0].ForeignKeys.Count);
            Assert.AreEqual("Invoice", result.Tables[0].ForeignKeys[0].ReferencedTable.Name);
            Assert.AreEqual("Customer", result.Tables[0].ForeignKeys[1].ReferencedTable.Name);
        }

        [TestMethod]
        [DataRow("json")]
        [DataRow("xml")]
        [DataRow("yaml")]
        public async Task RestoresEveryPortableLogicalType(string format)
        {
            var types = new[] { typeof(string), typeof(byte[]), typeof(bool), typeof(Guid), typeof(sbyte), typeof(byte),
                typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(decimal),
                typeof(float), typeof(double), typeof(DateTime), typeof(DateTimeOffset), typeof(TimeSpan) };
            var table = new TableSchema(new TableInfo("Types", null, provider: Provider()));
            for (var index = 0; index < types.Length; index++)
                table.Columns.Add(new ColumnInfo
                {
                    Field = new DbField("C" + index, false, false, true, types[index], null, null, null, null), Ordinal = index + 1
                });
            var document = SchemaDefinition.Create(table.Table, new[] { table }, RelatedTableBehavior.TableOnly);
            File.WriteAllText(filename, SchemaDefinitionSerializer.Serialize(document, format));
            var result = await Read(format, true);
            CollectionAssert.AreEqual(types, result.Tables.Single().Columns.Select(c => c.Field.Type).ToArray());
            Assert.IsTrue(result.Tables.Single().Columns.All(c => c.Field.IsNullable && !c.Field.IsIdentity && !c.Field.HasDefaultValue));
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TraceCanRedirectFilename(bool asynchronous)
        {
            var path = filename;
            File.WriteAllText(path, SchemaDefinitionSerializer.Serialize(Definition(RelatedTableBehavior.TableOnly), "json"));
            var trace = new Mock<ITrace>();
            if (asynchronous)
                trace.Setup(t => t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(), It.IsAny<CancellationToken>()))
                    .Callback<CancellableTraceLog, CancellationToken>((log, _) => log.Statement = path).Returns(Task.CompletedTask);
            else trace.Setup(t => t.BeforeExecution(It.IsAny<CancellableTraceLog>())).Callback<CancellableTraceLog>(log => log.Statement = path);
            try
            {
                filename = path + ".missing";
                Assert.AreEqual("Invoice", (await Read("json", asynchronous, trace.Object)).RootTable.Name);
            }
            finally { filename = path; }
        }

        #endregion

        #region Helpers

        private Task<SchemaDefinitionResult> Read(string format, bool asynchronous, ITrace trace = null, CancellationToken token = default)
        {
            if (format == "json") return asynchronous ? connection.ReadDefinitionFromJsonAsync(filename, "custom-read", trace, token)
                : Task.FromResult(connection.ReadDefinitionFromJson(filename, "custom-read", trace));
            if (format == "xml") return asynchronous ? connection.ReadDefinitionFromXmlAsync(filename, "custom-read", trace, token)
                : Task.FromResult(connection.ReadDefinitionFromXml(filename, "custom-read", trace));
            return asynchronous ? connection.ReadDefinitionFromYamlAsync(filename, "custom-read", trace, token)
                : Task.FromResult(connection.ReadDefinitionFromYaml(filename, "custom-read", trace));
        }

        private static string Provider() => typeof(ReadDefinitionTest).Assembly.GetName().Name
            .Replace("RepoDb.Schema.", "").Replace(".UnitTests", "");

        private static JsonObject Definition(RelatedTableBehavior behavior)
        {
            var invoice = new TableSchema(new TableInfo("Invoice", "sales", "Source", Provider()));
            var customer = new TableSchema(new TableInfo("Customer", "sales", "Source", Provider()));
            invoice.Columns.Add(new ColumnInfo
            {
                Field = new DbField("Id", true, true, false, typeof(long), null, null, null, "bigint", false, Provider()),
                Ordinal = 1, IdentitySeed = 7, IdentityIncrement = 3
            });
            invoice.Columns.Add(new ColumnInfo
            {
                Field = new DbField("Amount", true, false, false, typeof(decimal), null, null, null, "decimal(300,-2)", true, Provider()),
                Ordinal = 2, NumericPrecision = 300, NumericScale = -2, DefaultValue = ColumnDefaultValue.Literal,
                DefaultExpression = "((0))", ComputedExpression = "Amount * 2", Comment = "A: 'quote'\n雪 & <xml>\t\"value\""
            });
            invoice.Columns.Add(new ColumnInfo
            {
                Field = new DbField("Name", false, false, true, typeof(string), -1, null, null, "nvarchar", false, Provider()),
                Ordinal = 3, CharacterSet = "UTF-16", IsUnicode = true, LengthUnit = "utf16CodeUnits",
                Collation = "Latin1_General_100_CI_AS", TemporalPrecision = 6
            });
            foreach (var column in invoice.Columns.Take(2)) customer.Columns.Add(column);
            invoice.PrimaryKey = new PrimaryKeyInfo("PK_Invoice") { Columns = new[] { "Id", "Amount" }, IsClustered = true };
            customer.PrimaryKey = new PrimaryKeyInfo("PK_Customer") { Columns = new[] { "Id", "Amount" } };
            invoice.UniqueConstraints.Add(new UniqueConstraintInfo("UQ_Invoice") { Columns = new[] { "Name", "Amount" } });
            invoice.Indexes.Add(new IndexInfo("IX_Invoice")
            {
                Columns = new[] { "Name", "Amount" }, DescendingColumns = new[] { "Amount" }, IncludedColumns = new[] { "Id" },
                IsClustered = true, IsUnique = true, Filter = "Amount > 0"
            });
            invoice.CheckConstraints.Add(new CheckConstraintInfo("CK_Invoice") { Expression = "Amount >= 0" });
            invoice.ForeignKeys.Add(new ForeignKeyInfo("FK_Customer")
            {
                Columns = new[] { "Id", "Amount" }, ReferencedColumns = new[] { "Id", "Amount" }, ReferencedTable = customer.Table,
                DeleteRule = ForeignKeyRule.Cascade, UpdateRule = ForeignKeyRule.SetNull
            });
            var tables = new List<TableSchema> { invoice };
            if (behavior == RelatedTableBehavior.Parents || behavior == RelatedTableBehavior.ParentsAndChildren) tables.Add(customer);
            return SchemaDefinition.Create(invoice.Table, tables, behavior);
        }

        #endregion
    }
}
