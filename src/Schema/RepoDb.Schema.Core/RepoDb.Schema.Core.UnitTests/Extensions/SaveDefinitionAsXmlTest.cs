#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Exceptions;
using RepoDb.Interfaces;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Core.UnitTests.Extensions
{
    [TestClass]
    public class SaveDefinitionAsXmlTest : SaveDefinitionTestBase
    {
        #region Methods

        [TestMethod]
        public void TestSaveDefinitionAsXmlReplacesExistingFile()
        {
            // Setup
            var connection = new CustomDbConnection();
            File.WriteAllText(Filename, "previous definition");

            // Act
            connection.SaveDefinitionAsXml("sales.Invoice", Filename);
            var actual = File.ReadAllText(Filename);

            // Assert
            Assert.AreNotEqual("previous definition", actual);
        }

        [TestMethod]
        public void TestSaveDefinitionAsXmlLeavesNoTemporaryFiles()
        {
            // Setup
            var connection = new CustomDbConnection();
            File.WriteAllText(Filename, "previous definition");

            // Act
            connection.SaveDefinitionAsXml("sales.Invoice", Filename);
            var actual = Directory.GetFiles(DirectoryPath);

            // Assert
            Assert.AreEqual(1, actual.Length);
        }

        [TestMethod]
        public void TestSaveDefinitionAsXmlRejectsNullConnection()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            Action action = () => SaveDefinitionExtension.SaveDefinitionAsXml(null, "sales.Invoice", Filename);

            // Assert
            Assert.ThrowsExactly<ArgumentNullException>(action);
        }

        [TestMethod]
        public void TestSaveDefinitionAsXmlRejectsEmptyTableName()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            Action action = () => connection.SaveDefinitionAsXml(" ", Filename);

            // Assert
            Assert.ThrowsExactly<ArgumentException>(action);
        }

        [TestMethod]
        public void TestSaveDefinitionAsXmlRejectsEmptyFilename()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            Action action = () => connection.SaveDefinitionAsXml("sales.Invoice", " ");

            // Assert
            Assert.ThrowsExactly<ArgumentException>(action);
        }

        [TestMethod]
        public void TestSaveDefinitionAsXmlRejectsInvalidRelationshipBehavior()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            Action action = () => connection.SaveDefinitionAsXml("sales.Invoice", Filename, (RelatedTableBehavior)999);

            // Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(action);
        }

        [TestMethod]
        public void TestSaveDefinitionAsXmlRejectsNegativeTimeout()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            Action action = () => connection.SaveDefinitionAsXml("sales.Invoice", Filename, commandTimeout: -1);

            // Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(action);
        }

        [TestMethod]
        public void TestSaveDefinitionAsXmlRejectsMissingTable()
        {
            // Setup
            var connection = new CustomDbConnection();
            Reader.Setup(r => r.TableExists("missing")).Returns(false);

            // Act
            Action action = () => connection.SaveDefinitionAsXml("missing", Filename);

            // Assert
            Assert.ThrowsExactly<InvalidOperationException>(action);
        }

        [TestMethod]
        public void TestSaveDefinitionAsXmlRejectsMissingReader()
        {
            // Setup
            var connection = new CustomDbConnection();
            SchemaReaderMapper.Clear();

            // Act
            Action action = () => connection.SaveDefinitionAsXml("sales.Invoice", Filename);

            // Assert
            Assert.ThrowsExactly<MissingMappingException>(action);
        }

        [TestMethod]
        public void TestSaveDefinitionAsXmlPropagatesCatalogFailure()
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
            var exception = CaptureException(() => connection.SaveDefinitionAsXml("sales.Invoice", Filename,
                commandTimeout: 42,
                traceKey: "export-test",
                trace: trace));

            // Assert
            Assert.IsInstanceOfType<InvalidOperationException>(exception);
        }

        [TestMethod]
        public void TestSaveDefinitionAsXmlPassesCommandTimeoutToReader()
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
            var exception = CaptureException(() => connection.SaveDefinitionAsXml("sales.Invoice", Filename,
                commandTimeout: 42,
                traceKey: "export-test",
                trace: trace));

            // Assert
            Assert.AreEqual(42, observed.CommandTimeout);
        }

        [TestMethod]
        public void TestSaveDefinitionAsXmlPassesTraceKeyToReader()
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
            var exception = CaptureException(() => connection.SaveDefinitionAsXml("sales.Invoice", Filename,
                commandTimeout: 42,
                traceKey: "export-test",
                trace: trace));

            // Assert
            Assert.AreEqual("export-test", observed.TraceKey);
        }

        [TestMethod]
        public void TestSaveDefinitionAsXmlPassesTraceToReader()
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
            var exception = CaptureException(() => connection.SaveDefinitionAsXml("sales.Invoice", Filename,
                commandTimeout: 42,
                traceKey: "export-test",
                trace: trace));

            // Assert
            Assert.AreSame(trace, observed.Trace);
        }

        [TestMethod]
        public void TestSaveDefinitionAsXmlRestoresContextAfterFailure()
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
            var exception = CaptureException(() => connection.SaveDefinitionAsXml("sales.Invoice", Filename,
                commandTimeout: 42,
                traceKey: "export-test",
                trace: trace));

            // Assert
            Assert.IsNull(SchemaReadContext.Current);
        }

        [TestMethod]
        public void TestSaveDefinitionAsXmlPreservesExistingFileAfterFailure()
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
            var exception = CaptureException(() => connection.SaveDefinitionAsXml("sales.Invoice", Filename,
                commandTimeout: 42,
                traceKey: "export-test",
                trace: trace));

            // Assert
            Assert.AreEqual("previous definition", File.ReadAllText(Filename));
        }

        [TestMethod]
        public void TestSaveDefinitionAsXmlLeavesNoTemporaryFilesAfterFailure()
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
            var exception = CaptureException(() => connection.SaveDefinitionAsXml("sales.Invoice", Filename,
                commandTimeout: 42,
                traceKey: "export-test",
                trace: trace));

            // Assert
            Assert.AreEqual(1, Directory.GetFiles(DirectoryPath).Length);
        }

        [TestMethod]
        public void TestSaveDefinitionAsXmlPreservesJsonValuesAndTypes()
        {
            // Setup
            var connection = new CustomDbConnection();
            connection.SaveDefinitionAsJson("sales.Invoice", Filename, RelatedTableBehavior.Parents);
            var expected = JsonNode.Parse(File.ReadAllText(Filename));

            // Act
            connection.SaveDefinitionAsXml("sales.Invoice", Filename, RelatedTableBehavior.Parents);
            var actual = XDocument.Load(Filename);

            // Assert
            Assert.IsTrue(JsonNode.DeepEquals(expected, FromXml(actual.Root)));
        }

        [TestMethod]
        public void TestSaveDefinitionAsXmlDeclaresUtf8Encoding()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            connection.SaveDefinitionAsXml("sales.Invoice", Filename);
            var actual = File.ReadAllText(Filename);

            // Assert
            StringAssert.Contains(actual, "encoding=\"utf-8\"");
        }

        [TestMethod]
        public void TestSaveDefinitionAsXmlRejectsInvalidCharacters()
        {
            // Setup
            var connection = new CustomDbConnection();
            File.WriteAllText(Filename, "previous definition");
            Invoice.Columns[0].Comment = "invalid\u0001xml";

            // Act
            var exception = CaptureException(() => connection.SaveDefinitionAsXml("sales.Invoice", Filename));

            // Assert
            Assert.IsInstanceOfType<ArgumentException>(exception);
        }

        [TestMethod]
        public void TestSaveDefinitionAsXmlPreservesExistingFileForInvalidCharacters()
        {
            // Setup
            var connection = new CustomDbConnection();
            File.WriteAllText(Filename, "previous definition");
            Invoice.Columns[0].Comment = "invalid\u0001xml";

            // Act
            var exception = CaptureException(() => connection.SaveDefinitionAsXml("sales.Invoice", Filename));

            // Assert
            Assert.AreEqual("previous definition", File.ReadAllText(Filename));
        }

        [TestMethod]
        public void TestSaveDefinitionAsXmlRestoresContextForInvalidCharacters()
        {
            // Setup
            var connection = new CustomDbConnection();
            File.WriteAllText(Filename, "previous definition");
            Invoice.Columns[0].Comment = "invalid\u0001xml";

            // Act
            var exception = CaptureException(() => connection.SaveDefinitionAsXml("sales.Invoice", Filename));

            // Assert
            Assert.IsNull(SchemaReadContext.Current);
        }

        [TestMethod]
        public void TestSaveDefinitionAsXmlLeavesNoTemporaryFilesForInvalidCharacters()
        {
            // Setup
            var connection = new CustomDbConnection();
            File.WriteAllText(Filename, "previous definition");
            Invoice.Columns[0].Comment = "invalid\u0001xml";

            // Act
            var exception = CaptureException(() => connection.SaveDefinitionAsXml("sales.Invoice", Filename));

            // Assert
            Assert.AreEqual(1, Directory.GetFiles(DirectoryPath).Length);
        }

        #endregion

        #region Helpers

        private static JsonNode FromXml(XElement element)
        {
            switch ((string)element.Attribute("type"))
            {
                case "object": return new JsonObject(element.Elements().Select(e => new System.Collections.Generic.KeyValuePair<string, JsonNode>(e.Name.LocalName, FromXml(e))));
                case "array": return new JsonArray(element.Elements().Select(FromXml).ToArray());
                case "null": return null;
                case "string": return JsonValue.Create(element.Value);
                default: return JsonNode.Parse(element.Value);
            }
        }

        #endregion
    }
}
