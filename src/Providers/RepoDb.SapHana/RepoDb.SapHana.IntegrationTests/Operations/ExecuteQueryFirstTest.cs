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
    public class ExecuteQueryFirstTest
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

        #region ExecuteQueryFirst<dynamic>

        [TestMethod]
        public void TestHanaConnectionExecuteQueryFirstViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQueryFirst("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC;");

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.First().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.First(), kvp);
            }
        }

        [TestMethod]
        public void TestHanaConnectionExecuteQueryFirstViaDynamicsWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQueryFirst("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id;",
                    new { tables.Last().Id });

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.Last().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.Last(), kvp);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestHanaConnectionExecuteQueryFirstViaDynamicsIfNoRowsFound()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst("SELECT * FROM \"CompleteTable\";"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestHanaConnectionExecuteQueryFirstViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<HanaException>(() => connection.ExecuteQueryFirst("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestHanaConnectionExecuteQueryFirstViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<HanaException>(() => connection.ExecuteQueryFirst("SELEC * FROM \"CompleteTable\";"));
            }
        }

        #endregion

        #region ExecuteQueryFirstAsync<dynamic>

        [TestMethod]
        public async Task TestHanaConnectionExecuteQueryFirstAsyncViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQueryFirstAsync("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC;").ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.First().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.First(), kvp);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionExecuteQueryFirstAsyncViaDynamicsWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQueryFirstAsync("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id;",
                    new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.Last().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.Last(), kvp);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestHanaConnectionExecuteQueryFirstAsyncViaDynamicsIfNoRowsFound()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync("SELECT * FROM \"CompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestHanaConnectionExecuteQueryFirstAsyncViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<HanaException>(async () => await connection.ExecuteQueryFirstAsync("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestHanaConnectionExecuteQueryFirstAsyncViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<HanaException>(async () => await connection.ExecuteQueryFirstAsync("SELEC * FROM \"CompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #region ExecuteQueryFirst<TEntity>

        [TestMethod]
        public void TestHanaConnectionExecuteQueryFirst()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC;");

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public void TestHanaConnectionExecuteQueryFirstWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id;",
                    new { tables.Last().Id });

                // Assert
                Assert.AreEqual(tables.Last().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestHanaConnectionExecuteQueryFirstIfNoRowsFound()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM \"CompleteTable\";"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestHanaConnectionExecuteQueryFirstIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id;",
                    new { Id = -1L }));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestHanaConnectionExecuteQueryFirstIfTheParametersAreNotDefined()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<HanaException>(() => connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestHanaConnectionExecuteQueryFirstIfThereAreSqlStatementProblems()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<HanaException>(() => connection.ExecuteQueryFirst<CompleteTable>("SELEC * FROM \"CompleteTable\";"));
            }
        }

        #endregion

        #region ExecuteQueryFirstAsync<TEntity>

        [TestMethod]
        public async Task TestHanaConnectionExecuteQueryFirstAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC;").ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionExecuteQueryFirstAsyncWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id;",
                    new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Last().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestHanaConnectionExecuteQueryFirstAsyncIfNoRowsFound()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM \"CompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestHanaConnectionExecuteQueryFirstAsyncIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id;",
                    new { Id = -1L }).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestHanaConnectionExecuteQueryFirstAsyncIfTheParametersAreNotDefined()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<HanaException>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestHanaConnectionExecuteQueryFirstAsyncIfThereAreSqlStatementProblems()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<HanaException>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELEC * FROM \"CompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
