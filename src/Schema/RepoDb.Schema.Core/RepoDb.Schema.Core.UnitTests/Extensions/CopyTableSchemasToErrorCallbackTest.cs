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
    public class CopySchemasToErrorCallbackTest
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
        public void TestCopySchemasToDoesNotCallTheErrorCallbackIfNothingFails()
        {
            // Setup
            MapThreeTables();
            var errors = new List<CopySchemaError>();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Country", "Person", "Solo" }, new CustomDbConnection(), errorCallback: errors.Add);

            // Assert
            Assert.AreEqual(0, errors.Count);
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfAStatementFailsWithoutErrorCallback()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("ALTER TABLE", StringComparison.Ordinal) };

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() =>
                new CustomDbConnection().CopySchemaTo(new[] { "Country", "Person", "Solo" }, destination));
        }

        [TestMethod]
        public void TestCopySchemasToCallsTheErrorCallbackWithTheDetailsOfTheError()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("ALTER TABLE", StringComparison.Ordinal) };
            var errors = new List<CopySchemaError>();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Country", "Person", "Solo" }, destination, errorCallback: errors.Add);

            // Assert
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual("ALTER TABLE Person ADD FK;", errors[0].Statement, StringComparer.Ordinal);
            Assert.AreEqual(4, errors[0].StatementIndex);
            Assert.AreEqual("Person", errors[0].TableName, StringComparer.Ordinal);
            Assert.AreEqual("dbo", errors[0].SchemaName, StringComparer.Ordinal);
            Assert.IsInstanceOfType<InvalidOperationException>(errors[0].Exception);
        }

        [TestMethod]
        public void TestCopySchemasToErrorCallbackKnowsTheTableOfEachKindOfStatement()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection
            {
                FailWhen = c => c.Contains("Person", StringComparison.Ordinal)
            };
            var errors = new List<CopySchemaError>();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Country", "Person", "Solo" }, destination, errorCallback: errors.Add);

            // Assert
            Assert.AreEqual(3, errors.Count);
            CollectionAssert.AreEqual(new[] { 1, 3, 4 }, errors.Select(e => e.StatementIndex).ToArray());
            Assert.IsTrue(errors.All(e => e.TableName == "Person"));
        }

        [TestMethod]
        public void TestCopySchemasToErrorCallbackContinuesWithTheNextStatements()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("CREATE INDEX", StringComparison.Ordinal) };
            var created = new List<CopySchemaResult>();

            // Act
            new CustomDbConnection().CopySchemaTo(
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
        public void TestCopySchemasToReportsTheTableThatFailedWithItsErrors()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c == "CREATE TABLE Country;" };
            var created = new List<CopySchemaResult>();

            // Act
            new CustomDbConnection().CopySchemaTo(
                new[] { "Country", "Person", "Solo" },
                destination,
                createdCallback: created.Add,
                errorCallback: _ => { });

            // Assert
            Assert.AreEqual(3, created.Count);
            var country = created.Single(r => r.TableName == "Country");
            Assert.AreEqual(1, country.Errors.Count);
            Assert.AreEqual("CREATE TABLE Country;", country.Errors[0].Statement, StringComparer.Ordinal);
            Assert.AreEqual(CopySchemaOutcome.Failed, country.Outcome);
            Assert.IsTrue(created.Where(r => r.TableName != "Country").All(r => r.Errors.Count == 0 && r.Outcome == CopySchemaOutcome.Created));
        }

        [TestMethod]
        public void TestCopySchemasToErrorCallbackIsCalledForEveryError()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("CREATE TABLE", StringComparison.Ordinal) };
            var errors = new List<CopySchemaError>();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Country", "Person", "Solo" }, destination, errorCallback: errors.Add);

            // Assert
            CollectionAssert.AreEqual(new[] { "Country", "Person", "Solo" }, errors.Where(e => e.StatementIndex < 3).Select(e => e.TableName).ToArray());
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfTheErrorCallbackThrowsTheCapturedException()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("ALTER TABLE", StringComparison.Ordinal) };
            CopySchemaError raised = null;

            // Act
            var exception = Assert.Throws<InvalidOperationException>(() =>
                new CustomDbConnection().CopySchemaTo(new[] { "Country", "Person", "Solo" }, destination, errorCallback: e =>
                {
                    raised = e;
                    throw e.Exception;
                }));

            // Assert
            Assert.AreSame(raised.Exception, exception);
        }

        [TestMethod]
        public void TestCopySchemasToStopsIfTheErrorCallbackThrows()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c == "CREATE TABLE Person;" };
            var created = new List<string>();

            // Act/Assert
            Assert.Throws<NotSupportedException>(() =>
                new CustomDbConnection().CopySchemaTo(
                    new[] { "Country", "Person", "Solo" },
                    destination,
                    createdCallback: r => created.Add(r.TableName),
                    errorCallback: _ => throw new NotSupportedException()));

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Country;", "CREATE TABLE Person;" }, destination.ExecutedCommands);
            CollectionAssert.AreEqual(new[] { "Country" }, created);
        }

        [TestMethod]
        public void TestCopySchemasToStopsOnTheLastStatementIfTheErrorCallbackThrows()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("ALTER TABLE", StringComparison.Ordinal) };
            var created = new List<string>();

            // Act/Assert
            Assert.Throws<NotSupportedException>(() =>
                new CustomDbConnection().CopySchemaTo(
                    new[] { "Country", "Person", "Solo" },
                    destination,
                    createdCallback: r => created.Add(r.TableName),
                    errorCallback: _ => throw new NotSupportedException()));

            // Assert
            Assert.AreEqual(5, destination.ExecutedCommands.Count);
            CollectionAssert.AreEquivalent(new[] { "Country", "Solo" }, created);
        }

        [TestMethod]
        public void TestCopySchemasToErrorCallbackWhenTheStatementsOfTheTablesAreNotKnown()
        {
            // Setup
            MapReader(GetSchema("Country"), GetSchema("Person", 1, 0, 1));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;");
            var destination = new CustomDbConnection { FailWhen = c => c == "CREATE TABLE Person;" };
            var errors = new List<CopySchemaError>();
            var created = new List<CopySchemaResult>();

            // Act
            new CustomDbConnection().CopySchemaTo(
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
        public void TestCopySchemasToErrorsOfTheResultAreTheOnesThatWereRaised()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c.Contains("Person", StringComparison.Ordinal) };
            var errors = new List<CopySchemaError>();
            var created = new List<CopySchemaResult>();

            // Act
            new CustomDbConnection().CopySchemaTo(
                new[] { "Country", "Person", "Solo" },
                destination,
                createdCallback: created.Add,
                errorCallback: errors.Add);

            // Assert
            var person = created.Single(r => r.TableName == "Person");
            Assert.AreEqual(3, person.Errors.Count);
            Assert.IsTrue(person.Errors.SequenceEqual(errors));
            Assert.AreEqual(CopySchemaOutcome.Failed, person.Outcome);
        }

        [TestMethod]
        public void TestCopySchemasToResultHasNoErrorsIfNothingFails()
        {
            // Setup
            MapThreeTables();
            var created = new List<CopySchemaResult>();

            // Act
            new CustomDbConnection().CopySchemaTo(
                new[] { "Country", "Person", "Solo" },
                new CustomDbConnection(),
                createdCallback: created.Add,
                errorCallback: _ => { });

            // Assert
            Assert.AreEqual(3, created.Count);
            Assert.IsTrue(created.All(r => r.Errors.Count == 0 && r.Outcome == CopySchemaOutcome.Created));
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToWithErrorCallbackIfThereIsNoMappedSchemaReader()
        {
            // Setup
            MapComposer("CREATE TABLE Person;");
            var errors = 0;

            // Act/Assert
            Assert.Throws<RepoDb.Exceptions.MissingMappingException>(() =>
                new CustomDbConnection().CopySchemaTo(new[] { "Person" }, new CustomDbConnection(), errorCallback: e => errors++));
            Assert.AreEqual(0, errors);
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToWithErrorCallbackIfTheArgumentsAreNotValid()
        {
            // Setup
            var errors = 0;

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CustomDbConnection().CopySchemaTo(null, new CustomDbConnection(), errorCallback: e => errors++));
            Assert.AreEqual(0, errors);
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopySchemasToAsyncDoesNotCallTheErrorCallbackIfNothingFails()
        {
            // Setup
            MapThreeTables();
            var errors = new List<CopySchemaError>();

            // Act
            await new CustomDbConnection().CopySchemaToAsync(new[] { "Country", "Person", "Solo" }, new CustomDbConnection(), errorCallback: errors.Add);

            // Assert
            Assert.AreEqual(0, errors.Count);
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemasToAsyncIfAStatementFailsWithoutErrorCallback()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("ALTER TABLE", StringComparison.Ordinal) };

            // Act/Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                new CustomDbConnection().CopySchemaToAsync(new[] { "Country", "Person", "Solo" }, destination));
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncCallsTheErrorCallbackWithTheDetailsOfTheError()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("ALTER TABLE", StringComparison.Ordinal) };
            var errors = new List<CopySchemaError>();

            // Act
            await new CustomDbConnection().CopySchemaToAsync(new[] { "Country", "Person", "Solo" }, destination, errorCallback: errors.Add);

            // Assert
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual("ALTER TABLE Person ADD FK;", errors[0].Statement, StringComparer.Ordinal);
            Assert.AreEqual(4, errors[0].StatementIndex);
            Assert.AreEqual("Person", errors[0].TableName, StringComparer.Ordinal);
            Assert.IsInstanceOfType<InvalidOperationException>(errors[0].Exception);
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncErrorCallbackContinuesWithTheNextStatements()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("CREATE INDEX", StringComparison.Ordinal) };
            var created = new List<CopySchemaResult>();

            // Act
            await new CustomDbConnection().CopySchemaToAsync(
                new[] { "Country", "Person", "Solo" },
                destination,
                createdCallback: created.Add,
                errorCallback: _ => { });

            // Assert
            Assert.AreEqual(5, destination.ExecutedCommands.Count);
            CollectionAssert.AreEqual(new[] { "Country", "Solo", "Person" }, created.Select(r => r.TableName).ToArray());
            Assert.AreEqual(1, created[2].Errors.Count);
            Assert.AreEqual(CopySchemaOutcome.Failed, created[2].Outcome);
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemasToAsyncIfTheErrorCallbackThrowsTheCapturedException()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("ALTER TABLE", StringComparison.Ordinal) };
            CopySchemaError raised = null;

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                new CustomDbConnection().CopySchemaToAsync(new[] { "Country", "Person", "Solo" }, destination, errorCallback: e =>
                {
                    raised = e;
                    throw e.Exception;
                }));

            // Assert
            Assert.AreSame(raised.Exception, exception);
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncStopsIfTheErrorCallbackThrows()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection { FailWhen = c => c == "CREATE TABLE Person;" };
            var created = new List<string>();

            // Act/Assert
            await Assert.ThrowsAsync<NotSupportedException>(() =>
                new CustomDbConnection().CopySchemaToAsync(
                    new[] { "Country", "Person", "Solo" },
                    destination,
                    createdCallback: r => created.Add(r.TableName),
                    errorCallback: _ => throw new NotSupportedException()));

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Country;", "CREATE TABLE Person;" }, destination.ExecutedCommands);
            CollectionAssert.AreEqual(new[] { "Country" }, created);
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncDoesNotReportTheCancellationToTheErrorCallback()
        {
            // Setup
            MapThreeTables();
            var errors = 0;
            using (var source = new CancellationTokenSource())
            {
                source.Cancel();

                // Act/Assert
                await Assert.ThrowsAsync<OperationCanceledException>(() =>
                    new CustomDbConnection().CopySchemaToAsync(
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
