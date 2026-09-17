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
        public void TestSqlConnectionExecuteQueryFirstViaDynamics()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryFirst("SELECT * FROM [sc].[IdentityTable] ORDER BY Id ASC;");

                // Assert
                var item = tables[0];
                var kvp = result as IDictionary<string, object>;
                Assert.AreEqual(item.Id, Convert.ToInt32(kvp[nameof(IdentityTable.Id)],  CultureInfo.InvariantCulture));
                Assert.AreEqual(item.ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                Assert.AreEqual(item.ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                Assert.AreEqual(item.ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                Assert.AreEqual(item.ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryFirstViaDynamicsWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryFirst("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt = @ColumnInt;",
                    new { tables.Last().ColumnInt });

                // Assert
                var kvp = result as IDictionary<string, object>;
                Assert.AreEqual(tables.Last().Id, Convert.ToInt32(kvp[nameof(IdentityTable.Id)],  CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryFirstViaDynamicsWithStoredProcedure()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryFirst("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure);

                // Assert
                var kvp = result as IDictionary<string, object>;
                Assert.AreEqual(tables.Last().Id, Convert.ToInt32(kvp[nameof(IdentityTable.Id)],  CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryFirstViaDynamicsIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst("SELECT * FROM [sc].[IdentityTable];"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryFirstViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<SqlException>(() => connection.ExecuteQueryFirst("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryFirstViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<SqlException>(() => connection.ExecuteQueryFirst("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);"));
            }
        }

        #endregion

        #region ExecuteQueryFirstAsync<dynamic>

        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryFirstAsyncViaDynamics()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.ExecuteQueryFirstAsync("SELECT * FROM [sc].[IdentityTable] ORDER BY Id ASC;").ConfigureAwait(false);

                // Assert
                var item = tables[0];
                var kvp = result as IDictionary<string, object>;
                Assert.AreEqual(item.Id, Convert.ToInt32(kvp[nameof(IdentityTable.Id)], CultureInfo.InvariantCulture));
                Assert.AreEqual(item.ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                Assert.AreEqual(item.ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                Assert.AreEqual(item.ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                Assert.AreEqual(item.ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryFirstAsyncViaDynamicsWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.ExecuteQueryFirstAsync("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt = @ColumnInt;",
                    new { tables.Last().ColumnInt }).ConfigureAwait(false);

                // Assert
                var kvp = result as IDictionary<string, object>;
                Assert.AreEqual(tables.Last().Id, Convert.ToInt32(kvp[nameof(IdentityTable.Id)],  CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryFirstAsyncViaDynamicsWithStoredProcedure()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.ExecuteQueryFirstAsync("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).ConfigureAwait(false);

                // Assert
                var kvp = result as IDictionary<string, object>;
                Assert.AreEqual(tables.Last().Id, Convert.ToInt32(kvp[nameof(IdentityTable.Id)],  CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlConnectionExecuteQueryFirstAsyncViaDynamicsIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync("SELECT * FROM [sc].[IdentityTable];").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlConnectionExecuteQueryFirstAsyncViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<SqlException>(async () => await connection.ExecuteQueryFirstAsync("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlConnectionExecuteQueryFirstAsyncViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<SqlException>(async () => await connection.ExecuteQueryFirstAsync("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #region ExecuteQueryFirst<TEntity>

        [TestMethod]
        public void TestSqlConnectionExecuteQueryFirst()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryFirst<IdentityTable>("SELECT * FROM [sc].[IdentityTable] ORDER BY Id ASC;");

                // Assert
                Helper.AssertPropertiesEquality(tables[0], result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryFirstWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryFirst<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE Id = @Id;",
                    new { tables.Last().Id });

                // Assert
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryFirstWithStoredProcedure()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryFirst<IdentityTable>("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryFirstIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst<IdentityTable>("SELECT * FROM [sc].[IdentityTable];"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryFirstIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<SqlException>(() => connection.ExecuteQueryFirst<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryFirstIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<SqlException>(() => connection.ExecuteQueryFirst<IdentityTable>("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);"));
            }
        }

        #endregion

        #region ExecuteQueryFirstAsync<TEntity>

        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryFirstAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.ExecuteQueryFirstAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] ORDER BY Id ASC;").ConfigureAwait(false);

                // Assert
                Helper.AssertPropertiesEquality(tables[0], result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryFirstAsyncWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.ExecuteQueryFirstAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE Id = @Id;",
                    new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryFirstAsyncWithStoredProcedure()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.ExecuteQueryFirstAsync<IdentityTable>("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).ConfigureAwait(false);

                // Assert
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlConnectionExecuteQueryFirstAsyncIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable];").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlConnectionExecuteQueryFirstAsyncIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<SqlException>(async () => await connection.ExecuteQueryFirstAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlConnectionExecuteQueryFirstAsyncIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<SqlException>(async () => await connection.ExecuteQueryFirstAsync<IdentityTable>("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
