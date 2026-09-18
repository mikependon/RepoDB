#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.IntegrationTests.Operations
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
        public void TestSqlConnectionExecuteQuerySingleViaDynamics()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuerySingle("SELECT * FROM [sc].[IdentityTable] WHERE Id = @Id;",
                    new { tables.Last().Id });

                // Assert
                var kvp = result as IDictionary<string, object>;
                Assert.AreEqual(tables.Last().Id, Convert.ToInt32(kvp[nameof(IdentityTable.Id)], CultureInfo.InvariantCulture));
                Assert.AreEqual(tables.Last().ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                Assert.AreEqual(tables.Last().ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                Assert.AreEqual(tables.Last().ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                Assert.AreEqual(tables.Last().ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQuerySingleViaDynamicsWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuerySingle("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt = @ColumnInt;",
                    new { tables.Last().ColumnInt });

                // Assert
                var kvp = result as IDictionary<string, object>;
                Assert.AreEqual(tables.Last().Id, Convert.ToInt32(kvp[nameof(IdentityTable.Id)], CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQuerySingleViaDynamicsWithStoredProcedure()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuerySingle("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure);

                // Assert
                var kvp = result as IDictionary<string, object>;
                Assert.AreEqual(tables.Last().Id, Convert.ToInt32(kvp[nameof(IdentityTable.Id)], CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlConnectionExecuteQuerySingleViaDynamicsIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle("SELECT * FROM [sc].[IdentityTable];"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlConnectionExecuteQuerySingleViaDynamicsIfMultipleRowsFound()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                Assert.Throws<MultipleRowsFoundException>(() => connection.ExecuteQuerySingle("SELECT * FROM [sc].[IdentityTable];"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlConnectionExecuteQuerySingleViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<SqlException>(() => connection.ExecuteQuerySingle("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlConnectionExecuteQuerySingleViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<SqlException>(() => connection.ExecuteQuerySingle("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);"));
            }
        }

        #endregion

        #region ExecuteQuerySingleAsync<dynamic>

        [TestMethod]
        public async Task TestSqlConnectionExecuteQuerySingleAsyncViaDynamics()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.ExecuteQuerySingleAsync("SELECT * FROM [sc].[IdentityTable] WHERE Id = @Id;",
                    new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                var kvp = result as IDictionary<string, object>;
                Assert.AreEqual(tables.Last().Id, Convert.ToInt32(kvp[nameof(IdentityTable.Id)], CultureInfo.InvariantCulture));
                Assert.AreEqual(tables.Last().ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                Assert.AreEqual(tables.Last().ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                Assert.AreEqual(tables.Last().ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                Assert.AreEqual(tables.Last().ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteQuerySingleAsyncViaDynamicsWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.ExecuteQuerySingleAsync("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt = @ColumnInt;",
                    new { tables.Last().ColumnInt }).ConfigureAwait(false);

                // Assert
                var kvp = result as IDictionary<string, object>;
                Assert.AreEqual(tables.Last().Id, Convert.ToInt32(kvp[nameof(IdentityTable.Id)], CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteQuerySingleAsyncViaDynamicsWithStoredProcedure()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.ExecuteQuerySingleAsync("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).ConfigureAwait(false);

                // Assert
                var kvp = result as IDictionary<string, object>;
                Assert.AreEqual(tables.Last().Id, Convert.ToInt32(kvp[nameof(IdentityTable.Id)], CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlConnectionExecuteQuerySingleAsyncViaDynamicsIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM [sc].[IdentityTable];").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlConnectionExecuteQuerySingleAsyncViaDynamicsIfMultipleRowsFound()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM [sc].[IdentityTable];").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlConnectionExecuteQuerySingleAsyncViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<SqlException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlConnectionExecuteQuerySingleAsyncViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<SqlException>(async () => await connection.ExecuteQuerySingleAsync("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #region ExecuteQuerySingle<TEntity>

        [TestMethod]
        public void TestSqlConnectionExecuteQuerySingle()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuerySingle<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE Id = @Id;",
                    new { tables.Last().Id });

                // Assert
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQuerySingleWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuerySingle<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt = @ColumnInt;",
                    new { tables.Last().ColumnInt });

                // Assert
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQuerySingleWithStoredProcedure()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuerySingle<IdentityTable>("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlConnectionExecuteQuerySingleIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle<IdentityTable>("SELECT * FROM [sc].[IdentityTable];"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlConnectionExecuteQuerySingleIfMultipleRowsFound()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                Assert.Throws<MultipleRowsFoundException>(() => connection.ExecuteQuerySingle<IdentityTable>("SELECT * FROM [sc].[IdentityTable];"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlConnectionExecuteQuerySingleIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<SqlException>(() => connection.ExecuteQuerySingle<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlConnectionExecuteQuerySingleIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<SqlException>(() => connection.ExecuteQuerySingle<IdentityTable>("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);"));
            }
        }

        #endregion

        #region ExecuteQuerySingleAsync<TEntity>

        [TestMethod]
        public async Task TestSqlConnectionExecuteQuerySingleAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.ExecuteQuerySingleAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE Id = @Id;",
                    new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteQuerySingleAsyncWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.ExecuteQuerySingleAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt = @ColumnInt;",
                    new { tables.Last().ColumnInt }).ConfigureAwait(false);

                // Assert
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteQuerySingleAsyncWithStoredProcedure()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.ExecuteQuerySingleAsync<IdentityTable>("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).ConfigureAwait(false);

                // Assert
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlConnectionExecuteQuerySingleAsyncIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable];").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlConnectionExecuteQuerySingleAsyncIfMultipleRowsFound()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.ExecuteQuerySingleAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable];").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlConnectionExecuteQuerySingleAsyncIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<SqlException>(async () => await connection.ExecuteQuerySingleAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlConnectionExecuteQuerySingleAsyncIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<SqlException>(async () => await connection.ExecuteQuerySingleAsync<IdentityTable>("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
