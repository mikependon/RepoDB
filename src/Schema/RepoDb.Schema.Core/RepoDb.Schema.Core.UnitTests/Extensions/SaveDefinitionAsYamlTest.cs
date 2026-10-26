#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Exceptions;
using RepoDb.Interfaces;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Core.UnitTests.Extensions
{
    [TestClass]
    public class SaveDefinitionAsYamlTest : SaveDefinitionTestBase
    {
        #region Methods

        [TestMethod]
        public void TestSaveDefinitionAsYamlReplacesExistingFile()
        {
            // Setup
            var connection = new CustomDbConnection();
            File.WriteAllText(Filename, "previous definition");

            // Act
            connection.SaveDefinitionAsYaml("sales.Invoice", Filename);
            var actual = File.ReadAllText(Filename);

            // Assert
            Assert.AreNotEqual("previous definition", actual);
        }

        [TestMethod]
        public void TestSaveDefinitionAsYamlLeavesNoTemporaryFiles()
        {
            // Setup
            var connection = new CustomDbConnection();
            File.WriteAllText(Filename, "previous definition");

            // Act
            connection.SaveDefinitionAsYaml("sales.Invoice", Filename);
            var actual = Directory.GetFiles(DirectoryPath);

            // Assert
            Assert.AreEqual(1, actual.Length);
        }

        [TestMethod]
        public void TestSaveDefinitionAsYamlRejectsNullConnection()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            Action action = () => SaveDefinitionExtension.SaveDefinitionAsYaml(null, "sales.Invoice", Filename);

            // Assert
            Assert.ThrowsExactly<ArgumentNullException>(action);
        }

        [TestMethod]
        public void TestSaveDefinitionAsYamlRejectsEmptyTableName()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            Action action = () => connection.SaveDefinitionAsYaml(" ", Filename);

            // Assert
            Assert.ThrowsExactly<ArgumentException>(action);
        }

        [TestMethod]
        public void TestSaveDefinitionAsYamlRejectsEmptyFilename()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            Action action = () => connection.SaveDefinitionAsYaml("sales.Invoice", " ");

            // Assert
            Assert.ThrowsExactly<ArgumentException>(action);
        }

        [TestMethod]
        public void TestSaveDefinitionAsYamlRejectsInvalidRelationshipBehavior()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            Action action = () => connection.SaveDefinitionAsYaml("sales.Invoice", Filename, (RelatedTableBehavior)999);

            // Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(action);
        }

        [TestMethod]
        public void TestSaveDefinitionAsYamlRejectsNegativeTimeout()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            Action action = () => connection.SaveDefinitionAsYaml("sales.Invoice", Filename, commandTimeout: -1);

            // Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(action);
        }

        [TestMethod]
        public void TestSaveDefinitionAsYamlRejectsMissingTable()
        {
            // Setup
            var connection = new CustomDbConnection();
            Reader.Setup(r => r.TableExists("missing")).Returns(false);

            // Act
            Action action = () => connection.SaveDefinitionAsYaml("missing", Filename);

            // Assert
            Assert.ThrowsExactly<InvalidOperationException>(action);
        }

        [TestMethod]
        public void TestSaveDefinitionAsYamlRejectsMissingReader()
        {
            // Setup
            var connection = new CustomDbConnection();
            SchemaReaderMapper.Clear();

            // Act
            Action action = () => connection.SaveDefinitionAsYaml("sales.Invoice", Filename);

            // Assert
            Assert.ThrowsExactly<MissingMappingException>(action);
        }

        [TestMethod]
        public void TestSaveDefinitionAsYamlPropagatesCatalogFailure()
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
            var exception = CaptureException(() => connection.SaveDefinitionAsYaml("sales.Invoice", Filename,
                commandTimeout: 42,
                traceKey: "export-test",
                trace: trace));

            // Assert
            Assert.IsInstanceOfType<InvalidOperationException>(exception);
        }

        [TestMethod]
        public void TestSaveDefinitionAsYamlPassesCommandTimeoutToReader()
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
            var exception = CaptureException(() => connection.SaveDefinitionAsYaml("sales.Invoice", Filename,
                commandTimeout: 42,
                traceKey: "export-test",
                trace: trace));

            // Assert
            Assert.AreEqual(42, observed.CommandTimeout);
        }

        [TestMethod]
        public void TestSaveDefinitionAsYamlPassesTraceKeyToReader()
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
            var exception = CaptureException(() => connection.SaveDefinitionAsYaml("sales.Invoice", Filename,
                commandTimeout: 42,
                traceKey: "export-test",
                trace: trace));

            // Assert
            Assert.AreEqual("export-test", observed.TraceKey);
        }

        [TestMethod]
        public void TestSaveDefinitionAsYamlPassesTraceToReader()
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
            var exception = CaptureException(() => connection.SaveDefinitionAsYaml("sales.Invoice", Filename,
                commandTimeout: 42,
                traceKey: "export-test",
                trace: trace));

            // Assert
            Assert.AreSame(trace, observed.Trace);
        }

        [TestMethod]
        public void TestSaveDefinitionAsYamlRestoresContextAfterFailure()
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
            var exception = CaptureException(() => connection.SaveDefinitionAsYaml("sales.Invoice", Filename,
                commandTimeout: 42,
                traceKey: "export-test",
                trace: trace));

            // Assert
            Assert.IsNull(SchemaReadContext.Current);
        }

        [TestMethod]
        public void TestSaveDefinitionAsYamlPreservesExistingFileAfterFailure()
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
            var exception = CaptureException(() => connection.SaveDefinitionAsYaml("sales.Invoice", Filename,
                commandTimeout: 42,
                traceKey: "export-test",
                trace: trace));

            // Assert
            Assert.AreEqual("previous definition", File.ReadAllText(Filename));
        }

        [TestMethod]
        public void TestSaveDefinitionAsYamlLeavesNoTemporaryFilesAfterFailure()
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
            var exception = CaptureException(() => connection.SaveDefinitionAsYaml("sales.Invoice", Filename,
                commandTimeout: 42,
                traceKey: "export-test",
                trace: trace));

            // Assert
            Assert.AreEqual(1, Directory.GetFiles(DirectoryPath).Length);
        }

        [TestMethod]
        public void TestSaveDefinitionAsYamlWritesFormatIdentity()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            connection.SaveDefinitionAsYaml("sales.Invoice", Filename);
            var actual = File.ReadAllText(Filename);

            // Assert
            StringAssert.Contains(actual, "format: \"RepoDB.Schema\"");
        }

        [TestMethod]
        public void TestSaveDefinitionAsYamlPreservesEmptyForeignKeys()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            connection.SaveDefinitionAsYaml("sales.Invoice", Filename);
            var actual = File.ReadAllText(Filename);

            // Assert
            StringAssert.Contains(actual, "foreignKeys: []");
        }

        [TestMethod]
        public void TestSaveDefinitionAsYamlPreservesUnicodeMetadata()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            connection.SaveDefinitionAsYaml("sales.Invoice", Filename);
            var actual = File.ReadAllText(Filename);

            // Assert
            StringAssert.Contains(actual, "unicode: true");
        }

        [TestMethod]
        public void TestSaveDefinitionAsYamlPreservesNumericPrecision()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            connection.SaveDefinitionAsYaml("sales.Invoice", Filename);
            var actual = File.ReadAllText(Filename);

            // Assert
            StringAssert.Contains(actual, "precision: 18");
        }

        [TestMethod]
        public void TestSaveDefinitionAsYamlQuotesCommentsAndUnicode()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            connection.SaveDefinitionAsYaml("sales.Invoice", Filename);
            var actual = File.ReadAllText(Filename);

            // Assert
            StringAssert.Contains(actual, "A: 'quote'\\n\u96EA & <xml>");
        }

        [TestMethod]
        public void TestSaveDefinitionAsYamlOmitsUtf8ByteOrderMark()
        {
            // Setup
            var connection = new CustomDbConnection();

            // Act
            connection.SaveDefinitionAsYaml("sales.Invoice", Filename);
            var actual = File.ReadAllBytes(Filename);

            // Assert
            Assert.IsFalse(actual.Take(3).SequenceEqual(new byte[] { 239, 187, 191 }));
        }

        #endregion
    }
}
