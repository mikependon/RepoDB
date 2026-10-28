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
using RepoDb.Exceptions;
using RepoDb.Interfaces;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Core.UnitTests.Extensions
{
    [TestClass]
    public class SaveDefinitionAsyncTest : SaveDefinitionTestBase
    {
        #region Methods

        public static IEnumerable<object[]> Cases()
        {
            foreach (var format in new[] { "json", "xml", "yaml" })
                foreach (RelatedTableBehavior behavior in Enum.GetValues(typeof(RelatedTableBehavior)))
                    yield return new object[] { format, behavior };
        }

        [TestMethod]
        [DynamicData(nameof(Cases))]
        public async Task MatchesSynchronousOutputAndUsesOnlyAsyncReaderMethods(string format, RelatedTableBehavior behavior)
        {
            var connection = new CustomDbConnection();
            if (format == "json") connection.SaveDefinitionAsJson("sales.Invoice", Filename, behavior);
            else if (format == "xml") connection.SaveDefinitionAsXml("sales.Invoice", Filename, behavior);
            else connection.SaveDefinitionAsYaml("sales.Invoice", Filename, behavior);
            var expected = File.ReadAllBytes(Filename);
            Reader.Invocations.Clear();
            SetupAsync();

            await Save(connection, format, behavior);

            CollectionAssert.AreEqual(expected, File.ReadAllBytes(Filename));
            Assert.AreEqual(0, Directory.GetFiles(DirectoryPath, ".repodb-*.tmp").Length);
            Reader.Verify(r => r.TableExistsAsync("sales.Invoice", It.IsAny<CancellationToken>()), Times.Once);
            Reader.Verify(r => r.GetTableSchemaAsync("sales.Invoice", It.IsAny<CancellationToken>()), Times.Once);
            Reader.Verify(r => r.GetRelatedTablesAsync(It.IsAny<IEnumerable<string>>(), behavior, It.IsAny<CancellationToken>()),
                behavior == RelatedTableBehavior.TableOnly ? Times.Never() : Times.Once());
            Reader.Verify(r => r.TableExists(It.IsAny<string>()), Times.Never);
            Reader.Verify(r => r.GetTableSchema(It.IsAny<string>()), Times.Never);
            Reader.Verify(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), It.IsAny<RelatedTableBehavior>()), Times.Never);
            Assert.IsNull(SchemaReadContext.Current);
        }

        [TestMethod]
        [DataRow("json")]
        [DataRow("xml")]
        [DataRow("yaml")]
        public async Task ForwardsCancellationAndReadContextAcrossAwait(string format)
        {
            using var cancellation = new CancellationTokenSource();
            var trace = new Mock<ITrace>(MockBehavior.Strict).Object;
            SetupAsync();
            Reader.Setup(r => r.TableExistsAsync("sales.Invoice", cancellation.Token)).Returns(async () =>
            {
                await Task.Yield();
                Assert.AreEqual(19, SchemaReadContext.Current.CommandTimeout);
                Assert.AreEqual("async-save", SchemaReadContext.Current.TraceKey);
                Assert.AreSame(trace, SchemaReadContext.Current.Trace);
                return true;
            });
            await Save(new CustomDbConnection(), format, RelatedTableBehavior.Parents, cancellation.Token, trace);
            Reader.Verify(r => r.GetTableSchemaAsync("sales.Invoice", cancellation.Token), Times.Once);
            Reader.Verify(r => r.GetTableSchemaAsync("sales.Customer", cancellation.Token), Times.Once);
            Reader.Verify(r => r.GetRelatedTablesAsync(It.IsAny<IEnumerable<string>>(), RelatedTableBehavior.Parents, cancellation.Token), Times.Once);
            Assert.IsNull(SchemaReadContext.Current);
        }

        [TestMethod]
        [DynamicData(nameof(Cases))]
        public async Task CancellationPreservesExistingFileAndRestoresContext(string format, RelatedTableBehavior behavior)
        {
            File.WriteAllText(Filename, "original");
            using var cancellation = new CancellationTokenSource();
            SetupAsync();
            Reader.Setup(r => r.GetTableSchemaAsync("sales.Invoice", cancellation.Token)).Returns(() =>
            {
                cancellation.Cancel();
                return Task.FromResult(Invoice);
            });
            await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => Save(new CustomDbConnection(), format, behavior, cancellation.Token));
            Assert.AreEqual("original", File.ReadAllText(Filename));
            Assert.AreEqual(1, Directory.GetFiles(DirectoryPath).Length);
            Assert.IsNull(SchemaReadContext.Current);
        }

        [TestMethod]
        [DataRow("json")]
        [DataRow("xml")]
        [DataRow("yaml")]
        public async Task AlreadyCancelledTokenDoesNotReadOrWrite(string format)
        {
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();
            await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => Save(new CustomDbConnection(), format, RelatedTableBehavior.TableOnly, cancellation.Token));
            Assert.IsFalse(File.Exists(Filename));
            Assert.AreEqual(0, Reader.Invocations.Count);
        }

        [TestMethod]
        [DataRow("json")]
        [DataRow("xml")]
        [DataRow("yaml")]
        public async Task ReaderFailurePreservesFileAndRestoresContext(string format)
        {
            File.WriteAllText(Filename, "original");
            SetupAsync();
            Reader.Setup(r => r.GetTableSchemaAsync("sales.Invoice", It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("reader failed"));
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => Save(new CustomDbConnection(), format, RelatedTableBehavior.TableOnly));
            Assert.AreEqual("original", File.ReadAllText(Filename));
            Assert.IsNull(SchemaReadContext.Current);
        }

        [TestMethod]
        public async Task SerializationFailurePreservesFile()
        {
            File.WriteAllText(Filename, "original");
            SetupAsync();
            Invoice.Columns[0].Comment = "invalid XML \u0001";
            await Assert.ThrowsExactlyAsync<ArgumentException>(() => Save(new CustomDbConnection(), "xml", RelatedTableBehavior.TableOnly));
            Assert.AreEqual("original", File.ReadAllText(Filename));
            Assert.AreEqual(1, Directory.GetFiles(DirectoryPath).Length);
            Assert.IsNull(SchemaReadContext.Current);
        }

        [TestMethod]
        [DataRow("json")]
        [DataRow("xml")]
        [DataRow("yaml")]
        public async Task MissingTableAndMappingDoNotCreateFile(string format)
        {
            SetupAsync();
            Reader.Setup(r => r.TableExistsAsync("sales.Invoice", It.IsAny<CancellationToken>())).ReturnsAsync(false);
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => Save(new CustomDbConnection(), format, RelatedTableBehavior.TableOnly));
            SchemaReaderMapper.Clear();
            await Assert.ThrowsExactlyAsync<MissingMappingException>(() => Save(new CustomDbConnection(), format, RelatedTableBehavior.TableOnly));
            Assert.IsFalse(File.Exists(Filename));
        }

        [TestMethod]
        [DataRow("json")]
        [DataRow("xml")]
        [DataRow("yaml")]
        public async Task CommitFailureRemovesTemporaryFile(string format)
        {
            SetupAsync();
            Directory.CreateDirectory(Filename);
            var exception = await CaptureAsync(() => Save(new CustomDbConnection(), format, RelatedTableBehavior.TableOnly));
            Assert.IsNotNull(exception);
            Assert.IsTrue(exception is IOException || exception is UnauthorizedAccessException);
            Assert.AreEqual(0, Directory.GetFiles(DirectoryPath, ".repodb-*.tmp").Length);
            Assert.IsTrue(Directory.Exists(Filename));
        }

        [TestMethod]
        public async Task ValidatesRequiredArguments()
        {
            await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => ((IDbConnection)null).SaveDefinitionAsJsonAsync("T", Filename));
            await Assert.ThrowsExactlyAsync<ArgumentException>(() => new CustomDbConnection().SaveDefinitionAsJsonAsync(" ", Filename));
            await Assert.ThrowsExactlyAsync<ArgumentException>(() => new CustomDbConnection().SaveDefinitionAsYamlAsync("T", " "));
            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() => new CustomDbConnection().SaveDefinitionAsXmlAsync("T", Filename, (RelatedTableBehavior)99));
            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() => new CustomDbConnection().SaveDefinitionAsJsonAsync("T", Filename, commandTimeout: -1));
        }

        #endregion

        #region Helpers

        private void SetupAsync()
        {
            Reader.Setup(r => r.TableExistsAsync("sales.Invoice", It.IsAny<CancellationToken>())).ReturnsAsync(true);
            Reader.Setup(r => r.GetTableSchemaAsync("sales.Invoice", It.IsAny<CancellationToken>())).ReturnsAsync(Invoice);
            Reader.Setup(r => r.GetTableSchemaAsync("sales.Customer", It.IsAny<CancellationToken>())).ReturnsAsync(Customer);
            Reader.Setup(r => r.GetRelatedTablesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<RelatedTableBehavior>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { "sales.Invoice", "sales.Customer", "sales.Invoice" });
        }

        private Task Save(IDbConnection connection, string format, RelatedTableBehavior behavior, CancellationToken token = default, ITrace trace = null)
        {
            if (format == "json") return connection.SaveDefinitionAsJsonAsync("sales.Invoice", Filename, behavior, 19, "async-save", trace, token);
            if (format == "xml") return connection.SaveDefinitionAsXmlAsync("sales.Invoice", Filename, behavior, 19, "async-save", trace, token);
            return connection.SaveDefinitionAsYamlAsync("sales.Invoice", Filename, behavior, 19, "async-save", trace, token);
        }

        private static async Task<Exception> CaptureAsync(Func<Task> action)
        {
            try { await action(); return null; }
            catch (Exception exception) { return exception; }
        }

        #endregion
    }
}
