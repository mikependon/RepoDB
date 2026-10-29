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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Exceptions;
using RepoDb.Interfaces;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Extensions
{
    [TestClass]
    public class SaveDefinitionAsJsonTest : SaveDefinitionTestBase
    {
        #region Methods

        [TestMethod]
        public void TestSaveDefinitionAsJsonWritesFormatIdentity()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.AreEqual("RepoDB.Schema", (string)actual["format"]);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonWritesFormatVersion()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.AreEqual(2, (int)actual["formatVersion"]);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonTableOnlyIncludesOnlyRequestedTable()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.AreEqual(1, actual["tables"].AsArray().Count);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonTableOnlyOmitsForeignKeys()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.AreEqual(0, actual["tables"][0]["foreignKeys"].AsArray().Count);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonDoesNotMutateSourceForeignKeys()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.AreEqual(1, Invoice.ForeignKeys.Count);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonPreservesNumericPrecision()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.AreEqual(18, (int)actual["tables"][0]["columns"][1]["type"]["precision"]);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonPreservesNumericScale()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.AreEqual(4, (int)actual["tables"][0]["columns"][1]["type"]["scale"]);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonPreservesUnicodeMetadata()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.IsTrue((bool)actual["tables"][0]["columns"][0]["type"]["unicode"]);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonPreservesLengthUnit()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.AreEqual("utf16CodeUnits", (string)actual["tables"][0]["columns"][0]["type"]["lengthUnit"]);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonPreservesCharacterSet()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.AreEqual("UTF-16", (string)actual["tables"][0]["columns"][0]["type"]["characterSet"]);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonPreservesDefaultExpression()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.AreEqual("((0))", (string)actual["tables"][0]["columns"][1]["default"]["expression"]["sql"]);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonPreservesComments()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.AreEqual(Invoice.Columns[0].Comment, (string)actual["tables"][0]["columns"][1]["comment"]);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonPreservesPrimaryKeyColumnOrder()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            CollectionAssert.AreEqual(new[] { "Name", "Amount" }, actual["tables"][0]["primaryKey"]["columns"].AsArray().Select(value => value.GetValue<string>()).ToArray());
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonPreservesPrimaryKeyClustering()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.IsFalse((bool)actual["tables"][0]["primaryKey"]["clustered"]);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonPreservesUniqueConstraints()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.AreEqual(1, actual["tables"][0]["uniqueConstraints"].AsArray().Count);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonPreservesIndexDirection()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.AreEqual("descending", (string)actual["tables"][0]["indexes"][0]["columns"][0]["direction"]);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonPreservesIndexFilter()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.AreEqual("[Amount] > 0", (string)actual["tables"][0]["indexes"][0]["filter"]["sql"]);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonPreservesCheckConstraints()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.AreEqual(1, actual["tables"][0]["checkConstraints"].AsArray().Count);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonTableOnlySkipsRelationshipTraversal()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Reader.Verify(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), It.IsAny<RelatedTableBehavior>()), Times.Never);
        }

        [TestMethod]
        [DataRow(RelatedTableBehavior.Parents)]
        [DataRow(RelatedTableBehavior.Children)]
        [DataRow(RelatedTableBehavior.ParentsAndChildren)]
        public void TestSaveDefinitionAsJsonIncludesEachRelatedTableOnce(RelatedTableBehavior behavior)
        {
            // Setup
            var connection = new CustomDbConnection();
            Customer.ForeignKeys.Add(new ForeignKeyInfo("FK_Cycle") { ReferencedTable = Invoice.Table });
            Reader.Setup(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), behavior))
                .Returns(new[] { "sales.Invoice", "sales.Customer", "sales.Invoice" });

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename, behavior);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.AreEqual(2, actual["tables"].AsArray().Count);
        }

        [TestMethod]
        [DataRow(RelatedTableBehavior.Parents)]
        [DataRow(RelatedTableBehavior.Children)]
        [DataRow(RelatedTableBehavior.ParentsAndChildren)]
        public void TestSaveDefinitionAsJsonMarksIncludedReferencesAsInternal(RelatedTableBehavior behavior)
        {
            // Setup
            var connection = new CustomDbConnection();
            Customer.ForeignKeys.Add(new ForeignKeyInfo("FK_Cycle") { ReferencedTable = Invoice.Table });
            Reader.Setup(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), behavior))
                .Returns(new[] { "sales.Invoice", "sales.Customer", "sales.Invoice" });

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename, behavior);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.IsNull(actual["tables"][1]["foreignKeys"][0]["references"]["external"]);
        }

        [TestMethod]
        [DataRow(RelatedTableBehavior.Parents)]
        [DataRow(RelatedTableBehavior.Children)]
        [DataRow(RelatedTableBehavior.ParentsAndChildren)]
        public void TestSaveDefinitionAsJsonPreservesDeleteRule(RelatedTableBehavior behavior)
        {
            // Setup
            var connection = new CustomDbConnection();
            Customer.ForeignKeys.Add(new ForeignKeyInfo("FK_Cycle") { ReferencedTable = Invoice.Table });
            Reader.Setup(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), behavior))
                .Returns(new[] { "sales.Invoice", "sales.Customer", "sales.Invoice" });

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename, behavior);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.AreEqual("Cascade", (string)actual["tables"][1]["foreignKeys"][0]["onDelete"]);
        }

        [TestMethod]
        [DataRow(RelatedTableBehavior.Parents)]
        [DataRow(RelatedTableBehavior.Children)]
        [DataRow(RelatedTableBehavior.ParentsAndChildren)]
        public void TestSaveDefinitionAsJsonPreservesReferencedColumnOrder(RelatedTableBehavior behavior)
        {
            // Setup
            var connection = new CustomDbConnection();
            Customer.ForeignKeys.Add(new ForeignKeyInfo("FK_Cycle") { ReferencedTable = Invoice.Table });
            Reader.Setup(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), behavior))
                .Returns(new[] { "sales.Invoice", "sales.Customer", "sales.Invoice" });

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename, behavior);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            CollectionAssert.AreEqual(new[] { "Name", "Amount" }, actual["tables"][1]["foreignKeys"][0]["references"]["columns"].AsArray().Select(value => value.GetValue<string>()).ToArray());
        }

        [TestMethod]
        [DataRow(RelatedTableBehavior.Parents)]
        [DataRow(RelatedTableBehavior.Children)]
        [DataRow(RelatedTableBehavior.ParentsAndChildren)]
        public void TestSaveDefinitionAsJsonUsesRequestedRelationshipBehavior(RelatedTableBehavior behavior)
        {
            // Setup
            var connection = new CustomDbConnection();
            Customer.ForeignKeys.Add(new ForeignKeyInfo("FK_Cycle") { ReferencedTable = Invoice.Table });
            Reader.Setup(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), behavior))
                .Returns(new[] { "sales.Invoice", "sales.Customer", "sales.Invoice" });

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename, behavior);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Reader.Verify(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), behavior), Times.Once);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonDoesNotExpandExternalDependencies()
        {
            // Setup
            var connection = new CustomDbConnection();
            Reader.Setup(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), RelatedTableBehavior.Children))
                .Returns(new[] { "sales.Invoice" });

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename, RelatedTableBehavior.Children);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.AreEqual(1, actual["tables"].AsArray().Count);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonOmitsDerivedExternalFlag()
        {
            // Setup
            var connection = new CustomDbConnection();
            Reader.Setup(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), RelatedTableBehavior.Children))
                .Returns(new[] { "sales.Invoice" });

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename, RelatedTableBehavior.Children);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.IsNull(actual["tables"][0]["foreignKeys"][0]["references"]["external"]);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonOmitsDerivedExternalDependencies()
        {
            // Setup
            var connection = new CustomDbConnection();
            Reader.Setup(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), RelatedTableBehavior.Children))
                .Returns(new[] { "sales.Invoice" });

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename, RelatedTableBehavior.Children);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.IsNull(actual["externalDependencies"]);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonKeepsUnknownEncodingUnknown()
        {
            // Setup
            var connection = new CustomDbConnection();
            Invoice.Columns[1].Field = new DbField("Name", false, false, true, typeof(string), 100, null, null, "varchar", false, "Other");

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.IsNull(actual["tables"][0]["columns"][0]["type"]["unicode"]);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonPreservesAsciiCharacterSet()
        {
            // Setup
            var connection = new CustomDbConnection();
            Invoice.Columns[1].Field = new DbField("Name", false, false, true, typeof(string), 100, null, null, "varchar", false, "Other");
            Invoice.Columns[1].CharacterSet = "ASCII";
            Invoice.Columns[1].IsUnicode = false;

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.AreEqual("ASCII", (string)actual["tables"][0]["columns"][0]["type"]["characterSet"]);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonPreservesNonUnicodeFlag()
        {
            // Setup
            var connection = new CustomDbConnection();
            Invoice.Columns[1].Field = new DbField("Name", false, false, true, typeof(string), 100, null, null, "varchar", false, "Other");
            Invoice.Columns[1].CharacterSet = "ASCII";
            Invoice.Columns[1].IsUnicode = false;

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.IsFalse((bool)actual["tables"][0]["columns"][0]["type"]["unicode"]);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonPreservesLargePrecision()
        {
            // Setup
            var connection = new CustomDbConnection();
            var data = new DataTable();
            data.Columns.Add("NativeType", typeof(string));
            data.Rows.Add("numeric(1000,-2)");
            using (var record = data.CreateDataReader())
            {
                record.Read();
                SchemaColumnMetadata.Apply(Invoice.Columns[0], record);
            }

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.AreEqual(1000, (int)actual["tables"][0]["columns"][1]["type"]["precision"]);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonPreservesNegativeScale()
        {
            // Setup
            var connection = new CustomDbConnection();
            var data = new DataTable();
            data.Columns.Add("NativeType", typeof(string));
            data.Rows.Add("numeric(1000,-2)");
            using (var record = data.CreateDataReader())
            {
                record.Read();
                SchemaColumnMetadata.Apply(Invoice.Columns[0], record);
            }

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.AreEqual(-2, (int)actual["tables"][0]["columns"][1]["type"]["scale"]);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonPreservesNativeNumericType()
        {
            // Setup
            var connection = new CustomDbConnection();
            var data = new DataTable();
            data.Columns.Add("NativeType", typeof(string));
            data.Rows.Add("numeric(1000,-2)");
            using (var record = data.CreateDataReader())
            {
                record.Read();
                SchemaColumnMetadata.Apply(Invoice.Columns[0], record);
            }

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.AreEqual("numeric(1000,-2)", (string)actual["tables"][0]["columns"][1]["type"]["native"]);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonReplacesExistingFile()
        {
            // Setup
            var connection = new CustomDbConnection();
            File.WriteAllText(Filename, "previous definition");

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename);
            var actual = File.ReadAllText(Filename);

            // Assert
            Assert.AreNotEqual("previous definition", actual);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonLeavesNoTemporaryFiles()
        {
            // Setup
            var connection = new CustomDbConnection();
            File.WriteAllText(Filename, "previous definition");

            // Act
            connection.SaveDefinitionAsJson("sales.Invoice", Filename);
            var actual = Directory.GetFiles(DirectoryPath);

            // Assert
            Assert.AreEqual(1, actual.Length);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonRejectsNullConnection()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            Action action = () => SaveDefinitionExtension.SaveDefinitionAsJson(null, "sales.Invoice", Filename);

            // Assert
            Assert.ThrowsExactly<ArgumentNullException>(action);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonRejectsEmptyTableName()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            Action action = () => connection.SaveDefinitionAsJson(" ", Filename);

            // Assert
            Assert.ThrowsExactly<ArgumentException>(action);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonRejectsEmptyFilename()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            Action action = () => connection.SaveDefinitionAsJson("sales.Invoice", " ");

            // Assert
            Assert.ThrowsExactly<ArgumentException>(action);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonRejectsInvalidRelationshipBehavior()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            Action action = () => connection.SaveDefinitionAsJson("sales.Invoice", Filename, (RelatedTableBehavior)999);

            // Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(action);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonRejectsNegativeTimeout()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            Action action = () => connection.SaveDefinitionAsJson("sales.Invoice", Filename, commandTimeout: -1);

            // Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(action);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonRejectsMissingTable()
        {
            // Setup
            var connection = new CustomDbConnection();
            Reader.Setup(r => r.TableExists("missing")).Returns(false);

            // Act
            Action action = () => connection.SaveDefinitionAsJson("missing", Filename);

            // Assert
            Assert.ThrowsExactly<InvalidOperationException>(action);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonRejectsMissingReader()
        {
            // Setup
            var connection = new CustomDbConnection();
            SchemaReaderMapper.Clear();

            // Act
            Action action = () => connection.SaveDefinitionAsJson("sales.Invoice", Filename);

            // Assert
            Assert.ThrowsExactly<MissingMappingException>(action);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonPropagatesCatalogFailure()
        {
            // Setup
            var connection = new CustomDbConnection();
            File.WriteAllText(Filename, "previous definition");
            SchemaReadContext observed = null;
            var trace = new Mock<ITrace>().Object;
            Reader.Setup(r => r.TableExists("sales.Invoice")).Returns(() =>
            {
                observed = SchemaReadContext.Current;
                throw new InvalidOperationException("catalog failed");
            });

            // Act
            var exception = CaptureException(() => connection.SaveDefinitionAsJson("sales.Invoice", Filename,
                commandTimeout: 42,
                traceKey: "export-test",
                trace: trace));

            // Assert
            Assert.IsInstanceOfType<InvalidOperationException>(exception);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonPassesCommandTimeoutToReader()
        {
            // Setup
            var connection = new CustomDbConnection();
            File.WriteAllText(Filename, "previous definition");
            SchemaReadContext observed = null;
            var trace = new Mock<ITrace>().Object;
            Reader.Setup(r => r.TableExists("sales.Invoice")).Returns(() =>
            {
                observed = SchemaReadContext.Current;
                throw new InvalidOperationException("catalog failed");
            });

            // Act
            var exception = CaptureException(() => connection.SaveDefinitionAsJson("sales.Invoice", Filename,
                commandTimeout: 42,
                traceKey: "export-test",
                trace: trace));

            // Assert
            Assert.AreEqual(42, observed.CommandTimeout);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonPassesTraceKeyToReader()
        {
            // Setup
            var connection = new CustomDbConnection();
            File.WriteAllText(Filename, "previous definition");
            SchemaReadContext observed = null;
            var trace = new Mock<ITrace>().Object;
            Reader.Setup(r => r.TableExists("sales.Invoice")).Returns(() =>
            {
                observed = SchemaReadContext.Current;
                throw new InvalidOperationException("catalog failed");
            });

            // Act
            var exception = CaptureException(() => connection.SaveDefinitionAsJson("sales.Invoice", Filename,
                commandTimeout: 42,
                traceKey: "export-test",
                trace: trace));

            // Assert
            Assert.AreEqual("export-test", observed.TraceKey);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonPassesTraceToReader()
        {
            // Setup
            var connection = new CustomDbConnection();
            File.WriteAllText(Filename, "previous definition");
            SchemaReadContext observed = null;
            var trace = new Mock<ITrace>().Object;
            Reader.Setup(r => r.TableExists("sales.Invoice")).Returns(() =>
            {
                observed = SchemaReadContext.Current;
                throw new InvalidOperationException("catalog failed");
            });

            // Act
            var exception = CaptureException(() => connection.SaveDefinitionAsJson("sales.Invoice", Filename,
                commandTimeout: 42,
                traceKey: "export-test",
                trace: trace));

            // Assert
            Assert.AreSame(trace, observed.Trace);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonRestoresContextAfterFailure()
        {
            // Setup
            var connection = new CustomDbConnection();
            File.WriteAllText(Filename, "previous definition");
            SchemaReadContext observed = null;
            var trace = new Mock<ITrace>().Object;
            Reader.Setup(r => r.TableExists("sales.Invoice")).Returns(() =>
            {
                observed = SchemaReadContext.Current;
                throw new InvalidOperationException("catalog failed");
            });

            // Act
            var exception = CaptureException(() => connection.SaveDefinitionAsJson("sales.Invoice", Filename,
                commandTimeout: 42,
                traceKey: "export-test",
                trace: trace));

            // Assert
            Assert.IsNull(SchemaReadContext.Current);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonPreservesExistingFileAfterFailure()
        {
            // Setup
            var connection = new CustomDbConnection();
            File.WriteAllText(Filename, "previous definition");
            SchemaReadContext observed = null;
            var trace = new Mock<ITrace>().Object;
            Reader.Setup(r => r.TableExists("sales.Invoice")).Returns(() =>
            {
                observed = SchemaReadContext.Current;
                throw new InvalidOperationException("catalog failed");
            });

            // Act
            var exception = CaptureException(() => connection.SaveDefinitionAsJson("sales.Invoice", Filename,
                commandTimeout: 42,
                traceKey: "export-test",
                trace: trace));

            // Assert
            Assert.AreEqual("previous definition", File.ReadAllText(Filename));
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonLeavesNoTemporaryFilesAfterFailure()
        {
            // Setup
            var connection = new CustomDbConnection();
            File.WriteAllText(Filename, "previous definition");
            SchemaReadContext observed = null;
            var trace = new Mock<ITrace>().Object;
            Reader.Setup(r => r.TableExists("sales.Invoice")).Returns(() =>
            {
                observed = SchemaReadContext.Current;
                throw new InvalidOperationException("catalog failed");
            });

            // Act
            var exception = CaptureException(() => connection.SaveDefinitionAsJson("sales.Invoice", Filename,
                commandTimeout: 42,
                traceKey: "export-test",
                trace: trace));

            // Assert
            Assert.AreEqual(1, Directory.GetFiles(DirectoryPath).Length);
        }

        #endregion
    }
}
