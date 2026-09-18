#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sap.Data.Hana;
using RepoDb.Exceptions;
using RepoDb.SapHana.IntegrationTests.Models;
using RepoDb.SapHana.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SapHana.IntegrationTests.Operations
{
    [TestClass]
    public class ExecuteQuerySingleTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        #region ExecuteQuerySingle<dynamic>

        [TestMethod]
        public void TestHanaConnectionExecuteQuerySingleViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id;",
                    new { tables.Last().Id });

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.Last().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.Last(), kvp);
            }
        }

        [TestMethod]
        public void TestHanaConnectionExecuteQuerySingleViaDynamicsWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC LIMIT 1;");

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.First().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.First(), kvp);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestHanaConnectionExecuteQuerySingleViaDynamicsIfNoRowsFound()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\";"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestHanaConnectionExecuteQuerySingleViaDynamicsIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id;",
                    new { Id = -1L }));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestHanaConnectionExecuteQuerySingleViaDynamicsIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MultipleRowsFoundException>(() => connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\";"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestHanaConnectionExecuteQuerySingleViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<HanaException>(() => connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestHanaConnectionExecuteQuerySingleViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<HanaException>(() => connection.ExecuteQuerySingle("SELEC * FROM \"CompleteTable\";"));
            }
        }

        #endregion

        #region ExecuteQuerySingleAsync<dynamic>

        [TestMethod]
        public async Task TestHanaConnectionExecuteQuerySingleAsyncViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id;",
                    new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.Last().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.Last(), kvp);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionExecuteQuerySingleAsyncViaDynamicsWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC LIMIT 1;").ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.First().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.First(), kvp);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestHanaConnectionExecuteQuerySingleAsyncViaDynamicsIfNoRowsFound()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestHanaConnectionExecuteQuerySingleAsyncViaDynamicsIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id;",
                    new { Id = -1L }).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestHanaConnectionExecuteQuerySingleAsyncViaDynamicsIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestHanaConnectionExecuteQuerySingleAsyncViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<HanaException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestHanaConnectionExecuteQuerySingleAsyncViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<HanaException>(async () => await connection.ExecuteQuerySingleAsync("SELEC * FROM \"CompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #region ExecuteQuerySingle<TEntity>

        [TestMethod]
        public void TestHanaConnectionExecuteQuerySingle()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id;",
                    new { tables.Last().Id });

                // Assert
                Assert.AreEqual(tables.Last().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public void TestHanaConnectionExecuteQuerySingleWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC LIMIT 1;");

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestHanaConnectionExecuteQuerySingleIfNoRowsFound()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\";"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestHanaConnectionExecuteQuerySingleIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id;",
                    new { Id = -1L }));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestHanaConnectionExecuteQuerySingleIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MultipleRowsFoundException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\";"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestHanaConnectionExecuteQuerySingleIfTheParametersAreNotDefined()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<HanaException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestHanaConnectionExecuteQuerySingleIfThereAreSqlStatementProblems()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<HanaException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELEC * FROM \"CompleteTable\";"));
            }
        }

        #endregion

        #region ExecuteQuerySingleAsync<TEntity>

        [TestMethod]
        public async Task TestHanaConnectionExecuteQuerySingleAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id;",
                    new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Last().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionExecuteQuerySingleAsyncWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC LIMIT 1;").ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestHanaConnectionExecuteQuerySingleAsyncIfNoRowsFound()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestHanaConnectionExecuteQuerySingleAsyncIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id;",
                    new { Id = -1L }).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestHanaConnectionExecuteQuerySingleAsyncIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestHanaConnectionExecuteQuerySingleAsyncIfTheParametersAreNotDefined()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<HanaException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestHanaConnectionExecuteQuerySingleAsyncIfThereAreSqlStatementProblems()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<HanaException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELEC * FROM \"CompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
