#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Helpers
{
    [TestClass]
    public class SchemaCreatorTest
    {
        #region Helpers

        private static ISchemaComposer CreateComposer(bool supportsCreation)
        {
            var composer = new Mock<ISchemaComposer>();
            if (supportsCreation)
            {
                var support = composer.As<ISchemaCreationSupport>();
                support.SetupGet(s => s.SchemaParameterPrefix).Returns("@");
                support.SetupGet(s => s.SchemaExistsSql).Returns("SELECT COUNT(*) FROM schemas WHERE name = {0}");
                support.SetupGet(s => s.CreateSchemaSql).Returns("CREATE SCHEMA {0}");
                support.Setup(s => s.QuoteSchemaName(It.IsAny<string>())).Returns<string>(name => "[" + name + "]");
            }

            return composer.Object;
        }

        private static PlanTable[] CreateTables(params string[] schemaNames) =>
            schemaNames.Select((schemaName,
                position) =>
            {
                var schema = new TableSchema(new TableInfo("Table" + position, schemaName));
                return new PlanTable(schema,
                    schema,
                    schema.Table.Name);
            }).ToArray();

        private static CustomDbConnection CreateConnection(object count) =>
            new CustomDbConnection { ScalarResult = _ => count };

        #endregion

        #region Create

        [TestMethod]
        public void TestCreateCreatesTheSchemaThatDoesNotExist()
        {
            // Setup
            var connection = CreateConnection(0);

            // Act
            SchemaCreator.Create(connection,
                CreateComposer(true),
                CreateTables("dbo"),
                null,
                "MyTraceKey",
                null,
                null);

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE SCHEMA [dbo]" }, connection.ExecutedCommands);
        }

        [TestMethod]
        public void TestCreateDoesNotCreateTheSchemaThatExists()
        {
            // Setup
            var connection = CreateConnection(1);

            // Act
            SchemaCreator.Create(connection,
                CreateComposer(true),
                CreateTables("dbo"),
                null,
                "MyTraceKey",
                null,
                null);

            // Assert
            Assert.AreEqual(0, connection.ExecutedCommands.Count);
        }

        [TestMethod]
        public void TestCreateChecksEachSchemaOnlyOnce()
        {
            // Setup
            var connection = CreateConnection(0);

            // Act
            SchemaCreator.Create(connection,
                CreateComposer(true),
                CreateTables("dbo", "dbo", "sales"),
                null,
                "MyTraceKey",
                null,
                null);

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE SCHEMA [dbo]", "CREATE SCHEMA [sales]" }, connection.ExecutedCommands);
        }

        [TestMethod]
        public void TestCreateMatchesTheNamesOfTheSchemasAsTheyAre()
        {
            // Setup
            var connection = CreateConnection(0);

            // Act
            SchemaCreator.Create(connection,
                CreateComposer(true),
                CreateTables("dbo", "DBO"),
                null,
                "MyTraceKey",
                null,
                null);

            // Assert
            Assert.AreEqual(2, connection.ExecutedCommands.Count);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void TestCreateSkipsTheTablesWithoutASchema(string schemaName)
        {
            // Setup
            var connection = CreateConnection(0);

            // Act
            SchemaCreator.Create(connection,
                CreateComposer(true),
                CreateTables(schemaName),
                null,
                "MyTraceKey",
                null,
                null);

            // Assert
            Assert.AreEqual(0, connection.Details.Count);
        }

        [TestMethod]
        public void TestCreateDoesNothingIfTheComposerDoesNotSupportTheCreation()
        {
            // Setup
            var connection = CreateConnection(0);

            // Act
            SchemaCreator.Create(connection,
                CreateComposer(false),
                CreateTables("dbo"),
                null,
                "MyTraceKey",
                null,
                null);

            // Assert
            Assert.AreEqual(0, connection.Details.Count);
        }

        [TestMethod]
        public void TestCreateUsesTheCommandTimeoutAndTheTransaction()
        {
            // Setup
            var connection = CreateConnection(0);
            var transaction = connection.BeginTransaction();

            // Act
            SchemaCreator.Create(connection,
                CreateComposer(true),
                CreateTables("dbo"),
                45,
                "MyTraceKey",
                null,
                transaction);

            // Assert
            Assert.IsTrue(connection.Details.All(detail => detail.CommandTimeout == 45 && ReferenceEquals(detail.Transaction, transaction)));
        }

        #endregion

        #region Create Async

        [TestMethod]
        public async Task TestCreateAsyncCreatesTheSchemaThatDoesNotExist()
        {
            // Setup
            var connection = CreateConnection(0);

            // Act
            await SchemaCreator.CreateAsync(connection,
                CreateComposer(true),
                CreateTables("dbo"),
                null,
                "MyTraceKey",
                null,
                null,
                default);

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE SCHEMA [dbo]" }, connection.ExecutedCommands);
        }

        [TestMethod]
        public async Task TestCreateAsyncDoesNotCreateTheSchemaThatExists()
        {
            // Setup
            var connection = CreateConnection(1);

            // Act
            await SchemaCreator.CreateAsync(connection,
                CreateComposer(true),
                CreateTables("dbo"),
                null,
                "MyTraceKey",
                null,
                null,
                default);

            // Assert
            Assert.AreEqual(0, connection.ExecutedCommands.Count);
        }

        [TestMethod]
        public async Task TestCreateAsyncChecksEachSchemaOnlyOnce()
        {
            // Setup
            var connection = CreateConnection(0);

            // Act
            await SchemaCreator.CreateAsync(connection,
                CreateComposer(true),
                CreateTables("dbo", "dbo", "sales"),
                null,
                "MyTraceKey",
                null,
                null,
                default);

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE SCHEMA [dbo]", "CREATE SCHEMA [sales]" }, connection.ExecutedCommands);
        }

        [TestMethod]
        public async Task TestCreateAsyncDoesNothingIfTheComposerDoesNotSupportTheCreation()
        {
            // Setup
            var connection = CreateConnection(0);

            // Act
            await SchemaCreator.CreateAsync(connection,
                CreateComposer(false),
                CreateTables("dbo"),
                null,
                "MyTraceKey",
                null,
                null,
                default);

            // Assert
            Assert.AreEqual(0, connection.Details.Count);
        }

        [TestMethod]
        public async Task ThrowExceptionOnCreateAsyncIfTheTokenIsCancelled()
        {
            // Setup
            var connection = CreateConnection(0);
            using (var source = new CancellationTokenSource())
            {
                source.Cancel();

                // Act/Assert
                await Assert.ThrowsAsync<OperationCanceledException>(() => SchemaCreator.CreateAsync(connection,
                    CreateComposer(true),
                    CreateTables("dbo"),
                    null,
                    "MyTraceKey",
                    null,
                    null,
                    source.Token));
                Assert.AreEqual(0, connection.Details.Count);
            }
        }

        #endregion
    }
}
