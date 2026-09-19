#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.EnterpriseDb;
using RepoDb.Exceptions;
using RepoDb.EnterpriseDb.IntegrationTests.Models;
using RepoDb.EnterpriseDb.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.EnterpriseDb.IntegrationTests.Operations
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
        public void TestEnterpriseDbConnectionExecuteQueryFirstViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionExecuteQueryFirstViaDynamicsWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQueryFirst("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
                    new { tables.Last().Id });

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.Last().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.Last(), kvp);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestEnterpriseDbConnectionExecuteQueryFirstViaDynamicsIfNoRowsFound()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst("SELECT * FROM \"CompleteTable\";"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestEnterpriseDbConnectionExecuteQueryFirstViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EDBException>(() => connection.ExecuteQueryFirst("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestEnterpriseDbConnectionExecuteQueryFirstViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EDBException>(() => connection.ExecuteQueryFirst("SELEC * FROM \"CompleteTable\";"));
            }
        }

        #endregion

        #region ExecuteQueryFirstAsync<dynamic>

        [TestMethod]
        public async Task TestEnterpriseDbConnectionExecuteQueryFirstAsyncViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionExecuteQueryFirstAsyncViaDynamicsWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQueryFirstAsync("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
                    new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.Last().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.Last(), kvp);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestEnterpriseDbConnectionExecuteQueryFirstAsyncViaDynamicsIfNoRowsFound()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync("SELECT * FROM \"CompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestEnterpriseDbConnectionExecuteQueryFirstAsyncViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EDBException>(async () => await connection.ExecuteQueryFirstAsync("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestEnterpriseDbConnectionExecuteQueryFirstAsyncViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EDBException>(async () => await connection.ExecuteQueryFirstAsync("SELEC * FROM \"CompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #region ExecuteQueryFirst<TEntity>

        [TestMethod]
        public void TestEnterpriseDbConnectionExecuteQueryFirst()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC;");

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionExecuteQueryFirstWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
                    new { tables.Last().Id });

                // Assert
                Assert.AreEqual(tables.Last().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestEnterpriseDbConnectionExecuteQueryFirstIfNoRowsFound()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM \"CompleteTable\";"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestEnterpriseDbConnectionExecuteQueryFirstIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
                    new { Id = -1L }));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestEnterpriseDbConnectionExecuteQueryFirstIfTheParametersAreNotDefined()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EDBException>(() => connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestEnterpriseDbConnectionExecuteQueryFirstIfThereAreSqlStatementProblems()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EDBException>(() => connection.ExecuteQueryFirst<CompleteTable>("SELEC * FROM \"CompleteTable\";"));
            }
        }

        #endregion

        #region ExecuteQueryFirstAsync<TEntity>

        [TestMethod]
        public async Task TestEnterpriseDbConnectionExecuteQueryFirstAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC;").ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionExecuteQueryFirstAsyncWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
                    new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Last().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestEnterpriseDbConnectionExecuteQueryFirstAsyncIfNoRowsFound()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM \"CompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestEnterpriseDbConnectionExecuteQueryFirstAsyncIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
                    new { Id = -1L }).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestEnterpriseDbConnectionExecuteQueryFirstAsyncIfTheParametersAreNotDefined()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EDBException>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestEnterpriseDbConnectionExecuteQueryFirstAsyncIfThereAreSqlStatementProblems()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EDBException>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELEC * FROM \"CompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
