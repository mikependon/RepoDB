#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Schema;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Extensions
{
    [TestClass]
    public class CopyTableSchemasToErrorCallbackTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            SchemaReaderMapper.Clear();
            SchemaComposerMapper.Clear();
        }

        #region Helpers

        private static TableSchema GetSchema(string tableName, int columns = 1, int indexes = 0, int foreignKeys = 0)
        {
            var schema = new TableSchema(tableName, "dbo");
            for (var i = 0; i < columns; i++) schema.Columns.Add(new ColumnInfo());
            for (var i = 0; i < indexes; i++) schema.Indexes.Add(new IndexInfo(null));
            for (var i = 0; i < foreignKeys; i++) schema.ForeignKeys.Add(new ForeignKeyInfo(null));
            return schema;
        }

        private static void MapReader(params TableSchema[] schemas)
        {
            var relationships = schemas.Select(s => new RelationshipInfo { Schema = s }).ToList();
            var reader = new Mock<ISchemaReader>();
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<IEnumerable<string>>())).Returns(relationships);
            reader.Setup(r => r.GetDependencyOrderAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(relationships);
            SchemaReaderMapper.Add<CustomDbConnection>(reader.Object, true);
        }

        private static void MapComposer(params string[] statements)
        {
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns(statements);
            composer.Setup(c => c.ComposeSchema(It.IsAny<TableSchema>())).Returns<TableSchema>(s => new[] { $"CREATE TABLE {s.Table.Name};" });
            SchemaComposerMapper.Add<CustomDbConnection>(composer.Object, true);
        }

        private static void MapThreeTables()
        {
            MapReader(GetSchema("Country"), GetSchema("Person", 1, 1, 1), GetSchema("Solo"));
            MapComposer(
                "CREATE TABLE Country;",
                "CREATE TABLE Person;",
                "CREATE TABLE Solo;",
                "CREATE INDEX IX_Person;",
                "ALTER TABLE Person ADD FK;");
        }

        #endregion

        #region Sync

        [TestMethod]
        public void TestCopyTableSchemasToDoesNotCallTheErrorCallbackIfNothingFails()
        {
            // Setup
            MapThreeTables();
            var errors = new List<CopyTableSchemaError>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Country", "Person", "Solo" }, new CustomDbConnection(), errorCallback: errors.Add);

            // Assert
            Assert.AreEqual(0, errors.Count);
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfAStatementFailsWithoutErrorCallback()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("ALTER TABLE", StringComparison.Ordinal) };

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Country", "Person", "Solo" }, destination));
        }

        [TestMethod]
        public void TestCopyTableSchemasToCallsTheErrorCallbackWithTheDetailsOfTheError()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("ALTER TABLE", StringComparison.Ordinal) };
            var errors = new List<CopyTableSchemaError>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Country", "Person", "Solo" }, destination, errorCallback: errors.Add);

            // Assert
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual("ALTER TABLE Person ADD FK;", errors[0].Statement, StringComparer.Ordinal);
            Assert.AreEqual(4, errors[0].StatementIndex);
            Assert.AreEqual("Person", errors[0].TableName, StringComparer.Ordinal);
            Assert.AreEqual("dbo", errors[0].SchemaName, StringComparer.Ordinal);
            Assert.IsInstanceOfType<InvalidOperationException>(errors[0].Exception);
        }

        [TestMethod]
        public void TestCopyTableSchemasToErrorCallbackKnowsTheTableOfEachKindOfStatement()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection
            {
                FailWhen = c => c.Contains("Person", StringComparison.Ordinal)
            };
            var errors = new List<CopyTableSchemaError>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Country", "Person", "Solo" }, destination, errorCallback: errors.Add);

            // Assert
            Assert.AreEqual(3, errors.Count);
            CollectionAssert.AreEqual(new[] { 1, 3, 4 }, errors.Select(e => e.StatementIndex).ToArray());
            Assert.IsTrue(errors.All(e => e.TableName == "Person"));
        }

        [TestMethod]
        public void TestCopyTableSchemasToErrorCallbackContinuesWithTheNextStatements()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("CREATE INDEX", StringComparison.Ordinal) };
            var created = new List<CopyTableSchemaResult>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(
                new[] { "Country", "Person", "Solo" },
                destination,
                createdCallback: created.Add,
                errorCallback: _ => { });

            // Assert
            Assert.AreEqual(5, destination.ExecutedCommands.Count);
            CollectionAssert.AreEqual(new[] { "Country", "Solo", "Person" }, created.Select(r => r.TableName).ToArray());
            Assert.AreEqual(0, created[0].Errors.Count);
            Assert.AreEqual(0, created[1].Errors.Count);
            Assert.AreEqual(1, created[2].Errors.Count);
            Assert.AreEqual("CREATE INDEX IX_Person;", created[2].Errors[0].Statement, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopyTableSchemasToReportsTheTableThatFailedWithItsErrors()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c == "CREATE TABLE Country;" };
            var created = new List<CopyTableSchemaResult>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(
                new[] { "Country", "Person", "Solo" },
                destination,
                createdCallback: created.Add,
                errorCallback: _ => { });

            // Assert
            Assert.AreEqual(3, created.Count);
            var country = created.Single(r => r.TableName == "Country");
            Assert.AreEqual(1, country.Errors.Count);
            Assert.AreEqual("CREATE TABLE Country;", country.Errors[0].Statement, StringComparer.Ordinal);
            Assert.AreEqual(CopyTableSchemaOutcome.Failed, country.Outcome);
            Assert.IsTrue(created.Where(r => r.TableName != "Country").All(r => r.Errors.Count == 0 && r.Outcome == CopyTableSchemaOutcome.Created));
        }

        [TestMethod]
        public void TestCopyTableSchemasToErrorCallbackIsCalledForEveryError()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("CREATE TABLE", StringComparison.Ordinal) };
            var errors = new List<CopyTableSchemaError>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Country", "Person", "Solo" }, destination, errorCallback: errors.Add);

            // Assert
            CollectionAssert.AreEqual(new[] { "Country", "Person", "Solo" }, errors.Where(e => e.StatementIndex < 3).Select(e => e.TableName).ToArray());
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfTheErrorCallbackThrowsTheCapturedException()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("ALTER TABLE", StringComparison.Ordinal) };
            CopyTableSchemaError raised = null;

            // Act
            var exception = Assert.Throws<InvalidOperationException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Country", "Person", "Solo" }, destination, errorCallback: e =>
                {
                    raised = e;
                    throw e.Exception;
                }));

            // Assert
            Assert.AreSame(raised.Exception, exception);
        }

        [TestMethod]
        public void TestCopyTableSchemasToStopsIfTheErrorCallbackThrows()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c == "CREATE TABLE Person;" };
            var created = new List<string>();

            // Act/Assert
            Assert.Throws<NotSupportedException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(
                    new[] { "Country", "Person", "Solo" },
                    destination,
                    createdCallback: r => created.Add(r.TableName),
                    errorCallback: _ => throw new NotSupportedException()));

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Country;", "CREATE TABLE Person;" }, destination.ExecutedCommands);
            CollectionAssert.AreEqual(new[] { "Country" }, created);
        }

        [TestMethod]
        public void TestCopyTableSchemasToStopsOnTheLastStatementIfTheErrorCallbackThrows()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("ALTER TABLE", StringComparison.Ordinal) };
            var created = new List<string>();

            // Act/Assert
            Assert.Throws<NotSupportedException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(
                    new[] { "Country", "Person", "Solo" },
                    destination,
                    createdCallback: r => created.Add(r.TableName),
                    errorCallback: _ => throw new NotSupportedException()));

            // Assert
            Assert.AreEqual(5, destination.ExecutedCommands.Count);
            CollectionAssert.AreEquivalent(new[] { "Country", "Solo" }, created);
        }

        [TestMethod]
        public void TestCopyTableSchemasToErrorCallbackWhenTheStatementsOfTheTablesAreNotKnown()
        {
            // Setup
            MapReader(GetSchema("Country"), GetSchema("Person", 1, 0, 1));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;");
            var destination = new CustomDbConnection { FailWhen = c => c == "CREATE TABLE Person;" };
            var errors = new List<CopyTableSchemaError>();
            var created = new List<CopyTableSchemaResult>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(
                new[] { "Country", "Person" },
                destination,
                createdCallback: created.Add,
                errorCallback: errors.Add);

            // Assert
            Assert.AreEqual(1, errors.Count);
            Assert.IsNull(errors[0].TableName);
            Assert.IsNull(errors[0].SchemaName);
            Assert.AreEqual(2, created.Count);
            Assert.IsTrue(created.All(r => r.Errors.Count == 1 && ReferenceEquals(r.Errors[0], errors[0])));
        }

        [TestMethod]
        public void TestCopyTableSchemasToErrorsOfTheResultAreTheOnesThatWereRaised()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c.Contains("Person", StringComparison.Ordinal) };
            var errors = new List<CopyTableSchemaError>();
            var created = new List<CopyTableSchemaResult>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(
                new[] { "Country", "Person", "Solo" },
                destination,
                createdCallback: created.Add,
                errorCallback: errors.Add);

            // Assert
            var person = created.Single(r => r.TableName == "Person");
            Assert.AreEqual(3, person.Errors.Count);
            Assert.IsTrue(person.Errors.SequenceEqual(errors));
            Assert.AreEqual(CopyTableSchemaOutcome.Failed, person.Outcome);
        }

        [TestMethod]
        public void TestCopyTableSchemasToResultHasNoErrorsIfNothingFails()
        {
            // Setup
            MapThreeTables();
            var created = new List<CopyTableSchemaResult>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(
                new[] { "Country", "Person", "Solo" },
                new CustomDbConnection(),
                createdCallback: created.Add,
                errorCallback: _ => { });

            // Assert
            Assert.AreEqual(3, created.Count);
            Assert.IsTrue(created.All(r => r.Errors.Count == 0 && r.Outcome == CopyTableSchemaOutcome.Created));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToWithErrorCallbackIfThereIsNoMappedSchemaReader()
        {
            // Setup
            MapComposer("CREATE TABLE Person;");
            var errors = 0;

            // Act/Assert
            Assert.Throws<RepoDb.Exceptions.MissingMappingException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" }, new CustomDbConnection(), errorCallback: e => errors++));
            Assert.AreEqual(0, errors);
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToWithErrorCallbackIfTheArgumentsAreNotValid()
        {
            // Setup
            var errors = 0;

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(null, new CustomDbConnection(), errorCallback: e => errors++));
            Assert.AreEqual(0, errors);
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncDoesNotCallTheErrorCallbackIfNothingFails()
        {
            // Setup
            MapThreeTables();
            var errors = new List<CopyTableSchemaError>();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Country", "Person", "Solo" }, new CustomDbConnection(), errorCallback: errors.Add);

            // Assert
            Assert.AreEqual(0, errors.Count);
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncIfAStatementFailsWithoutErrorCallback()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("ALTER TABLE", StringComparison.Ordinal) };

            // Act/Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Country", "Person", "Solo" }, destination));
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncCallsTheErrorCallbackWithTheDetailsOfTheError()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("ALTER TABLE", StringComparison.Ordinal) };
            var errors = new List<CopyTableSchemaError>();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Country", "Person", "Solo" }, destination, errorCallback: errors.Add);

            // Assert
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual("ALTER TABLE Person ADD FK;", errors[0].Statement, StringComparer.Ordinal);
            Assert.AreEqual(4, errors[0].StatementIndex);
            Assert.AreEqual("Person", errors[0].TableName, StringComparer.Ordinal);
            Assert.IsInstanceOfType<InvalidOperationException>(errors[0].Exception);
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncErrorCallbackContinuesWithTheNextStatements()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("CREATE INDEX", StringComparison.Ordinal) };
            var created = new List<CopyTableSchemaResult>();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(
                new[] { "Country", "Person", "Solo" },
                destination,
                createdCallback: created.Add,
                errorCallback: _ => { });

            // Assert
            Assert.AreEqual(5, destination.ExecutedCommands.Count);
            CollectionAssert.AreEqual(new[] { "Country", "Solo", "Person" }, created.Select(r => r.TableName).ToArray());
            Assert.AreEqual(1, created[2].Errors.Count);
            Assert.AreEqual(CopyTableSchemaOutcome.Failed, created[2].Outcome);
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncIfTheErrorCallbackThrowsTheCapturedException()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("ALTER TABLE", StringComparison.Ordinal) };
            CopyTableSchemaError raised = null;

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Country", "Person", "Solo" }, destination, errorCallback: e =>
                {
                    raised = e;
                    throw e.Exception;
                }));

            // Assert
            Assert.AreSame(raised.Exception, exception);
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncStopsIfTheErrorCallbackThrows()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c == "CREATE TABLE Person;" };
            var created = new List<string>();

            // Act/Assert
            await Assert.ThrowsAsync<NotSupportedException>(() =>
                new CustomDbConnection().CopyTableSchemaToAsync(
                    new[] { "Country", "Person", "Solo" },
                    destination,
                    createdCallback: r => created.Add(r.TableName),
                    errorCallback: _ => throw new NotSupportedException()));

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Country;", "CREATE TABLE Person;" }, destination.ExecutedCommands);
            CollectionAssert.AreEqual(new[] { "Country" }, created);
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncDoesNotReportTheCancellationToTheErrorCallback()
        {
            // Setup
            MapThreeTables();
            var errors = 0;
            using (var source = new CancellationTokenSource())
            {
                source.Cancel();

                // Act/Assert
                await Assert.ThrowsAsync<OperationCanceledException>(() =>
                    new CustomDbConnection().CopyTableSchemaToAsync(
                        new[] { "Country", "Person", "Solo" },
                        new CustomDbConnection(),
                        errorCallback: e => errors++,
                        cancellationToken: source.Token));
                Assert.AreEqual(0, errors);
            }
        }

        #endregion
    }
}
